using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowInformation : MonoBehaviour
{
    public Button footbook;
    public Button instragrum;
    public Button xwitter;
    public Canvas footbook_C;
    public Canvas instragrum_C;
    public Canvas xwitter_C;
    // Start is called before the first frame update
    void Start()
    {
        footbook_C.gameObject.SetActive(false);
        instragrum_C.gameObject.SetActive(false);
        xwitter_C.gameObject.SetActive(false);
        footbook.onClick.AddListener(OnFootbookClick);
        instragrum.onClick.AddListener(OninstragrumClick);
        xwitter.onClick.AddListener(OnxwitterClick);
    }
    void OnFootbookClick()
    {
        footbook_C.gameObject.SetActive(true);
        instragrum_C.gameObject.SetActive(false);
        xwitter_C.gameObject.SetActive(false);
    }
    void OninstragrumClick()
    {
        footbook_C.gameObject.SetActive(false);
        instragrum_C.gameObject.SetActive(true);
        xwitter_C.gameObject.SetActive(false);
    }

    void OnxwitterClick()
    {
        footbook_C.gameObject.SetActive(false);
        instragrum_C.gameObject.SetActive(false);
        xwitter_C.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
