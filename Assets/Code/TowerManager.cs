using System.Collections;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private Transform towerParent; // 부모 오브젝트
    [SerializeField] private Transform enemyParent; // 몬스터 오브젝트

    public static TowerManager instance;
    public bool sale = false;
    public bool reverse = false;
    public bool IsAttackChange = false;
    public bool buttonRandom = false;
    public bool cantSell = false;
    public bool bigProjectile = false;
    public bool smallProjectile = false;
    public bool IsAllC = false;
    public bool AllC = false;
    public bool reSell = false;
    public bool forcedSale = false;

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

    public void TwiceSellAllTower()
    {
        Tower[] towers = towerParent.GetComponentsInChildren<Tower>();

        foreach (Tower tower in towers)
        {
            GameManager.instance.Gold += tower.price * 2;
            tower.RemoveTower();
            AudioManager.instance.PlaySFX("Sell");
            CameraShakeComponent.instance.StartShake();
        }
    }

    public void UpgradeAllTower(float v)
    {
        Debug.Log("업그레이드 갱신!");
        Tower[] towers = towerParent.GetComponentsInChildren<Tower>();

        if (IsAllC)
        {
            AllC = true;
            foreach (Tower tower in towers)
            {
                if (tower == null) continue;

                if (tower.cost != "C")
                {
                    AllC = false;
                    break;
                }
            }
        }

        if (AllC) v += 0.5f;

        foreach (Tower tower in towers)
        {
            tower.damage = tower.baseDamage * (1 + v);
            tower.maxHp = tower.baseMaxHp * (1 + v);
            // 체력 = max 체력을 하면 풀회복이 되고, 안하면 소환했을 때 maxhp를 받지 않고 basehp만 받게됨.
            tower.range = tower.baseRange * (1 + v);
            tower.speed = Mathf.Max(tower.baseSpeed * (1f - v),0.2f); // 최대 공속 0.2f
            if (bigProjectile)
            {
                if (tower.towerType == "Range")
                {
                    tower.damage += tower.baseDamage;
                    tower.speed += tower.baseSpeed;
                }
            }
            if (smallProjectile)
            {
                if (tower.towerType == "Range")
                {
                    tower.damage -= tower.baseDamage * 0.5f;
                    tower.speed = Mathf.Max(tower.speed - (tower.baseSpeed * 0.5f),0.2f);
                }
            }
        }
    }

    public void ChangeAttackFormToWarrior()
    {
        Tower[] towers = towerParent.GetComponentsInChildren<Tower>();

        foreach (Tower tower in towers)
        {
            if (tower.towerType == "Melee")
            {
                tower.IsAttackChange = true;
            }
        }

        IsAttackChange = true;
    }

    public void ActivateBurningTalent()
    {
        StartCoroutine(BurningRoutine());
    }

    private IEnumerator BurningRoutine()
    {
        
        while (true)
        {
            UpgradeAllTower(TowerMaker.instance.upgradeVal);

            if (!GameManager.instance.isStart)
            {
                yield return null; // 한 프레임 쉬고 다시 체크
                continue; // 아래 로직 건너뛰고 while 루프 재진입
            }

            Tower[] towers = towerParent.GetComponentsInChildren<Tower>();

            foreach (Tower tower in towers)
            {
                if (tower == null) continue;

                tower.hp = Mathf.Max(1, tower.hp - 3f);

                tower.TakeDamage(0f);

                tower.ApplyHpScaling();
            }

            yield return new WaitForSeconds(1f); // 초당 1번
        }
    }

    public void ActivateHealingTalent()
    {
        StartCoroutine(HealingRoutine());
    }

    private IEnumerator HealingRoutine()
    {

        while (true)
        {
            if (!GameManager.instance.isStart)
            {
                yield return null; // 한 프레임 쉬고 다시 체크
                continue; // 아래 로직 건너뛰고 while 루프 재진입
            }

            Tower[] towers = towerParent.GetComponentsInChildren<Tower>();

            foreach (Tower tower in towers)
            {
                tower.hp = Mathf.Min(tower.hp + 50,tower.maxHp ); // 체력 회복, 최대 체력 초과 방지
                GameObject effectInstance = GameManager.instance.pool.Get(15);
                effectInstance.transform.position = tower.transform.position;
                effectInstance.SetActive(true);

            }
            CameraShakeComponent.instance.StartShake();
            AudioManager.instance.PlaySFX("P_Heal");
            yield return new WaitForSeconds(5f); // 초당 1번
        }
    }

    public void TwicePrice()
    {
        Tower[] towers = towerParent.GetComponentsInChildren<Tower>();

        foreach (Tower tower in towers)
        {
            tower.price = Mathf.Min(tower.price * 2, 40);
        }
    }
}