using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Include;

public class ActionInformation {
	private bool m_IsStop;
	public bool IsStop{ get{ return m_IsStop;} 
		set { 
			if(value&&!IsTryNotStop)
				foreach (RemoveSkillInfo RSinfo in RemoveSkillInfoList) 
				{
					RSinfo.RemoveSkill ();
				}
			m_IsStop=value;
		}}
	public CharacterMono Sender{ get; set;}
	public CharacterMono Recipient{ get; set;}
	public Action InjuredFeedback{ get; set;}
	public Action HitFeedback{ get; set;}
	public Block TargetBlock{get;set;}
	public int Damage{get;set;}
	public CharacterMono ExtraCharacter{ get; set;}
	public List<RemoveSkillInfo> RemoveSkillInfoList{get; set;} 
	public bool IsTryNotStop;//不触发Stop
	public bool CanCounterattack;
	public ActionInformation(bool isStop=false,CharacterMono sender=null,CharacterMono recipient=null,Action injuredFeedback=null,Action hitFeedback=null,Block targetBlock=null,int damage=0,CharacterMono extraCharacter=null,bool isTryNotStop=false,bool canCounterattack=true)
	{
		IsStop = isStop;
		Sender = sender;
		Recipient = recipient;
		InjuredFeedback = injuredFeedback;
		HitFeedback = hitFeedback;
		TargetBlock = targetBlock;
		Damage = damage;
		ExtraCharacter = extraCharacter;
		RemoveSkillInfoList = new List<RemoveSkillInfo> ();
		IsTryNotStop = isTryNotStop;
		CanCounterattack = canCounterattack;
	}
	public ActionInformation(ActionInformation a)
	{
		IsStop = a.IsStop;
		Sender = a.Sender;
		Recipient = a.Recipient;
		InjuredFeedback = a.InjuredFeedback;
		HitFeedback = a.HitFeedback;
		TargetBlock = a.TargetBlock;
		Damage = a.Damage;
		ExtraCharacter = a.ExtraCharacter;
		RemoveSkillInfoList = new List<RemoveSkillInfo> ();
		IsTryNotStop = a.IsTryNotStop;
		CanCounterattack = a.CanCounterattack;
	}
}
