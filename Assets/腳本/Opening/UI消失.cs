using UnityEngine;

public class CanvasDisappear : MonoBehaviour
{
    public GameObject canvasA; // �n������UI

    public void DisappearCanvas()
    {
        // ���� Canvas ����ܪ��A
        canvasA.SetActive(false);
    }
}
