using Microsoft.SemanticKernel.Plugins.Memory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Memory;
using System.Globalization;
#pragma warning disable SKEXP0052
#pragma warning disable SKEXP0011
#pragma warning disable SKEXP0026
#pragma warning disable SKEXP0003

namespace TransAndRag
{
    internal class Program
    {

        //AOAI
        private const string deploy_Model = "xxx";
        private const string gpt4ModelId = "xxx";
        private const string aoai_Endpoint = "https://xxx.openai.azure.com";
        private const string api_Key = "xxx";
        private const string embedding_DeploymentName = "xxx";
        private const string embedding_ModelId = "xxx";
        private const string embedding_CollectionName = "Law";

        static async Task Main(string[] args)
        {
            //AOAI
            var kernel = new KernelBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: deploy_Model,
                modelId: gpt4ModelId,
                endpoint: aoai_Endpoint,
                apiKey: api_Key)
            .Build();

            var memoryWithCustomDb = new MemoryBuilder()
            .WithAzureOpenAITextEmbeddingGeneration(embedding_DeploymentName, embedding_ModelId, aoai_Endpoint, api_Key)
            .WithMemoryStore(new VolatileMemoryStore())
            .Build();

            //Init KM
            await ImportKm(memoryWithCustomDb);

            // Import the Plugin from the plugins directory.
            var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
            var plugin = kernel.ImportPluginFromPromptDirectory(Path.Combine(pluginsDirectory, "QAPlugin"));
            KernelFunction assistantResultsFun = plugin["AssistantResults"];
            KernelFunction transFun = plugin["Translate"];

            #region show kernel plugins and functions
            // var functions = kernel.Plugins.GetFunctionsMetadata();
            // Console.WriteLine("*****************************************");
            // Console.WriteLine("****** Registered plugins and functions ******");
            // Console.WriteLine("*****************************************");
            // Console.WriteLine();
            // foreach (KernelFunctionMetadata func in functions)
            // {
            //     Console.WriteLine($"func_Name >  {func.Name}, func_Description > {func.Description}");
            //     if (func.Parameters.Count > 0)
            //     {
            //         Console.WriteLine("func_Params > ");
            //         foreach (var p in func.Parameters)
            //         {
            //             Console.WriteLine($"Parameter_Name > {p.Name}: {p.Description}");
            //             Console.WriteLine($"Parameter_DefaultValue > '{p.DefaultValue}'");
            //         }
            //     }
            // }
            // Console.WriteLine("=========================================\n\n");
            #endregion

            while (true)
            {
                Console.WriteLine("Assistant > 您好，您想問什麼呢? ( What can I help you with? ) \n");
                Console.Write("User > ");

                var query = Console.ReadLine();
                Console.Write("\n");

                if (string.Compare(query, "exit", true) == 0)
                {
                    Console.WriteLine("Assistant > bye........");
                    break;
                }

                if (string.IsNullOrEmpty(query))
                {
                    return;
                }

                //自動翻譯
                var transQuery = (await kernel.InvokeAsync(transFun, arguments: new()
                    {{"query_input", query }})).ToString();

                //RAG Search
                var searchResult = memoryWithCustomDb
                    .SearchAsync(embedding_CollectionName, transQuery, minRelevanceScore: 0.8);

                var refContent = string.Empty;

                await foreach (var kmResult in searchResult)
                {
                    refContent += $"{kmResult.Metadata.Text}\n\n";
                }

                #region show kernel prompt render

                // kernel.PromptRendering += (sender, e) =>
                // {
                //     Console.WriteLine("========== Prompt Rendering Start ================");
                //     Console.WriteLine($"Function.Name => {e.Function.Name} ");
                //     Console.WriteLine("========== Prompt Rendering End ================");

                // };

                // kernel.PromptRendered += (sender, e) =>
                // {
                //     Console.WriteLine("========== Prompt Rendering Start ================");
                //     Console.WriteLine($"RenderedPrompt => {e.RenderedPrompt} ");
                //     Console.WriteLine("========== Prompt Rendering End ================");
                // };

                #endregion

                //return
                var assistantResult = await kernel.InvokeAsync(assistantResultsFun, arguments: new() { { "query_input", query }, { "reference_content", refContent } });

                Console.WriteLine($"Assistant > {assistantResult.GetValue<string>()} \n \n");
            }
        }

        private static async Task ImportKm(ISemanticTextMemory memoryBuilder)
        {
            //知識庫轉存向量儲存
            await memoryBuilder.SaveInformationAsync(embedding_CollectionName, id: "第1條", text: "本細則依檔案法（以下簡稱本法）第二十九條規定訂定之。");
            await memoryBuilder.SaveInformationAsync(embedding_CollectionName, id: "第2條第1項", text: "本法第二條第二款所稱管理程序，指依文書處理或機關業務相關法令規定，完成核定、發文或辦結之程序。");
            await memoryBuilder.SaveInformationAsync(embedding_CollectionName, id: "第2條第2項", text: "本法第二條第二款所稱文字或非文字資料及其附件，指各機關處理公務或因公務而產生之各類紀錄資料及其附件，包括各機關所持有或保管之文書、圖片、紀錄、照片、錄影（音）、微縮片、電腦處理資料等，可供聽、讀、閱覽或藉助科技得以閱覽或理解之文書或物品。");
            await memoryBuilder.SaveInformationAsync(embedding_CollectionName, id: "第3條", text: "各機關管理檔案，應依本法第四條規定，並參照檔案中央主管機關訂定之機關檔案管理單位及人員配置基準，設置或指定專責單位或人員。");
            await memoryBuilder.SaveInformationAsync(embedding_CollectionName, id: "第4條第1項", text: "各機關依本法第五條規定，經該管機關核准，將檔案運往國外者，應先以微縮、電子或其他方式儲存，並經管理該檔案機關首長核定。");
            await memoryBuilder.SaveInformationAsync(embedding_CollectionName, id: "第4條第2項", text: "前項檔案如屬永久保存之機關檔案，並應經檔案中央主管機關同意。");
            await memoryBuilder.SaveInformationAsync(embedding_CollectionName, id: "第5條", text: "各機關依本法第六條第二項規定，將檔案中之器物交有關機構保管時，應訂定書面契約或作成紀錄存查。");
        }
    }
}

/*
    sample case : 
    1.永久保存的機關檔案可以做為公開資料嗎?
    2.파일을 해외로 배송하는 데 제한이 있나요? (要把檔案運到國外，有什麼限制嗎)
    3.公文書法で規制される内容とは何ですか？ (檔案法所規範的內容是指哪些)
    4.Can permanently preserved agency files be turned into public information?
*/