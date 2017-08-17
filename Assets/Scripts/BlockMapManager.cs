using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Include;

public class BlockMapManager : MonoBehaviour {
	public static BlockMapManager instance;
	public GameObject blockPrefab;
	public Block[,] CurrentMap{ get;private set;}
	private GameObject BlockPoint;

	// Use this for initialization
	void Awake () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {

	}
	public void CreatedMap(int x,int y)
	{
		CurrentMap=new Block[x,y];
		BlockPoint = new GameObject ();
		for (int i = 0; i < x; i++) 
		{
			for (int j = 0; j < y; j++) 
			{
				GameObject GO = (GameObject)Instantiate (blockPrefab, BlockPoint.transform);
				Block block=GO.GetComponent<Block> ();
				block.Init (i,j);
				CurrentMap [i,j] = block;
			}
		}
	}
	public Block GetBlock(Vector2 pos)
	{
		float x = pos.x+0.5f;
		float y = pos.y+0.5f;
		if(x>=0&&x<CurrentMap.GetLength(0)&&y>=0&&y<CurrentMap.GetLength(1))
			return CurrentMap [(int)x,(int)y];
		return null;
	}
	public Block GetBlock(Vector2 pos,Direction dir,int index=1)
	{
		if (dir == Direction.Up)
			return GetBlock (new Vector2 (pos.x, pos.y + index));
		if (dir == Direction.Down)
			return GetBlock (new Vector2 (pos.x, pos.y - index));
		if (dir == Direction.Right)
			return GetBlock (new Vector2 (pos.x + index, pos.y));
		if (dir == Direction.Left)
			return GetBlock (new Vector2 (pos.x - index, pos.y));
		return null;
	}
	public List<Block> GetBlockListWith4(Vector2 pos,int distance,bool hasInner=true,bool hasSelf=false)
	{
		List<Block> list = new List<Block> ();
		foreach(Block block in CurrentMap) 
		{
			if (hasInner)
			{
				if (DistanceWith4 (pos, block.pos) <= distance)
					list.Add (block);
			} 
			else
			{
				if (DistanceWith4 (pos, block.pos) == distance)
					list.Add (block);
			}
		}
		if (!hasSelf)
			list.Remove (GetBlock(pos));
		return list;
	}
	public List<Block> GetBlockListWith8(Vector2 pos,int distance,bool hasInner=true,bool hasSelf=false)
	{
		List<Block> list = new List<Block> ();
		foreach(Block block in CurrentMap) 
		{
			if (hasInner)
			{
				if (DistanceWith8 (pos, block.pos) <= distance)
					list.Add (block);
			} 
			else
			{
				if (DistanceWith8 (pos, block.pos) == distance)
					list.Add (block);
			}
		}
		if (!hasSelf)
			list.Remove (GetBlock(pos));
		return list;
	}
	public int DistanceWith8(Vector2 v1,Vector2 v2)
	{
		return (int)(Mathf.Abs (v1.x - v2.x) > Mathf.Abs (v1.y - v2.y)
			? Mathf.Abs (v1.x - v2.x) : Mathf.Abs (v1.y - v2.y));
	}
	public int DistanceWith4(Vector2 v1,Vector2 v2)
	{
		return (int)(Mathf.Abs (v1.x - v2.x)+ Mathf.Abs (v1.y - v2.y));
	}
	public int GetMapMaxLength()
	{
		return CurrentMap.GetLength (0) > CurrentMap.GetLength (1) ? CurrentMap.GetLength (0) : CurrentMap.GetLength (1);
	}
	public void SetMapNormal()
	{
		foreach (Block block in CurrentMap) 
		{
			block.SetNormal ();
		}
	}
}
