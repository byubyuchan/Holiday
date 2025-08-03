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
        skills.Add(new BossSkill { skillName = "Normal Attack", cooldown = 2f, lastUseTime = -2f, weight = 20, useSkillAction = UseNormalAttack });
        skills.Add(new BossSkill { skillName = "Flying Attack", cooldown = 5f, lastUseTime = -5f, weight = 40, useSkillAction = UseFlyingAttack });
        skills.Add(new BossSkill { skillName = "Thunder Attack", cooldown = 8f, lastUseTime = -8f, weight = 40, useSkillAction = UseThunderAttack });
    }

    public override void TryUseSkill()
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
    void UseNormalAttack()
    {
        Debug.Log("보스: 일반 공격 사용!");
        anim.SetTrigger("Attack");
        tower.TakeDamage(damage);
        GameObject effectInstance = GameManager.instance.pool.Get(12);
        effectInstance.transform.position = tower.transform.position;
        effectInstance.SetActive(true);
        AudioManager.instance.PlaySFX("B_Attack");

    }
    void UseFlyingAttack()
    {
        Debug.Log("보스: 공중 공격 사용!"); // 데미지 낮추고 광역 공격으로 변경!
        anim.SetTrigger("Hurt");
        for (int i = 0; i < 10; i++)
        {
            tower.TakeDamage(damage/3);
            GameObject effectInstance = GameManager.instance.pool.Get(12);
            effectInstance.transform.position = tower.transform.position;
            effectInstance.SetActive(true);
        }
        AudioManager.instance.PlaySFX("B_Attack2");
    }

    void UseThunderAttack()
    {
        Debug.Log("보스: 번개 공격 사용!");
        for (int i = 0; i < 5; i++)
        {
            FireProjectile(tower.gameObject);
            AudioManager.instance.PlaySFX("B_Dark");
        }

    }

    public override void Dead()
    {
        base.Dead(); // 기존 죽음 처리 먼저 실행
        if (CutsceneManager.instance != null)
        {
            // 죽을 때 컷씬 실행
            CutsceneManager.instance.PlayBossCutscene(this.transform, "Clear!",0.5f);
        }
        AudioManager.instance.PlaySFX("E_Dead1"); //B_Dead 바꿀 거 찾기
    }

    public override void MoveTowardsTarget()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance > range) // range 보다 멀면 이동
            {
                this.rigid.linearVelocity = direction * speed;
            }
            else
            {
                this.rigid.linearVelocity = Vector2.zero; // 가까우면 멈춤
            }
        }
    }

}
