using UnityEngine;

public class TowerMaker : MonoBehaviour
{
    [SerializeField]
    public GameObject towerPrefab;
    
    public void SpawnTower(Transform tileTransform)
    {
        Instantiate(towerPrefab, tileTransform.position, Quaternion.identity);
    }
}
