using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    private Animator _animator;

    private int _speedId = 0;

	JoystickMain	_joystick;
	Transform		_transform;
	public float	_turnSmoothing = 15f;
	// 人物速度 实际0~10.5m/s
	private float   _walkSpeed = 1.6f;
    private float   _runSpeed = 5.3f;

    public enum Locomotion
    {
		walk = 0,
		run,
    };

    void Start () {
        _animator = GetComponent<Animator>();
		_speedId = Animator.StringToHash("Speed");

		GameObject UIPanel = GameObject.FindWithTag(Tags.UIPanel);
		_joystick = UIPanel.GetComponent<JoystickMain>();
		_transform = GetComponent<Transform>();
	}
	
	void Update () {
		//Quaternion rotatio = ;
		//rotatio.z = 50;
		Debug.Log("rotation = "+ _joystick.GetRotation());
		Rigidbody r = GetComponent<Rigidbody>();
		Vector3 targetDir = new Vector3(0, _joystick.GetRotation(), 0);
		Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
		Quaternion newRotation = Quaternion.Lerp(r.rotation, targetRotation,
													_turnSmoothing * Time.deltaTime);
		r.MoveRotation(newRotation);
		//_transform.rotation = new Quaternion(0, _joystick.GetRotation(), 0, 0);
		//_animator.SetFloat(_speedId, 6f, 0, Time.deltaTime);

		//AnimatorStateInfo animatorInfo = _animator.GetCurrentAnimatorStateInfo(0);
		//if (animatorInfo.IsName("Run")) {
		//    _animator.speed = 2f;
		//} else {
		//    _animator.speed = 1f;
		//}
	}
}
