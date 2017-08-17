using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Include;

public abstract class Skill{
	private int m_OrderIndex;
	public SkillType skillType;
	public string name;
	public Skill(SkillType skillT=SkillType.None,int orderIndex=0)
	{
		name = "None";
		skillType = skillT;
		m_OrderIndex = orderIndex;
	}
	public Skill(string n="None",SkillType skillT=SkillType.None,int orderIndex=0)
	{
		name = n;
		skillType = skillT;
		m_OrderIndex = orderIndex;
	}
	public virtual IEnumerator ExertEffect(ActionInformation info)
	{
		Debug.Log ("BUG");
		yield return 0;
	}
	public virtual void CharacterDeath(CharacterMono one)
	{
		
	}
	public bool CompareTo(Skill skill)
	{
		if (m_OrderIndex > skill.m_OrderIndex)
			return true;
		return false;
	}
	public static Skill GetSkill(SkillID id)
	{
		if(id==SkillID.AssassinHide)
			return new AssassinHideSkill ();
		if(id==SkillID.AssassinLoveliness)
			return new AssassinLonelinessSkill ();
		if(id==SkillID.AssassinSneakAttack)
			return new AssassinSneakAttackSkill ();
		if(id==SkillID.KnightGuard)
			return new KnightGuardSkill ();
		if(id==SkillID.KnightSpirit)
			return new KnightSpiritSkill ();
		if(id==SkillID.KnightStrike)
			return new KnightStrikeSkill ();
		if(id==SkillID.SwordmanBackHit)
			return new SwordmanBackHitSkill ();
		if(id==SkillID.SwordmanInvisible)
			return new SwordmanInvisibleSkill ();
		if(id==SkillID.SwordmanSpirit)
			return new SwordmanSpiritSkill ();
		if (id == SkillID.CowboyBucketGun)
			return new CowboyBucketGunSkill ();
		if (id == SkillID.CowboyBucketGunActive)
			return new CowboyBucketGunActiveSkill ();
		Debug.Log ("BUG");
		return null;
	}
}
