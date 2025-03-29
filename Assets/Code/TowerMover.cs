using UnityEngine;

public class TowerMover : MonoBehaviour
{
    [SerializeField] private TowerSelector towerSelector;
    public Tower selectedTower; // 현재 선택된 타워
    public Tile tile;
    public bool IsMoving; // 이동 모드 여부

    public void StartMove()
    {
        IsMoving = true;
    }

    public void MoveToTile(Tile tile)
    {
        if (!IsMoving || selectedTower == null)
        {
            EndMove();
            return;
        }

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
        if (selectedTower != null)
        {
            selectedTower.RemoveTower();
            GameManager.instance.Gold += 5;
            selectedTower = null;
            TowerInfo.instance.HideUI();
            towerSelector.ResetTile();
        }
    }

    public void EndMove()
    {
        selectedTower = null;
        IsMoving = false;
        Debug.Log("타워 이동 모드 비활성화");
    }
}
