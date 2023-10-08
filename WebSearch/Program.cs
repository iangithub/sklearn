using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.ChatCompletion;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Skills.Web;
using Microsoft.SemanticKernel.Skills.Web.Bing;
//using Microsoft.SemanticKernel.Skills.Web.Google;

namespace WebSearch
{
    internal class Program
    {
        private const string deploy_model = "xxx";
        private const string aoai_Endpoint = "https://xxx.openai.azure.com";
        private const string api_Key = "xxxx";
        private const string bingSerach_Key = "xxxx";

        static async Task Main(string[] args)
        {


            //builder.WithOpenAIChatCompletionService  //using OpenAI
            var kernel = new KernelBuilder()
                .WithAzureChatCompletionService(
                 deploy_model,   // Azure OpenAI Deployment Name
                 aoai_Endpoint, // Azure OpenAI Endpoint
                 api_Key  // Azure OpenAI Key
                ).Build();

            // Connector
            var bingConnector = new BingConnector(bingSerach_Key);
            kernel.ImportSkill(new WebSearchEngineSkill(bingConnector), "bing");

            // Import the OrchestratorPlugin from the plugins directory.
            var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
            var plugin = kernel.ImportSemanticSkillFromDirectory(pluginsDirectory, "QASkill");

            string query = string.Empty;
            while (true)
            {
                Console.WriteLine("bot: 你想問什麼事情呢? (結束請輸入exit) \n");
                Console.Write("you: ");
                query = Console.ReadLine(); //ex: "2023年杭州亞運棒球項目，中華隊是第幾名";
                Console.Write("\n");

                if (query.ToLower() == "exit")
                {
                    break;
                }

                var bingResult = await kernel.Skills.GetFunction("bing", "search").InvokeAsync(query);

                var variables = new ContextVariables
                {
                    ["ans_result"] = bingResult.ToString(),
                    ["query_input"] = query
                };

                //叫用GPT模型等待生成結果
                var result = (await kernel.RunAsync(variables, plugin["AssistantResults"])).Result;

                Console.WriteLine($"bot: {result} ");
            }
        }
    }
}