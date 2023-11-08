using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace MixSemanticFunAndNativeFun
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

            await WriteEmail(kernel);

        }

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
            Console.WriteLine("========= Semantic Function result ======================\n\n");
            var emailResult = await kernel.RunAsync(emailFun, new ContextVariables() { { "CusCode", "C2023001" } });

            Console.WriteLine(emailResult.GetValue<string>());
        }
    }
}