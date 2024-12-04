using UnityEngine;
using System;
using CandyCoded.env;

/// <summary>
/// ���������ܼƬO�_���T�]�m�åi�H�QŪ�����}���C
/// ���}���|�b Unity Console ����X�C�������ܼƪ��ȡA�Y�����T�]�m�h��ܿ��~�C
/// </summary>
public class TestEnv : MonoBehaviour
{
    [Header("Environment Variable Names")]
    [Tooltip("OpenAI API �������ܼƦW�١A�Ω���լO�_���T�]�m")]
    public string openAiApiKeyName = "OPENAI_API_KEY";

    [Tooltip("Azure API �������ܼƦW�١A�Ω���լO�_���T�]�m")]
    public string azureApiKeyName = "AZURE_API_KEY";

    [Tooltip("Azure Region �������ܼƦW�١A�Ω���լO�_���T�]�m")]
    public string azureRegionName = "AZURE_REGION";

    /// <summary>
    /// Unity �� Start �ƥ�A�b�����Ұʮɰ���C
    /// ����k���զU�����ܼƬO�_�]�m���T�A�ÿ�X�����T���C
    /// </summary>
    void Start()
    {
        // ���� OPENAI_API_KEY
        env.TryParseEnvironmentVariable(openAiApiKeyName, out string openAiApiKey);
        if (!string.IsNullOrEmpty(openAiApiKey))
        {
            Debug.Log($"�����ܼ� {openAiApiKeyName} ���Ȭ��G{openAiApiKey}");
        }
        else
        {
            Debug.LogError($"�����ܼ� {openAiApiKeyName} ���]�m�ά��šI");
        }

        // ���� AZURE_API_KEY
        env.TryParseEnvironmentVariable(azureApiKeyName, out string azureApiKey);
        if (!string.IsNullOrEmpty(azureApiKey))
        {
            Debug.Log($"�����ܼ� {azureApiKeyName} ���Ȭ��G{azureApiKey}");
        }
        else
        {
            Debug.LogError($"�����ܼ� {azureApiKeyName} ���]�m�ά��šI");
        }

        // ���� AZURE_REGION
        env.TryParseEnvironmentVariable(azureRegionName, out string azureRegion);
        if (!string.IsNullOrEmpty(azureRegion))
        {
            Debug.Log($"�����ܼ� {azureRegionName} ���Ȭ��G{azureRegion}");
        }
        else
        {
            Debug.LogError($"�����ܼ� {azureRegionName} ���]�m�ά��šI");
        }
    }
}
