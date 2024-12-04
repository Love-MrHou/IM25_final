using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    public GameObject canvasA; // 要消失的UI
    public GameObject canvasB; // 要顯示的UI

    public void SwitchCanvas()
    {
        // 切換 Canvas 的顯示狀態
        canvasA.SetActive(false);
        canvasB.SetActive(true);
    }
}
