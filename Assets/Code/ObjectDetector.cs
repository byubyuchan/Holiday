using UnityEngine;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField] private TileSelector tileSelector;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                tileSelector.SelectTile(hit.transform);
            }
            else tileSelector.ResetTile();
        }
    }
}