using UnityEngine;

public class ObjectDetector : MonoBehaviour
{

    [SerializeField]
    public TowerMaker towerMaker;
    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.CompareTag("Tile"))
                {
                    towerMaker.SpawnTower(hit.transform);
                }
                else Debug.Log("타일맵이 아닙니다.");
            }
            else Debug.Log("레이캐스트된 부분이 없습니다.");
        }
    }
}
