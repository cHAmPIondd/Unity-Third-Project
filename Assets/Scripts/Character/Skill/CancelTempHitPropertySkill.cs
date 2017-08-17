using UnityEngine;
using System.Collections;
using Include;
public class CancelTempHitPropertySkill : Skill {
	//命中后非static
	private CharacterMono.HitPoint m_HitPoint;
	public CancelTempHitPropertySkill(CharacterMono.HitPoint hitPoint)
		:base(SkillType.HitAfter|SkillType.Settlement)
	{
		m_HitPoint = hitPoint;
	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		info.Sender.extraHitPoint-=m_HitPoint;
		info.RemoveSkillInfoList.Add (new RemoveSkillInfo (info.Sender.afterHitSkillList,this,info.Sender));
		yield break;
	}
}
