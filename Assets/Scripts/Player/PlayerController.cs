using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    private Animator _animator;

    private int _speedId = 0;

	JoystickMain	_joystick;
	Transform		_transform;
	public float	_turnSmoothing = 15f;
	// 人物速度 实际0~10.5m/s
	private float   _walkSpeed = 2.2f;
    private float   _runSpeed = 7f;
	public  float	_speedDampTime = 0.2f;

  //  public enum Locomotion
  //  {
		//Idle = 0,
		//walk,
		//run,
  //  };

    void Start () {
        _animator = GetComponent<Animator>();
		_speedId = Animator.StringToHash("Speed");

		GameObject UIPanel = GameObject.FindWithTag(Tags.UIPanel);
		_joystick = UIPanel.GetComponent<JoystickMain>();
		_transform = GetComponent<Transform>();
	}
	
	void FixedUpdate () {
		//Quaternion rotatio = ;
		//rotatio.z = 50;
		//Vector3 temp_vec = _joystick.GetTargetDirection();
		//Debug.Log("rotation = " + temp_vec.x + " "+ temp_vec.y + " " + temp_vec.z);

		Rigidbody r = GetComponent<Rigidbody>();
		Vector3 targetDir = _joystick.GetTargetDirection();
		Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
		transform.rotation = targetRotation;
		//Quaternion newRotation = Quaternion.Lerp(r.rotation, targetRotation,
		//											_turnSmoothing * Time.deltaTime);
		//r.MoveRotation(newRotation);

		//_transform.rotation = new Quaternion(0, _joystick.GetRotation(), 0, 0);
		Player.Locomotion locomotion = _joystick.GetLocomotion();
		float speed = 0f;
		switch(locomotion) {
			case Player.Locomotion.Idle:
				speed = 0;
				break;
			case Player.Locomotion.walk:
				speed = _walkSpeed;
				break;
			case Player.Locomotion.run:
				speed = _runSpeed;
				break;
		};
		_animator.SetFloat(_speedId, speed, _speedDampTime, Time.deltaTime);

		//AnimatorStateInfo animatorInfo = _animator.GetCurrentAnimatorStateInfo(0);
		//if (animatorInfo.IsName("Run")) {
		//    _animator.speed = 2f;
		//} else {
		//    _animator.speed = 1f;
		//}
	}
}
