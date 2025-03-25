using UnityEngine;
using System.Collections;

public class DamageFlashEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor; // 원래 색상 저장
    public Color flashColor = Color.red; // 타격 시 나타날 색상
    public float flashDuration = 0.2f; // 색상이 유지되는 시간

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
        if (spriteRenderer != null)
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

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration / (blinkCount * 2));
        }
    }
}