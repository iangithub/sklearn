using Microsoft.SemanticKernel;
using Microsoft.VisualBasic;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

string Gpt4DeploymentName = "xxx";
string Gpt4ModelId = "xxx";
string AzureOpenAIEndpoint = "https://xxx.openai.azure.com/";
string AzureOpenAIApiKey = "xxx";
string currentDirectory = Directory.GetCurrentDirectory();

Kernel kernel = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(
                    deploymentName: Gpt4DeploymentName,
                    endpoint: AzureOpenAIEndpoint,
                    apiKey: AzureOpenAIApiKey,
                    modelId: Gpt4ModelId)
                .Build();

// Load prompt from resource
using StreamReader reader = new(Path.Combine(currentDirectory, "Plugins", "Chat.yaml"));
KernelFunction prompt = kernel.CreateFunctionFromPromptYaml(
    reader.ReadToEnd(),
    promptTemplateFactory: new HandlebarsPromptTemplateFactory()
);


// #pragma warning disable SKEXP0004 
// kernel.PromptRendered += (sender, args) =>
// {
//     Console.WriteLine("=========== PromptRendered Start ===========");
//     Console.WriteLine(args.RenderedPrompt);
//     Console.WriteLine("=========== PromptRendered End ===========\n\n");

// };
// #pragma warning restore SKEXP0004 


ChatHistory chatMessages = [];


while (true)
{
    // Get user input
    System.Console.Write("User > ");
    chatMessages.AddUserMessage(Console.ReadLine()!);

    // Enable auto invocation of kernel functions
    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    // Get the chat completions
    var result = kernel.InvokeStreamingAsync<StreamingChatMessageContent>(
        prompt,
        arguments: new(openAIPromptExecutionSettings) {
            { "messages", chatMessages }
        });


    ChatMessageContent? chatMessageContent = null;

    await foreach (var content in result)
    {
        System.Console.Write(content);
        if (chatMessageContent == null)
        {
            System.Console.Write("Assistant > ");
            chatMessageContent = new ChatMessageContent(
                content.Role ?? AuthorRole.Assistant,
                content.ModelId!,
                content.Content!,
                content.InnerContent,
                content.Encoding,
                content.Metadata);
        }
        else
        {
            chatMessageContent.Content += content;
        }
    }
    System.Console.WriteLine("\n");

    chatMessages.Add(chatMessageContent!);
}