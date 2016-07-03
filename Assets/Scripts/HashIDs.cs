using UnityEngine;
using System.Collections;

public class HashIDs : MonoBehaviour {
	public int		GunShootStaticState;

	public int		SpeedFloat;
	public int		WeaponTypeInt;
	public int		EmitBool;
	public int		TurnAngleFloat;
	public int		WeaponStateInt;
	public int		ShootBool;

	// Use this for initialization
	void Start () {
		GunShootStaticState = Animator.StringToHash("BothHands.ShootStatic");

		SpeedFloat = Animator.StringToHash("Speed");
		WeaponTypeInt = Animator.StringToHash("WeaponType");
		EmitBool = Animator.StringToHash("Emit");
		TurnAngleFloat = Animator.StringToHash("TurnAngle");
		WeaponStateInt = Animator.StringToHash("WeaponState");
		ShootBool = Animator.StringToHash("Shoot");
	}
}
