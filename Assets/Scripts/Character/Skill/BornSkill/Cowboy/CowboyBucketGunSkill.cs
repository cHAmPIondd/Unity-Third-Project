using UnityEngine;
using System.Collections;
using Include;

public class CowboyBucketGunSkill : Skill {
	//被攻击前非static
	public CowboyBucketGunSkill()
		:base("枪斗术<二>",SkillType.BeAttackBefore)
	{

	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		if (info.Recipient.GetHitableArea ().Contains (BlockMapManager.instance.GetBlock (info.Sender.Pos))) {
			info.CanCounterattack = false;
			yield return info.Recipient.StartCoroutine (
				info.Recipient.Counterattack(new ActionInformation(recipient:info.Sender)));
		}
		yield break;
	}
}
