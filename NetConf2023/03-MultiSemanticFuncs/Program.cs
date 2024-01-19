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
