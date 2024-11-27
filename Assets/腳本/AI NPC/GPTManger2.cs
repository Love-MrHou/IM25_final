using UnityEngine;
using UnityEngine.UI;
using OpenAI_API;
using OpenAI_API.Chat;
using System.Collections.Generic;
using OpenAI_API.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;
using System;

public class GPTManager2 : MonoBehaviour
{
    public Text gptOutputText; // 用於顯示 GPT 回應的 UI Text
    public Text text; // 用於接收使用者輸入的 UI Text
    public string characterIntroduction; // NPC 的角色介紹，用於 GPT 的系統提示詞

    public AudioSource audioSource; // 音頻來源，播放合成語音
    private OpenAIAPI api; // OpenAI API 客戶端
    private SpeechSynthesizer synthesizer; // Azure 語音合成器
    private bool isSpeaking = false; // 控制語音合成的狀態，避免重複呼叫

    void Start()
    {
        string openAiApiKey = "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";
        if (!string.IsNullOrEmpty(openAiApiKey))
        {
            api = new OpenAIAPI(openAiApiKey);
        }
        else
        {
            Debug.LogError("Failed to read OpenAI API key from environment variables.");
            return;
        }

        string azureApiKey = "155998f0555f47ae9ad78430ef6491aa";
        string azureRegion = "eastus";
        if (!string.IsNullOrEmpty(azureApiKey) && !string.IsNullOrEmpty(azureRegion))
        {
            var config = SpeechConfig.FromSubscription(azureApiKey, azureRegion);
            config.SpeechSynthesisLanguage = "zh-TW";
            config.SpeechSynthesisVoiceName = "zh-CN-YunjianNeural";
            var audioConfig = AudioConfig.FromDefaultSpeakerOutput();
            synthesizer = new SpeechSynthesizer(config, audioConfig);
        }
        else
        {
            Debug.LogError("Failed to read Azure API key or region from environment variables.");
            return;
        }
    }

    // 接收使用者輸入並處理
    public async void ProcessInputFromText()//
    {
        if (isSpeaking) return; // 如果正在進行語音合成，則不執行
        isSpeaking = true; // 標記語音合成進行中

        string userInput = text.text; // 取得使用者輸入的文字

        if (string.IsNullOrEmpty(userInput))
        {
            Debug.LogError("User input is empty."); // 如果輸入為空，顯示錯誤訊息
            isSpeaking = false; // 重置語音合成狀態
            return;
        }

        // 準備傳遞給 GPT 的聊天訊息清單
        List<ChatMessage> messages = new List<ChatMessage>
        {
            new ChatMessage(ChatMessageRole.System, characterIntroduction), // NPC 角色介紹作為系統提示
            new ChatMessage(ChatMessageRole.User, userInput) // 使用者輸入作為用戶訊息
        };

        isSpeaking = false; // 重置語音合成狀態

        try
        {
            // 呼叫 OpenAI API，獲取 GPT 的回應
            var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
            {
                Model = Model.ChatGPTTurbo, // 使用 GPT-3.5 Turbo 模型
                Temperature = 0.9, // 調整回應的創造性程度
                MaxTokens = 100, // 限制回應的字數
                Messages = messages // 傳遞聊天訊息清單
            });

            // 檢查是否成功獲取回應
            if (chatResult != null && chatResult.Choices != null && chatResult.Choices.Count > 0)
            {
                string gptResponse = chatResult.Choices[0].Message.Content; // 取得 GPT 的回應內容

                if (gptOutputText != null)
                {
                    gptOutputText.text = gptResponse; // 將 GPT 回應顯示在 UI Text 中
                    await TextToSpeech(gptResponse); // 將 GPT 回應轉換為語音
                }
                else
                {
                    Debug.LogError("gptOutputText property is null! Assign a UI Text element to it."); // 如果 UI 元素為空，顯示錯誤訊息
                }
            }
            else
            {
                Debug.LogError("Failed to get valid response from GPT."); // 如果回應無效，顯示錯誤訊息
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during OpenAI API call: {ex.Message}"); // 處理 API 呼叫中的例外
        }

        isSpeaking = false; // 重置語音合成狀態
    }

    // 將文字轉換為語音
    private async Task TextToSpeech(string text)
    {
        try
        {
            var result = await synthesizer.SpeakTextAsync(text); // 使用 Azure 語音合成

            // 如果語音合成成功，播放生成的語音
            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                var sampleCount = result.AudioData.Length / 2;
                var audioData = new float[sampleCount];
                for (var i = 0; i < sampleCount; ++i)
                {
                    audioData[i] = (short)(result.AudioData[i * 2 + 1] << 8 | result.AudioData[i * 2]) / 32768.0F; // 轉換音頻數據格式
                }

                var audioClip = AudioClip.Create("SynthesizedAudio", sampleCount, 1, 16000, false); // 創建音頻剪輯
                audioClip.SetData(audioData, 0); // 設定音頻數據
                audioSource.clip = audioClip; // 將音頻剪輯分配給音源
                audioSource.Play(); // 播放音頻
                Debug.Log("Speech synthesis succeeded!"); // 紀錄成功訊息
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                Debug.LogError($"Speech synthesis canceled: {cancellation.ErrorDetails}"); // 如果語音合成被取消，顯示錯誤訊息
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during speech synthesis: {ex.Message}"); // 處理語音合成中的例外
        }
        finally
        {
            isSpeaking = false; // 重置語音合成狀態
        }
    }

    private void OnDestroy()
    {
        synthesizer.Dispose(); // 釋放語音合成器資源
    }

    public void OnButtonClick()
    {
        ProcessInputFromText(); // 按鈕點擊觸發處理使用者輸入
    }
}