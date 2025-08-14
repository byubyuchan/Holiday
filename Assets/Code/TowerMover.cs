using UnityEngine;

public class TowerMover : MonoBehaviour
{
    [SerializeField] private TowerSelector towerSelector;
    public Tower selectedTower; // 현재 선택된 타워
    public Tile tile;
    public bool IsMoving; // 이동 모드 여부

    public void StartMove()
    {
        if (GameManager.instance.isStart)
        {
            GameManager.instance.ShowMessage("전투 중에는 이동이 불가합니다!");
            AudioManager.instance.PlaySFX("Cant2");
            CameraShakeComponent.instance.StartShake();
            return;
        }
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
            towerSelector.ResetTile();
            GameManager.instance.ShowMessage("용사를 판매했습니다!");
        }
    }

    public void EndMove()
    {
        selectedTower = null;
        IsMoving = false;
    }
}
