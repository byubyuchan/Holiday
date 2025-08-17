using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private Transform towerParent; // 부모 오브젝트
    [SerializeField] private Transform enemyParent; // 몬스터 오브젝트

    public static TowerManager instance;
    public bool sale = false;
    public bool reverse = false;

    private void Awake()
    {
        instance = this;
    }

    public void HealAllTowers()
    {
        if (CutsceneManager.instance.cutsceneflag == 1) return;
        Tower[] towers = towerParent.GetComponentsInChildren<Tower>(); 
        Enemy[] enemys = enemyParent.GetComponentsInChildren<Enemy>();

        if (sale)
        {
            if (GameManager.instance.Gold < 3)
            {
                GameManager.instance.ShowMessage("골드가 모자랍니다!");
                AudioManager.instance.PlaySFX("Cant");
                CameraShakeComponent.instance.StartShake();
                return;
            }

            GameManager.instance.Gold -= 3;
            if (reverse)
            {
                foreach (Enemy enemy in enemys)
                {
                    if (enemy != null)
                    {
                        enemy.TakeDamage(100f);
                    }
                }
                GameManager.instance.ShowMessage("파괴 마법을 걸었습니다!");
                CameraShakeComponent.instance.StartShake();
            }
            else
            {
                GameManager.instance.ShowMessage("회복 마법을 걸었습니다!");
                CameraShakeComponent.instance.StartShake();
                AudioManager.instance.PlaySFX("P_Heal");
            }

        }
        else
        {
            if (GameManager.instance.Gold < 5)
            {
                GameManager.instance.ShowMessage("골드가 모자랍니다!");
                AudioManager.instance.PlaySFX("Cant");
                CameraShakeComponent.instance.StartShake();
                return;
            }
            GameManager.instance.Gold -= 5;
            if (reverse)
            {
                foreach (Enemy enemy in enemys)
                {
                    if (enemy != null)
                    {
                        enemy.TakeDamage(100f);
                    }
                }
                GameManager.instance.ShowMessage("파괴 마법을 걸었습니다!");
                CameraShakeComponent.instance.StartShake();
            }
            else
            {
                GameManager.instance.ShowMessage("회복 마법을 걸었습니다!");
                CameraShakeComponent.instance.StartShake();
                AudioManager.instance.PlaySFX("P_Heal");
            }
        }

        foreach (Tower tower in towers)
        {
            if (tower != null)
            {
                tower.hp = tower.maxHp; // 체력 회복, 최대 체력 초과 방지
                GameObject effectInstance = GameManager.instance.pool.Get(15);
                effectInstance.transform.position = tower.transform.position;
                effectInstance.SetActive(true);
            }
        }
    }

    public void DestroyAllTower()
    {
        Tower[] towers = towerParent.GetComponentsInChildren<Tower>();

        foreach (Tower tower in towers)
        {
            GameManager.instance.Gold += tower.price;
            tower.RemoveTower();
            AudioManager.instance.PlaySFX("Sell");
            CameraShakeComponent.instance.StartShake();
        }
    }

    public void UpgradeAllTower(float v)
    {
        Tower[] towers = towerParent.GetComponentsInChildren<Tower>();

        foreach (Tower tower in towers)
        {
            tower.damage += tower.damage * v;
            tower.maxHp += tower.maxHp * v;
            tower.hp += tower.hp * v;
            tower.range += tower.range * v;
            tower.speed *= 1f - v;
        }
        TowerMaker.instance.upgradeVal += v;
    }
}