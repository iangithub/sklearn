using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace SerializingSemanticFunction
{
    internal class Program
    {
        private const string deploy_model = "xxx";
        private const string aoai_Endpoint = "https://xxxx.openai.azure.com";
        private const string api_Key = "xxxx";

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
            var plugin = kernel.ImportSemanticSkillFromDirectory(pluginsDirectory, "WriterPlugin");

            //few-shot,帶入方文山詞創作範例
            string fewshot_sample = @"
竹籬上 停留著 蜻蜓
玻璃瓶裡插滿 小小 森林
青春 嫩綠的很 鮮明

百葉窗 折射的 光影
像有著心事的 一張 表情
而你 低頭拆信 想知道關於我的事情

月色搖晃樹影 穿梭在熱帶雨林
你離去的原因從來不說明
你的謊像陷阱我最後才清醒
幸福只是水中的倒影

月色搖晃樹影穿梭在熱帶雨林
悲傷的雨不停全身血淋淋
那深陷在沼澤我不堪的愛情
是我無能為力的傷心

蘆葦花開歲已寒 若霜又降路遙漫長
牆外是誰在吟唱 鳳求凰
梨園台上 西皮二黃
卻少了妳 無人問暖
誰在彼岸 天涯一方

在夢裡我醞釀著情緒
等回憶等那一種熟悉
人世間最溫柔的消息
是曾經被你擁入懷裡
";

            var contextVars = new ContextVariables()
            {
                ["fewshot_sample"] = fewshot_sample,
                ["post_subject"] = "ChatGPT對校園教育的衝擊"
            };

            //叫用GPT模型等得生成結果
            var result = (await kernel.RunAsync(contextVars, plugin["FacebookPoster"])).Result;

            Console.WriteLine(result);

        }
    }
}