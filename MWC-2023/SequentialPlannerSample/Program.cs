using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planners;

namespace SequentialPlannerSample
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

            // Import the Plugin from the plugins directory.
            var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
            kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, "WriterPlugin");

            var planner = new SequentialPlanner(kernel);
            var goal = "寫一則關於chatgpt對教育場景影響的臉書貼文，然後翻譯該則貼文為英文.";
            var plan = await planner.CreatePlanAsync(goal);

            Console.WriteLine("============ Original plan ====================");

            foreach (var step in plan.Steps)
            {
                Console.WriteLine($"step : {step.Name}");
            }
            Console.WriteLine("\n\n");


            var result = await kernel.RunAsync(plan);
            Console.WriteLine("============ Result ====================");
            Console.WriteLine(result.GetValue<string>());
        }
    }
}