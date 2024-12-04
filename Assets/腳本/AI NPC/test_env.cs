using UnityEngine;
using System;
using CandyCoded.env;

/// <summary>
/// 測試環境變數是否正確設置並可以被讀取的腳本。
/// 此腳本會在 Unity Console 中輸出每個環境變數的值，若未正確設置則顯示錯誤。
/// </summary>
public class TestEnv : MonoBehaviour
{
    [Header("Environment Variable Names")]
    [Tooltip("OpenAI API 的環境變數名稱，用於測試是否正確設置")]
    public string openAiApiKeyName = "OPENAI_API_KEY";

    [Tooltip("Azure API 的環境變數名稱，用於測試是否正確設置")]
    public string azureApiKeyName = "AZURE_API_KEY";

    [Tooltip("Azure Region 的環境變數名稱，用於測試是否正確設置")]
    public string azureRegionName = "AZURE_REGION";

    /// <summary>
    /// Unity 的 Start 事件，在場景啟動時執行。
    /// 此方法測試各環境變數是否設置正確，並輸出相關訊息。
    /// </summary>
    void Start()
    {
        // 測試 OPENAI_API_KEY
        env.TryParseEnvironmentVariable(openAiApiKeyName, out string openAiApiKey);
        if (!string.IsNullOrEmpty(openAiApiKey))
        {
            Debug.Log($"環境變數 {openAiApiKeyName} 的值為：{openAiApiKey}");
        }
        else
        {
            Debug.LogError($"環境變數 {openAiApiKeyName} 未設置或為空！");
        }

        // 測試 AZURE_API_KEY
        env.TryParseEnvironmentVariable(azureApiKeyName, out string azureApiKey);
        if (!string.IsNullOrEmpty(azureApiKey))
        {
            Debug.Log($"環境變數 {azureApiKeyName} 的值為：{azureApiKey}");
        }
        else
        {
            Debug.LogError($"環境變數 {azureApiKeyName} 未設置或為空！");
        }

        // 測試 AZURE_REGION
        env.TryParseEnvironmentVariable(azureRegionName, out string azureRegion);
        if (!string.IsNullOrEmpty(azureRegion))
        {
            Debug.Log($"環境變數 {azureRegionName} 的值為：{azureRegion}");
        }
        else
        {
            Debug.LogError($"環境變數 {azureRegionName} 未設置或為空！");
        }
    }
}
