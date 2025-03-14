using UnityEngine;

public class GoalMovement : MonoBehaviour
{
    public float speed = 1f; // 이동 속도
    public float movementRange = 5f; // 위아래로 움직일 범위
    private float startY; // 초기 Y 위치

    void Awake()
    {
        // 초기 Y 위치를 저장
        startY = transform.position.y;
    }

    void Update()
    {
        // Y 축을 기준으로 PingPong 방식으로 움직이게 함
        float newY = startY + Mathf.PingPong(Time.time * speed, movementRange);
        transform.position = new Vector2(transform.position.x, newY);
    }
}