using UnityEngine;
using UnityEngine.SceneManagement; // 引入場景管理

public class SceneChangeButton : MonoBehaviour
{
    // 設置要加載的場景名稱
    public string sceneName;
    public GameObject ChangeButton;

    void Start()
    {

    }

    // 當按下按鈕時調用此方法
    public void ChangeSceneButton()
    {
        // 檢查是否激活
        if (ChangeButton.activeSelf)
        {
            // 加載指定的場景
            SceneManager.LoadScene(sceneName);
        }
    }
}
