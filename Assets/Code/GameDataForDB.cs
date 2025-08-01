using System;
using UnityEngine;
using UnityEngine.Events;

public class GameDataForDB : MonoBehaviour
{
    public UnityEvent<int> OnStageChanged;
    public UnityEvent<int> OnGoldChanged;

    private int _stage;
    private int _gold;
    //값이 변경되면 작동하게 함
    public int Stage
    {
        get => _stage;
        set
        {
            if (_stage != value)
            {
                _stage = value;
                OnStageChanged?.Invoke(_stage);
            }
        }
    }

    public int Gold
    {
        get => _gold;
        set
        {
            if (_gold != value)
            {
                _gold = value;
                OnGoldChanged?.Invoke(_gold);
            }
        }
    }
}
