using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planners;

namespace SequentialPlannerSample
{
    internal class Program
    {
        private const string deploy_model = "xxx";
        private const string aoai_Endpoint = "https://xxx.openai.azure.com";
        private const string api_Key = "xxx";

        static async Task Main(string[] args)
        {
            var kernel = new KernelBuilder()
                  .WithAzureChatCompletionService(
                   deploy_model,   // Azure OpenAI Deployment Name
                   aoai_Endpoint, // Azure OpenAI Endpoint
                   api_Key  // Azure OpenAI Key
                  ).Build();

            // Import the Plugin from the plugins directory.
            var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
            kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory
                , "WriterPlugin");

            var planner = new SequentialPlanner(kernel);
            var plan = await planner.CreatePlanAsync("寫一則關於chatgpt對教育場景影響的臉書貼文，然後翻譯該則貼文為英文.");
            //var plan = await planner.CreatePlanAsync("寫一則關於chatgpt對教育場景影響的臉書貼文，並且翻譯成英文.");

            Console.WriteLine("Original plan:");

            foreach (var step in plan.Steps)
            {
                Console.WriteLine($"step : {step.Name}");
            }


            var result = await kernel.RunAsync(plan);

            Console.WriteLine("Result:");
            Console.WriteLine(result.GetValue<string>());
        }
    }
}