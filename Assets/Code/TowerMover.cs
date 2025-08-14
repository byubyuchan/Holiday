using UnityEngine;

public class TowerMover : MonoBehaviour
{
    [SerializeField] private TowerSelector towerSelector;
    public Tower selectedTower; // 현재 선택된 타워
    public Tile tile;
    public bool IsMoving; // 이동 모드 여부

    private void Update()
    {
        // 단축키 입력 체크
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartMove();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TowerSell();
        }

        // 필요하면 다른 단축키도 추가 가능
    }


    public void StartMove()
    {
        if (GameManager.instance.isStart)
        {
            GameManager.instance.ShowMessage("전투 중에는 이동이 불가합니다!");
            AudioManager.instance.PlaySFX("Cant2");
            CameraShakeComponent.instance.StartShake();
            return;
        }
        // Hover된 타일에 타워가 있으면 selectedTower 갱신
        if (towerSelector.selectedTile != null && towerSelector.selectedTile.GetComponent<Tile>().currentTower != null)
        {
            selectedTower = towerSelector.selectedTile.GetComponent<Tile>().currentTower;
        }

        if (selectedTower == null)
        {
            GameManager.instance.ShowMessage("이동할 타워를 먼저 선택하세요!");
            AudioManager.instance.PlaySFX("Cant2");
            return;
        }

        GameManager.instance.ShowMessage("이동할 위치를 선택하세요!");
        AudioManager.instance.PlaySFX("Select");
        IsMoving = true;
    }

    public void MoveToTile(Tile tile)
    {
        if (!IsMoving || selectedTower == null)
        {
            EndMove();
            return;
        }

        GameManager.instance.ShowMessage("위치를 이동하였습니다!");
        AudioManager.instance.PlaySFX("Select");
        if (tile.currentTower == null) // 빈 타일이면 이동
        {
            selectedTower.MoveToTile(tile, false);
            EndMove();
        }
        else // 다른 타워가 있다면 위치 교환
        {
            selectedTower.SwapWithTower(tile.currentTower);
            EndMove();
        }
    }

    public void TowerSell()
    {
        if (GameManager.instance.isStart)
        {
            GameManager.instance.ShowMessage("전투 중에는 용사를 판매할 수 없습니다!");
            AudioManager.instance.PlaySFX("Cant2");
            CameraShakeComponent.instance.StartShake();
            return;
        }
        if (selectedTower != null || GameManager.instance.isStart)
        {
            selectedTower.RemoveTower();
            GameManager.instance.Gold += selectedTower.price;
            selectedTower = null;
            TowerInfo.instance.HideUI();
            towerSelector.ResetTile(true);
            GameManager.instance.ShowMessage("용사를 판매했습니다!");
            AudioManager.instance.PlaySFX("Sell");
        }
    }

    public void EndMove()
    {
        selectedTower = null;
        IsMoving = false;
    }
}
