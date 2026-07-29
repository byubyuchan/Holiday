using UnityEngine;
using System.Collections;

public class DamageFlashEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Color originalColor; // 원래 색상 저장
    public Color flashColor = Color.red; // 타격 시 나타날 색상
    public float flashDuration = 0.2f; // 색상이 유지되는 시간

    public bool isSlowed = false; // 슬로우 효과가 적용되었는지 여부
    private Color slowColor = new Color(0.5f, 0.5f, 0.5f, 1.0f); // 어두운 색상

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // 원래 색상 저장
        }
    }

    public void Flash()
    {
        if (!gameObject.activeInHierarchy) // 활성 상태 확인
        {
            return;
        }

        if (spriteRenderer != null) // 활성 상태 확인
        {
            StartCoroutine(FlashCoroutine());
        }

    }

    private IEnumerator FlashCoroutine()
    {
        int blinkCount = 2; // 깜빡임 횟수

        for (int i = 0; i < blinkCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration / (blinkCount * 2));

            if (isSlowed)
            {
                spriteRenderer.color = slowColor;
            }
            else
            {
                spriteRenderer.color = originalColor;
            }
            yield return new WaitForSeconds(flashDuration / (blinkCount * 2));
        }
    }

    public void SetSlowEffect(bool active)
    {
        isSlowed = active;
        if (isSlowed)
        {
            spriteRenderer.color = slowColor; // 색상을 어둡게 변경
        }
        else
        {
            spriteRenderer.color = originalColor; // 원래 색상으로 복귀
        }
    }
}