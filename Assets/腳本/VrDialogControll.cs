using UnityEngine;
using Valve.VR; // �T�O���w�� SteamVR ����

public class VRDialogController : MonoBehaviour
{
    // �j�w SteamVR ���s�ƥ�
    public SteamVR_Action_Boolean buttonAction;
    public SteamVR_Input_Sources handType; // ��ܬO���@���ⱱ��

    public GameObject dialogPanel; // ��ܮت� GameObject
    public Transform cameraTransform; // ���Y�]�Y���^�� Transform
    public float distanceFromCamera = 8.0f; // ��ܮػP���Y���Z��

    private bool isDialogActive = false;

    void Update()
    {
        // �ˬd���s�O�_�Q���U
        if (buttonAction.GetStateDown(handType))
        {
            ToggleDialog();
        }
    }

    void ToggleDialog()
    {
        isDialogActive = !isDialogActive;

        if (isDialogActive)
        {
            // ��ܹ�ܮبó]�m��m
            dialogPanel.SetActive(true);

            // �p���ܮ���ܪ���m�A�Ϩ�b���Y���e��@�w�Z��
            Vector3 forwardPosition = cameraTransform.position + cameraTransform.forward * distanceFromCamera;
            dialogPanel.transform.position = forwardPosition;

            // �Ϲ�ܮش¦V���Y
            dialogPanel.transform.rotation = Quaternion.LookRotation(dialogPanel.transform.position - cameraTransform.position);
        }
        else
        {
            // ���ù�ܮ�
            dialogPanel.SetActive(false);
        }
    }
}
