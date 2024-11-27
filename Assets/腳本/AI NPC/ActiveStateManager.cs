using UnityEngine;

public class ActiveStateManager : MonoBehaviour
{
    public GameObject targetObjectA; // 需要檢測的 GameObject
    public GameObject targetObjectB; // 根據狀態切換的 GameObject

    void Update()
    {
        // 如果 A 是激活的，那麼關閉 B
        if (targetObjectA.activeSelf && targetObjectB.activeSelf)
        {
            targetObjectB.SetActive(false);
            Debug.Log($"{targetObjectA.name} 激活，已關閉 {targetObjectB.name}");
        }
        // 如果 A 被關閉，可以重新啟用 B
        /*
         * else if (!targetObjectA.activeSelf && !targetObjectB.activeSelf)
        {
            targetObjectB.SetActive(true);
            Debug.Log($"{targetObjectA.name} 未激活，已啟用 {targetObjectB.name}");
        }
        */
    }
}
