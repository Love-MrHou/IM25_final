using UnityEngine;
using UnityEngine.SceneManagement; // �ޤJ�����޲z

public class SceneChangeButton : MonoBehaviour
{
    // �]�m�n�[���������W��
    public string sceneName;
    public GameObject ChangeButton;

    void Start()
    {

    }

    // ����U���s�ɽեΦ���k
    public void ChangeSceneButton()
    {
        // �ˬd�O�_�E��
        if (ChangeButton.activeSelf)
        {
            // �[�����w������
            SceneManager.LoadScene(sceneName);
        }
    }
}
