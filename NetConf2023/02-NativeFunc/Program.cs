using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.PromptTemplate.Handlebars;
using Microsoft.SemanticKernel.Plugins;
using System.Reflection;
using MyPlugins;

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
using StreamReader reader = new(Path.Combine(currentDirectory, "Plugins", "ChatPlugin", "Chat.yaml"));
KernelFunction prompt = kernel.CreateFunctionFromPromptYaml(
    reader.ReadToEnd(),
    promptTemplateFactory: new HandlebarsPromptTemplateFactory()
);

//Load Plugins
kernel.Plugins.AddFromType<DataTimePlugin>();

// Create the chat history
ChatHistory chatMessages = [];

while (true)
{


    // Get user input
    System.Console.Write("User > ");
    var userMessage = Console.ReadLine()!;
    chatMessages.AddUserMessage(userMessage);

    // Enable auto invocation of kernel functions
    OpenAIPromptExecutionSettings settings = new()
    {
        FunctionCallBehavior = FunctionCallBehavior.AutoInvokeKernelFunctions
    };

    var result = kernel.InvokeStreamingAsync<StreamingChatMessageContent>(
        prompt,
        arguments: new(settings) {
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
