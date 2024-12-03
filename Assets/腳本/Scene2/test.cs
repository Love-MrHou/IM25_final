using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyProject.Dialogs
{
    public class Test : MonoBehaviour
    {
        public Text dialogText;
        public Button[] buttons;
        public TextAsset[] inkAssets;
        public GameObject disappearui;

        public GameObject characterDialogPrefab;
        public GameObject vendorIntroPrefab;

        public GameObject greenVitalPrefab;
        public GameObject elegancePrefab;
        public GameObject ecoEssentialsPrefab;

        public Text greenVitalText;
        public Text eleganceText;
        public Text ecoEssentialsText;

        public Button[] greenVitalButtons;
        public Button[] eleganceButtons;
        public Button[] ecoEssentialsButtons;

        private Story story = null;
        private List<string> dialogHistory = new List<string>();
        private int currentDialogIndex = -1;
        private int currentInkAssetIndex = 0;

        public delegate void OnDialogUpdateHandler(Story story);
        public event OnDialogUpdateHandler OnDialogUpdate;

        void Start()
        {
            // 初始化隱藏所有按鈕
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }

            // 隱藏所有其他UI，僅顯示主要對話框
            HideAllPrefabsAndTextsExceptCharacterDialog();

            if (inkAssets.Length > 0)
            {
                StartDialog(inkAssets[currentInkAssetIndex]);
            }
        }

        public bool StartDialog(TextAsset inkAsset)
        {
            if (story != null)
            {
                story = null;
            }

            story = new Story(inkAsset.text);
            dialogHistory.Clear();
            currentDialogIndex = -1;
            NextDialog();
            return true;
        }

        public void NextDialog()
        {
            if (story == null) return;

            if (!story.canContinue && story.currentChoices.Count == 0)
            {
                Debug.Log("劇本結束。");

                currentInkAssetIndex++;
                if (currentInkAssetIndex < inkAssets.Length)
                {
                    disappearui.SetActive(false);
                    StartDialog(inkAssets[currentInkAssetIndex]);
                    return;
                }

                Debug.Log("所有劇本已完成。");
                return;
            }

            if (story.canContinue)
            {
                string nextLine = story.Continue();
                if (!string.IsNullOrWhiteSpace(nextLine))
                {
                    dialogHistory.Add(nextLine);
                    currentDialogIndex = dialogHistory.Count - 1;
                    dialogText.text = nextLine;
                }
                else
                {
                    NextDialog(); // 跳過空白對話
                    return;
                }

                if (story.currentTags.Contains("GreenVital Foods代表"))
                {
                    ShowDialog(greenVitalPrefab, greenVitalText, nextLine, greenVitalButtons);
                }
                else if (story.currentTags.Contains("Elegance Accessories代表"))
                {
                    ShowDialog(elegancePrefab, eleganceText, nextLine, eleganceButtons);
                }
                else if (story.currentTags.Contains("EcoEssentials代表"))
                {
                    ShowDialog(ecoEssentialsPrefab, ecoEssentialsText, nextLine, ecoEssentialsButtons);
                }
                else if (story.currentTags.Contains("系統分析師1"))
                {
                    if (characterDialogPrefab != null)
                    {
                        characterDialogPrefab.SetActive(false);
                    }

                    if (vendorIntroPrefab != null)
                    {
                        vendorIntroPrefab.SetActive(true);
                    }
                }
                else
                {
                    ShowMainDialogButtons();
                }

                OnDialogUpdate?.Invoke(story);

                if (story.currentChoices.Count > 0)
                {
                    if (story.currentTags.Contains("GreenVital Foods代表"))
                    {
                        SetChoices(greenVitalButtons);
                    }
                    else if (story.currentTags.Contains("Elegance Accessories代表"))
                    {
                        SetChoices(eleganceButtons);
                    }
                    else if (story.currentTags.Contains("EcoEssentials代表"))
                    {
                        SetChoices(ecoEssentialsButtons);
                    }
                    else
                    {
                        SetChoices(buttons);
                    }
                }
                else
                {
                    HideMainDialogButtons();
                }
            }
        }

        private void HideMainDialogButtons()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        private void ShowMainDialogButtons()
        {
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                buttons[i].gameObject.SetActive(true);
                buttons[i].GetComponentInChildren<Text>().text = story.currentChoices[i].text;

                int choiceIndex = i;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => MakeChoice(choiceIndex));
            }

            for (int i = story.currentChoices.Count; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        private void SetChoices(Button[] buttons)
        {
            if (story.currentChoices.Count > buttons.Length)
            {
                Debug.LogError("選項數量超過了可用按鈕的數量。");
                return;
            }

            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                buttons[i].gameObject.SetActive(true);
                buttons[i].GetComponentInChildren<Text>().text = story.currentChoices[i].text;

                int choiceIndex = i;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => MakeChoice(choiceIndex));
            }

            for (int i = story.currentChoices.Count; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        public void MakeChoice(int index)
        {
            if (index >= 0 && index < story.currentChoices.Count)
            {
                story.ChooseChoiceIndex(index);

                bool isRepresentativeDialogActive =
                    greenVitalPrefab.activeSelf || elegancePrefab.activeSelf || ecoEssentialsPrefab.activeSelf;

                HideAllPrefabsAndTexts();

                if (characterDialogPrefab != null)
                {
                    characterDialogPrefab.SetActive(true);
                }

                NextDialog();

                if (isRepresentativeDialogActive && characterDialogPrefab.activeSelf)
                {
                    NextDialog();
                }
            }
            else
            {
                Debug.LogError("選項索引超出範圍");
            }
        }

        private void HideAllPrefabsAndTextsExceptCharacterDialog()
        {
            greenVitalPrefab.SetActive(false);
            elegancePrefab.SetActive(false);
            ecoEssentialsPrefab.SetActive(false);

            if (vendorIntroPrefab != null)
                vendorIntroPrefab.SetActive(false);

            greenVitalText.gameObject.SetActive(false);
            eleganceText.gameObject.SetActive(false);
            ecoEssentialsText.gameObject.SetActive(false);
        }

        private void HideAllPrefabsAndTexts()
        {
            greenVitalPrefab.SetActive(false);
            elegancePrefab.SetActive(false);
            ecoEssentialsPrefab.SetActive(false);

            if (characterDialogPrefab != null)
                characterDialogPrefab.SetActive(false);

            if (vendorIntroPrefab != null)
                vendorIntroPrefab.SetActive(false);

            greenVitalText.gameObject.SetActive(false);
            eleganceText.gameObject.SetActive(false);
            ecoEssentialsText.gameObject.SetActive(false);
        }

        private void ShowDialog(GameObject prefab, Text text, string nextLine, Button[] buttons)
        {
            HideMainDialogButtons();
            HideAllPrefabsAndTexts();

            prefab.SetActive(true);
            text.gameObject.SetActive(true);

            text.text = nextLine;

            SetChoices(buttons);
        }

        public void Back()
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}

