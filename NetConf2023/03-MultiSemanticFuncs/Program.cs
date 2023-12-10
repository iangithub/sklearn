using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.PromptTemplate.Handlebars;
using Microsoft.SemanticKernel.Plugins;
using System.Reflection;

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

// Load prompt from Directory
using StreamReader reader = new(Path.Combine(currentDirectory, "Plugins", "WriterPlugin", "Writer.yaml"));
KernelFunction writerPlugin = kernel.CreateFunctionFromPromptYaml(reader.ReadToEnd());

using StreamReader reader2 = new(Path.Combine(currentDirectory, "Plugins", "TranslatePlugin", "Translate.yaml"));
KernelFunction transPlugin = kernel.CreateFunctionFromPromptYaml(reader2.ReadToEnd());

while (true)
{
    System.Console.Write("User > ");
    var userMessage = Console.ReadLine()!;

    var writerResult = (await kernel.InvokeAsync(writerPlugin, arguments: new()
        {
            { "topic", userMessage }
        })).ToString();

    var finalResult = (await kernel.InvokeAsync(transPlugin, arguments: new()
        {
            { "topic", userMessage },{ "postcontent", writerResult }
        })).ToString();

    System.Console.WriteLine($"Assistant > {finalResult}");
}
