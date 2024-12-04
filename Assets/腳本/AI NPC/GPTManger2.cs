using UnityEngine;
using UnityEngine.UI;
using OpenAI_API;
using OpenAI_API.Chat;
using System.Collections.Generic;
using OpenAI_API.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;
using System;
using CandyCoded.env;

public class GPTManager2 : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("用於顯示 GPT 回應的文字元件")]
    public Text gptOutputText;

    [Tooltip("用於接收使用者輸入的文字元件")]
    public Text inputText;

    [Tooltip("NPC 的角色介紹，用於 GPT 的系統提示詞")]
    public string characterIntroduction;

    [Header("Audio Configuration")]
    [Tooltip("音頻來源，用於播放合成語音")]
    public AudioSource audioSource;

    [Header("Environment Variables")]
    [Tooltip("OpenAI API 的環境變數名稱")]
    private readonly string openAiApiKeyName = "OPENAI_API_KEY";

    [Tooltip("Azure API 的環境變數名稱")]
    private readonly string azureApiKeyName = "AZURE_API_KEY";

    [Tooltip("Azure 地區的環境變數名稱")]
    private readonly string azureRegionName = "AZURE_REGION";

    private OpenAIAPI api; // OpenAI API 客戶端
    private SpeechSynthesizer synthesizer; // Azure 語音合成器
    private bool isSpeaking = false; // 語音合成狀態標誌
    private object threadLocker = new object(); // 用於多執行緒同步

    void Start()
    {
        InitializeOpenAIAPI();
        InitializeAzureSpeechAPI();

        // 檢查 UI 綁定
        if (gptOutputText == null)
        {
            Debug.LogError("gptOutputText 未設置！請在 Inspector 中綁定 UI 元件。");
        }

        if (inputText == null)
        {
            Debug.LogError("inputText 未設置！請在 Inspector 中綁定 UI 元件。");
        }
    }

    private void InitializeOpenAIAPI()
    {
        if (env.TryParseEnvironmentVariable(openAiApiKeyName, out string openAiApiKey))
        {
            api = new OpenAIAPI(openAiApiKey);
            Debug.Log("OpenAI API 初始化成功！");
        }
        else
        {
            Debug.LogError($"環境變數 {openAiApiKeyName} 未設置或為空！");
        }
    }

    private void InitializeAzureSpeechAPI()
    {
        if (env.TryParseEnvironmentVariable(azureApiKeyName, out string azureApiKey) &&
            env.TryParseEnvironmentVariable(azureRegionName, out string azureRegion))
        {
            var config = SpeechConfig.FromSubscription(azureApiKey, azureRegion);
            config.SpeechSynthesisLanguage = "zh-TW";
            config.SpeechSynthesisVoiceName = "zh-TW-HsiaoChenNeural";
            var audioConfig = AudioConfig.FromDefaultSpeakerOutput();
            synthesizer = new SpeechSynthesizer(config, audioConfig);
            Debug.Log("Azure Speech API 初始化成功！");
        }
        else
        {
            Debug.LogError($"環境變數 {azureApiKeyName} 或 {azureRegionName} 未設置或為空！");
        }
    }

    /// <summary>
    /// 處理使用者輸入並與 GPT 交互。
    /// </summary>
    public async void ProcessInputFromText()
    {
        lock (threadLocker)
        {
            if (isSpeaking) return;
            isSpeaking = true;
        }

        string userInput = inputText?.text;
        if (string.IsNullOrEmpty(userInput))
        {
            Debug.LogError("使用者輸入為空！");
            lock (threadLocker)
            {
                isSpeaking = false;
            }
            return;
        }

        List<ChatMessage> messages = new List<ChatMessage>
        {
            new ChatMessage(ChatMessageRole.System, characterIntroduction),
            new ChatMessage(ChatMessageRole.User, userInput)
        };

        try
        {
            var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
            {
                Model = Model.ChatGPTTurbo,
                Temperature = 0.9,
                MaxTokens = 100,
                Messages = messages
            });

            if (chatResult?.Choices != null && chatResult.Choices.Count > 0)
            {
                string gptResponse = chatResult.Choices[0].Message.Content;

                if (gptOutputText != null)
                {
                    gptOutputText.text = gptResponse;
                    await TextToSpeech(gptResponse); // 合成語音
                }
                else
                {
                    Debug.LogError("gptOutputText 未綁定 UI 元件！");
                }
            }
            else
            {
                Debug.LogError("未獲取有效的 GPT 回應！");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"GPT API 呼叫錯誤：{ex.Message}");
        }
        finally
        {
            lock (threadLocker)
            {
                isSpeaking = false;
            }
        }
    }

    /// <summary>
    /// 使用 Azure 語音合成將文字轉換為語音。
    /// </summary>
    /// <param name="text">要合成的文字</param>
    private async Task TextToSpeech(string text)
    {
        try
        {
            var result = await synthesizer.SpeakTextAsync(text);

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                Debug.Log("語音合成成功！");
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                Debug.LogError($"語音合成被取消：{cancellation.ErrorDetails}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"語音合成過程中發生錯誤：{ex.Message}");
        }
    }

    private void OnDestroy()
    {
        synthesizer?.Dispose();
    }

    public void OnButtonClick()
    {
        ProcessInputFromText();
    }
}
