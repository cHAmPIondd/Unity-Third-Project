using UnityEngine;
using System.Collections;
using Include;

public class KnightGuardingSkill : Skill {
	//骑士守护技能发动中,回合开始前
	private int m_RoundNum=0;
	private Skill m_Skill;
	public KnightGuardingSkill(Skill skill)
		:base(SkillType.RoundStartBefore|SkillType.FightTime)
	{
		m_Skill = skill;
	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		m_RoundNum++;
		if (m_RoundNum >= 3)//回合数到达 
		{
			info.RemoveSkillInfoList.Add(new RemoveSkillInfo(info.Sender.beforeRoundStartSkillList,this,info.Sender));
			RemoveStaticSkill ();
		}
		yield break;
	}
	public void RemoveStaticSkill()
	{
		CharacterMono.RemoveStaticSkill(CharacterMono.staticBeforeBeAttackedSkillList,m_Skill);
	}
}
