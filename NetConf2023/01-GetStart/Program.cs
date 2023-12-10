using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.PromptTemplate.Handlebars;
using Microsoft.SemanticKernel.Plugins;
using Microsoft.VisualBasic;
using System.Reflection;
using Newtonsoft.Json;

string Gpt4DeploymentName = "xxx";
string Gpt4ModelId = "xxx";
string AzureOpenAIEndpoint = "https://xxx.openai.azure.com/";
string AzureOpenAIApiKey = "xxx";
string currentDirectory = Directory.GetCurrentDirectory();

Kernel kernel = new KernelBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: Gpt4DeploymentName,
                modelId: Gpt4ModelId,
                endpoint: AzureOpenAIEndpoint,
                apiKey: AzureOpenAIApiKey)
            .Build();

// Load prompt from resource
using StreamReader reader = new(Path.Combine(currentDirectory, "Plugins", "Chat.yaml"));
KernelFunction prompt = kernel.CreateFunctionFromPromptYaml(
    reader.ReadToEnd(),
    promptTemplateFactory: new HandlebarsPromptTemplateFactory()
);


ChatHistory chatMessages = [];


while (true)
{
    // Get user input
    System.Console.Write("User > ");
    chatMessages.AddUserMessage(Console.ReadLine()!);

    // Get the chat completions
    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        FunctionCallBehavior = FunctionCallBehavior.AutoInvokeKernelFunctions
    };

    var result = kernel.InvokeStreamingAsync<StreamingChatMessageContent>(
        prompt,
        arguments: new(openAIPromptExecutionSettings) {
            { "messages", chatMessages }
        });

    // Print the chat completions
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
    System.Console.WriteLine("\n\n");

    chatMessages.AddMessage(chatMessageContent!);
}