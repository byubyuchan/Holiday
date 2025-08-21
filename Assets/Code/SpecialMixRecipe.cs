using UnityEngine;
using System.Collections.Generic;

//조합에 필요한 개별 재료 정보
[System.Serializable]
public class RecipeIngredient
{
    public string towerType; // 타워 종류 (예: "Warrior", "Mage", "Tank")
    public string cost;      // 타워 등급 (예: "A", "B", "C")
    public int quantity;     // 필요한 개수
}
public enum SpecialTowerType
{
    WarriorPrincess, // 여전사
    King,            // 왕
    Monk,            // 수도승
    Bringer          // 브링어
}

//조합법 전체를 담는 스크립터블 오브젝트
[CreateAssetMenu(fileName = "New Special Mix Recipe", menuName = "Tower/Special Mix Recipe")]
public class SpecialMixRecipe : ScriptableObject
{
    public string recipeName; // 조합법 이름 (예: "여전사 조합")
    public List<RecipeIngredient> ingredients; // 필요한 재료 목록
    public SpecialTowerType ResultType;
    public GameObject resultTowerPrefab; // 조합 결과로 나올 타워 프리팹
    public int resultTowerIndex;
}