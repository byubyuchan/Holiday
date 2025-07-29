using UnityEngine;

public class FrameManager : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void OnDropdownEvent(int value)
    {
        // 드롭다운에서 선택된 값에 따라 프레임 레이트를 설정
        switch (value)
        {
            case 0: // 30 FPS
                Application.targetFrameRate = 30;
                break;
            case 1: // 60 FPS
                Application.targetFrameRate = 60;
                break;
            case 2: // 144 FPS
                Application.targetFrameRate = 144;
                break;
            case 3: // 240 FPS
                Application.targetFrameRate = 240;
                break;
            default: // 기본값은 240 FPS 테스트용이므로 수정요망
                Application.targetFrameRate = 240;
                break;
        }
    }
}
