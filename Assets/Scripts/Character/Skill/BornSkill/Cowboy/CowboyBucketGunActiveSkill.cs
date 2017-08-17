using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Include;
public class CowboyBucketGunActiveSkill : ActiveSkill {
	private List<CharacterMono> m_TargetList= new List<CharacterMono> ();
	//移动后主动技能
	public CowboyBucketGunActiveSkill()
		:base("枪斗术<一>",SkillType.ActiveSkill)
	{
		StepList.Add (new Step(BucketGun));
	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		info.Sender.SubHealthPoint (5);
		info.CanCounterattack = false;
		info.HitFeedback = HitAnimation;
		info.Recipient = m_TargetList [0];
		yield return info.Sender.StartCoroutine (info.Sender.Attack (info));
		for(int i=1;i<m_TargetList.Count;i++)
		{
			ActionInformation newInfo= new ActionInformation (info);
			newInfo.HitFeedback = HitAnimation;
			if(i==m_TargetList.Count-1)
				newInfo.HitFeedback = HitAnimation_2;
			newInfo.Recipient = m_TargetList [i];
			yield return newInfo.Sender.StartCoroutine (info.Sender.Attack (newInfo));
		}
		m_TargetList.Clear ();
		GameManager.instance.NextStep (2);
		yield break;
	}
	public override bool CanExert()
	{
		if (GameManager.instance.CurrentStepIndex <(int)StepID.BeforeHit) 
		{
			UIManager.instance.Warning ("枪斗术只能在移动后使用");
			return false;
		}
		foreach(Block block in GameManager.instance.CurrentCharacter.GetHitableArea())
		{
			if(block.character!=null&&block.character.team!=GameManager.instance.CurrentCharacter.team)
			{
				m_TargetList.Add (block.character);
			}
		}
		if (m_TargetList.Count == 0) 
		{
			UIManager.instance.Warning ("没有可攻击目标");
			return false;
		}
		return true;
	}
	public IEnumerator BucketGun()
	{
		yield return GameManager.instance.StartCoroutine (
			ExertEffect(new ActionInformation(sender:GameManager.instance.CurrentCharacter)));
	}
	public IEnumerator HitAnimation(ActionInformation info)
	{
		info.Sender.Animator.CrossFadeInFixedTime ("Hit", 0.2f);
		GameManager.instance.StartCoroutine(info.Sender.TurnDirectionAnimation (CharacterMono.GetDirection(info.Sender.Pos,info.Recipient.Pos)));
		float hitTime = Time.time + info.Sender.animationData.hitPrelude-info.Recipient.animationData.injuredPrelude;
		while (hitTime>Time.time) 
		{
			yield return 0;
		}
		info.Recipient.StartCoroutine (info.InjuredFeedback(info));
	}
	public IEnumerator HitAnimation_2(ActionInformation info)
	{
		info.Sender.Animator.CrossFadeInFixedTime ("Hit_2", 0.2f);
		GameManager.instance.StartCoroutine(info.Sender.TurnDirectionAnimation (CharacterMono.GetDirection(info.Sender.Pos,info.Recipient.Pos)));
		float hitTime = Time.time + info.Sender.animationData.hitPrelude-info.Recipient.animationData.injuredPrelude;
		while (hitTime>Time.time) 
		{
			yield return 0;
		}
		info.Recipient.StartCoroutine (info.InjuredFeedback(info));
	}
}
