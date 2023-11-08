using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Plugins.Memory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Memory;
using System.Globalization;
using System.Threading.Tasks;

namespace TransAndRagUseSemanticFun
{
    internal class Program
    {
        private const string deploy_Model = "gpt4demo";
        private const string embedding_Model = "textembedding";
        private const string aoai_Endpoint = "https://demo0222.openai.azure.com";
        private const string api_Key = "ece107c84324497e9987d15f916845e4";
        private const string embedding_CollectionName = "Law";

        static async Task Main(string[] args)
        {
            var kernel = new KernelBuilder()
                .WithAzureChatCompletionService(
                 deploy_Model,   // Azure OpenAI Deployment Name
                 aoai_Endpoint, // Azure OpenAI Endpoint
                 api_Key  // Azure OpenAI Key
                ).Build();


            var memoryWithCustomDb = new MemoryBuilder()
            .WithAzureTextEmbeddingGenerationService(embedding_Model, aoai_Endpoint, api_Key)
            .WithMemoryStore(new VolatileMemoryStore())
            .Build();

            //Init KM
            await ImportKm(memoryWithCustomDb);

            // Import the Plugin from the plugins directory.
            var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");

            //semantic fun
            kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, "QAPlugin");
            var assistantResultsFun = kernel.Functions.GetFunction("QAPlugin", "AssistantResults");
            var transFun = kernel.Functions.GetFunction("QAPlugin", "Translate");

            //native fun
            kernel.ImportFunctions(new Plugins.QAPlugin.KmSearch(), "KmSearchPlugin");
            var kmSearchFun = kernel.Functions.GetFunction("KmSearchPlugin", "GetKm");

            while (true)
            {
                Console.WriteLine("bot: 您好，我是法規知識大師，您想問什麼呢? \n");
                Console.Write("you: ");

                //var query = "永久保存的機關檔案可以做為公開資料嗎?";
                //var query = "不禮讓行人，會有罸則嗎";
                //var query = "闖紅燈罸多少錢啊";
                //var query = "檔案要傳送到國外可以嗎";
                var query = Console.ReadLine();
                Console.Write("\n");

                if (string.Compare(query, "exit", true) == 0)
                {
                    break;
                }

                if (string.IsNullOrEmpty(query))
                {
                    return;
                }


                var cv = new ContextVariables() {
                    { "query_input", query },
                    { "trans_input", string.Empty },
                    { "ans_ref", string.Empty },
                };

                var result = await kernel.RunAsync(
                    cv,
                    transFun,
                    kmSearchFun,
                    assistantResultsFun
                );


                //return
                Console.WriteLine($"bot: {result.GetValue<string>()} \n \n");

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