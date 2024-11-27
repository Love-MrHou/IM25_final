using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class meetingchoice : MonoBehaviour
{
    // 每個角色對話框的Text和三個Button，以及一個"下一步"按鈕
    public Text systemAnalystText;
    public Button systemAnalystButton1;
    public Button systemAnalystButton2;
    public Button systemAnalystButton3;
    public Button systemAnalystNextButton; // 系統分析師的下一步按鈕

    public Text engineerText;
    public Button engineerButton1;
    public Button engineerButton2;
    public Button engineerButton3;
    public Button engineerNextButton; // 工程師的下一步按鈕

    public Text designerText;
    public Button designerButton1;
    public Button designerButton2;
    public Button designerButton3;
    public Button designerNextButton; // 設計師的下一步按鈕

    public GameObject systemAnalystPrefab; // 系統分析師的Prefab
    public GameObject engineerPrefab; // 工程師的Prefab
    public GameObject designerPrefab; // 設計師的Prefab

    public TextAsset _inkAssets; // Inky 劇本
    private List<string> dialogHistory = new List<string>(); // 儲存對話紀錄
    private int currentDialogIndex = -1; // 目前的劇情index
    Story story = null;

    void Start()
    {
        HideAllDialogues(); // 初始化時隱藏所有角色的對話框
        StartDialog(_inkAssets);
        if (story.canContinue)
        {
            string nextLine = story.Continue();
            if (!string.IsNullOrWhiteSpace(nextLine))
            {
                dialogHistory.Add(nextLine); // 將對話添加到歷史
                currentDialogIndex = dialogHistory.Count - 1; // 更新當前對話索引
                ShowDialogue(nextLine); // 顯示對話內容並更新UI
            }
            else
            {
                Debug.LogWarning("Empty line detected in dialog. Skipping...");
                NextDialog(); // 遇到空白時自動跳過
            }
        }

        // 綁定“下一步”按鈕的事件
        systemAnalystNextButton.onClick.AddListener(NextDialog);
        engineerNextButton.onClick.AddListener(NextDialog);
        designerNextButton.onClick.AddListener(NextDialog);
    }

    public bool StartDialog(TextAsset inkAssets)
    {
        if (story != null) return false;
        story = new Story(inkAssets.text); // 初始化Story
        return true;
    }

    public void NextDialog()
    {
        if (story == null) return;

        // 檢查故事是否已經結束並且沒有可供選擇的選項
        if (!story.canContinue && story.currentChoices.Count == 0)
        {
            Debug.Log("Dialog End");
            story = null;
            return;
        }

        // 如果還有對話可以繼續
        while (story.canContinue)
        {
            string nextLine = story.Continue();

            // 檢查是否為空白行或空字串
            if (!string.IsNullOrWhiteSpace(nextLine))
            {
                dialogHistory.Add(nextLine); // 將對話添加到歷史
                currentDialogIndex++;
                ShowDialogue(nextLine); // 顯示對話內容並更新UI
                break; // 結束循環，顯示此行對話
            }
            else
            {
                Debug.LogWarning("Empty line detected in dialog. Skipping...");
            }
        }

        // 顯示選項按鈕（如果有選項）
        if (story.currentChoices.Count > 0)
        {
            SetChoices();
        }
    }

    private void SetChoices()
    {
        // 每次顯示選項之前，隱藏所有按鈕
        systemAnalystButton1.gameObject.SetActive(false);
        systemAnalystButton2.gameObject.SetActive(false);
        systemAnalystButton3.gameObject.SetActive(false);

        engineerButton1.gameObject.SetActive(false);
        engineerButton2.gameObject.SetActive(false);
        engineerButton3.gameObject.SetActive(false);

        designerButton1.gameObject.SetActive(false);
        designerButton2.gameObject.SetActive(false);
        designerButton3.gameObject.SetActive(false);

        // 根據選項數量，顯示並設置對應按鈕
        for (int i = 0; i < story.currentChoices.Count; i++)
        {
            var choiceText = story.currentChoices[i].text;

            // 捕捉當前索引到局部變數，避免閉包問題
            int choiceIndex = i;

            // 根據角色顯示對應的按鈕
            switch (GetCurrentSpeaker())
            {
                case "系統分析師":
                    ShowChoiceButton(systemAnalystButton1, systemAnalystButton2, systemAnalystButton3, choiceText, choiceIndex);
                    break;
                case "工程師":
                    ShowChoiceButton(engineerButton1, engineerButton2, engineerButton3, choiceText, choiceIndex);
                    break;
                case "設計師":
                    ShowChoiceButton(designerButton1, designerButton2, designerButton3, choiceText, choiceIndex);
                    break;
            }
        }
    }

    private void ShowChoiceButton(Button button1, Button button2, Button button3, string choiceText, int choiceIndex)
    {
        Button buttonToUse = choiceIndex switch
        {
            0 => button1,
            1 => button2,
            _ => button3
        };

        buttonToUse.GetComponentInChildren<Text>().text = choiceText;
        buttonToUse.gameObject.SetActive(true);
        buttonToUse.onClick.RemoveAllListeners();
        buttonToUse.onClick.AddListener(() => MakeChoice(choiceIndex));
    }

    public void MakeChoice(int index)
    {
        // 檢查選項索引是否在範圍內
        if (index >= 0 && index < story.currentChoices.Count)
        {
            story.ChooseChoiceIndex(index); // 選擇對應的選項
            HideAllDialogues(); // 隱藏所有角色的對話框
            NextDialog(); // 顯示下一段對話
        }
        else
        {
            Debug.LogError("選項索引超出範圍：" + index);
        }
    }

    public void back()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // 顯示當前角色的對話框和內容，並顯示對應的下一步按鈕
    private void ShowDialogue(string nextLine)
    {
        HideAllDialogues(); // 每次顯示前先隱藏所有角色的對話框

        string currentSpeaker = GetCurrentSpeaker(); // 取得當前講話角色1
        switch (currentSpeaker)
        {
            case "系統分析師":
                systemAnalystText.text = nextLine;
                systemAnalystPrefab.SetActive(true); // 顯示系統分析師的Prefab
                systemAnalystNextButton.gameObject.SetActive(true); // 顯示系統分析師的下一步按鈕
                break;
            case "工程師":
                engineerText.text = nextLine;
                engineerPrefab.SetActive(true); // 顯示工程師的Prefab
                engineerNextButton.gameObject.SetActive(true); // 顯示工程師的下一步按鈕
                break;
            case "設計師":
                designerText.text = nextLine;
                designerPrefab.SetActive(true); // 顯示設計師的Prefab
                designerNextButton.gameObject.SetActive(true); // 顯示設計師的下一步按鈕
                break;
            default:
                Debug.LogWarning("未知的角色標註: " + currentSpeaker);
                break;
        }
    }

    // 根據當前對話段落的標註來取得講話的角色
    private string GetCurrentSpeaker()
    {
        if (story.currentTags.Count > 0)
        {
            return story.currentTags[0]; // 假設角色標註為第一個tag
        }
        return string.Empty;
    }

    // 隱藏所有角色的對話框和按鈕
    // 隱藏所有角色的對話框和按鈕
    private void HideAllDialogues()
    {
        systemAnalystPrefab.SetActive(false);
        engineerPrefab.SetActive(false);
        designerPrefab.SetActive(false);

        // 隱藏按鈕
        systemAnalystButton1.gameObject.SetActive(false);
        systemAnalystButton2.gameObject.SetActive(false);
        systemAnalystButton3.gameObject.SetActive(false);
        systemAnalystNextButton.gameObject.SetActive(false);

        engineerButton1.gameObject.SetActive(false);
        engineerButton2.gameObject.SetActive(false);
        engineerButton3.gameObject.SetActive(false);
        engineerNextButton.gameObject.SetActive(false);

        designerButton1.gameObject.SetActive(false);
        designerButton2.gameObject.SetActive(false);
        designerButton3.gameObject.SetActive(false);
        designerNextButton.gameObject.SetActive(false);
    }

}
