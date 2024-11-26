using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangerButton : MonoBehaviour
{
    public string[] sceneName; // 場景名稱的陣列

    public void ChangeSceneButton(GameObject buttonObject)
    {
        Text btnText = buttonObject.GetComponentInChildren<Text>();

        if (btnText != null)
        {
            string text = btnText.text;
            Debug.Log($"按鈕文字內容: {text}");

            int sceneIndex = -1; // 預設值為 -1，表示未找到匹配

            // 根據按鈕文字設置場景索引
            switch (text)
            {
                case "進行會議模擬" or "會議模擬":
                    sceneIndex = 0;
                    break;

                case "返回大廳" or "":
                    sceneIndex = 1;
                    break;

                case "進行教育訓練":
                    sceneIndex = 2;
                    break;

                default:
                    Debug.LogWarning($"未定義的按鈕文字: {text}");
                    break;
            }

            // 加載場景前進行檢查
            if (sceneIndex >= 0 && sceneIndex < sceneName.Length)
            {
                Debug.Log($"載入場景: {sceneName[sceneIndex]}");
                SceneManager.LoadScene(sceneName[sceneIndex]);
            }
            else
            {
                Debug.Log($"場景索引 {sceneIndex} 超出陣列範圍或無效。請檢查 sceneName 設置。");
            }
        }
        else
        {
            Debug.LogError("按鈕內沒有找到文字組件！");
        }
    }
}
