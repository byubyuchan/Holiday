using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private Transform towerParent; // 부모 오브젝트

    public static TowerManager instance;
    public bool sale = false;

    private void Awake()
    {
        instance = this;
    }

    public void HealAllTowers()
    {
        if (CutsceneManager.instance.cutsceneflag == 1) return;
        Tower[] towers = towerParent.GetComponentsInChildren<Tower>(); // 부모의 모든 자식 타워 가져오기
        if (sale)
        {
            if (GameManager.instance.Gold < 3)
            {
                GameManager.instance.ShowMessage("골드가 모자랍니다!");
                return;
            }
            GameManager.instance.Gold -= 3;
            GameManager.instance.ShowMessage("회복 마법을 걸었습니다!");
            CameraShakeComponent.instance.StartShake();
            AudioManager.instance.PlaySFX("P_Heal");
        }
        else
        {
            if (GameManager.instance.Gold < 5)
            {
                GameManager.instance.ShowMessage("골드가 모자랍니다!");
                return;
            }
            GameManager.instance.Gold -= 5;
            GameManager.instance.ShowMessage("회복 마법을 걸었습니다!");
            CameraShakeComponent.instance.StartShake();
            AudioManager.instance.PlaySFX("P_Heal");
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
}