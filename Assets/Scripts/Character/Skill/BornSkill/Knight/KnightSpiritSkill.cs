using UnityEngine;
using System.Collections;
using Include;

public class KnightSpiritSkill : Skill {
	//命中后非static
	public KnightSpiritSkill()
		:base("骑士道",SkillType.HitAfter)
	{
	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		info.Recipient.AddSkill(info.Recipient.afterInjuredSkillList,new BeKnightSpiritSkill());
		yield break;
	}
}
