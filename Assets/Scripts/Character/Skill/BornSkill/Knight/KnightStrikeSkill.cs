using UnityEngine;
using System.Collections;
using Include;

public class KnightStrikeSkill : ActiveSkill {
	//移动前主动技能
	public Direction Direction{get;set;}
	public KnightStrikeSkill()
		:base("冲锋",SkillType.ActiveSkill)
	{
		StepList.Add (new Step(ChooseDirection));
	}
	public override IEnumerator ExertEffect(ActionInformation info)
	{
		info.Sender.SubHealthPoint (5);
		for (int i = 1; i <= info.Sender.moveRate + info.Sender.extraMoveRate+1; i++) 
		{
			Block block = BlockMapManager.instance.GetBlock (info.Sender.Pos, Direction, i);
			if (block == null) {
				yield return info.Sender.StartCoroutine (
					info.Sender.MoveAnimation (new ActionInformation (
						targetBlock: BlockMapManager.instance.GetBlock (info.Sender.Pos, Direction, i - 1))));
				GameManager.instance.NextStep (4);
				yield break;
			}
			CharacterMono character = block.character;
			if (character != null) 
			{
				yield return info.Sender.StartCoroutine(
					info.Sender.MoveAnimation (new ActionInformation(
						targetBlock:BlockMapManager.instance.GetBlock (info.Sender.Pos, Direction, i - 1))));
				if (character.team != info.Sender.team) 
				{
					CharacterMono.HitPoint hitPoint=((i - 1) * 0.5f * info.Sender.hitPoint);
					info.Sender.extraHitPoint+=hitPoint;
					info.Sender.AddSkill (info.Sender.afterHitSkillList, new CancelTempHitPropertySkill(hitPoint));
					ActionInformation actInfo=new ActionInformation (recipient: character);
					yield return info.Sender.StartCoroutine(info.Sender.Hit(actInfo));
					yield return info.Sender.StartCoroutine(info.Sender.InjuredAnimation(actInfo));
				}
				GameManager.instance.NextStep (4);
				yield break;
			}
		}
		yield return info.Sender.StartCoroutine(
			info.Sender.MoveAnimation (new ActionInformation(
				targetBlock:BlockMapManager.instance.GetBlock (info.Sender.Pos,Direction,info.Sender.moveRate + info.Sender.extraMoveRate))));
		GameManager.instance.NextStep (4);
		yield break;
	}
	private IEnumerator ChooseDirection()
	{
		SceneManager.instance.knightArrow.gameObject.SetActive (true);
		Direction = Direction.Down;
		SceneManager.instance.knightArrow.rotation = Quaternion.Euler (new Vector3 (90, 0, 0));
		SceneManager.instance.knightArrow.position = GameManager.instance.CurrentCharacter.transform.position+new Vector3(0,0.01f,0);
		while (true) {
			yield return 0;
			if (GameManager.instance.GetKeyDown (InputKeyCode.Up)) {
				SceneManager.instance.knightArrow.rotation = Quaternion.Euler (new Vector3 (90, 180, 0));
				Direction = Direction.Up;
			} else if (GameManager.instance.GetKeyDown (InputKeyCode.Left)) {
				SceneManager.instance.knightArrow.rotation = Quaternion.Euler (new Vector3 (90, 90, 0));
				Direction = Direction.Left;
			} else if (GameManager.instance.GetKeyDown (InputKeyCode.Down)) {
				SceneManager.instance.knightArrow.rotation = Quaternion.Euler (new Vector3 (90, 0, 0));
				Direction = Direction.Down;
			} else if (GameManager.instance.GetKeyDown (InputKeyCode.Right)) {
				SceneManager.instance.knightArrow.rotation = Quaternion.Euler (new Vector3 (90, 270, 0));
				Direction = Direction.Right;
			}
			if (GameManager.instance.GetKeyDown (InputKeyCode.OK)) {
				Block block = BlockMapManager.instance.GetBlock (GameManager.instance.CurrentCharacter.Pos, Direction, 1);
				if (block == null||block.character!=null) 
				{
					UIManager.instance.Warning ("前方有障碍，无法冲锋");
					continue;
				}
				SceneManager.instance.knightArrow.gameObject.SetActive (false);
				yield return GameManager.instance.StartCoroutine (
					ExertEffect(new ActionInformation(sender:GameManager.instance.CurrentCharacter)));
				yield break;
			}
			else if (GameManager.instance.GetKeyDown (InputKeyCode.Return)) 
			{
				SceneManager.instance.knightArrow.gameObject.SetActive (false);
				GameManager.instance.NextStep (0);
				yield break;
			}
		}
	}

	public override bool CanExert()
	{
		if (GameManager.instance.CurrentStepIndex > (int)StepID.BeforeMove) 
		{
			UIManager.instance.Warning ("冲锋技能只能在移动前使用");
			return false;
		}
		return true;
	}
}
