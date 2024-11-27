using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShowDetailsSystemAnalysis : MonoBehaviour
{
    public Sprite[] sprites; //依序顯示的圖片
    public Image imageDisplay; //要顯示的image的位置
    public Button nextButton; //切換圖片的按鈕
    public Canvas currentCanvas; //顯示圖片的canva
    public Canvas additionalCanvas; //顯示三個ui的canva

    private int currentIndex = 0; //用來顯示目前圖片
    private int previousIndex = -1; //用來隱藏上一張圖片
    private int count = 1;
    void Start()
    {
        ShowSprite(currentIndex);
        nextButton.onClick.AddListener(OnButtonClick);
        additionalCanvas.gameObject.SetActive(false);
    }

    void OnButtonClick()
    {
        if (previousIndex != -1)
        {
            HideSprite(previousIndex); //隱藏上一張圖片
        }

        currentIndex++; //顯示的圖片索引值+1

        if (currentIndex < sprites.Length) //如果還有圖片要顯示
        {
            ShowSprite(currentIndex); //顯示現在的圖片
        }
        else
        {
            if (count >= 1)
            {
                additionalCanvas.gameObject.SetActive(true); // 把顯示社交軟體的canva顯示
                count--;
            }
            currentCanvas.gameObject.SetActive(false); // 把顯示圖片的canva隱藏
        }
    }

    void ShowSprite(int index)
    {
        imageDisplay.sprite = sprites[index]; //顯示現在的圖片
        imageDisplay.enabled = true; // enable = true
        previousIndex = index; // 把前一個圖片的索引值設為目前圖片的索引 用來下一張圖片顯示時隱藏
    }

    void HideSprite(int index)
    {
        imageDisplay.enabled = false;
    }
}
