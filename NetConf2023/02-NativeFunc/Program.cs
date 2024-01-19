using Microsoft.SemanticKernel;
using Microsoft.VisualBasic;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using MyPlugins;

string Gpt4DeploymentName = "xxx";
string Gpt4ModelId = "xxx";
string AzureOpenAIEndpoint = "https://xxx.openai.azure.com/";
string AzureOpenAIApiKey = "xxx";
string currentDirectory = Directory.GetCurrentDirectory();

Kernel kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: Gpt4DeploymentName,
                modelId: Gpt4ModelId,
                endpoint: AzureOpenAIEndpoint,
                apiKey: AzureOpenAIApiKey)
            .Build();

// Load prompt from resource
using StreamReader reader = new(Path.Combine(currentDirectory, "Plugins", "ChatPlugin", "Chat.yaml"));
KernelFunction prompt = kernel.CreateFunctionFromPromptYaml(
    reader.ReadToEnd(),
    promptTemplateFactory: new HandlebarsPromptTemplateFactory()
);

//Load Plugins
kernel.Plugins.AddFromType<DataTimePlugin>();


#pragma warning disable SKEXP0004 

kernel.PromptRendered += (sender, args) =>
{
    Console.WriteLine("=========== PromptRendered Start ===========");
    Console.WriteLine(args.RenderedPrompt);
    Console.WriteLine("=========== PromptRendered End ===========\n\n");
};


kernel.FunctionInvoking += (sender, args) =>
{
    Console.WriteLine("=========== FunctionInvoking Start ===========");
    Console.WriteLine(args.Function.Name);
    Console.WriteLine("=========== FunctionInvoking End ===========\n\n");

};

#pragma warning restore SKEXP0004 


// Create the chat history
ChatHistory chatMessages = [];

while (true)
{
    // Get user input
    System.Console.Write("User > ");
    var userMessage = Console.ReadLine()!;
    chatMessages.AddUserMessage(userMessage);

    // Enable auto invocation of kernel functions
    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
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
    System.Console.WriteLine("\n");

    chatMessages.Add(chatMessageContent!);

}
