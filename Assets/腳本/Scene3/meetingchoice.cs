using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class meetingchoice : MonoBehaviour
{
    public Text systemAnalystText;
    public Button systemAnalystButton1;
    public Button systemAnalystButton2;
    public Button systemAnalystButton3;
    public Button systemAnalystNextButton;

    public Text engineerText;
    public Button engineerButton1;
    public Button engineerButton2;
    public Button engineerButton3;
    public Button engineerNextButton;

    public Text designerText;
    public Button designerButton1;
    public Button designerButton2;
    public Button designerButton3;
    public Button designerNextButton;

    public GameObject systemAnalystPrefab;
    public GameObject engineerPrefab;
    public GameObject designerPrefab;

    public TextAsset _inkAssets;
    private List<string> dialogHistory = new List<string>();
    private int currentDialogIndex = -1;
    Story story = null;

    void Start()
    {
        HideAllDialogues();
        StartDialog(_inkAssets);
        if (story.canContinue)
        {
            string nextLine = story.Continue();
            if (!string.IsNullOrWhiteSpace(nextLine))
            {
                dialogHistory.Add(nextLine);
                currentDialogIndex = dialogHistory.Count - 1;
                ShowDialogue(nextLine);
            }
            else
            {
                Debug.LogWarning("Empty line detected in dialog. Skipping...");
                NextDialog();
            }
        }

        systemAnalystNextButton.onClick.AddListener(NextDialog);
        engineerNextButton.onClick.AddListener(NextDialog);
        designerNextButton.onClick.AddListener(NextDialog);
    }

    public bool StartDialog(TextAsset inkAssets)
    {
        if (story != null) return false;
        story = new Story(inkAssets.text);
        return true;
    }

    public void NextDialog()
    {
        if (story == null) return;

        if (!story.canContinue && story.currentChoices.Count == 0)
        {
            Debug.Log("Dialog End");
            story = null;
            return;
        }

        while (story.canContinue)
        {
            string nextLine = story.Continue();

            if (!string.IsNullOrWhiteSpace(nextLine))
            {
                dialogHistory.Add(nextLine);
                currentDialogIndex++;
                ShowDialogue(nextLine);
                break;
            }
            else
            {
                Debug.LogWarning("Empty line detected in dialog. Skipping...");
            }
        }

        if (story.currentChoices.Count > 0)
        {
            SetChoices();
        }
    }

    private void SetChoices()
    {
        systemAnalystButton1.gameObject.SetActive(false);
        systemAnalystButton2.gameObject.SetActive(false);
        systemAnalystButton3.gameObject.SetActive(false);

        engineerButton1.gameObject.SetActive(false);
        engineerButton2.gameObject.SetActive(false);
        engineerButton3.gameObject.SetActive(false);

        designerButton1.gameObject.SetActive(false);
        designerButton2.gameObject.SetActive(false);
        designerButton3.gameObject.SetActive(false);

        for (int i = 0; i < story.currentChoices.Count; i++)
        {
            var choiceText = story.currentChoices[i].text;

            int choiceIndex = i;

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

        var buttonText = buttonToUse.GetComponentInChildren<Text>();
        buttonText.text = choiceText;


        buttonToUse.gameObject.SetActive(true);
        buttonToUse.onClick.RemoveAllListeners();
        buttonToUse.onClick.AddListener(() => MakeChoice(choiceIndex));
    }

    public void MakeChoice(int index)
    {
        if (index >= 0 && index < story.currentChoices.Count)
        {
            story.ChooseChoiceIndex(index);
            HideAllDialogues();
            NextDialog();
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

    private void ShowDialogue(string nextLine)
    {
        HideAllDialogues();

        string currentSpeaker = GetCurrentSpeaker();
        switch (currentSpeaker)
        {
            case "系統分析師":
                systemAnalystText.text = nextLine;
                systemAnalystPrefab.SetActive(true);
                systemAnalystNextButton.gameObject.SetActive(true);
                break;
            case "工程師":
                engineerText.text = nextLine;
                engineerPrefab.SetActive(true);
                engineerNextButton.gameObject.SetActive(true);
                break;
            case "設計師":
                designerText.text = nextLine;
                designerPrefab.SetActive(true);
                designerNextButton.gameObject.SetActive(true);
                break;
            default:
                Debug.LogWarning("未知的角色標註: " + currentSpeaker);
                break;
        }
    }

    private string GetCurrentSpeaker()
    {
        if (story.currentTags.Count > 0)
        {
            return story.currentTags[0];
        }
        return string.Empty;
    }

    private void HideAllDialogues()
    {
        systemAnalystPrefab.SetActive(false);
        engineerPrefab.SetActive(false);
        designerPrefab.SetActive(false);

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
