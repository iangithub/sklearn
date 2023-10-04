using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel;
using Azure.Core;

namespace NativeFunctionSample
{
    internal class Program
    {
        private const string deploy_model = "xxx";
        private const string aoai_Endpoint = "https://xxx.openai.azure.com";
        private const string api_Key = "xxxx";

        static async Task Main(string[] args)
        {
            var kernel = new KernelBuilder()
                 .WithAzureChatCompletionService(
                  deploy_model,   // Azure OpenAI Deployment Name
                  aoai_Endpoint, // Azure OpenAI Endpoint
                  api_Key  // Azure OpenAI Key
                 ).Build();

            await Sample2Async(kernel);
        }

        static async Task Sample1Async(IKernel kernel)
        {
            // Import the Plugin .
            var crmPlugin = kernel.ImportSkill(new Plugins.CrmPlugin.Customer(), "CrmPlugin");

            // 經由Native Function取得指定客戶編號的連絡人姓名
            var cusContactName = await kernel.RunAsync("C2023001", crmPlugin["GetCustomerContact"]);

            Console.WriteLine(cusContactName);
        }

        static async Task Sample2Async(IKernel kernel)
        {
            // Import the Plugin .
            var crmPlugin = kernel.ImportSkill(new Plugins.CrmPlugin.Customer(), "CrmPlugin");

            // Import the Plugin from the plugins directory.
            var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
            var plugin = kernel.ImportSemanticSkillFromDirectory(pluginsDirectory, "WriterPlugin");

            //調用CrmPlugin/GetCustomerContact function拿到客戶姓名
            var getCusContact = kernel.Skills.GetFunction("CrmPlugin", "GetCustomerContact");
            string cusContact = (await kernel.RunAsync("C2023001", getCusContact)).Result;

            //調用WriterPlugin/Email function寫email內容
            var writeEmail = kernel.Skills.GetFunction("WriterPlugin", "Email");
            string email = (await kernel.RunAsync(cusContact, writeEmail)).Result;


            Console.WriteLine(email);
        }
    }
}