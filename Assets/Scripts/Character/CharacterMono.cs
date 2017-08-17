using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Include;

public sealed class CharacterMono:MonoBehaviour {
	private static readonly float MOVE_RATE=6;	
	public CharacterID characterID;
	public SkillID[] skillIDArray;
	[System.Serializable]
	public class AnimationData
	{
		public float hitPrelude;
		public float injuredPrelude;
	}
	public AnimationData animationData;
	[System.Serializable]
	public class HitPoint
	{
		public int Min;
		public int Max;
		public HitPoint(int min,int max)
		{
			Min=min;
			Max=max;
		}
		public static implicit operator HitPoint(int num)
		{
			return new HitPoint (num,num); 
		}
		public static implicit operator HitPoint(float num)
		{
			return new HitPoint ((int)num,(int)num); 
		}
		public static HitPoint operator-(HitPoint thisHitPoint,HitPoint subNum)
		{
			return new HitPoint(thisHitPoint.Min - subNum.Min,thisHitPoint.Max - subNum.Max);
		}
		public static HitPoint operator+(HitPoint thisHitPoint,HitPoint addNum)
		{
			return new HitPoint(thisHitPoint.Min + addNum.Min,thisHitPoint.Max + addNum.Max);
		}
		public static HitPoint operator*(HitPoint thisHitPoint,HitPoint mulNum)
		{
			return new HitPoint(thisHitPoint.Min * mulNum.Min,thisHitPoint.Max * mulNum.Max);
		}
	}
	public HitPoint hitPoint=new HitPoint(0,0);//攻击
	[HideInInspector]public HitPoint extraHitPoint=new HitPoint(0,0);//额外攻击加成
	public int healthPoint;//血量
	[HideInInspector]public int extraHealthPoint;//额外血量加成
	public int armorPoint;//护甲
	[HideInInspector]public int extraArmorPoint;//额外护甲加成
	public int moveRate;//移动速度
	[HideInInspector]public int extraMoveRate;//额外速度加成

	public Team team;//队伍
	public Age age;//年龄
	public Sex sex;//性别
	public MoveMode moveMode;
	public Direction Direction{ get;private set;}//朝向
	private Quaternion m_TargetRotation;//目标方向
	public Vector2 Pos{get;private set;}//位置
	public Vector2[] hitableArea;//可攻击区域

	public static List<Skill> allStaticSkillList = new List<Skill> ();
	public List<Skill> allSkillList = new List<Skill> ();

	public List<ActiveSkill> activeSkillList = new List<ActiveSkill> (); 


	public Animator Animator{ get;private set;}


	void Awake()
	{
		Animator = GetComponent<Animator> ();
		foreach (SkillID id in skillIDArray) 
		{
			Skill skill = Skill.GetSkill (id);
			CharacterManager.instance.AddSkill (this,skill);
		}
	}
	void Start()
	{
		
	}

	//战前准备
	public List<Skill> fightPreSkillList=new List<Skill>();
	public static List<Skill> staticFightPreSkillList=new List<Skill>();
	public IEnumerator FightPre(ActionInformation info)
	{
		TurnDirection (GetDirection(Pos, Pos));
		info.Sender = this;
		foreach (Skill skill in staticFightPreSkillList) 
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach (Skill skill in fightPreSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach(RemoveSkillInfo RSinfo in info.RemoveSkillInfoList)
		{
			RSinfo.RemoveSkill ();
		}
	}
	//回合开始
	public List<Skill> beforeRoundStartSkillList=new List<Skill>();
	public List<Skill> afterRoundStartSkillList=new List<Skill>();
	public static List<Skill> staticBeforeRoundStartSkillList=new List<Skill>();
	public static List<Skill> staticAfterRoundStartSkillList=new List<Skill>();
	public IEnumerator RoundStart(ActionInformation info)
	{
		info.Sender = this;
		foreach (Skill skill in staticBeforeRoundStartSkillList) 
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach (Skill skill in beforeRoundStartSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}


		foreach(Skill skill in staticAfterRoundStartSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach(Skill skill in afterRoundStartSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach(RemoveSkillInfo RSinfo in info.RemoveSkillInfoList)
		{
			RSinfo.RemoveSkill ();
		}
	}

	//移动
	public List<Skill> beforeMoveSkillList=new List<Skill>();
	public List<Skill> afterMoveSkillList=new List<Skill>();
	public static List<Skill> staticBeforeMoveSkillList=new List<Skill>();
	public static List<Skill> staticAfterMoveSkillList=new List<Skill>();
	public IEnumerator Move(ActionInformation info)
	{
		info.Sender = this;
		foreach(Skill skill in staticBeforeMoveSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach (Skill skill in beforeMoveSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		yield return StartCoroutine (MoveAnimation(info));
		foreach(Skill skill in staticAfterMoveSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach(Skill skill in afterMoveSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach(RemoveSkillInfo RSinfo in info.RemoveSkillInfoList)
		{
			RSinfo.RemoveSkill ();
		}
	}
	//攻击
	public List<Skill> beforeAttackSkillList=new List<Skill>();
	public List<Skill> afterAttackSkillList=new List<Skill>();
	public static List<Skill> staticBeforeAttackSkillList=new List<Skill>();
	public static List<Skill> staticAfterAttackSkillList=new List<Skill>();
	public IEnumerator Attack(ActionInformation info)
	{
		info.Sender = this;
		yield return StartCoroutine (CanAttack(info));
		if (info.IsStop)
		{
			yield break;
		}
		yield return StartCoroutine (CanBeAttacked(info));
		if (info.IsStop)
		{
			yield break;
		}
		//攻击前
		foreach(Skill skill in staticBeforeAttackSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach (Skill skill in beforeAttackSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		//能否成功命中
		yield return StartCoroutine(info.Recipient.BeAttacked(info));
		if (info.IsStop)
		{
			yield break;
		}
		//命中
		yield return StartCoroutine(Hit(info));
		if (info.IsStop)
		{
			yield break;
		}
		//攻击后
		foreach(Skill skill in staticAfterAttackSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach(Skill skill in afterAttackSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		if(info.HitFeedback==null)
			info.HitFeedback = new Action (HitAnimation);
		if(info.InjuredFeedback==null)
			info.InjuredFeedback = new Action (info.Recipient.InjuredAnimation);
		yield return StartCoroutine (info.HitFeedback(info));
		foreach(RemoveSkillInfo RSinfo in info.RemoveSkillInfoList)
		{
			RSinfo.RemoveSkill ();
		}
		if(info.Recipient.GetHitableArea().Contains(BlockMapManager.instance.GetBlock(info.Sender.Pos))&&info.CanCounterattack)
			yield return StartCoroutine (info.Recipient.Counterattack(new ActionInformation(recipient:info.Sender)));
	}
	//命中
	public List<Skill> beforeHitSkillList=new List<Skill>();
	public List<Skill> afterHitSkillList=new List<Skill>();
	public List<Skill> staticBeforeHitSkillList=new List<Skill>();
	public List<Skill> staticAfterHitSkillList=new List<Skill>();
	public IEnumerator Hit(ActionInformation info)
	{
		info.Sender = this;

		foreach(Skill skill in staticBeforeHitSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach (Skill skill in beforeHitSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		yield return StartCoroutine(info.Recipient.Injured (info));

		foreach(Skill skill in staticAfterHitSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach (Skill skill in afterHitSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
	}
	//被攻击
	public List<Skill> beforeBeAttackedSkillList=new List<Skill>();
	public static List<Skill> staticBeforeBeAttackedSkillList=new List<Skill>();
	public IEnumerator BeAttacked(ActionInformation info)
	{
		foreach(Skill skill in staticBeforeBeAttackedSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach (Skill skill in beforeBeAttackedSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
	}
	//受伤
	public List<Skill> beforeInjuredSkillList=new List<Skill>();
	public List<Skill> afterInjuredSkillList=new List<Skill>();
	public static List<Skill> staticBeforeInjuredSkillList=new List<Skill>();
	public static List<Skill> staticAfterInjuredSkillList=new List<Skill>();
	public IEnumerator Injured(ActionInformation info)
	{
		foreach(Skill skill in staticBeforeInjuredSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach (Skill skill in beforeInjuredSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		info.Damage = info.Sender.GetRandomHitPoint ()-(info.Recipient.armorPoint+info.Recipient.extraArmorPoint);
		info.Damage = info.Damage > 0 ? info.Damage : 0;
		foreach(Skill skill in staticAfterInjuredSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach(Skill skill in afterInjuredSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
	}
	//反击
	public List<Skill> beforeCounterattackSkillList=new List<Skill>();
	public List<Skill> afterCounterattackSkillList=new List<Skill>();
	public static List<Skill> staticBeforeCounterattackSkillList=new List<Skill>();
	public static List<Skill> staticAfterCounterattackSkillList=new List<Skill>();
	public IEnumerator Counterattack(ActionInformation info)
	{
		info.Sender = this;
		//反击前
		foreach(Skill skill in staticBeforeCounterattackSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach (Skill skill in beforeCounterattackSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		//能否成功命中
		yield return StartCoroutine(info.Recipient.BeCounterattacked(info));
		if (info.IsStop)
		{
			yield break;
		}
		//命中
		yield return StartCoroutine(Hit(info));
		if (info.IsStop)
		{
			yield break;
		}
		//攻击后
		foreach(Skill skill in staticAfterCounterattackSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach(Skill skill in afterCounterattackSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		if(info.HitFeedback==null)
			info.HitFeedback = new Action (HitAnimation);
		if(info.InjuredFeedback==null)
			info.InjuredFeedback = new Action (info.Recipient.InjuredAnimation);
		yield return StartCoroutine (info.HitFeedback(info));
		foreach(RemoveSkillInfo RSinfo in info.RemoveSkillInfoList)
		{
			RSinfo.RemoveSkill ();
		}
	}
	//被反击
	public List<Skill> beforeBeCounterattackedSkillList=new List<Skill>();
	public static List<Skill> staticBeforeBeCounterattackedSkillList=new List<Skill>();
	public IEnumerator BeCounterattacked(ActionInformation info)
	{
		foreach(Skill skill in staticBeforeBeAttackedSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach (Skill skill in beforeBeCounterattackedSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
	}
	//回合结束
	public List<Skill> beforeRoundEndSkillList=new List<Skill>();
	public List<Skill> afterRoundEndSkillList=new List<Skill>();
	public static List<Skill> staticBeforeRoundEndSkillList=new List<Skill>();
	public static List<Skill> staticAfterRoundEndSkillList=new List<Skill>();
	public IEnumerator RoundEnd(ActionInformation info)
	{
		info.Sender = this;
		foreach(Skill skill in staticBeforeRoundEndSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach (Skill skill in beforeRoundEndSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}


		foreach(Skill skill in staticAfterRoundEndSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach(Skill skill in afterRoundEndSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach(RemoveSkillInfo RSinfo in info.RemoveSkillInfoList)
		{
			RSinfo.RemoveSkill ();
		}
	}

	public List<Skill> beforeCanAttackSkillList = new List<Skill> ();
	public List<Skill> staticBeforeCanAttackSkillList = new List<Skill> ();
	public IEnumerator CanAttack(ActionInformation info)
	{
		foreach(Skill skill in staticBeforeCanAttackSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach(Skill skill in beforeCanAttackSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
	}
	public List<Skill> beforeCanBeAttackedSkillList = new List<Skill> ();
	public List<Skill> staticBeforeCanBeAttackedSkillList = new List<Skill> ();
	public IEnumerator CanBeAttacked(ActionInformation info)
	{
		foreach(Skill skill in staticBeforeCanBeAttackedSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
		foreach(Skill skill in beforeCanBeAttackedSkillList)
		{
			yield return StartCoroutine(skill.ExertEffect (info));
			if (info.IsStop)
			{
				yield break;
			}
		}
	}

	//获得移动范围
//	public List<Skill> afterCountMovableSkillList = new List<Skill> ();
//	public List<Skill> staticAfterCountMovableSkillList = new List<Skill> ();
	public List<Block> GetMovableArea()
	{
		Stack<Block> stack1 = new Stack<Block> ();
		Stack<Block> stack2 = new Stack<Block> ();
		List<Block> list = new List<Block> ();
		stack1.Push (BlockMapManager.instance.GetBlock (Pos));
		if (moveMode == MoveMode.Normal) 
		{
			for (int i = 0; i < moveRate + extraMoveRate; i++) {
				while (stack1.Count != 0) {
					Block sourceBlock = stack1.Pop ();
					CountBlockOfArea (list, stack2, sourceBlock, Direction.Up);
					CountBlockOfArea (list, stack2, sourceBlock, Direction.Down);
					CountBlockOfArea (list, stack2, sourceBlock, Direction.Right);
					CountBlockOfArea (list, stack2, sourceBlock, Direction.Left);
				}
				Stack<Block> stack = stack1;
				stack1 = stack2;
				stack2 = stack;
			}
		}
		else if (moveMode == MoveMode.Straight) 
		{
			for (int i = 0; i < moveRate + extraMoveRate; i++) 
			{
				while (stack1.Count != 0) {
					Block sourceBlock = stack1.Pop ();
					CountBlockOfArea (list, stack2, sourceBlock, Direction.Up);
					CountBlockOfArea (list, stack2, sourceBlock, Direction.Down);
				}
				Stack<Block> stack = stack1;
				stack1 = stack2;
				stack2 = stack;	
			}
			stack1.Clear ();
			stack2.Clear();
			stack1.Push (BlockMapManager.instance.GetBlock (Pos));
			for (int i = 0; i < moveRate + extraMoveRate; i++) 
			{
				while (stack1.Count != 0) {
					Block sourceBlock = stack1.Pop ();
					CountBlockOfArea (list, stack2, sourceBlock, Direction.Right);
					CountBlockOfArea (list, stack2, sourceBlock, Direction.Left);
				}
				Stack<Block> stack = stack1;
				stack1 = stack2;
				stack2 = stack;
			}
		}
		return list;
	}
	private void CountBlockOfArea(List<Block> list,Stack<Block> stack2,Block b,Direction dir)
	{//计算格子能否过人和行走
		Block block = BlockMapManager.instance.GetBlock (b.pos, dir);
		if (block != null && block.CanThrough (team) && !list.Contains (block)) {
			if(block.CanStay())
				list.Add (block);
			stack2.Push (block);
		}
	}
	//获得攻击范围
	public List<Block> GetHitableArea(Vector2? pos=null)
	{
		List<Block> list = new List<Block> ();
		foreach (Vector2 v2 in hitableArea) 
		{
			Block block = BlockMapManager.instance.GetBlock (v2 + (pos??Pos));
			if(block !=null)
				list.Add (block);
		}
		return list;
	}
	//
	public void SubHealthPoint(int num)
	{
		if (extraHealthPoint < 10) 
		{
			healthPoint -= (10-extraHealthPoint);
			extraHealthPoint = 0;
		}
		else
			extraHealthPoint -= 10;
		UIManager.instance.SubHealthEffect (num.ToString(),transform.position);
	}
	public int GetRandomHitPoint()
	{
		return Random.Range ((hitPoint + extraHitPoint).Min, (hitPoint + extraHitPoint).Max+1);
	}

	public void TurnDirection(Direction dir)
	{
		Direction = dir;
		if (Direction == Direction.Down)
			transform.rotation = Quaternion.Euler (new Vector3(0,-90,0));
		else if (Direction == Direction.Left)
			transform.rotation = Quaternion.Euler (new Vector3(0,0,0));
		else if (Direction == Direction.Right)
			transform.rotation = Quaternion.Euler (new Vector3(0,180,0));
		else if (Direction == Direction.Up)
			transform.rotation = Quaternion.Euler (new Vector3(0,90,0));
	}
	public static Direction GetDirection(Vector2 pos1,Vector2 pos2)
	{
		int xOffset=(int)(pos1.x - pos2.x);
		int yOffset=(int)(pos1.y - pos2.y);
		if (Mathf.Abs (xOffset) > Mathf.Abs (yOffset)) 
		{
			if (xOffset > 0) 
			{
				return Direction.Left;
			}
			else
			{
				return Direction.Right;
			}
		} 
		else 
		{
			if (yOffset > 0) 
			{
				return Direction.Down;
			} 
			else 
			{
				return Direction.Up;
			}
		}
	}
	public int DistanceWith8(CharacterMono character)
	{
		return (int)(Mathf.Abs (Pos.x - character.Pos.x) > Mathf.Abs (Pos.y - character.Pos.y)
			? Mathf.Abs (Pos.x - character.Pos.x) : Mathf.Abs (Pos.y - character.Pos.y));
	}
	public int DistanceWith4(CharacterMono character)
	{
		return (int)(Mathf.Abs (Pos.x - character.Pos.x)+ Mathf.Abs (Pos.y - character.Pos.y));
	}
	public void AddSkill(List<Skill> list,Skill skill)
	{
		allSkillList.Add (skill);
		for(int i=0;i<list.Count;i++)
		{
			if (!list [i].CompareTo (skill)) //优先插入
			{
				list.Insert (i,skill);
				return;
			}
		}
		//不优先
		list.Add (skill);
	}
	public void AddSkill(List<ActiveSkill> list,ActiveSkill skill)
	{
		allSkillList.Add (skill);
		for(int i=0;i<list.Count;i++)
		{
			if (!list [i].CompareTo (skill)) //优先插入
			{
				list.Insert (i,skill);
				return;
			}
		}
		//不优先
		list.Add (skill);
	}
	public static void AddStaticSkill(List<Skill> list,Skill skill)
	{
		CharacterMono.allStaticSkillList.Add (skill);
		for(int i=0;i<list.Count;i++)
		{
			if (!list [i].CompareTo (skill)) //优先插入
			{
				list.Insert (i,skill);
				return;
			}
		}
		//不优先
		list.Add (skill);
	}
	public void RemoveSkill(List<Skill> list,Skill skill)
	{
		allSkillList.Remove (skill);
		list.Remove (skill);
	}
	public static void RemoveStaticSkill(List<Skill> list,Skill skill)
	{
		CharacterMono.allStaticSkillList.Remove (skill);
		list.Remove(skill);
	}
	public static Skill FindSkill<T>(List<Skill> list)
	{
		for (int i = 0; i < list.Count; i++) 
		{
			if (list [i] is T) 
			{
				return list [i];
			}
		}
		return null;
	}
	public void SetPosition(Block block)
	{
		Block block2=BlockMapManager.instance.GetBlock (Pos);
		if (block2.character == this)
			block2.character = null;
		block.character = this;
		Pos = block.pos;
		transform.position = new Vector3 (block.transform.position.x, 0, block.transform.position.z);
	}

	void OnDestroy()
	{
		foreach (Skill skill in staticAfterMoveSkillList)
			skill.CharacterDeath (this);
	}
	//Animation
	public IEnumerator MoveAnimation(ActionInformation info)
	{
		List<Block> list=GetPath (info.TargetBlock);
		if (list [list.Count - 1] != BlockMapManager.instance.GetBlock (Pos)) 
		{
			Animator.CrossFadeInFixedTime ("Move", 0.2f);
			for (int i = 0; i < list.Count; i++) {
				StartCoroutine (TurnDirectionAnimation (GetDirection (i - 1 < 0 ? Pos : list [i - 1].pos, list [i].pos)));
				while (true) {
					yield return 0;
					Vector3 target = new Vector3 (list [i].transform.position.x, 0, list [i].transform.position.z);
					transform.position += (target - transform.position).normalized * MOVE_RATE * Time.deltaTime;
					if (Vector3.SqrMagnitude (target - transform.position) < 0.01)
						break;
				}
			}
			TurnDirection (GetDirection (list.Count - 2 >= 0 ? list [list.Count - 2].pos : Pos, info.TargetBlock.pos));
			SetPosition (info.TargetBlock);
			Animator.CrossFadeInFixedTime ("Idle", 0.2f);
		}
	}
	private List<Block> GetPath(Block targetBlock)
	{
		Queue<Block> queue1=new Queue<Block>();
		Queue<Block> queue2=new Queue<Block>();
		List<Block> list=new List<Block>();
		queue1.Enqueue (BlockMapManager.instance.GetBlock(Pos));
		for(int i=0;i<moveRate+extraMoveRate;i++)
		{
			while (queue1.Count != 0) 
			{
				Block sourceBlock = queue1.Dequeue ();
				if (sourceBlock == null) {
					list.Add (null);
					queue2.Enqueue (null);
					list.Add (null);
					queue2.Enqueue (null);
					list.Add (null);
					queue2.Enqueue (null);
					list.Add (null);
					queue2.Enqueue (null);
				}
				else 
				{
					CountBlockOfPath (list, queue2, ref sourceBlock, Direction.Up,targetBlock);
					if (sourceBlock == null)
						return list;
					CountBlockOfPath (list, queue2, ref sourceBlock, Direction.Down,targetBlock);
					if (sourceBlock == null)
						return list;
					CountBlockOfPath (list, queue2, ref sourceBlock, Direction.Right,targetBlock);
					if (sourceBlock == null)
						return list;
					CountBlockOfPath (list, queue2, ref sourceBlock, Direction.Left,targetBlock);
					if (sourceBlock == null)
						return list;
				}
			}
			Queue<Block> queue = queue1;
			queue1 = queue2;
			queue2 = queue;
		}
		return list;
	}
	private void CountBlockOfPath(List<Block> list,Queue<Block> queue2,ref Block sourceBlock,Direction dir,Block targetBlock)
	{//计算格子能否过人
		Block block = BlockMapManager.instance.GetBlock (sourceBlock.pos, dir);
		if (block == targetBlock) 
		{
			list.Add (block);
			int i;
			int d = list.Count;
			for(i=1;;i++)
			{
				d -= (int)Mathf.Pow (4, i);
				if (d <= 0) 
				{
					d += (int)Mathf.Pow (4,i);
					break;
				}
			}
			List<Block> list2 = new List<Block> ();
			for (; i > 0; i--) //i为第几步,d为4叉树这一层的第几个数
			{
				int d2 = 0;
				for (int j = 1; j < i; j++) 
				{
					d2+= (int)Mathf.Pow (4, j);
				}
				list2.Add (list[d2+d-1]);
				d=(d+3) / 4;
			}
			list.Clear ();
			list.AddRange(list2);
			list.Reverse ();
			queue2.Clear ();
			sourceBlock = null;
			return;
		}
		if (block != null && block.CanThrough (team) && !list.Contains (block)) {
			list.Add (block);
			queue2.Enqueue (block);
		} 
		else
		{
			list.Add (null);
			queue2.Enqueue (null);
		}
	}
	public IEnumerator TurnDirectionAnimation(Direction dir)
	{
		if (dir == Direction.Down)
			m_TargetRotation = Quaternion.Euler (new Vector3(0,-90,0));
		else if (dir == Direction.Left)
			m_TargetRotation = Quaternion.Euler (new Vector3(0,0,0));
		else if (dir == Direction.Right)
			m_TargetRotation = Quaternion.Euler (new Vector3(0,180,0));
		else if (dir == Direction.Up)
			m_TargetRotation = Quaternion.Euler (new Vector3(0,90,0));
		else
			Debug.Log ("Bug");
		while (true) 
		{
			yield return 0;
			transform.rotation=Quaternion.Lerp (transform.rotation,m_TargetRotation,5*Time.deltaTime);
			if (Quaternion.Angle (transform.rotation, m_TargetRotation)<2)
				break;
		}
		transform.rotation = m_TargetRotation;
	}
	public IEnumerator HitAnimation(ActionInformation info)
	{
		info.Sender.Animator.CrossFadeInFixedTime ("Hit", 0.2f);
		StartCoroutine(info.Sender.TurnDirectionAnimation (GetDirection(info.Sender.Pos,info.Recipient.Pos)));
		info.Sender.TurnDirection(GetDirection(info.Sender.Pos,info.Recipient.Pos));
		float hitTime = Time.time + info.Sender.animationData.hitPrelude-info.Recipient.animationData.injuredPrelude;
		while (hitTime>Time.time) 
		{
			yield return 0;
		}
		yield return StartCoroutine (info.InjuredFeedback(info));
	}
	public IEnumerator InjuredAnimation(ActionInformation info)
	{
		info.Recipient.TurnDirection(GetDirection(info.Recipient.Pos,info.Sender.Pos));
		info.Recipient.Animator.CrossFadeInFixedTime ("Injured",0.2f);
		yield return new WaitForSeconds(info.Recipient.animationData.injuredPrelude+0.1f);
		info.Recipient.SubHealthPoint (info.Damage);
	}
}
