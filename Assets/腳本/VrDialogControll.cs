using UnityEngine;
using Valve.VR; // 確保有安裝 SteamVR 插件

public class VRDialogController : MonoBehaviour
{
    // 綁定 SteamVR 按鈕事件
    public SteamVR_Action_Boolean buttonAction;
    public SteamVR_Input_Sources handType; // 選擇是哪一隻手控制

    public GameObject dialogPanel; // 對話框的 GameObject
    public Transform cameraTransform; // 鏡頭（頭盔）的 Transform
    public float distanceFromCamera = 8.0f; // 對話框與鏡頭的距離

    private bool isDialogActive = false;

    void Update()
    {
        // 檢查按鈕是否被按下
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
            // 顯示對話框並設置位置
            dialogPanel.SetActive(true);

            // 計算對話框顯示的位置，使其在鏡頭正前方一定距離
            Vector3 forwardPosition = cameraTransform.position + cameraTransform.forward * distanceFromCamera;
            dialogPanel.transform.position = forwardPosition;

            // 使對話框朝向鏡頭
            dialogPanel.transform.rotation = Quaternion.LookRotation(dialogPanel.transform.position - cameraTransform.position);
        }
        else
        {
            // 隱藏對話框
            dialogPanel.SetActive(false);
        }
    }
}
