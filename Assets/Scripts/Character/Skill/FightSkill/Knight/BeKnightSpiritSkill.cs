using UnityEngine;
using System.Collections;
using Include;

public class BeKnightSpiritSkill : Skill {
	//受伤后非static
	public BeKnightSpiritSkill()
		:base(SkillType.InjuredAfter|SkillType.FightTime,-1000)//最后运算
	{

	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		if (info.Recipient.healthPoint+info.Recipient.extraHealthPoint-info.Damage <= 0)
			info.Damage=info.Recipient.healthPoint+info.Recipient.extraHealthPoint-1;
		info.RemoveSkillInfoList.Add (new RemoveSkillInfo (info.Recipient.afterInjuredSkillList,this,info.Recipient));
		yield break;
	}
}
