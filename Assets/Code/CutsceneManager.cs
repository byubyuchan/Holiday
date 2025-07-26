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

    void Awake()
    {
        instance = this;
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    public void PlayBossCutscene(Transform bossTransform, string bossName)
    {
        StartCoroutine(PlayCutsceneCoroutine(bossTransform, bossName));
    }

    private IEnumerator PlayCutsceneCoroutine(Transform bossTransform, string bossName)
    {
        originalCamPos = mainCamera.transform.position;
        originalCamSize = mainCamera.orthographicSize;

        bossNameText.text = bossName;
        bossNameText.gameObject.SetActive(true);

        float elapsed = 0f;
        float targetCamSize = originalCamSize * 0.5f; // 확대 (수치는 조절)

        while (elapsed < cutsceneDuration)
        {
            elapsed += Time.deltaTime;

            // 카메라 위치를 보스 위치로 부드럽게 이동
            Vector3 targetPos = new Vector3(bossTransform.position.x, bossTransform.position.y, originalCamPos.z);
            mainCamera.transform.position = Vector3.Lerp(originalCamPos, targetPos, elapsed / (cutsceneDuration * 0.5f));

            // 카메라 확대
            mainCamera.orthographicSize = Mathf.Lerp(originalCamSize, targetCamSize, elapsed / (cutsceneDuration * 0.5f));

            yield return null;
        }
        bossNameText.gameObject.SetActive(false);
        // 원래 화면으로 되돌리기
        mainCamera.transform.position = originalCamPos;
        mainCamera.orthographicSize = originalCamSize;
    }
}