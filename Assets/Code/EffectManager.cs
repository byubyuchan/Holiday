using System.Collections;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public float lifetime = 2f; // 이펙트 유지 시간
    private EffectDamage effectDamage;

    private void OnEnable()
    {
        StartCoroutine(DeactivateEffect(lifetime));
    }

    private IEnumerator DeactivateEffect(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (effectDamage != null) effectDamage.ResetDamageList(); // 데미지 기록 초기화

        gameObject.SetActive(false);
    }
}