using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using UnityEngine;
using UnityEngine.UI;
public class TTSDemo : MonoBehaviour
{
    public AudioSource audioSource;
    public Text textUI;  // 用來顯示和儲存要轉換成語音的文字

    private string previousText = "";
    private string currentText = ""; // 用來儲存目前在Text UI中要轉換的文字
    private SpeechSynthesizer synthesizer;
    public string speechSynthesisVoiceName;

    void Start()
    {
        if (textUI == null)
        {
            Debug.LogError("Text UI is not assigned! Please assign a Text UI element.");
        }

        // 使用 PullAudioOutputStream 來控制音頻資料，不再直接播放
        var config = SpeechConfig.FromSubscription("155998f0555f47ae9ad78430ef6491aa", "eastus");
        config.SpeechSynthesisLanguage = "zh-CN";
        config.SpeechSynthesisVoiceName = speechSynthesisVoiceName; // "zh-TW-HsiaoChenNeural"

        var audioConfig = AudioConfig.FromStreamOutput(new PullAudioOutputStream());
        synthesizer = new SpeechSynthesizer(config, audioConfig);
    }

    void Update()
    {
        // 如果Text UI中的文字和之前的文字不同，則進行語音合成
        if (textUI != null && textUI.text != previousText)
        {
            currentText = textUI.text; // 將Text UI中的文字更新到currentText

            // 如果當前語音正在播放，停止並中斷
            if (audioSource.isPlaying)
            {
                audioSource.Stop(); // 停止目前正在播放的音頻
            }

            // 如果正在進行語音合成，停止合成
            StopCurrentSpeechSynthesis();

            // 重新合成並播放新的語音
            SynthesizeAndPlayText(currentText);
            previousText = currentText; // 更新previousText
        }
    }

    public async void SynthesizeAndPlayText(string text)
    {
        var result = await synthesizer.SpeakTextAsync(text); // 合成Text UI中的文字

        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            var sampleCount = result.AudioData.Length / 2;
            var audioData = new float[sampleCount];
            for (var i = 0; i < sampleCount; ++i)
            {
                audioData[i] = (short)(result.AudioData[i * 2 + 1] << 8 | result.AudioData[i * 2]) / 32768.0F;
            }

            var audioClip = AudioClip.Create("SynthesizedAudio", sampleCount, 1, 16000, false);
            audioClip.SetData(audioData, 0);
            audioSource.clip = audioClip;
            audioSource.Play();

            Debug.Log("語音合成成功！");
        }
        else if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
            Debug.LogError(cancellation.ErrorDetails);
        }
    }

    // 停止當前語音合成的函數
    public void StopCurrentSpeechSynthesis()
    {
        if (synthesizer != null)
        {
            synthesizer.StopSpeakingAsync().Wait(); // 停止目前的語音合成
        }
    }

    // 用來更新Text UI中的文字的函數，當外部需要更新時可以調用此函數
    public void UpdateTextUI(string newText)
    {
        if (textUI != null)
        {
            textUI.text = newText; // 將Text UI中的文字更新
        }
        else
        {
            Debug.LogError("Text UI is not assigned.");
        }
    }
}
