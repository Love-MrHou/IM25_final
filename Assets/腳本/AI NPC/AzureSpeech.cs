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

        using (var recognizer = new SpeechRecognizer(config)) // �ϥΫ��w���t�m��l�ƻy�����Ѿ�
        {
            lock (threadLocker) // ��w�h��������@�ɸ귽�A�T�O������w��
            {
                waitingForReco = true; // �]�m���ݻy�����Ѫ����A�� true
            }

            // �եλy�����Ѿ��i��@���y�����Ѿާ@�A�õ��ݵ��G
            var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

            // �ˬd�y�����ѵ��G
            string newMessage = string.Empty; // �Ω�s�x���G�T��
            if (result.Reason == ResultReason.RecognizedSpeech) // �p�G���\���ѻy��
            {
                newMessage = result.Text; // �������Ѩ쪺�y����r
            }
            else if (result.Reason == ResultReason.NoMatch) // �p�G�y�����Q���\����
            {
                newMessage = "NOMATCH: Speech could not be recognized."; // ���ܵL�k���ѻy��
            }
            else if (result.Reason == ResultReason.Canceled) // �p�G�y�����ѳQ����
            {
                var cancellation = CancellationDetails.FromResult(result); // �����������ԲӸ�T
                newMessage = $"CANCELED: Reason={cancellation.Reason} ErrorDetails={cancellation.ErrorDetails}"; // �O��������]�M���~�Ա�
            }

            // �A����w�h��������@�ɸ귽�A��s�T���M���A
            lock (threadLocker)
            {
                message = newMessage; // ��s����T���ܼơA�N���G�s�J
                waitingForReco = false; // �]�m���ݻy�����Ѫ����A�� false�A��ܤw����
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