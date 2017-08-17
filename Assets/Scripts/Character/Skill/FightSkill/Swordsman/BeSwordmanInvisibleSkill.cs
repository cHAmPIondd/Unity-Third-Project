using UnityEngine;
using System.Collections;
using Include;

public class BeSwordmanInvisibleSkill : Skill {
	//移动后static技能
	private CharacterMono m_Character;
	private SwordmanInvisibleSkill m_Skill;
	public BeSwordmanInvisibleSkill(CharacterMono character,SwordmanInvisibleSkill skill)
		:base(SkillType.MoveAfter|SkillType.FightTime)
	{
		m_Character = character;
		m_Skill = skill;
	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		if (info.Sender.team != m_Character.team) 
		{
			if (info.Sender.GetMovableArea ().Contains (BlockMapManager.instance.GetBlock (m_Character.Pos))) 
			{
				m_Skill.UpdateInvisible (info.Sender, true);
				yield break;
			}
		}
		m_Skill.UpdateInvisible (info.Sender, false);
		yield break;
	}
	public override void CharacterDeath(CharacterMono one)
	{
		m_Skill.UpdateInvisible (one,false);
	}
}
	
