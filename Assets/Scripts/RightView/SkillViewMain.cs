using UnityEngine;
using System.Collections;
using FairyGUI;

public class SkillViewMain : MonoBehaviour {
	GComponent	_mainView;
	GComponent	_skillView;
	GButton[]	_skill;
	GImage[]    _mask;
	int			touchId;
	static int  _skill_num = 3;
	int[]		_cur_weapon_num = new int[_skill_num];
	
	enum WeaponState
	{
		WS_INVALID = -1,
		WS_RAISE,
		WS_SHOOT,
		WS_LOWER,
		WS_END,
	}

	enum WeaponType
	{
		WT_INVALID = -1,
		WT_GUN = 1,
	}
	static int	_weapon_num = 3;
	float[]		_default_shoot_time = new float[_weapon_num];
	float[]		_cur_shoot_speed = new float[_weapon_num];

	void Awake() {
		_default_shoot_time[0] = 1.1f;
		_cur_shoot_speed[0] = 1f;

		_cur_weapon_num[0] = 1;
	}

	void Start () {
		UIPackage.AddPackage("RightView");
		_skillView = UIPackage.CreateObject("RightView", "Main").asCom;

		_mainView = this.GetComponent<UIPanel>().ui;
		_mainView.AddChild(_skillView);
		Vector2 main_view_size = _mainView.size;
		_skillView.SetSize(main_view_size.x, main_view_size.y);

		_skill = new GButton[_skill_num];
		_mask = new GImage[_skill_num];
		for (int i = 0; i < _skill_num; ++i) {
			_skill[i] = _skillView.GetChild("b"+(i+1).ToString()).asButton;
			_skill[i].icon = UIPackage.GetItemURL("RightView", i.ToString());
			_skill[i].draggable = true;
			GButton btn = _skill[i];
			int index = i;
			btn.onDragStart.Add((EventContext context) =>
			{
				//Cancel the original dragging, and start a new one with a agent.
				context.PreventDefault();
				DragDropManager.inst.StartDrag(btn, btn.icon, btn.icon, (int)context.data);
			});
			btn.onTouchBegin.Add((EventContext context) =>
			{
				onTouchDown(context, index);
			});

			_mask[i] = _skill[i].GetChild("mask").asImage;
			_mask[i].fillAmount = 0;
		}

		touchId = -1;
	}
	
	void FixedUpdate () {
		
	}

	void onTouchDown(EventContext context, int index) {
		if (touchId == -1)//First touch
		{
			InputEvent evt = (InputEvent)context.data;
		}
	}
}
