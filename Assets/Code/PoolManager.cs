using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 프리펩들을 보관할 배열
    public GameObject[] prefabs;
    // 풀 담당을 하는 리스트 배열
    public List<GameObject>[] pools;
    // 변수와 리스트는 1:1 비율
    private void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];
        for (int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<GameObject>();
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;
        // 선택한 풀의 비활성 게임 오브젝트 접근
        foreach (GameObject item in pools[index])
        {
            // 발견된다면 select 변수에 할당
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        // 만약 모두 사용되고 있다면?
        if (!select)
        {
            // 새롭게 생성하여 select 변수에 할당 / transform으로 PoolManager 하위로 할당한다.
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }

        return select;
    }
}
