﻿using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace MixSemanticFunAndNativeFun
{
    internal class Program
    {
        //OpenAI
        private const string openai_Key = "xxx";
        private const string openai_deploy_Model = "gpt-4-1106-preview";

        static async Task Main(string[] args)
        {
            //OpenAI
            var kernel = new KernelBuilder()
                .WithOpenAIChatCompletionService(
                 modelId: openai_deploy_Model,   // OpenAI Deployment Name
                 apiKey: openai_Key  // OpenAI Key
                ).Build();

            await WriteEmail(kernel);
            //await WriteEmail2(kernel);

        }

        //semantic function調用native function範例
        private static async Task WriteEmail(IKernel kernel)
        {
            // Load native plugin into the kernel function collection
            // Functions loaded here are available as "CrmPlugin.*"
            var crmPlugin = kernel.ImportFunctions(new Plugins.CrmPlugin.Customer(), "CrmPlugin");

            var writePlugin = "WriterPlugin";
            var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
            kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, writePlugin);
            var emailFun = kernel.Functions.GetFunction(writePlugin, "Email");

            // Show the result
            Console.WriteLine("========= Semantic Function result ======================");
            var emailResult = await kernel.RunAsync(emailFun, new ContextVariables() { { "CusCode", "C2023001" } });

            Console.WriteLine(emailResult.GetValue<string>());
        }

        //native function 傳遞多個參數範例(共享上下文SKContext)
        private static async Task WriteEmail2(IKernel kernel)
        {
            // Load native plugin into the kernel function collection
            // Functions loaded here are available as "CrmPlugin.*"
            var crmPlugin = kernel.ImportSkill(new Plugins.CrmPlugin.Customer(), "CrmPlugin");

            // Import the Plugin from the plugins directory.
            var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
            var writePlugin = kernel.ImportSemanticSkillFromDirectory(pluginsDirectory, "WriterPlugin");
            var contextVars = new ContextVariables()
            {
                ["CusCode"] = "C2023001",
                ["Area"] = "KH"
            };


            // Show the result
            Console.WriteLine("--- Semantic Function result");
            var result = await kernel.RunAsync(contextVars, writePlugin["Email2"]);
            Console.WriteLine(result);
        }
    }
}