using UnityEngine;
using System.Collections;
using Include;

public class AssassinSneakAttackSkill : Skill {
	//命中前非static
	public AssassinSneakAttackSkill()
		:base("偷袭",SkillType.HitBefore)
	{

	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		info.Recipient.AddSkill (info.Recipient.afterInjuredSkillList, new BeAssassinSneakAttackSkill ());
		yield break;
	}
}
