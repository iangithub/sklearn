using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InlineSemanticFunction
{
    internal static class InlineSample
    {
        public static async Task NormalParameterAsync(IKernel kernel)
        {
            const string funcDefinition = @"
            你是一位facebook小編，請使用輕鬆灰諧的語氣，撰寫下列主題的貼文，內容500個字以內，#zh-tw
            ###
            {{$input}}
            ###
            ";

            var excuseFunction = kernel.CreateSemanticFunction(funcDefinition,
                skillName: "FacebookAgent", functionName: "Post",
                maxTokens: 2000, temperature: 0.2,
                description: "產生facebook貼文");

            var result = await kernel.RunAsync(@"ChatGPT對校園教育的衝擊", excuseFunction);
            Console.WriteLine(result);

        }

        public static async Task CustomizeParameterAsync(IKernel kernel)
        {
            const string funcDefinition = @"
你是一位facebook小編，請使用輕鬆灰諧的語氣，撰寫下列主題的貼文，內容500個字以內，#zh-tw
###
{{$post_subject}}
###
";

            var excuseFunction = kernel.CreateSemanticFunction(funcDefinition,
                skillName: "FacebookAgent", functionName: "Post",
                maxTokens: 2000, temperature: 0.2,
                description: "產生facebook貼文");

            var contextVars = new ContextVariables()
            {
                ["post_subject"] = "ChatGPT對校園教育的衝擊"
            };

            var result = await kernel.RunAsync(excuseFunction, contextVars);
            Console.WriteLine(result);
        }

        public static async Task FewShotSampleAsync(IKernel kernel)
        {
            const string funcDefinition = @"
你是一位facebook小編，請先思考一下下列提供的範例寫作風格
###
{{$fewshot_sample}}
###

接著撰寫下列主題的貼文，內容500個字以內，#zh-tw
###
{{$post_subject}}
###

";
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

            var excuseFunction = kernel.CreateSemanticFunction(funcDefinition,
                skillName: "FacebookAgent", functionName: "Post",
                maxTokens: 2000, temperature: 0.2,
                description: "產生facebook貼文");
            var contextVars = new ContextVariables()
            {
                ["fewshot_sample"] = fewshot_sample,
                ["post_subject"] = "ChatGPT對校園教育的衝擊"
            };

            var result = await kernel.RunAsync(excuseFunction, contextVars);
            Console.WriteLine(result);

        }
    }
}
