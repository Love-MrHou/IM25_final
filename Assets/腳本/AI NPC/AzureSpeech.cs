using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class AzureSpeech : MonoBehaviour
{
    // Hook up the two properties below with a Text and Button object in your UI.
    public Text outputText;
    public Button startRecoButton;

    private object threadLocker = new object();
    private bool waitingForReco;
    private string message;

    private bool micPermissionGranted = false;

#if PLATFORM_ANDROID
    // Required to manifest microphone permission, cf.
    // https://docs.unity3d.com/Manual/android-manifest.html
    private Microphone mic;
#endif

    public async void ButtonClick()
    {
        var config = SpeechConfig.FromSubscription("155998f0555f47ae9ad78430ef6491aa", "eastus");
        config.SpeechRecognitionLanguage = "zh-TW";

        using (var recognizer = new SpeechRecognizer(config)) // 使用指定的配置初始化語音辨識器
        {
            lock (threadLocker) // 鎖定多執行緒的共享資源，確保執行緒安全
            {
                waitingForReco = true; // 設置等待語音辨識的狀態為 true
            }

            // 調用語音辨識器進行一次語音辨識操作，並等待結果
            var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

            // 檢查語音辨識結果
            string newMessage = string.Empty; // 用於存儲結果訊息
            if (result.Reason == ResultReason.RecognizedSpeech) // 如果成功辨識語音
            {
                newMessage = result.Text; // 提取辨識到的語音文字
            }
            else if (result.Reason == ResultReason.NoMatch) // 如果語音未被成功辨識
            {
                newMessage = "NOMATCH: Speech could not be recognized."; // 提示無法辨識語音
            }
            else if (result.Reason == ResultReason.Canceled) // 如果語音辨識被取消
            {
                var cancellation = CancellationDetails.FromResult(result); // 提取取消的詳細資訊
                newMessage = $"CANCELED: Reason={cancellation.Reason} ErrorDetails={cancellation.ErrorDetails}"; // 記錄取消原因和錯誤詳情
            }

            // 再次鎖定多執行緒的共享資源，更新訊息和狀態
            lock (threadLocker)
            {
                message = newMessage; // 更新全域訊息變數，將結果存入
                waitingForReco = false; // 設置等待語音辨識的狀態為 false，表示已完成
            }
        }
    }

    void Start()
    {
        if (outputText == null)
        {
            UnityEngine.Debug.LogError("outputText property is null! Assign a UI Text element to it.");
        }
        else if (startRecoButton == null)
        {
            message = "startRecoButton property is null! Assign a UI Button to it.";
            UnityEngine.Debug.LogError(message);
        }
        else
        {
            // Continue with normal initialization, Text and Button objects are present.

#if PLATFORM_ANDROID
            // Request to use the microphone, cf.
            // https://docs.unity3d.com/Manual/android-RequestingPermissions.html
            message = "Waiting for mic permission";
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
#else
            micPermissionGranted = true;
            //message = "Click button to recognize speech";
#endif
            startRecoButton.onClick.AddListener(ButtonClick);
        }
    }

    void Update()
    {
#if PLATFORM_ANDROID
        if (!micPermissionGranted && Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            micPermissionGranted = true;
            message = "Click button to recognize speech";
        }
#endif

        lock (threadLocker)
        {
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