using UnityEngine;
using UnityEngine.UI;

public class Showimage : MonoBehaviour
{
    public Button yourButton;
    public Image imageToToggle;

    void Start()
    {
        imageToToggle.gameObject.SetActive(false);
        yourButton.onClick.AddListener(ToggleImage);
    }

    void ToggleImage()
    {
        imageToToggle.gameObject.SetActive(!imageToToggle.gameObject.activeSelf);
    }
}