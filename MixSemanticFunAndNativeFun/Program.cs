using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace MixSemanticFunAndNativeFun
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
    }
}