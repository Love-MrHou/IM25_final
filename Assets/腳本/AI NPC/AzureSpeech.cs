using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using CandyCoded.env;


/// <summary>
/// 使用 Azure 語音辨識的 Unity 腳本，支持 Android 和其他平台。
/// 包括環境變數初始化、按鈕交互邏輯及麥克風許可權處理。
/// </summary>
public class AzureSpeech : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("用於顯示語音辨識結果的文字元件")]
    public Text outputText;

    [Tooltip("用於啟動語音辨識的按鈕元件")]
    public Button startRecoButton;

    [Header("Environment Variable Names")]
    [Tooltip("Azure 語音 API 的環境變數名稱")]
    private string azureApiKeyName = "AZURE_API_KEY";

    [Tooltip("Azure 區域的環境變數名稱")]
    private string azureRegionName = "AZURE_REGION";

    private string azureApiKey; // 存儲從環境變數讀取的 Azure API 金鑰
    private string azureRegion; // 存儲從環境變數讀取的 Azure 區域
    private bool micPermissionGranted = false; // 是否授予麥克風許可權

    private object threadLocker = new object(); // 用於多執行緒同步的鎖
    private bool waitingForReco = false; // 是否正在等待語音辨識完成
    private string message = string.Empty; // 用於顯示的訊息

    void Start()
    {
        // 檢查並初始化環境變數
        if (env.TryParseEnvironmentVariable(azureApiKeyName, out azureApiKey) &&
            env.TryParseEnvironmentVariable(azureRegionName, out azureRegion))
        {
            Debug.Log($"成功讀取環境變數：{azureApiKeyName}={azureApiKey}, {azureRegionName}={azureRegion}");
        }
        else
        {
            Debug.LogError($"環境變數 {azureApiKeyName} 或 {azureRegionName} 未設置或為空！");
            return;
        }

        // 檢查 UI 元件是否綁定
        if (outputText == null)
        {
            Debug.LogError("outputText 未綁定，請在 Inspector 中設置對應的 UI Text 元件！");
            return;
        }

        if (startRecoButton == null)
        {
            Debug.LogError("startRecoButton 未綁定，請在 Inspector 中設置對應的 UI Button 元件！");
            return;
        }

        // 添加按鈕點擊事件
        startRecoButton.onClick.AddListener(ButtonClick);


    }

    public async void ButtonClick()
    {
        // 配置 Azure 語音辨識
        var config = SpeechConfig.FromSubscription(azureApiKey, azureRegion);
        config.SpeechRecognitionLanguage = "zh-TW";

        // 初始化語音辨識器
        using (var recognizer = new SpeechRecognizer(config))
        {
            lock (threadLocker)
            {
                waitingForReco = true; // 設置等待辨識狀態
            }

            // 執行語音辨識並獲取結果
            var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);
            string newMessage;

            switch (result.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    newMessage = $"{result.Text}";
                    break;
                case ResultReason.NoMatch:
                    newMessage = "未能辨識語音";
                    break;
                case ResultReason.Canceled:
                    var cancellation = CancellationDetails.FromResult(result);
                    newMessage = $"辨識取消：原因={cancellation.Reason}, 錯誤={cancellation.ErrorDetails}";
                    break;
                default:
                    newMessage = "未知錯誤發生";
                    break;
            }

            lock (threadLocker)
            {
                message = newMessage; // 更新顯示訊息
                waitingForReco = false; // 辨識結束
            }
        }
    }

    void Update()
    {

        lock (threadLocker)
        {
            // 更新 UI
            if (startRecoButton != null)
            {
                startRecoButton.interactable = !waitingForReco && micPermissionGranted;
            }
            if (outputText != null)
            {
                outputText.text = message;
            }
        }
    }
}
