using UnityEngine;
using System.Collections;
using Include;

public class SwordmanBackHitSkill : Skill {
	//命中前非static
	public SwordmanBackHitSkill()
		:base("背刺",SkillType.HitBefore)
	{
		
	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		Direction direction = CharacterMono.GetDirection (info.Sender.Pos,info.Recipient.Pos);
		if (direction == info.Recipient.Direction) 
		{
			CharacterMono.HitPoint hitPoint=(2*info.Sender.hitPoint);
			info.Sender.extraHitPoint+=hitPoint;
			info.Sender.AddSkill (info.Sender.afterHitSkillList, new CancelTempHitPropertySkill(hitPoint));
		}
		yield break;
	}
}
