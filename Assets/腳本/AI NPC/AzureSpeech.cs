using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using CandyCoded.env;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

/// <summary>
/// �ϥ� Azure �y�����Ѫ� Unity �}���A��� Android �M��L���x�C
/// �]�A�����ܼƪ�l�ơB���s�椬�޿�γ��J���\�i�v�B�z�C
/// </summary>
public class AzureSpeech : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("�Ω���ܻy�����ѵ��G����r����")]
    public Text outputText;

    [Tooltip("�Ω�Ұʻy�����Ѫ����s����")]
    public Button startRecoButton;

    [Header("Environment Variable Names")]
    [Tooltip("Azure �y�� API �������ܼƦW��")]
    private string azureApiKeyName = "AZURE_API_KEY";

    [Tooltip("Azure �ϰ쪺�����ܼƦW��")]
    private string azureRegionName = "AZURE_REGION";

    private string azureApiKey; // �s�x�q�����ܼ�Ū���� Azure API ���_
    private string azureRegion; // �s�x�q�����ܼ�Ū���� Azure �ϰ�
    private bool micPermissionGranted = false; // �O�_�¤����J���\�i�v

    private object threadLocker = new object(); // �Ω�h������P�B����
    private bool waitingForReco = false; // �O�_���b���ݻy�����ѧ���
    private string message = string.Empty; // �Ω���ܪ��T��

    void Start()
    {
        // �ˬd�ê�l�������ܼ�
        if (env.TryParseEnvironmentVariable(azureApiKeyName, out azureApiKey) &&
            env.TryParseEnvironmentVariable(azureRegionName, out azureRegion))
        {
            Debug.Log($"���\Ū�������ܼơG{azureApiKeyName}={azureApiKey}, {azureRegionName}={azureRegion}");
        }
        else
        {
            Debug.LogError($"�����ܼ� {azureApiKeyName} �� {azureRegionName} ���]�m�ά��šI");
            return;
        }

        // �ˬd UI ����O�_�j�w
        if (outputText == null)
        {
            Debug.LogError("outputText ���j�w�A�Цb Inspector ���]�m������ UI Text ����I");
            return;
        }

        if (startRecoButton == null)
        {
            Debug.LogError("startRecoButton ���j�w�A�Цb Inspector ���]�m������ UI Button ����I");
            return;
        }

        // �K�[���s�I���ƥ�
        startRecoButton.onClick.AddListener(ButtonClick);

#if PLATFORM_ANDROID
        // �ШD���J���\�i�v
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
            message = "���b�ШD���J���\�i�v...";
        }
#else
        micPermissionGranted = true;
        message = "�I�����s�}�l�y������";
#endif
    }

    public async void ButtonClick()
    {
        // �t�m Azure �y������
        var config = SpeechConfig.FromSubscription(azureApiKey, azureRegion);
        config.SpeechRecognitionLanguage = "zh-TW";

        // ��l�ƻy�����Ѿ�
        using (var recognizer = new SpeechRecognizer(config))
        {
            lock (threadLocker)
            {
                waitingForReco = true; // �]�m���ݿ��Ѫ��A
            }

            // ����y�����Ѩ�������G
            var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);
            string newMessage;

            switch (result.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    newMessage = $"{result.Text}";
                    break;
                case ResultReason.NoMatch:
                    newMessage = "������ѻy��";
                    break;
                case ResultReason.Canceled:
                    var cancellation = CancellationDetails.FromResult(result);
                    newMessage = $"���Ѩ����G��]={cancellation.Reason}, ���~={cancellation.ErrorDetails}";
                    break;
                default:
                    newMessage = "�������~�o��";
                    break;
            }

            lock (threadLocker)
            {
                message = newMessage; // ��s��ܰT��
                waitingForReco = false; // ���ѵ���
            }
        }
    }

    void Update()
    {
#if PLATFORM_ANDROID
        // ��s���J���\�i�v���A
        if (!micPermissionGranted && Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            micPermissionGranted = true;
            message = "�I�����s�}�l�y������";
        }
#endif

        lock (threadLocker)
        {
            // ��s UI
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
