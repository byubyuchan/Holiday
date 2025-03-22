using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public float lifetime = 2f; // 이펙트 유지 시간

    private void Start()
    {
        Destroy(gameObject, lifetime); // lifetime 이후 오브젝트 삭제
    }
}