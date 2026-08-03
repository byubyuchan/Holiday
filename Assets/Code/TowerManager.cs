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
        if (CutsceneManager.instance.cutsceneflag == 1)
            return;

        int cost = sale ? 3 : 5;

        if (GameManager.instance.Gold < cost)
        {
            GameManager.instance.ShowMessage("골드가 모자랍니다!");
            AudioManager.instance.PlaySFX("Cant");
            CameraShakeComponent.instance.StartShake();
            return;
        }

        GameManager.instance.Gold -= cost;

        if (reverse)
        {
            Enemy[] enemies = enemyParent.GetComponentsInChildren<Enemy>();

            foreach (Enemy enemy in enemies)
            {
                if (enemy != null)
                    enemy.TakeDamage(100f);
            }

            GameManager.instance.ShowMessage("파괴 마법을 걸었습니다!");
            CameraShakeComponent.instance.StartShake();
            return;
        }

        Tower[] towers = towerParent.GetComponentsInChildren<Tower>();

        foreach (Tower tower in towers)
        {
            if (tower == null)
                continue;

            tower.hp = tower.maxHp;

            GameObject effectInstance = GameManager.instance.pool.Get(15);
            effectInstance.transform.position = tower.transform.position;
            effectInstance.SetActive(true);
        }

        GameManager.instance.ShowMessage("회복 마법을 걸었습니다!");
        CameraShakeComponent.instance.StartShake();
        AudioManager.instance.PlaySFX("P_Heal");
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
                if (tower == null)
                    continue;

                if (tower.cost != "C")
                {
                    AllC = false;
                    break;
                }
            }
        }
        else
        {
            AllC = false;
        }

        float finalUpgradeValue = v;

        if (AllC)
        {
            finalUpgradeValue += 0.5f;
        }

        foreach (Tower tower in towers)
        {
            if (tower == null)
                continue;

            float damageBonus = finalUpgradeValue;
            float hpBonus = finalUpgradeValue;
            float rangeBonus = finalUpgradeValue;
            float speedBonus = finalUpgradeValue;

            if (tower.towerType == "Range")
            {
                if (bigProjectile)
                {
                    // 베이스 공격력의 100% 추가
                    damageBonus += 1f;

                    // 베이스 공격 간격의 100% 추가: 느려짐
                    speedBonus -= 1f;
                }

                if (smallProjectile)
                {
                    // 베이스 공격력의 50% 감소
                    damageBonus -= 0.5f;

                    // 베이스 공격 간격의 50% 감소: 빨라짐
                    speedBonus += 0.5f;
                }
            }

            tower.damage = tower.baseDamage * (1f + damageBonus);
            tower.maxHp = tower.baseMaxHp * (1f + hpBonus);
            tower.range = tower.baseRange * (1f + rangeBonus);

            tower.speed = Mathf.Max(
                tower.baseSpeed * (1f - speedBonus),
                0.2f
            );
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

    public void HealSkill()
    {
        Tower[] towers = towerParent.GetComponentsInChildren<Tower>();

        foreach (Tower tower in towers)
        {
            if (tower != null)
            {
                tower.hp = Mathf.Min(tower.hp + 200, tower.maxHp);
                GameManager.instance.PlayEffect(15, tower.transform.position);
            }
        }
    }

    public void SlowEnemy()
    {
        Enemy[] enemys = enemyParent.GetComponentsInChildren<Enemy>();
        foreach (Enemy enemy in enemys)
        {
            if (enemy != null)
            {
                DamageFlashEffect flashEffect = enemy.GetComponent<DamageFlashEffect>();

                if (flashEffect != null)
                {
                    flashEffect.SetSlowEffect(true);
                }

                enemy.speed *= 0.5f;
                enemy.speed = Mathf.Min(enemy.speed, 3f);
                enemy.TakeDamage(50f);
            }
        }
    }

    public void ResetAnimSpeed()
    {
        Tower[] towers = towerParent.GetComponentsInChildren<Tower>();

        foreach (Tower tower in towers)
        {
            if (tower != null)
            {
                Animator anim = tower.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.speed = 1f;
                }
            }
        }
    }
}