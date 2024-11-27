using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyProject.Dialogs
{
    public class Scene2_Script : MonoBehaviour
    {
        public Text dialogText;  // ��ܹ�ܪ� Text
        public Button[] buttons;  // �D��ܮت��ﶵ���s
        public TextAsset[] inkAssets;  // �h�� Inky �@�����}�C�]�C�� NPC �i�H���h�Ӽ@���^
        public GameObject disappearui;

        // ����� Prefab
        public GameObject characterDialogPrefab;  // "�H�����ui(�j�ƪ�)" prefab
        public GameObject vendorIntroPrefab;  // "�t�ӭI������ (1)" prefab

        // �s�W�T��Prefab�M�T��Text
        public GameObject greenVitalPrefab;  // "GreenVital Foods" ��Prefab
        public GameObject elegancePrefab;    // "Elegance Accessories" ��Prefab
        public GameObject ecoEssentialsPrefab;  // "EcoEssentials" ��Prefab

        public Text greenVitalText;  // "GreenVital Foods" ������Text
        public Text eleganceText;    // "Elegance Accessories" ������Text
        public Text ecoEssentialsText;  // "EcoEssentials" ������Text

        // ���C�ӹ�ܮطs�W�@�ի��s
        public Button[] greenVitalButtons;   // GreenVital Foods ���ﶵ���s
        public Button[] eleganceButtons;     // Elegance Accessories ���ﶵ���s
        public Button[] ecoEssentialsButtons;  // EcoEssentials ���ﶵ���s

        private Story story = null;
        private List<string> dialogHistory = new List<string>();  // ��ܾ��v����
        private int currentDialogIndex = -1;  // ��e��ܯ���
        private int currentInkAssetIndex = 0;  // ��e�@������

        // �s�W�@�Өƥ�ӳq����ܧ�s
        public delegate void OnDialogUpdateHandler(Story story);
        public event OnDialogUpdateHandler OnDialogUpdate;

        void Start()
        {
            // ��l�����éҦ����s
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }

            // ���n���� characterDialogPrefab�A�O�����b�@�}�l���
            HideAllPrefabsAndTextsExceptCharacterDialog();

            if (inkAssets.Length > 0)
            {
                // �}�l�Ĥ@�Ӽ@��
                StartDialog(inkAssets[currentInkAssetIndex]);
            }
        }

        public bool StartDialog(TextAsset inkAsset)
        {
            if (story != null)
            {
                story = null;  // ���m�G�ƹ�H
            }

            story = new Story(inkAsset.text);  // ��l�Ʒs�� Story
            dialogHistory.Clear();  // �M�Ź�ܾ��v
            currentDialogIndex = -1;  // ���m��ܯ���
            NextDialog();  // ��ܹ��
            return true;
        }

        public void NextDialog()
        {
            if (story == null) return;

            // �p�G��ܤw�g�����A�åB�S���i��ܪ��ﶵ�A���դ�����U�@�Ӽ@��
            if (!story.canContinue && story.currentChoices.Count == 0)
            {
                Debug.Log("�@�������C");

                // ������U�@�Ӽ@���]�p�G���^
                currentInkAssetIndex++;
                if (currentInkAssetIndex < inkAssets.Length)
                {
                    disappearui.SetActive(false);
                    Debug.Log("������U�@�Ӽ@���C");
                    StartDialog(inkAssets[currentInkAssetIndex]);  // �[���U�@�Ӽ@��
                    return;
                }

                Debug.Log("�Ҧ��@���w�����C");
                return;
            }

            // �p�G�٦���ܥi�H�~��
            if (story.canContinue)
            {
                string nextLine = story.Continue();
                if (!string.IsNullOrWhiteSpace(nextLine))
                {
                    dialogHistory.Add(nextLine);  // �N��ܲK�[����v
                    currentDialogIndex = dialogHistory.Count - 1;
                    dialogText.text = nextLine;  // ��ܷs�����
                }

                // �������ҨèM�w���s�ͦ���m
                if (story.currentTags.Contains("GreenVital Foods�N��"))
                {
                    // ���åD�n��ܮت����s�A�æb GreenVital ����ܮإͦ��ﶵ
                    ShowDialog(greenVitalPrefab, greenVitalText, nextLine, greenVitalButtons);
                }
                else if (story.currentTags.Contains("Elegance Accessories�N��"))
                {
                    // ���åD�n��ܮت����s�A�æb Elegance ����ܮإͦ��ﶵ
                    ShowDialog(elegancePrefab, eleganceText, nextLine, eleganceButtons);
                }
                else if (story.currentTags.Contains("EcoEssentials�N��"))
                {
                    // ���åD�n��ܮت����s�A�æb EcoEssentials ����ܮإͦ��ﶵ
                    ShowDialog(ecoEssentialsPrefab, ecoEssentialsText, nextLine, ecoEssentialsButtons);
                }
                else if (story.currentTags.Contains("�t�Τ��R�v1"))
                {
                    // ���åD�n��ܮءA��� vendorIntroPrefab
                    if (characterDialogPrefab != null)
                    {
                        characterDialogPrefab.SetActive(false);
                    }

                    if (vendorIntroPrefab != null)
                    {
                        vendorIntroPrefab.SetActive(true);  // ��� "�t�ӭI������ (1)"
                    }
                }
                else
                {
                    // �S���N����ҡA�h�b�D�n��ܮإͦ��ﶵ
                    ShowMainDialogButtons();
                }

                // �o�e�q���A����L�t�Ϊ��D��ܧ�s�F
                OnDialogUpdate?.Invoke(story);

                // �b���ﶵ�����p�U�A�ͦ����s
                if (story.currentChoices.Count > 0)
                {
                    // �p�G���N����ҡA�ͦ������N���ܮت��ﶵ���s
                    if (story.currentTags.Contains("GreenVital Foods�N��"))
                    {
                        SetChoices(greenVitalButtons);
                    }
                    else if (story.currentTags.Contains("Elegance Accessories�N��"))
                    {
                        SetChoices(eleganceButtons);
                    }
                    else if (story.currentTags.Contains("EcoEssentials�N��"))
                    {
                        SetChoices(ecoEssentialsButtons);
                    }
                    else
                    {
                        // �S���N����ҮɡA�ͦ��D�n��ܮت��ﶵ���s
                        SetChoices(buttons);
                    }
                }
                else
                {
                    // �S���ﶵ�����ë��s
                    HideMainDialogButtons();
                }
            }
        }



        private void HideMainDialogButtons()
        {
            // ���åD�n��ܮت��Ҧ��ﶵ���s
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        private void ShowMainDialogButtons()
        {
            // ��ܥD�n��ܮت��ﶵ���s�A�ó]�m�ﶵ�奻
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                buttons[i].gameObject.SetActive(true);
                buttons[i].GetComponentInChildren<Text>().text = story.currentChoices[i].text;

                // �ϥΧ����ܼƮ����ﶵ���ޡA�קK���]���D
                int choiceIndex = i;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => MakeChoice(choiceIndex));
            }

            // ���æh�l�����s
            for (int i = story.currentChoices.Count; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        private void SetChoices(Button[] buttons)
        {
            // �T�O story.currentChoices ���ƶq���W�L���s�ƶq
            if (story.currentChoices.Count > buttons.Length)
            {
                Debug.LogError("�ﶵ�ƶq�W�L�F�i�Ϋ��s���ƶq�C");
                return;
            }

            // ��ܹ������N����s�A�ó]�m�ﶵ�奻
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                buttons[i].gameObject.SetActive(true);
                buttons[i].GetComponentInChildren<Text>().text = story.currentChoices[i].text;

                // �ϥΧ����ܼƮ����ﶵ���ޡA�קK���]���D
                int choiceIndex = i;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => MakeChoice(choiceIndex));
            }

            // ���æh�l�����s
            for (int i = story.currentChoices.Count; i < buttons.Length; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }


        public void MakeChoice(int index)
        {
            if (index >= 0 && index < story.currentChoices.Count)
            {
                // ��� Ink �G�Ƥ����ﶵ
                story.ChooseChoiceIndex(index);

                // �P�_��e�O�q�N���ܮت�^
                bool isRepresentativeDialogActive =
                    greenVitalPrefab.activeSelf || elegancePrefab.activeSelf || ecoEssentialsPrefab.activeSelf;

                // ���éҦ��N���ܮ�
                HideAllPrefabsAndTexts();

                // ��ܥD�n��ܮ�
                if (characterDialogPrefab != null)
                {
                    characterDialogPrefab.SetActive(true);
                }

                // �եΤU�@�q���
                NextDialog();  // �`�W�I�s

                // �p�G���e�O�b�N���ܮءA�^��D�n��ܮخɦA�եΤ@�� NextDialog
                if (isRepresentativeDialogActive && characterDialogPrefab.activeSelf)
                {
                    NextDialog();  // �i���U�@�q�A�O�ҳѾl�ﶵ���T���
                }
            }
            else
            {
                Debug.LogError("�ﶵ���޶W�X�d��");
            }
        }



        // �u���åN��M�I�����Ъ� Prefab�A�O���H�����ui(�j�ƪ�)���
        private void HideAllPrefabsAndTextsExceptCharacterDialog()
        {
            // ���éҦ��N��Prefab
            greenVitalPrefab.SetActive(false);
            elegancePrefab.SetActive(false);
            ecoEssentialsPrefab.SetActive(false);

            // ���üt�ӭI������ Prefab
            if (vendorIntroPrefab != null)
                vendorIntroPrefab.SetActive(false);

            // ���éҦ��N��Text
            greenVitalText.gameObject.SetActive(false);
            eleganceText.gameObject.SetActive(false);
            ecoEssentialsText.gameObject.SetActive(false);
        }

        // ���éҦ�Prefab�MText�A�]�A "�H�����ui(�j�ƪ�)"
        private void HideAllPrefabsAndTexts()
        {
            // ���éҦ��N��Prefab
            greenVitalPrefab.SetActive(false);
            elegancePrefab.SetActive(false);
            ecoEssentialsPrefab.SetActive(false);

            // ���í즳�� "�H�����ui(�j�ƪ�)" Prefab
            if (characterDialogPrefab != null)
                characterDialogPrefab.SetActive(false);

            // ���üt�ӭI������ Prefab
            if (vendorIntroPrefab != null)
                vendorIntroPrefab.SetActive(false);

            // ���éҦ��N��Text
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
            // ���åD�n��ܮت����s
            HideMainDialogButtons();

            // ���éҦ����N���ܮةM�奻
            HideAllPrefabsAndTexts();

            // ��ܹ������N���ܮةM�奻
            prefab.SetActive(true);
            text.gameObject.SetActive(true);

            // �]�m��ܤ奻
            text.text = nextLine;

            // ��ܹ����N���ﶵ���s
            SetChoices(buttons);
        }

    }
}
