namespace Include
{
	using System.Collections;
	public enum Team
	{
		PlayerTeam,
		EnemyTeam,
	}
	public enum Age
	{
		Child,
		Adolescent,
		Adult,
		Agedness,
	}
	public enum Sex
	{
		Male,
		Female,
		Neutral,
	}
	public enum Direction
	{
		Up=1,
		Down=-1,
		Left=2,
		Right=-2,
	}
	public enum MoveMode
	{
		Normal,
		Straight,
	}
	public enum SkillType
	{
		None=0,
		FightPre=1,
		RoundStartBefore=2,
		RoundStartAfter=4,
		MoveBefore=8,
		MoveAfter=16,
		AttackBefore=32,
		AttackAfter=64,
		HitBefore=128,
		HitAfter=256,
		BeAttackBefore=512,
		BeCounterattackedBefore=1024,
		InjuredBefore=2048,
		InjuredAfter=4096,
		RoundEndBefore=8192,
		RoundEndAfter=16384,
		CountMovableAreaAfter=32768,
		FightTime=65536,
		Settlement=131072,
		ActiveSkill=262144,
		CounterattackedBefore=524288,
		CanAttackBefore=1048576,
		CanBeAttackedBefore=2097152,

	}

	public enum StepID
	{
		None=0,
		BeforeMove=1,
		Move=2,
		BeforeHit=3,
		Hit=4,
		Over=5
	}
	public enum CharacterID
	{
		FirstEnemy,
		SecondEnemy,
		Knight,
		Swordman,
		Assassin,
		Cowboy
	}
	public enum SkillID
	{
		AssassinHide,
		AssassinLoveliness,
		AssassinSneakAttack,
		KnightGuard,
		KnightSpirit,
		KnightStrike,
		SwordmanBackHit,
		SwordmanInvisible,
		SwordmanSpirit,
		CowboyBucketGunActive,
		CowboyBucketGun,
	}
	public enum InputKeyCode
	{
		Up,
		Down,
		Left,
		Right,
		OK,
		Return,
	}
	//玩家操作步骤
	public delegate IEnumerator Step();
	//角色动作
	public delegate IEnumerator Action(ActionInformation info);
}