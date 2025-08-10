using UnityEngine;

public class HealButtonText : MonoBehaviour
{
    public UnityEngine.UI.Text healButtonText;
    
    private void Update()
    {
        if (TowerManager.instance.sale)
        {
            healButtonText.text = "회복(3G)";
        }
        else
        {
            healButtonText.text = "회복(5G)";
        }
    }
}
