using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyProject.Dialogs
{
    public class test : MonoBehaviour
    {
        public Text dialogText;  // 顯示對話的 Text
        public Button[] buttons;  // 主對話框的選項按鈕
        public TextAsset[] inkAssets;  // 多個 Inky 劇本的陣列（每個 NPC 可以有多個劇本）
        public GameObject disappearui;

        // 原先的 Prefab
        public GameObject characterDialogPrefab;  // "人物對話ui(強化版)" prefab
        public GameObject vendorIntroPrefab;  // "廠商背景介紹 (1)" prefab

        // 新增三個Prefab和三個Text
        public GameObject greenVitalPrefab;  // "GreenVital Foods" 的Prefab
        public GameObject elegancePrefab;    // "Elegance Accessories" 的Prefab
        public GameObject ecoEssentialsPrefab;  // "EcoEssentials" 的Prefab

        public Text greenVitalText;  // "GreenVital Foods" 對應的Text
        public Text eleganceText;    // "Elegance Accessories" 對應的Text
        public Text ecoEssentialsText;  // "EcoEssentials" 對應的Text

        // 為每個對話框新增一組按鈕
        public Button[] greenVitalButtons;   // GreenVital Foods 的選項按鈕
        public Button[] eleganceButtons;     // Elegance Accessories 的選項按鈕
        public Button[] ecoEssentialsButtons;  // EcoEssentials 的選項按鈕

        private Story story = null;
        private List<string> dialogHistory = new List<string>();  // 對話歷史紀錄
        private int currentDialogIndex = -1;  // 當前對話索引
        private int currentInkAssetIndex = 0;  // 當前劇本索引

        // 新增一個事件來通知對話更新
        public delegate void OnDialogUpdateHandler(Story story);
        public event OnDialogUpdateHandler OnDialogUpdate;

        void Start()
        {
            // 初始化隱藏所有按鈕
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }

            // 不要隱藏 characterDialogPrefab，保持它在一開始顯示
            HideAllPrefabsAndTextsExceptCharacterDialog();

            if (inkAssets.Length > 0)
            {
                // 開始第一個劇本
                StartDialog(inkAssets[currentInkAssetIndex]);
            }
        }

        public bool StartDialog(TextAsset inkAsset)
        {
            if (story != null)
            {
                story = null;  // 重置故事對象
            }

            story = new Story(inkAsset.text);  // 初始化新的 Story
            dialogHistory.Clear();  // 清空對話歷史
            currentDialogIndex = -1;  // 重置對話索引
            NextDialog();  // 顯示對話
            return true;
        }

        public void NextDialog()
        {
            if (story == null) return;

            // 如果對話已經結束，並且沒有可選擇的選項，嘗試切換到下一個劇本
            if (!story.canContinue && story.currentChoices.Count == 0)
            {
                Debug.Log("劇本結束。");

                // 切換到下一個劇本（如果有）
                currentInkAssetIndex++;
                if (currentInkAssetIndex < inkAssets.Length)
                {
                    disappearui.SetActive(false);
                    Debug.Log("切換到下一個劇本。");
                    StartDialog(inkAssets[currentInkAssetIndex]);  // 加載下一個劇本
                    return;
                }

                Debug.Log("所有劇本已完成。");
                return;
            }

            // 如果還有對話可以繼續
            if (story.canContinue)
            {
                string nextLine = story.Continue();
                if (!string.IsNullOrWhiteSpace(nextLine))
                {
                    dialogHistory.Add(nextLine);  // 將對話添加到歷史
                    currentDialogIndex = dialogHistory.Count - 1;
                    dialogText.text = nextLine;  // 顯示新的對話
                }

                // 偵測標籤並決定按鈕生成位置
                if (story.currentTags.Contains("GreenVital Foods代表"))
                {
                    // 隱藏主要對話框的按鈕，並在 GreenVital 的對話框生成選項
                    ShowDialog(greenVitalPrefab, greenVitalText, nextLine, greenVitalButtons);
                }
                else if (story.currentTags.Contains("Elegance Accessories代表"))
                {
                    // 隱藏主要對話框的按鈕，並在 Elegance 的對話框生成選項
                    ShowDialog(elegancePrefab, eleganceText, nextLine, eleganceButtons);
                }
                else if (story.currentTags.Contains("EcoEssentials代表"))
                {
                    // 隱藏主要對話框的按鈕，並在 EcoEssentials 的對話框生成選項
                    ShowDialog(ecoEssentialsPrefab, ecoEssentialsText, nextLine, ecoEssentialsButtons);
                }
                else if (story.currentTags.Contains("系統分析師1"))
                {
                    // 隱藏主要對話框，顯示 vendorIntroPrefab
                    if (characterDialogPrefab != null)
                    {
                        characterDialogPrefab.SetActive(false);
                    }

                    if (vendorIntroPrefab != null)
                    {
                        vendorIntroPrefab.SetActive(true);  // 顯示 "廠商背景介紹 (1)"
                    }
                }
                else
                {
                    // 沒有代表標籤，則在主要對話框生成選項
                    ShowMainDialogButtons();
                }

                // 發送通知，讓其他系統知道對話更新了
                OnDialogUpdate?.Invoke(story);

                // 在有選項的情況下，生成按鈕
                if (story.currentChoices.Count > 0)
                {
                    // 如果有代表標籤，生成對應代表對話框的選項按鈕
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
                        // 沒有代表標籤時，生成主要對話框的選項按鈕
                        SetChoices(buttons);
                    }
                }
                else
                {
                    // 沒有選項時隱藏按鈕
                    HideMainDialogButtons();
                }
            }
        }



        private void HideMainDialogButtons()
        {
            // 隱藏主要對話框的所有選項按鈕
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        private void ShowMainDialogButtons()
        {
            // 顯示主要對話框的選項按鈕，並設置選項文本
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                buttons[i].gameObject.SetActive(true);
                buttons[i].GetComponentInChildren<Text>().text = story.currentChoices[i].text;

                // 使用局部變數捕捉選項索引，避免閉包問題
                int choiceIndex = i;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => MakeChoice(choiceIndex));
            }

            // 隱藏多餘的按鈕
            for (int i = story.currentChoices.Count; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        private void SetChoices(Button[] buttons)
        {
            // 確保 story.currentChoices 的數量不超過按鈕數量
            if (story.currentChoices.Count > buttons.Length)
            {
                Debug.LogError("選項數量超過了可用按鈕的數量。");
                return;
            }

            // 顯示對應的代表按鈕，並設置選項文本
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                buttons[i].gameObject.SetActive(true);
                buttons[i].GetComponentInChildren<Text>().text = story.currentChoices[i].text;

                // 使用局部變數捕捉選項索引，避免閉包問題
                int choiceIndex = i;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => MakeChoice(choiceIndex));
            }

            // 隱藏多餘的按鈕
            for (int i = story.currentChoices.Count; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }


        public void MakeChoice(int index)
        {
            if (index >= 0 && index < story.currentChoices.Count)
            {
                // 選擇 Ink 故事中的選項
                story.ChooseChoiceIndex(index);

                // 判斷當前是從代表對話框返回
                bool isRepresentativeDialogActive =
                    greenVitalPrefab.activeSelf || elegancePrefab.activeSelf || ecoEssentialsPrefab.activeSelf;

                // 隱藏所有代表對話框
                HideAllPrefabsAndTexts();

                // 顯示主要對話框
                if (characterDialogPrefab != null)
                {
                    characterDialogPrefab.SetActive(true);
                }

                // 調用下一段對話
                NextDialog();  // 常規呼叫

                // 如果之前是在代表對話框，回到主要對話框時再調用一次 NextDialog
                if (isRepresentativeDialogActive && characterDialogPrefab.activeSelf)
                {
                    NextDialog();  // 進行到下一段，保證剩餘選項正確顯示
                }
            }
            else
            {
                Debug.LogError("選項索引超出範圍");
            }
        }



        // 只隱藏代表和背景介紹的 Prefab，保持人物對話ui(強化版)顯示
        private void HideAllPrefabsAndTextsExceptCharacterDialog()
        {
            // 隱藏所有代表的Prefab
            greenVitalPrefab.SetActive(false);
            elegancePrefab.SetActive(false);
            ecoEssentialsPrefab.SetActive(false);

            // 隱藏廠商背景介紹 Prefab
            if (vendorIntroPrefab != null)
                vendorIntroPrefab.SetActive(false);

            // 隱藏所有代表的Text
            greenVitalText.gameObject.SetActive(false);
            eleganceText.gameObject.SetActive(false);
            ecoEssentialsText.gameObject.SetActive(false);
        }

        // 隱藏所有Prefab和Text，包括 "人物對話ui(強化版)"
        private void HideAllPrefabsAndTexts()
        {
            // 隱藏所有代表的Prefab
            greenVitalPrefab.SetActive(false);
            elegancePrefab.SetActive(false);
            ecoEssentialsPrefab.SetActive(false);

            // 隱藏原有的 "人物對話ui(強化版)" Prefab
            if (characterDialogPrefab != null)
                characterDialogPrefab.SetActive(false);

            // 隱藏廠商背景介紹 Prefab
            if (vendorIntroPrefab != null)
                vendorIntroPrefab.SetActive(false);

            // 隱藏所有代表的Text
            greenVitalText.gameObject.SetActive(false);
            eleganceText.gameObject.SetActive(false);
            ecoEssentialsText.gameObject.SetActive(false);
        }

        public void back()
        {
            SceneManager.LoadScene("SampleScene");
        }

        private void ShowDialog(GameObject prefab, Text text, string nextLine, Button[] buttons)
        {
            // 隱藏主要對話框的按鈕
            HideMainDialogButtons();

            // 隱藏所有的代表對話框和文本
            HideAllPrefabsAndTexts();

            // 顯示對應的代表對話框和文本
            prefab.SetActive(true);
            text.gameObject.SetActive(true);

            // 設置對話文本
            text.text = nextLine;

            // 顯示對應代表的選項按鈕
            SetChoices(buttons);
        }

    }
}
