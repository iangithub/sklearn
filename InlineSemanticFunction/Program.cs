using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace InlineSemanticFunction
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

            //Inline Semantic Function Intro
            await InlineSample.NormalParameterAsync(kernel);

            //Customize Parameter
            //await InlineSample.CustomizeParameterAsync(kernel);

            //Few-shot
           //await InlineSample.FewShotSampleAsync(kernel);

        }
    }
}