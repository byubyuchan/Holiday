using UnityEngine;

public class Tile : MonoBehaviour
{

    public bool IsBuildTower;
    // 자동구현 프로퍼티 = set과 get 함수가 자동으로 작성된다.
    public Tower currentTower;

    private void Awake()
    {
        IsBuildTower = false;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = -3;
        }
        currentTower = null;
    }
}
