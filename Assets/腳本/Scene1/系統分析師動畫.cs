using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public Transform cameraTransform;  // 用於引用主相機的 Transform
    private Animator animator;         // 用於控制動畫狀態
    private bool isTalking = false;    // 記錄是否已經進入交談狀態
    private bool isPlayerInRange = false; // 記錄玩家是否在觸發器範圍內

    void Start()
    {
        // 獲取人物的 Animator 組件
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 如果玩家在觸發器範圍內，持續讓人物面向相機
        if (isPlayerInRange)
        {
            FaceCamera();
        }
    }

    // 當相機進入人物的觸發器範圍時
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // 確保是相機進入範圍
        {
            isPlayerInRange = true;  // 標記玩家在範圍內
            StartTalking(); // 切換為說話狀態
        }
    }

    // 當相機離開人物的觸發器範圍時
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  // 確保是相機離開範圍
        {
            isPlayerInRange = false;  // 標記玩家不在範圍內
            StopTalking(); // 切換回 idle 狀態
        }
    }

    // 讓人物面向相機的函數
    void FaceCamera()
    {
        Vector3 directionToCamera = cameraTransform.position - transform.position;
        directionToCamera.y = 0;

        if (directionToCamera.sqrMagnitude < 0.01f) return; // 防止方向為零導致計算錯誤

        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }


    // 開始交談（切換到 talk 動畫）
    void StartTalking()
    {
        if (!isTalking)
        {
            isTalking = true;
            animator.SetBool("IsTalk", true); // 觸發 Animator 切換到 talk 動畫
        }
    }

    // 停止交談（切換回 idle 動畫）
    void StopTalking()
    {
        if (isTalking)
        {
            isTalking = false;
            animator.SetBool("IsTalk", false); // 切換回 idle 動畫
        }
    }
}
