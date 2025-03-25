using UnityEngine;

public class TileSelector : MonoBehaviour
{
    [SerializeField] private TowerMaker towerMaker;
    private Transform selectedTile;
    private int defaultSortingOrder = -5;

    public void SelectTile(Transform tileTransform)
    {
        if (!tileTransform.CompareTag("Tile")) return;

        Tile tile = tileTransform.GetComponent<Tile>();

        if (selectedTile == null)
        {
            selectedTile = tileTransform;
            SetTileSorting(selectedTile, 0);
        }
        else if (selectedTile == tileTransform)
        {
            towerMaker.SpawnTower(tile);
            ResetTile();
        }
        else
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
        SpriteRenderer tileRenderer = tile.GetComponent<SpriteRenderer>();
        if (tile == null) return;
        {
            tileRenderer.sortingOrder = sortingOrder;
            tileRenderer.color = new Color(tileRenderer.color.r, tileRenderer.color.g, tileRenderer.color.b, 0.5f); // 알파값으로 투명도 조절
        }
    }
}
