using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    public GameObject canvasA; // �n������UI
    public GameObject canvasB; // �n��ܪ�UI

    public void SwitchCanvas()
    {
        // ���� Canvas ����ܪ��A
        canvasA.SetActive(false);
        canvasB.SetActive(true);
    }
}
