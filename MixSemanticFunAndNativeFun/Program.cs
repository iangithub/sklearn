using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace MixSemanticFunAndNativeFun
{
    internal class Program
    {
        private const string deploy_model = "gpt4demo";
        private const string aoai_Endpoint = "https://demo0222.openai.azure.com";
        private const string api_Key = "89d1060fc53e438491c2078d2da84e0d";


        static async Task Main(string[] args)
        {
            var kernel = new KernelBuilder()
                .WithAzureChatCompletionService(
                 deploy_model,   // Azure OpenAI Deployment Name
                 aoai_Endpoint, // Azure OpenAI Endpoint
                 api_Key  // Azure OpenAI Key
                ).Build();
            
           // await WriteEmail(kernel);
            await WriteEmail2(kernel);

        }

        //semantic function調用native function範例
        private static async Task WriteEmail(IKernel kernel)
        {
            // Load native plugin into the kernel function collection
            // Functions loaded here are available as "CrmPlugin.*"
            var crmPlugin = kernel.ImportSkill(new Plugins.CrmPlugin.Customer(), "CrmPlugin");

            // Import the Plugin from the plugins directory.
            var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
            var writePlugin = kernel.ImportSemanticSkillFromDirectory(pluginsDirectory, "WriterPlugin");
            var contextVars = new ContextVariables()
            {
                ["CusCode"] = "C2023001"
            };


            // Show the result
            Console.WriteLine("--- Semantic Function result");
            var result = await kernel.RunAsync(contextVars, writePlugin["Email"]);
            Console.WriteLine(result);
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