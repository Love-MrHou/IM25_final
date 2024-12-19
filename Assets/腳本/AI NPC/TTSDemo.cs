using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using UnityEngine;
using UnityEngine.UI;
using CandyCoded.env;
using System.Threading.Tasks;

public class TTSDemo : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("用於顯示並存放要轉換成語音的文字")]
    public Text textUI;

    [Tooltip("用於播放合成語音的音頻源")]
    public AudioSource audioSource;

    [Header("Speech Configuration")]
    [Tooltip("Azure 語音合成的聲音名稱，例如 zh-TW-HsiaoChenNeural")]
    public string speechSynthesisVoiceName;

    [Header("Environment Variables")]
    [Tooltip("Azure API 的環境變數名稱")]
    private readonly string azureApiKeyName = "AZURE_API_KEY";

    [Tooltip("Azure 地區的環境變數名稱")]
    private readonly string azureRegionName = "AZURE_REGION";

    private SpeechSynthesizer synthesizer; // Azure 語音合成器
    private string previousText = ""; // 儲存前一次的文字
    private string currentText = ""; // 儲存當前的文字

    private object threadLocker = new object(); // 用於執行緒同步

    void Start()
    {
        InitializeAzureSpeechAPI();

        // 檢查 UI 元件
        if (textUI == null)
        {
            Debug.LogError("Text UI 未設置！請在 Inspector 中設置一個 Text 元件。");
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource 未設置！請在 Inspector 中設置一個 AudioSource 元件。");
        }
    }

    /// <summary>
    /// 初始化 Azure 語音合成 API。
    /// </summary>
    private void InitializeAzureSpeechAPI()
    {
        if (env.TryParseEnvironmentVariable(azureApiKeyName, out string azureApiKey) &&
            env.TryParseEnvironmentVariable(azureRegionName, out string azureRegion))
        {
            var config = SpeechConfig.FromSubscription(azureApiKey, azureRegion);
            config.SpeechSynthesisLanguage = "zh-CN";
            config.SpeechSynthesisVoiceName = speechSynthesisVoiceName;

            var audioConfig = AudioConfig.FromStreamOutput(new PullAudioOutputStream());
            synthesizer = new SpeechSynthesizer(config, audioConfig);

            Debug.Log("Azure Speech API 初始化成功！");
        }
        else
        {
            Debug.LogError($"環境變數 {azureApiKeyName} 或 {azureRegionName} 未設置或為空！");
        }
    }

    void Update()
    {
        if (textUI != null && textUI.text != previousText)
        {
            lock (threadLocker)
            {
                currentText = textUI.text;

                if (audioSource.isPlaying)
                {
                    audioSource.Stop(); // 停止正在播放的音頻
                }

                StopCurrentSpeechSynthesis(); // 停止當前語音合成
                SynthesizeAndPlayText(currentText); // 合成並播放新語音

                previousText = currentText; // 更新文字記錄
            }
        }
    }

    /// <summary>
    /// 合成並播放文字。
    /// </summary>
    /// <param name="text">要合成的文字</param>
    public async void SynthesizeAndPlayText(string text)
    {
        try
        {
            var result = await synthesizer.SpeakTextAsync(text);

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                Debug.Log("語音合成成功！");
                PlaySynthesizedAudio(result.AudioData);
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"語音合成過程中發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 將語音資料轉換為音頻並播放。
    /// </summary>
    /// <param name="audioData">語音合成的音頻資料</param>
    private void PlaySynthesizedAudio(byte[] audioData)
    {
        var sampleCount = audioData.Length / 2;
        var audioSamples = new float[sampleCount];

        for (var i = 0; i < sampleCount; ++i)
        {
            audioSamples[i] = (short)(audioData[i * 2 + 1] << 8 | audioData[i * 2]) / 32768.0F;
        }

        var audioClip = AudioClip.Create("SynthesizedAudio", sampleCount, 1, 16000, false);
        audioClip.SetData(audioSamples, 0);
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    /// <summary>
    /// 停止當前語音合成。
    /// </summary>
    public void StopCurrentSpeechSynthesis()
    {
        if (synthesizer != null)
        {
            synthesizer.StopSpeakingAsync().Wait();
        }
    }

    /// <summary>
    /// 更新 Text UI 的內容。
    /// </summary>
    /// <param name="newText">新的文字內容</param>
    public void UpdateTextUI(string newText)
    {
        if (textUI != null)
        {
            textUI.text = newText;
        }
        else
        {
            Debug.LogError("Text UI 未設置！");
        }
    }

    private void OnDestroy()
    {
        synthesizer?.Dispose();
    }
}
