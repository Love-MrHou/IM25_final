using UnityEngine;

public class CanvasDisappear : MonoBehaviour
{
    public GameObject canvasA; // 要消失的UI

    public void DisappearCanvas()
    {
        // 切換 Canvas 的顯示狀態
        canvasA.SetActive(false);
    }
}
