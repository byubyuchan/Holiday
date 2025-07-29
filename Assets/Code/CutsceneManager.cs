using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance;
    public Camera mainCamera;
    public Text bossNameText; // Canvas에 있는 Text 연결
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

    private IEnumerator PlayBossCutsceneCoroutine(Transform bossTransform, string bossName, float power) // 파워는 확대량
    {
        cutsceneflag = 1;
        originalCamPos = mainCamera.transform.position;
        originalCamSize = mainCamera.orthographicSize;

        bossNameText.text = bossName;
        bossNameText.gameObject.SetActive(true);

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
        bossNameText.gameObject.SetActive(false);
        // 원래 화면으로 되돌리기
        mainCamera.transform.position = originalCamPos;
        mainCamera.orthographicSize = originalCamSize;
        cutsceneflag = 0;
    }
    private IEnumerator PlayTowerCutsceneCoroutine(Transform towerTransform, string towerName, float power, int time)
    {
        cutsceneflag = 1; // 컷신이 이미 실행되었는지 확인하는 플래그
        originalCamPos = mainCamera.transform.position;
        originalCamSize = mainCamera.orthographicSize;

        bossNameText.text = towerName;
        bossNameText.gameObject.SetActive(true);

        float interval = 0.44f;

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

        bossNameText.gameObject.SetActive(false);
        // 원래 화면으로 완전히 되돌리기
        mainCamera.transform.position = originalCamPos;
        mainCamera.orthographicSize = originalCamSize;
        cutsceneflag = 0;
    }
}