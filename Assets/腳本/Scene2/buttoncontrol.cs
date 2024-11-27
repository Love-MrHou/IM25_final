using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttoncontrol : MonoBehaviour
{
    // 定義三個不同的 prefab，分別對應三個按鈕
    public GameObject originalPrefab;  // 原本的 prefab
    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject prefab3;
    public GameObject finalPrefab;  // 最終顯示的 prefab
    public GameObject extraPrefab;  // 額外顯示的 prefab，當 finalPrefab 顯示後顯示

    // 用於保存當前顯示的 prefab
    private GameObject currentPrefab;
    // 用於保存原始的 prefab，當返回時顯示
    private GameObject previousPrefab;

    // 記錄按鈕點擊的次數
    private int returnClickCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        // 初始化時，隱藏所有 prefab
        if (originalPrefab != null) originalPrefab.SetActive(true);
        if (prefab1 != null) prefab1.SetActive(false);
        if (prefab2 != null) prefab2.SetActive(false);
        if (prefab3 != null) prefab3.SetActive(false);
        if (extraPrefab != null) extraPrefab.SetActive(false);  // 初始化時隱藏額外的 prefab

        // 初始化設置 currentPrefab 為原本的 prefab
        currentPrefab = originalPrefab;
    }

    // Function 1: 顯示 prefab1，並隱藏當前顯示的 prefab
    public void ShowPrefab1()
    {
        SwitchPrefab(prefab1);
    }

    // Function 2: 顯示 prefab2，並隱藏當前顯示的 prefab
    public void ShowPrefab2()
    {
        SwitchPrefab(prefab2);
    }

    // Function 3: 顯示 prefab3，並隱藏當前顯示的 prefab
    public void ShowPrefab3()
    {
        SwitchPrefab(prefab3);
    }

    // 通用的切換 prefab 方法，顯示新 prefab 並關閉當前顯示的 prefab
    private void SwitchPrefab(GameObject newPrefab)
    {
        if (currentPrefab != null)
        {
            currentPrefab.SetActive(false);  // 隱藏當前 prefab
        }

        if (newPrefab != null)
        {
            newPrefab.SetActive(true);  // 顯示新 prefab
            previousPrefab = currentPrefab;  // 將當前的 prefab 記錄為 previousPrefab
            currentPrefab = newPrefab;  // 將當前顯示的 prefab 設置為新 prefab
        }
    }

    // 返回原本的 prefab
    public void ReturnToPreviousPrefab()
    {
        if (currentPrefab != null)
        {
            currentPrefab.SetActive(false);  // 隱藏當前顯示的 prefab
        }

        if (previousPrefab != null)
        {
            previousPrefab.SetActive(true);  // 顯示之前的 prefab
            currentPrefab = previousPrefab;  // 更新 currentPrefab 為原本的 prefab
            previousPrefab = originalPrefab;  // 重置 previousPrefab 為原始的
        }

        // 計算返回次數
        returnClickCount++;
        if (returnClickCount >= 3)
        {
            ShowFinalPrefab();  // 顯示最終的 prefab 當返回次數達到 3 次
        }
    }

    // 顯示最終的 prefab 並顯示額外的 prefab
    private void ShowFinalPrefab()
    {
        if (currentPrefab != null)
        {
            currentPrefab.SetActive(false);  // 隱藏當前顯示的 prefab
        }

        if (finalPrefab != null)
        {
            finalPrefab.SetActive(true);  // 顯示最終的 prefab
            currentPrefab = finalPrefab;  // 設置為當前顯示的 prefab
        }

        // 顯示額外的 prefab
        if (extraPrefab != null)
        {
            extraPrefab.SetActive(true);  // 顯示額外的 prefab
        }
    }
}
