using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance;
    public Camera mainCamera;
    public float cutsceneDuration = 4f;
    private Vector3 originalCamPos;
    private float originalCamSize;
    public int cutsceneflag;

    void Awake()
    {
        instance = this;
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    public void PlayBossCutscene(Transform bossTransform, string bossName, float power)
    {
        StartCoroutine(PlayBossCutsceneCoroutine(bossTransform, bossName, power));
    }

    public void PlayTowerCutscene(Transform towerTransform, string towerName, float power, int time)
    {
        StartCoroutine(PlayTowerCutsceneCoroutine(towerTransform, towerName, power, time));
    }
    public void PlayDeathCutscene(Transform bossTransform, string bossName, float power)
    {
        StartCoroutine(PlayDeathCutsceneCoroutine(bossTransform, bossName, power));
    }

    private IEnumerator PlayBossCutsceneCoroutine(Transform bossTransform, string bossName, float power) // 파워는 확대량
    {
        if (TowerInfo.instance != null)
        {
            TowerInfo.instance.HideUI();
        }
        cutsceneflag = 1;
        originalCamPos = mainCamera.transform.position;
        originalCamSize = mainCamera.orthographicSize;

        GameManager.instance.ShowMessage(bossName, 4f);

        float elapsed = 0f;
        float targetCamSize = originalCamSize * power; // 확대 (수치는 조절)

        while (elapsed < cutsceneDuration) // 4초간 지속
        {
            elapsed += Time.deltaTime;

            // 카메라 위치를 보스 위치로 부드럽게 이동
            Vector3 targetPos = new Vector3(bossTransform.position.x, bossTransform.position.y, originalCamPos.z);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, elapsed / (cutsceneDuration * power));

            // 카메라 확대
            mainCamera.orthographicSize = Mathf.Lerp(originalCamSize, targetCamSize, elapsed / (cutsceneDuration * power));
            yield return null;
        }
        // 원래 화면으로 되돌리기
        mainCamera.transform.position = originalCamPos;
        mainCamera.orthographicSize = originalCamSize;
        cutsceneflag = 0;
    }
    private IEnumerator PlayTowerCutsceneCoroutine(Transform towerTransform, string towerName, float power, int time)
    {
        if (TowerInfo.instance != null)
        {
            TowerInfo.instance.HideUI();
        }
        cutsceneflag = 1; // 컷신이 이미 실행되었는지 확인하는 플래그
        originalCamPos = mainCamera.transform.position;
        originalCamSize = mainCamera.orthographicSize;

        float interval = 0.44f;
        GameManager.instance.ShowMessage(towerName, time * 0.54f);

        for (int i = 1; i <= time; i++)
        {
            float targetPower = Mathf.Lerp(1f, power, (float)i / time);
            float targetCamSize = originalCamSize * targetPower;

            Vector3 targetPos = new Vector3(towerTransform.position.x, towerTransform.position.y, originalCamPos.z);

            // 줌인
            float t = 0f;
            while (t < interval) // 프레임마다 자연스러운 확대를 위한 while문
            {
                t += Time.deltaTime;
                float progress = Mathf.Clamp01(t / interval); // 0 ~ 1 사이로 제한
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, progress); // Lerp = progress만큼 보간하며 확대
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetCamSize, progress);
                yield return null;
            }
            // 약간 텀 주기
            yield return new WaitForSeconds(0.1f);
        }
        // 원래 화면으로 완전히 되돌리기
        mainCamera.transform.position = originalCamPos;
        mainCamera.orthographicSize = originalCamSize;
        cutsceneflag = 0;
    }

    private IEnumerator PlayDeathCutsceneCoroutine(Transform bossTransform, string bossName, float power)
    {
        if (TowerInfo.instance != null)
        {
            TowerInfo.instance.HideUI();
        }

        cutsceneflag = 1;
        originalCamPos = mainCamera.transform.position;
        originalCamSize = mainCamera.orthographicSize;

        GameManager.instance.ShowMessage(bossName, 4f);

        float elapsed = 0f;
        float targetCamSize = originalCamSize * power;
        float shakeInterval = 0.5f; // 이펙트와 쉐이크 빈도
        float shakeTimer = 0f;

        while (elapsed < cutsceneDuration)
        {
            elapsed += Time.deltaTime;
            shakeTimer += Time.deltaTime;

            // 카메라 이동 및 줌
            Vector3 targetPos = new Vector3(bossTransform.position.x, bossTransform.position.y, originalCamPos.z);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, elapsed / (cutsceneDuration * power));
            mainCamera.orthographicSize = Mathf.Lerp(originalCamSize, targetCamSize, elapsed / (cutsceneDuration * power));

            // 일정 간격으로 흔들기 및 이펙트 생성
            if (shakeTimer >= shakeInterval)
            {
                shakeTimer = 0f;

                CameraShakeComponent.instance.StartShake(0.2f, 0.5f);

                // 랜덤한 위치로 이펙트 생성
                Vector3 randomOffset = new Vector3(
                    Random.Range(-5f, 5f),
                    Random.Range(-5f, 5f),
                    0f // Z는 고정
                );

                GameObject effectInstance = GameManager.instance.pool.Get(5);
                effectInstance.transform.position = bossTransform.position + randomOffset;
                effectInstance.SetActive(true);
            }

            yield return null;
        }

        // 원래 화면 복귀
        mainCamera.transform.position = originalCamPos;
        mainCamera.orthographicSize = originalCamSize;
        cutsceneflag = 0;
    }
}