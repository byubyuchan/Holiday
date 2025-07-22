using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public static CameraController instance; // 싱글톤 인스턴스
    public Transform cameraParent;

    public float transitionSpeed = 2f; // 이동 속도
    public float moveDistance = 10f; // 한 번에 이동할 거리

    public float leftLimit = -40f;  // 왼쪽 제한
    public float rightLimit = 100f; // 오른쪽 제한

    public void MoveCameraLeft()
    {
        float currentX = cameraParent.position.x;
        float targetX = Mathf.Max(currentX - moveDistance, leftLimit);
        Vector3 targetPosition = new Vector3(targetX, cameraParent.position.y, cameraParent.position.z);
        StartCoroutine(MoveCameraToPosition(targetPosition));
    }

    public void MoveCameraRight()
    {
        float currentX = cameraParent.position.x;
        float targetX = Mathf.Min(currentX + moveDistance, rightLimit);
        Vector3 targetPosition = new Vector3(targetX, cameraParent.position.y, cameraParent.position.z);
        StartCoroutine(MoveCameraToPosition(targetPosition));
    }

    private IEnumerator MoveCameraToPosition(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPos = cameraParent.position;

        while (elapsedTime < 1f)
        {
            cameraParent.position = Vector3.Lerp(startPos, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * transitionSpeed;
            yield return null;
        }

        cameraParent.position = targetPosition;
    }
}
