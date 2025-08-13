using UnityEngine;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
    [SerializeField]
    public InputField playerNameInput;
    [SerializeField]
    DataBaseConnectingTest dbConnector;

    private string playerName = null;

    private void Awake()
    {
        playerName = playerNameInput.GetComponent<InputField>().text;
    }

    private void Update()
    {
        // 엔터 키를 눌렀을 때 이름 입력 버튼을 호출
        if (playerName.Length > 0 && Input.GetKeyDown(KeyCode.Return))
        {
            InputNameButton();
        }
    }

    public void InputNameButton()
    {
        playerName = playerNameInput.text;
        dbConnector.saveName(playerName);
        GameManager.instance.playerName = playerName;
        gameObject.SetActive(false);
    }
}
