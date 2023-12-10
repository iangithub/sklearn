using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.ChatCompletion;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using static System.Runtime.InteropServices.JavaScript.JSType;
//using Microsoft.SemanticKernel.Skills.Web.Google;

namespace WebSearch
{
    internal class Program
    {
        //AOAI
        private const string deploy_Model = "xxx";
        private const string aoai_Endpoint = "https://xxx.openai.azure.com";
        private const string api_Key = "xxx";
        private const string bingSerach_Key = "xxx";

        //OpenAI
        private const string openai_Key = "xxx";
        private const string openai_deploy_Model = "gpt-4-1106-preview";

        static async Task Main(string[] args)
        {
            //Azure OpenAI
            //var kernel = new KernelBuilder()
            //    .WithAzureChatCompletionService(
            //     deploy_Model,   // Azure OpenAI Deployment Name
            //     aoai_Endpoint, // Azure OpenAI Endpoint
            //     api_Key  // Azure OpenAI Key
            //    ).Build();


            //OpenAI
            var kernel = new KernelBuilder()
                .WithOpenAIChatCompletionService(
                 modelId: openai_deploy_Model,   // OpenAI Deployment Name
                 apiKey: openai_Key  // OpenAI Key
                ).Build();


            // Connector
            var searchPluginName = "bing";
            var bingConnector = new BingConnector(bingSerach_Key);
            kernel.ImportFunctions(new WebSearchEnginePlugin(bingConnector), searchPluginName);

            // Import the semantic function from the plugins directory.
            var qaPlugin = "QAPlugin";
            var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
            kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, qaPlugin);
            var assistantResultsFun = kernel.Functions.GetFunction(qaPlugin, "AssistantResults");

            string question = string.Empty;
            while (true)
            {
                Console.WriteLine("bot: 你想問什麼事情呢? (結束請輸入exit) \n");
                Console.Write("you: ");
                question = Console.ReadLine();
                Console.Write("\n");

                if (question.ToLower() == "exit")
                {
                    break;
                }

                var searchFun = kernel.Functions.GetFunction(searchPluginName, "search");
                var searchResult = await kernel.RunAsync(question, searchFun);

                //return
                var assistantResult = await kernel.RunAsync(assistantResultsFun, new ContextVariables() { { "query_input", question }, { "ans_result", searchResult.ToString() } });
                Console.WriteLine($"bot: {assistantResult.GetValue<string>()} \n \n");
            }
        }
    }
}