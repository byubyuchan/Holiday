using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private Transform towerParent; // 부모 오브젝트

    public void HealAllTowers()
    {
        Tower[] towers = towerParent.GetComponentsInChildren<Tower>(); // 부모의 모든 자식 타워 가져오기
        if (GameManager.instance.Gold < 5)
        {
            return;
        }
        GameManager.instance.Gold -= 5;

        foreach (Tower tower in towers)
        {
            if (tower != null)
            {
                tower.hp = tower.maxHp; // 체력 회복, 최대 체력 초과 방지
            }
        }
    }
}