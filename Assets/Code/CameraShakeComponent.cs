using System.Collections;
using UnityEngine;

public class CameraShakeComponent : MonoBehaviour
{
    public static CameraShakeComponent instance;
    private float currentmag = -1f;

    private void Awake()
    {
        instance = this;
    }
    //외부에서 호출하여 카메라 쉐이크를 시작하는 함수
    public void StartShake(float duration = 0.3f, float magnitude = 0.1f)
    {

        if (magnitude <= currentmag) return;

        currentmag = magnitude;

        if (CutsceneManager.instance.cutsceneflag == 0)
            StartCoroutine(Shake(duration, magnitude));
    }

    // 카메라를 흔드는 코루틴 함수
    // duration: 카메라가 흔들리는 시간 (초 단위)
    // magnitude: 흔들림의 강도 (흔들림의 최대 범위)
    public IEnumerator Shake(float duration, float magnitude)
    {
        // 경과 시간을 추적하는 변수
        float elapsed = 0.0f;


        // 경과 시간이 지정된 지속 시간보다 작을 동안 반복
        while (elapsed < duration)
        {
            // x와 y축으로 무작위 흔들림을 생성합니다.
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // 카메라의 위치를 흔들림 값으로 업데이트
            // z축 위치는 원래 위치를 유지
            transform.localPosition = new Vector3(x, y, 0);

            // 경과 시간을 갱신
            elapsed += Time.deltaTime;

            // 다음 프레임까지 대기
            //yield return null;
            yield return new WaitForSeconds(0.01f);
        }

        // 흔들림이 끝난 후 카메라를 원래 위치로
        transform.localPosition = new Vector3();
        currentmag = -1f;
    }
}