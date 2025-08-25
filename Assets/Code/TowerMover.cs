using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TowerMover : MonoBehaviour
{
    [SerializeField] private TowerSelector towerSelector;
    [SerializeField] private List<SpecialMixRecipe> specialMixRecipes;

    public Tower selectedTower; // 현재 선택된 타워
    public Tile tile;
    public bool IsMoving; // 이동 모드 여부

    private void Update()
    {
        // 단축키 입력 체크
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartMove();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TowerSell();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            TowerLevelUp();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            SpecialMix();
        }

        // 필요하면 다른 단축키도 추가 가능
    }


    public void StartMove()
    {
        if (CutsceneManager.instance.cutsceneflag == 1) return;

        if (TowerMaker.instance.cantMove)
        {
            GameManager.instance.ShowMessage("저주로 인해 이동이 불가능합니다!");
            AudioManager.instance.PlaySFX("Cant");
            CameraShakeComponent.instance.StartShake();
            return;
        }

        if (GameManager.instance.isStart)
        {
            GameManager.instance.ShowMessage("전투 중에는 이동이 불가능합니다!");
            AudioManager.instance.PlaySFX("Cant2");
            CameraShakeComponent.instance.StartShake();
            return;
        }

        // Hover된 타일에 타워가 있으면 selectedTower 갱신
        if (towerSelector.selectedTile != null && towerSelector.selectedTile.GetComponent<Tile>().currentTower != null)
        {
            selectedTower = towerSelector.selectedTile.GetComponent<Tile>().currentTower;
        }

        if (selectedTower == null)
        {
            GameManager.instance.ShowMessage("이동할 타워를 먼저 선택하세요!");
            AudioManager.instance.PlaySFX("Cant2");
            return;
        }

        GameManager.instance.ShowMessage("이동할 위치를 선택하세요!");
        AudioManager.instance.PlaySFX("Select");
        IsMoving = true;
    }

    public void MoveToTile(Tile tile)
    {
        if (!IsMoving || selectedTower == null)
        {
            EndMove();
            return;
        }

        GameManager.instance.ShowMessage("위치를 이동하였습니다!");
        AudioManager.instance.PlaySFX("Select");
        if (tile.currentTower == null) // 빈 타일이면 이동
        {
            selectedTower.MoveToTile(tile, false);
            EndMove();
        }
        else // 다른 타워가 있다면 위치 교환
        {
            selectedTower.SwapWithTower(tile.currentTower);
            EndMove();
        }
    }

    public void TowerSell()
    {
        if (CutsceneManager.instance.cutsceneflag == 1) return;

        if (TowerManager.instance.cantSell)
        {
            GameManager.instance.ShowMessage("저주로 인해 판매할 수 없습니다!");
            AudioManager.instance.PlaySFX("Cant");
            CameraShakeComponent.instance.StartShake();
            return;
        }

        if (GameManager.instance.isStart)
        {
            GameManager.instance.ShowMessage("전투 중에는 용사를 판매할 수 없습니다!");
            AudioManager.instance.PlaySFX("Cant2");
            CameraShakeComponent.instance.StartShake();
            return;
        }

        if (selectedTower != null)
        {
            selectedTower.RemoveTower();
            GameManager.instance.Gold += selectedTower.price;
            selectedTower = null;
            TowerInfo.instance.HideUI();
            towerSelector.ResetTile(true);
            GameManager.instance.ShowMessage("용사를 판매했습니다!");
            AudioManager.instance.PlaySFX("Sell");
            StartCoroutine(DelayUpgradeCheck());
        }
    }
    private IEnumerator DelayUpgradeCheck()
    {
        yield return null; // 한 프레임 대기
        TowerManager.instance.UpgradeAllTower(TowerMaker.instance.upgradeVal);
    }

    public void EndMove()
    {
        selectedTower = null;
        IsMoving = false;
    }

    public void TowerLevelUp()
    {

        if (CutsceneManager.instance.cutsceneflag == 1) return;

        if (GameManager.instance.isStart)
        {
            GameManager.instance.ShowMessage("전투 중에는 강화가 불가능합니다!");
            AudioManager.instance.PlaySFX("Cant2");
            CameraShakeComponent.instance.StartShake();
            return;
        }

        if (selectedTower.cost == "A")
        {
            GameManager.instance.ShowMessage("이미 최고 등급입니다!");
            AudioManager.instance.PlaySFX("Cant");
            CameraShakeComponent.instance.StartShake();
            return;
        }

        Tower[] allTowers = Object.FindObjectsByType<Tower>(FindObjectsSortMode.None);
        List<Tower> matchingTowers = new List<Tower>();

        foreach (Tower tower in allTowers)
        {
            if (tower.cost == selectedTower.cost && tower.anim.runtimeAnimatorController == selectedTower.anim.runtimeAnimatorController)
            {
                matchingTowers.Add(tower);
            }
        }

        if (matchingTowers.Count >= 5)
        {
            int removeCnt = 4;
            // selectedTower를 제외한 나머지 제거
            foreach (Tower tower in matchingTowers)
            {
                if (tower != selectedTower)
                {
                    tower.RemoveTower();
                    removeCnt--;
                }
                if (removeCnt == 0) break;
            }
            selectedTower.LevelUp();
        }
        else
        {
            GameManager.instance.ShowMessage("같은 종류의 타워가 5개 이상 필요합니다!");
            AudioManager.instance.PlaySFX("Cant");
            CameraShakeComponent.instance.StartShake();
        }
    }

    public void SpecialMix()
    {
        if (CutsceneManager.instance.cutsceneflag == 1) return;

        if (selectedTower == null)
        {
            GameManager.instance.ShowMessage("조합할 용사를 먼저 선택하세요!");
            AudioManager.instance.PlaySFX("Cant2");
            return;
        }
    
        if (GameManager.instance.isStart)
        {
            GameManager.instance.ShowMessage("전투 중에는 조합이 불가능합니다!");
            AudioManager.instance.PlaySFX("Cant2");
            return;
        }

        

        // 2. 현재 맵에 있는 모든 타워 목록을 가져옵니다.
        List<Tower> allTowersOnField = new List<Tower>(Object.FindObjectsByType<Tower>(FindObjectsSortMode.None));

            // 3. 실행 가능한 레시피를 찾습니다.
            SpecialMixRecipe foundRecipe = FindMatchingRecipe(allTowersOnField);

        if (foundRecipe != null)
        {
            // 4. 레시피를 찾았다면, 조합을 실행합니다.
            ConsumeAndCreate(foundRecipe, allTowersOnField);
            GameManager.instance.ShowMessage(foundRecipe.recipeName + " 조합 성공!");
            AudioManager.instance.PlaySFX("LevelUp"); // 조합 성공 효과음
        }
        else
        {
            // 5. 가능한 레시피가 없다면, 메시지를 표시합니다.
            GameManager.instance.ShowMessage("특수조합에 필요한 영웅이 부족합니다!");
            AudioManager.instance.PlaySFX("Cant");
            CameraShakeComponent.instance.StartShake();
        }
    }

    // selectedTower를 기준으로 조합 가능한 레시피를 찾는 함수
    private SpecialMixRecipe FindMatchingRecipe(List<Tower> allTowers)
    {
        // 모든 레시피를 하나씩 확인
        foreach (var recipe in specialMixRecipes)
        {
            // 현재 선택한 타워가 이 레시피의 재료 중 하나인지 확인
            bool isIngredient = false;
            foreach (var ingredient in recipe.ingredients)
            {
                if (ingredient.towerType == selectedTower.towerType && ingredient.cost == selectedTower.cost)
                {
                    isIngredient = true;
                    break;
                }
            }

            // 선택한 타워가 재료가 아니라면, 다음 레시피로 넘어감
            if (!isIngredient) continue;

            // 재료가 맞다면, 맵 전체에 모든 재료가 충분한지 확인
            if (CheckAllIngredients(recipe, allTowers))
            {
                return recipe; // 모든 재료가 충분하면, 이 레시피를 반환
            }
        }

        return null; // 가능한 레시피가 없으면 null 반환
    }


    // 레시피에 필요한 모든 재료가 맵에 있는지 확인하는 함수
    private bool CheckAllIngredients(SpecialMixRecipe recipe, List<Tower> allTowers)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            // 맵에 있는 타워들 중, 현재 재료와 일치하는 타워의 개수를 셉니다.
            int count = allTowers.FindAll(tower => 
                tower.towerType == ingredient.towerType && tower.cost == ingredient.cost
            ).Count;

            // 센 개수가 필요한 개수보다 적으면, 이 레시피는 불가능합니다.
            if (count < ingredient.quantity)
            {
                return false;
            }
        }

        // 모든 재료가 충분하면 true를 반환합니다.
        return true;
    }

        // 재료를 소모하고 결과물을 생성하는 함수 (수정된 버전)
    private void ConsumeAndCreate(SpecialMixRecipe recipe, List<Tower> allTowersOnField)
    {
        Tile originalTile = selectedTower.tile; // 새 타워가 생성될 위치 저장

        // 소모할 타워들을 관리하기 위해 원본 리스트를 복사합니다.
        List<Tower> availableTowers = new List<Tower>(allTowersOnField);

        // 1. 선택된 타워를 최우선으로 소모 목록에서 제거합니다.
        availableTowers.Remove(selectedTower);
        selectedTower.RemoveTower();

        // 2. 레시피의 각 재료를 순회하며 나머지 재료들을 소모합니다.
        foreach (var ingredient in recipe.ingredients)
        {
            // 현재 재료가 selectedTower와 동일한 종류인지 확인합니다.
            if (ingredient.towerType == selectedTower.towerType && ingredient.cost == selectedTower.cost)
            {
                // selectedTower는 이미 소모했으므로, 필요한 수량에서 1개를 뺍니다.
                int neededQuantity = ingredient.quantity - 1;

                // 추가로 더 필요한 만큼만 필드에서 찾아 제거합니다.
                for (int i = 0; i < neededQuantity; i++)
                {
                    // availableTowers 리스트에서 일치하는 타워를 찾습니다.
                    Tower towerToConsume = availableTowers.Find(t => t.towerType == ingredient.towerType && t.cost == ingredient.cost);
                    if (towerToConsume != null)
                    {
                        availableTowers.Remove(towerToConsume);
                        towerToConsume.RemoveTower();
                    }
                }
            }
            else
            {
                // selectedTower와 다른 종류의 재료는 필요한 수량만큼 모두 제거합니다.
                for (int i = 0; i < ingredient.quantity; i++)
                {
                    Tower towerToConsume = availableTowers.Find(t => t.towerType == ingredient.towerType && t.cost == ingredient.cost);
                    if (towerToConsume != null)
                    {
                        availableTowers.Remove(towerToConsume);
                        towerToConsume.RemoveTower();
                    }
                }
            }
        }

        if (originalTile != null)
        {
            GameObject newTowerObj = Instantiate(recipe.resultTowerPrefab, originalTile.transform.position, Quaternion.identity);
            newTowerObj.transform.SetParent(TowerMaker.instance.towerParent);
            Tower newTower = newTowerObj.GetComponent<Tower>();

            newTower.InitAsSpecial(recipe.resultTowerIndex);
            newTower.MoveToTile(originalTile, false);
        }

        selectedTower = null;
        TowerInfo.instance.HideUI();
        towerSelector.ResetTile(true);
        StartCoroutine(DelayUpgradeCheck());
        }
}
