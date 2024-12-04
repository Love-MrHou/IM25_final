using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUIandObject : MonoBehaviour
{
    public GameObject Canva;
    public GameObject exclamationMark;

    void Start()
    {
        if (Canva == null)
        {
            Debug.LogError("Canva object is not assigned.");
            return;
        }

        Canva.SetActive(false);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 确保只有玩家触发
        {
            exclamationMark.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // 确保只有玩家触发
        {
            exclamationMark.SetActive(true);
        }
    }

    
}
