using UnityEngine;
using FairyGUI;

public class JoystickMain : MonoBehaviour
{
    bool is_show_text = true;

    GComponent _mainView;
	GTextField _text;
	JoystickModule _joystick;

	void Start()
	{
		Application.targetFrameRate = 60;
		Stage.inst.onKeyDown.Add(OnKeyDown);
        //GRoot.inst.SetContentScaleFactor(1136, 640);
        _mainView = this.GetComponent<UIPanel>().ui;

		_text = _mainView.GetChild("n9").asTextField;
        if (!is_show_text) {
            GTextField text = _mainView.GetChild("n8").asTextField;
            text.visible = false;
            _text.visible = false;
        }

		_joystick = new JoystickModule(_mainView);
		_joystick.onMove.Add(__joystickMove);
		_joystick.onEnd.Add(__joystickEnd);
	}

	void __joystickMove(EventContext context)
	{
        if (!is_show_text) {
            return;
        }
		float degree = (float)context.data;
		_text.text = "" + degree;
	}

	void __joystickEnd()
	{
		_text.text = "";
	}

	void OnKeyDown(EventContext context)
	{
		if (context.inputEvent.keyCode == KeyCode.Escape)
		{
			Application.Quit();
		}
	}

	public float GetRotation() {
		return _joystick.rotation;
	}

	public Vector3 GetTargetDirection() {
		return _joystick.targetDir.normalized;
	}

	public Player.Locomotion GetLocomotion() {
		return _joystick.locomotion;
	}
}