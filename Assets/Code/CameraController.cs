using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera; // 이동할 카메라
    public float transitionSpeed = 2f; // 이동 속도
    public float moveDistance = 10f; // 한 번에 이동할 거리

    private Vector3 startPosition; // 카메라 시작 위치

    public float leftLimit = -40f;  // 왼쪽 제한
    public float rightLimit = 100f; // 오른쪽 제한

    void Start()
    {
        // 초기 카메라 위치 저장
        startPosition = mainCamera.transform.position;
    }

    // 왼쪽으로 이동하는 함수
    public void MoveCameraLeft()
    {
        // 현재 카메라 위치에서 이동할 거리만큼 더한 값을 목표 위치로 설정
        float targetX = Mathf.Max(startPosition.x - moveDistance, leftLimit);
        Vector3 targetPosition = new Vector3(targetX, startPosition.y, startPosition.z);
        StartCoroutine(MoveCameraToPosition(targetPosition));
        startPosition = targetPosition; // 카메라 위치 업데이트
    }

    // 오른쪽으로 이동하는 함수
    public void MoveCameraRight()
    {
        // 현재 카메라 위치에서 이동할 거리만큼 더한 값을 목표 위치로 설정
        float targetX = Mathf.Min(startPosition.x + moveDistance, rightLimit);
        Vector3 targetPosition = new Vector3(targetX, startPosition.y, startPosition.z);
        StartCoroutine(MoveCameraToPosition(targetPosition));
        startPosition = targetPosition; // 카메라 위치 업데이트
    }

    // 카메라가 목표 위치로 이동하는 코루틴
    private IEnumerator MoveCameraToPosition(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPos = mainCamera.transform.position;

        while (elapsedTime < 1f)
        {
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * transitionSpeed;
            yield return null;
        }

        mainCamera.transform.position = targetPosition; // 정확히 목표 위치에 맞추기
    }
}
