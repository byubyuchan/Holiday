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
    [SerializeField] private Text SellText; // 판매용 텍스트
    [SerializeField] private Image towerIcon; // 타워 아이콘 이미지


    public Tower infoTower;
    public static TowerInfo instance;
    private void Start()
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

        if (infoTower) infoTower.HideRange();

        infoTower = tower;

        // 선택된 타워 정보 업데이트
        infoPanel.SetActive(true);
        towerTypeText.text = $"등급 : {tower.cost}";
        hpText.text = $"체력 : {(int)tower.hp}/{(int)tower.maxHp}";
        speedText.text = $"공격속도 : {tower.speed:F2}";
        damageText.text = $"공격력 : {tower.damage:F2}";
        rangeText.text = $"사거리 : {tower.range:F2}";
        SellText.text = $"판매 : +{tower.price}G (Q)";

        infoTower.ShowRange();

        // 아이콘 업데이트
        SpriteRenderer spriteRenderer = tower.GetComponent<SpriteRenderer>(); // 타워의 SpriteRenderer 가져오기
        if (spriteRenderer != null)
        {
            towerIcon.sprite = spriteRenderer.sprite; // 아이콘에 스프라이트 적용
            AdjustIconSize(spriteRenderer.sprite); // 아이콘 크기 조정
        }
    }

    public void Update()
    {
        if (infoPanel.activeSelf)
        {
            towerTypeText.text = $"등급 : {infoTower.cost}";
            hpText.text = $"체력 : {(int)infoTower.hp}/{(int)infoTower.maxHp}";
            speedText.text = $"공격속도 : {infoTower.speed:F2}";
            damageText.text = $"공격력 : {infoTower.damage:F2}";
            rangeText.text = $"사거리 : {infoTower.range:F2}";
            SellText.text = $"판매 : +{infoTower.price}G (Q)";

            // 실시간으로 아이콘 업데이트 (필요 시)
            SpriteRenderer spriteRenderer = infoTower.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                towerIcon.sprite = spriteRenderer.sprite;
            }
        }
    }

    private void AdjustIconSize(Sprite sprite)
    {
        if (sprite == null) return;
        RectTransform rectTransform = towerIcon.GetComponent<RectTransform>();
        // 스프라이트의 실제 크기 가져오기
        float width = sprite.rect.width;
        float height = sprite.rect.height;
        rectTransform.sizeDelta = new Vector2(Mathf.Min(width*2,400) , Mathf.Min(height * 2, 400));

    }

    public void HideUI()
    {
        if (infoTower != null)
        {
            infoTower.HideRange(); // RangeCircle 비활성화
            infoTower = null;
        }
        infoPanel.SetActive(false);
    }
}
