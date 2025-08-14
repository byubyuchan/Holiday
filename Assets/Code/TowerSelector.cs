using UnityEngine;

public class TowerSelector : MonoBehaviour
{
    [SerializeField] private TowerMaker towerMaker;
    [SerializeField] private TowerMover towerMover;
    [SerializeField] private TowerInfo towerInfoUI;
    public Transform selectedTile;
    private int defaultSortingOrder = -5;

    public void HoverTile(Transform tileTransform)
    {
        if (!tileTransform.CompareTag("Tile"))
        {
            towerInfoUI.UpdateTowerInfo(null);
            return;
        }

        Tile tile = tileTransform.GetComponent<Tile>();

        // 타일 선택 표시
        if (selectedTile != null && selectedTile != tileTransform) ResetTile(false);

        selectedTile = tileTransform;
        SetTileSorting(selectedTile, 0);

        // 이동 모드일 때는 selectedTower를 덮어쓰지 않음
        if (!towerMover.IsMoving)
        {
            if (tile.currentTower != null)
            {
                towerMover.selectedTower = tile.currentTower;
                towerInfoUI.UpdateTowerInfo(tile.currentTower);
            }
            else
            {
                towerInfoUI.UpdateTowerInfo(null);
            }
        }
        else
        {
            // 이동 모드라면 단순히 UI만 갱신
            if (tile.currentTower != null)
                towerInfoUI.UpdateTowerInfo(tile.currentTower);
            else
                towerInfoUI.UpdateTowerInfo(null);
        }


    }

    public void SelectTile(Transform tileTransform)
    {
        if (!tileTransform.CompareTag("Tile")) return;

        Tile tile = tileTransform.GetComponent<Tile>();

        if (towerMover.IsMoving) // 이동 모드인 경우
        {
            towerMover.MoveToTile(tile); // 이동 또는 교환 처리
            ResetTile(true);
            return;
        }

        towerMover.tile = tile;

        if (selectedTile == tileTransform) // 같은 타일 클릭 시 타워 설치
        {
            towerMaker.SpawnTower(tile);
            ResetTile(true);
        }
    }

    public void ResetTile(bool resetTower)
    {
        if (selectedTile != null)
        {
            SetTileSorting(selectedTile, defaultSortingOrder);
            selectedTile = null;
        }

        if (resetTower) towerMover.selectedTower = null;
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
