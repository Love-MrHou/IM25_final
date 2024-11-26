using Ink.Runtime;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class DialogManger : MonoBehaviour
{
    void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
        StartDialog(_inkAssets);
        if (story.canContinue)
        {
            string nextLine = story.Continue();
            if (!string.IsNullOrWhiteSpace(nextLine))
            {
                dialogHistory.Add(nextLine); // 將對話添加到歷史
                currentDialogIndex = dialogHistory.Count - 1; // 更新當前對話索引
                diaglogText.text = nextLine; // 確保對話框不顯示空白
            }
            else
            {
                Debug.LogWarning("Empty line detected in dialog. Skipping...");
                NextDialog(); // 遇到空白時自動跳過
            }
        }
    }
    public Text diaglogText;//顯示劇情的text
    public Button[] buttons;//選項的按鈕
    public TextAsset _inkAssets;//inky劇本
    private List<string> dialogHistory = new List<string>();//儲存對話紀錄
    private int currentDialogIndex = -1;//目前的劇情index
    Story story = null;
    public bool StartDialog(TextAsset inkAssets)
    {
        if (story != null) return false;
        story = new Story(inkAssets.text); //new Story 裡面放json檔的文字，讓 Story 初始化
        return true;
    }
    public void ShowPreviousDialog()
    {
        if (currentDialogIndex > 0)// 確保不會超出範圍
        {
            currentDialogIndex--;
            diaglogText.text = dialogHistory[currentDialogIndex]; // 顯示上一句對話

            // 隱藏所有選項按鈕
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("已經是第一句對話了。");
        }

        Debug.Log(dialogHistory[currentDialogIndex]);
    }

    public void NextDialog()
    {
        if (story == null) return;

        // 如果故事不能繼續並且沒有選項，則表示對話已經結束
        if (!story.canContinue && story.currentChoices.Count == 0)
        {
            Debug.Log("Dialog End");
            story = null;
            return;
        }

        // 如果用戶正在查看歷史對話
        if (currentDialogIndex < dialogHistory.Count - 1)
        {
            // 移動到下一句對話
            currentDialogIndex++;
            diaglogText.text = dialogHistory[currentDialogIndex];

            // 如果下一句對話對應的歷史對話中不包含選項，隱藏選項按鈕
            if (story.currentChoices.Count > 0 && currentDialogIndex == dialogHistory.Count - 1)
            {
                SetChoices(); // 顯示選項
            }
            else
            {
                // 隱藏所有按鈕
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].gameObject.SetActive(false);
                }
            }
        }
        else if (story.canContinue)
        {
            // 如果歷史到達最後，繼續下一句故事
            string nextLine = story.Continue();
            if (!string.IsNullOrWhiteSpace(nextLine))
            {
                dialogHistory.Add(nextLine);
                currentDialogIndex++; // 移動到下一句對話
                diaglogText.text = nextLine; // 顯示對話
            }
            else
            {
                Debug.LogWarning("Empty line detected in dialog. Skipping...");
                NextDialog(); // 遇到空白時自動跳過
            }

            // 設定選項按鈕，如果有選項
            if (story.currentChoices.Count > 0)
            {
                SetChoices();
            }
        }

        Debug.Log(dialogHistory[currentDialogIndex]);
    }
    private void SetChoices()
    {
        for (int i = 0; i < story.currentChoices.Count; i++)
        {
            buttons[i].gameObject.SetActive(true);
            buttons[i].GetComponentInChildren<Text>().text = story.currentChoices[i].text;

        }
    }

    public void MakeChoice(int index)
    {
        story.ChooseChoiceIndex(index); // 使用 ChooseChoiceIndex 選擇當前選項
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false); // 隱藏選項按鈕
        }
        NextDialog();
    }
    public void back()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
