using UnityEngine;
using System.Collections;
using Include;

public class Block : MonoBehaviour {
	private static readonly float BLOCK_LENGTH=3f;
	private static readonly float BLOCK_POS_Y=-0.5f;
	[SerializeField]private Material m_NormalMaterial; 
	[SerializeField]private Material m_MoveMaterial; 
	[SerializeField]private Material m_HitMaterial; 
	[SerializeField]private Material m_HitAndMoveMaterial; 
	private MeshRenderer m_Renderer;
	[HideInInspector]public Vector2 pos;
	[HideInInspector]public CharacterMono character;
	void Awake()
	{
		m_Renderer = GetComponent<MeshRenderer> ();
	}
	public void Init(int x,int y)
	{
		pos = new Vector2 (x, y);
		transform.position = new Vector3 (BLOCK_LENGTH*x,BLOCK_POS_Y,BLOCK_LENGTH*y);
	}
	public void SetNormal()
	{
		m_Renderer.material = m_NormalMaterial;
	}
	public void SetMovable()
	{
		m_Renderer.material = m_MoveMaterial;
	}
	public void SetHitable()
	{
		m_Renderer.material = m_HitMaterial;
	}
	public void SetHitableAndMovable()
	{
		m_Renderer.material = m_HitAndMoveMaterial;
	}
	public bool CanStay()
	{
		if (character == null)
			return true;
		return false;
	}
	public bool CanThrough(Team team)
	{
		if (character == null)
			return true;
		if (character.team == team)
			return true;
		return false;
	}
}
