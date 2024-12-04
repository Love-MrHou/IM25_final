using UnityEngine;

public class ObjectPingPong : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The range within which the object will move up and down")]
    [SerializeField] private float deltaY = 0.2f;   // 位移範圍

    [Tooltip("The speed at which the object moves")]
    [SerializeField] private float speed = 1.0f;    // 移動速度

    private float initialY;       // 初始 y 位置

    void Start()
    {
        // 抓取物件的初始 y 位置
        initialY = transform.position.y;
    }

    void Update()
    {
        // 使用 Mathf.PingPong 來實現上下移動效果
        float newY = initialY + Mathf.PingPong(Time.time * speed, deltaY * 2) - deltaY;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
