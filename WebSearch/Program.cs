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
        private const string deploy_model = "gpt4demo";
        private const string aoai_Endpoint = "https://demo0222.openai.azure.com";
        private const string api_Key = "b13848c58d164024813abe2429467233";
        private const string bingSerach_Key = "160e2d29fadc4feeb0dd35c01faec289";

        static async Task Main(string[] args)
        {
            Console.WriteLine("bot: 你想問什麼事情呢? \n");
            Console.Write("you: ");
            string query =Console.ReadLine();
            Console.Write("\n");

            //string query = "杜德偉近期受到哪一隻中職球隊的邀請？";

            //builder.WithOpenAIChatCompletionService  //using OpenAI
            var kernel = new KernelBuilder()
                .WithAzureChatCompletionService(
                 deploy_model,   // Azure OpenAI Deployment Name
                 aoai_Endpoint, // Azure OpenAI Endpoint
                 api_Key  // Azure OpenAI Key
                ).Build();

            var bingConnector = new BingConnector(bingSerach_Key);
            kernel.ImportSkill(new WebSearchEngineSkill(bingConnector), "bing");

            var bingResult = await kernel.Func("bing", "search").InvokeAsync(query);

            var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(),"Plugins");
            // Import the OrchestratorPlugin from the plugins directory.
            var plugin = kernel.ImportSemanticSkillFromDirectory(pluginsDirectory, "QASkill");

            var variables = new ContextVariables
            {
                ["ans_result"] = bingResult.ToString(),
                ["query_input"] = query
            };
           
            //叫用GPT模型等得生成結果
            var result = (await kernel.RunAsync(variables, plugin["AssistantResults"])).Result;

            Console.WriteLine($"bot: {result} \n (ref:{bingResult})");
        }
    }
}