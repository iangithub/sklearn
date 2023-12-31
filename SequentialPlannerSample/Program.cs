﻿using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planners;

namespace SequentialPlannerSample
{
    internal class Program
    {
        //OpenAI
        private const string openai_Key = "xxx";
        private const string openai_deploy_Model = "gpt-4-1106-preview";


        static async Task Main(string[] args)
        {
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