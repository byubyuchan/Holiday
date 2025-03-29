using UnityEngine;

public class TowerSelector : MonoBehaviour
{
    [SerializeField] private TowerMaker towerMaker;
    [SerializeField] private TowerMover towerMover;
    [SerializeField] private TowerInfo towerInfoUI;
    public Transform selectedTile;
    private int defaultSortingOrder = -5;

    public void SelectTile(Transform tileTransform)
    {
        if (!tileTransform.CompareTag("Tile")) return;

        Tile tile = tileTransform.GetComponent<Tile>();

        if (towerMover.IsMoving) // 타워 이동 모드인 경우
        {
            towerMover.MoveToTile(tile); // 타워 이동 또는 교환 처리
            ResetTile();
            return;
        }

        towerMover.tile = tile;
        towerMover.selectedTower = tile.currentTower;

        if (tile.currentTower != null) // 타일에 타워가 있는 경우
        {
            towerInfoUI.UpdateTowerInfo(tile.currentTower); // UI 업데이트
            SetTileSorting(selectedTile, 0);
        }
        else // 타일에 타워가 없는 경우
        {
            towerInfoUI.UpdateTowerInfo(null); // UI 숨김 처리
        }

        if (selectedTile == null) // 처음 선택한 타일
        {
            selectedTile = tileTransform;
            SetTileSorting(selectedTile, 0);
        }
        else if (selectedTile == tileTransform) // 같은 타일 클릭 시 타워 설치
        {
            towerMaker.SpawnTower(tile);
            ResetTile();
        }
        else // 다른 타일 클릭 시 선택 변경
        {
            ResetTile();
            selectedTile = tileTransform;
            SetTileSorting(selectedTile, 0);
        }
    }

    public void ResetTile()
    {
        if (selectedTile != null)
        {
            SetTileSorting(selectedTile, defaultSortingOrder);
            selectedTile = null;
        }
    }

    private void SetTileSorting(Transform tile, int sortingOrder)
    {
        if (tile == null) return;
        SpriteRenderer tileRenderer = tile.GetComponent<SpriteRenderer>();
        if (tileRenderer == null) return;
        {
            tileRenderer.sortingOrder = sortingOrder;
            tileRenderer.color = new Color(tileRenderer.color.r, tileRenderer.color.g, tileRenderer.color.b, 0.5f); // 알파값으로 투명도 조절
        }
    }
}
