using UnityEngine;
using UnityEngine.UI;

public class TowerInfo : MonoBehaviour
{
    [SerializeField] private GameObject infoPanel; // 정보 패널 (타워 정보 표시)
    [SerializeField] private Text towerTypeText; // 타워 유형 텍스트
    [SerializeField] private Text hpText; // 체력 텍스트
    [SerializeField] private Text speedText; // 속도 텍스트
    [SerializeField] private Text damageText; // 공격력 텍스트
    [SerializeField] private Text rangeText; // 사거리 텍스트

    public Tower infoTower;
    public static TowerInfo instance;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateTowerInfo(Tower tower)
    {
        if (tower == null) // 선택된 타워가 없으면 패널 숨김
        {
            infoPanel.SetActive(false);
            return;
        }
        infoTower = tower;

        // 선택된 타워 정보 업데이트
        infoPanel.SetActive(true);
        towerTypeText.text = $"타워 등급: {tower.towerType}";
        hpText.text = $"체력: {tower.hp}/{tower.maxHp}";
        speedText.text = $"공격속도: {tower.speed}";
        damageText.text = $"공격력: {tower.damage}";
        rangeText.text = $"사거리: {tower.range}";
    }

    public void Update()
    {
        if (infoPanel.activeSelf)
        {
            towerTypeText.text = $"타워 등급: {infoTower.towerType}";
            hpText.text = $"체력: {infoTower.hp}/{infoTower.maxHp}";
            speedText.text = $"공격속도: {infoTower.speed}";
            damageText.text = $"공격력: {infoTower.damage}";
            rangeText.text = $"사거리: {infoTower.range}";
        }
    }

    public void HideUI()
    {
        infoPanel.SetActive(false);
    }
}
