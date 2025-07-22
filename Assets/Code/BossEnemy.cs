using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BossEnemy : Enemy
{
    [System.Serializable]
    public class BossSkill
    {
        public string skillName;
        public float cooldown;
        public float lastUseTime;
        public int weight;  // 가중치

        public System.Action useSkillAction; // 스킬 함수 델리게이트
    }

    public List<BossSkill> skills = new List<BossSkill>();

    void Start()
    {
        // 스킬 초기 설정
        skills.Add(new BossSkill { skillName = "Smash", cooldown = 5f, lastUseTime = -5f, weight = 30, useSkillAction = UseSmash });
        skills.Add(new BossSkill { skillName = "Fireball", cooldown = 8f, lastUseTime = -8f, weight = 50, useSkillAction = UseFireball });
        skills.Add(new BossSkill { skillName = "Roar", cooldown = 12f, lastUseTime = -12f, weight = 20, useSkillAction = UseRoar });
    }

    void Update()
    {
        if (!isLive) return;

        TryUseSkill();
    }

    void TryUseSkill()
    {
        // 사용 가능한 스킬 리스트
        List<BossSkill> availableSkills = new List<BossSkill>();

        foreach (var skill in skills)
        {
            if (Time.time - skill.lastUseTime >= skill.cooldown)
            {
                availableSkills.Add(skill);
            }
        }

        if (availableSkills.Count > 0)
        {
            // 가중치 기반 랜덤 선택
            BossSkill selectedSkill = ChooseWeightedSkill(availableSkills);
            selectedSkill.useSkillAction?.Invoke(); // null 체크 후 스킬 사용
            selectedSkill.lastUseTime = Time.time;
        }
    }

    BossSkill ChooseWeightedSkill(List<BossSkill> skills)
    {
        int totalWeight = 0;
        foreach (var skill in skills) totalWeight += skill.weight;

        int randomValue = Random.Range(0, totalWeight);
        int currentSum = 0;

        foreach (var skill in skills)
        {
            currentSum += skill.weight;
            if (randomValue < currentSum)
                return skill;
        }

        return skills[0]; // fallback
    }

    void UseSmash()
    {
        Debug.Log("보스: Smash 사용!");
        // Smash 로직 구현
    }

    void UseFireball()
    {
        Debug.Log("보스: Fireball 사용!");
        // Fireball 로직 구현
    }

    void UseRoar()
    {
        Debug.Log("보스: Roar 사용!");
        // Roar 로직 구현
    }

}
