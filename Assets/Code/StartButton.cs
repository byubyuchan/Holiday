using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        // Start the loading process for the main game scene
        LoadingSceneController.Instance.LoadScene("MainScene");
    }
}
