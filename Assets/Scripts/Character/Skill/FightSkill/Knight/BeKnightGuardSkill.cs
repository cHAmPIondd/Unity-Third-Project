using UnityEngine;
using System.Collections;
using Include;

public class BeKnightGuardSkill : Skill {
	//全局被攻击前static骑士守护受体技能
	private CharacterMono m_Character;
	public BeKnightGuardSkill(CharacterMono character)
		:base(SkillType.BeAttackBefore|SkillType.FightTime)
	{
		m_Character = character;
	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		if (m_Character == null) //骑士已死
		{
			info.RemoveSkillInfoList.Add (new RemoveSkillInfo (CharacterMono.staticBeforeBeAttackedSkillList,this));
			yield break;
		}
		if (CharacterMono.FindSkill<KnightGuardingSkill> (info.Recipient.beforeRoundStartSkillList) != null)
			yield break;
		if (m_Character.team != info.Recipient.team)
			yield break;
		
		if(m_Character.DistanceWith8 (info.Recipient)<=1)
		{
			info.IsStop = true;
			yield return info.Sender.StartCoroutine(info.Sender.Attack(
				new ActionInformation(
					recipient:m_Character,
					injuredFeedback:new Action(InjuredAnimation),
					hitFeedback:new Action(HitAnimation),
					extraCharacter:info.Recipient,
					canCounterattack:false)));
			if (info.Recipient.GetHitableArea ().Contains (BlockMapManager.instance.GetBlock (info.Sender.Pos)) && info.CanCounterattack) 
			{
				yield return info.Recipient.StartCoroutine (info.Recipient.Counterattack (new ActionInformation (recipient: info.Sender)));
			}
		}
		yield break;
	}
	public IEnumerator HitAnimation(ActionInformation info)
	{
		info.Sender.Animator.CrossFade ("Hit",0.2f);
		info.Sender.StartCoroutine(info.Sender.TurnDirectionAnimation (CharacterMono.GetDirection(info.Sender.Pos,info.ExtraCharacter.Pos)));
		float hitTime = Time.time + info.Sender.animationData.hitPrelude;
		while (hitTime>Time.time) 
		{
			yield return 0;
		}
		yield return info.Sender.StartCoroutine (info.InjuredFeedback(info));
	}

	public IEnumerator InjuredAnimation(ActionInformation info)
	{
		if (UIManager.instance.knightShield.gameObject.activeInHierarchy) 
		{
			Transform tran= (Transform)GameObject.Instantiate (UIManager.instance.knightShield,UIManager.instance.knightShield.parent);
			tran.GetComponent<UIImageEffect> ().Init ();
			tran.position = UIManager.instance.mainCamera.WorldToScreenPoint (info.ExtraCharacter.transform.position);
		} 
		else
		{
			UIManager.instance.knightShield.gameObject.SetActive (true);
			UIManager.instance.knightShield.position = UIManager.instance.mainCamera.WorldToScreenPoint (info.ExtraCharacter.transform.position);
		}
		info.Recipient.SubHealthPoint (info.Damage);
		yield return 0;
	}
}
