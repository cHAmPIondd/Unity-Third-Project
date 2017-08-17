using UnityEngine;
using System.Collections;
using Include;

public class KnightGuardSkill : ActiveSkill {
	//移动后主动技能
	//骑士守护技能发动入口
	public KnightGuardSkill()
		:base("守护",SkillType.ActiveSkill)
	{
		StepList.Add (Guard);
	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		//移除旧技能
		KnightGuardingSkill skill = (CharacterMono.FindSkill<KnightGuardingSkill> (info.Sender.beforeRoundStartSkillList))as KnightGuardingSkill;
		if (skill != null) {
			skill.RemoveStaticSkill ();
			info.RemoveSkillInfoList.Add (new RemoveSkillInfo(info.Sender.beforeRoundStartSkillList,skill,info.Sender));
		}
		//发动新技能
		info.Sender.SubHealthPoint (5);
		Skill tempSkill=new BeKnightGuardSkill (info.Sender);
		info.Sender.AddSkill(info.Sender.beforeRoundStartSkillList,new KnightGuardingSkill(tempSkill));
		CharacterMono.AddStaticSkill(CharacterMono.staticBeforeBeAttackedSkillList,tempSkill);
		GameManager.instance.NextStep (2);
		yield break;
	}
	public override bool CanExert()
	{
		if (GameManager.instance.CurrentStepIndex <(int)StepID.BeforeHit) 
		{
			UIManager.instance.Warning ("守护技能只能在移动后使用");
			return false;
		}
		return true;
	}
	public IEnumerator Guard()
	{
		yield return GameManager.instance.StartCoroutine (
			ExertEffect(new ActionInformation(sender:GameManager.instance.CurrentCharacter)));
	}
}
