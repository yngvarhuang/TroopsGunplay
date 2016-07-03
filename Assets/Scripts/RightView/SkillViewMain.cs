using UnityEngine;
using System.Collections;
using FairyGUI;

public class SkillViewMain : MonoBehaviour {
	Animator _animator;
	HashIDs hash;
	GComponent	_mainView;
	GComponent	_skillView;
	GButton[]	_skill;
	GImage[]    _mask;
	int			touchId;
	static int  _skill_num = 3;
	int[]		_cur_weapon = new int[_skill_num];
	bool		is_over_skill_icon = false;

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
	struct WeaponInfo
	{
		public float	default_shoot_time;    // 1.0攻速时射击间隔时间
		public float	cur_shoot_speed;       // 当前射击速度
		public int		bullet_num;            // 子弹有多少个，射击几下就要换弹
		public float	reset_bullet_time;     // 换弹时间
		public float	cur_reset_bullet_time; // 当前换弹还剩下的时间
	}
	WeaponInfo[]	_weapon_info = new WeaponInfo[_weapon_num];

	void Awake() {
		_cur_weapon[0] = 1;
		_cur_weapon[1] = (int)WeaponType.WT_INVALID;
		_cur_weapon[2] = (int)WeaponType.WT_INVALID;

		//ref WeaponInfo info = _weapon_info[0];
		_weapon_info[0].default_shoot_time = 1.1f;
		_weapon_info[0].cur_shoot_speed = 1f;
		_weapon_info[0].bullet_num = 10;
		_weapon_info[0].reset_bullet_time = 5f;
		_weapon_info[0].cur_reset_bullet_time = -1f;
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
				context.PreventDefault();
				if ((int)WeaponType.WT_INVALID == _cur_weapon[index]) {
					context.StopPropagation();
					return;
				}
				is_over_skill_icon = false;
				// 让其它图标可拖拽
				DragDropManager.inst.StartDrag(btn, btn.icon, btn.icon, (int)context.data);
			});
			btn.onTouchBegin.Add((EventContext context) =>
			{
				is_over_skill_icon = false;
				onTouchDown(context, index);
			});

			_mask[i] = _skill[i].GetChild("mask").asImage;
			_mask[i].fillAmount = 0;
		}

		touchId = -1;
		
		_animator = GameObject.FindWithTag(Tags.MainRole).GetComponent<Animator>();
		hash = GameObject.FindWithTag(Tags.GameController).GetComponent<HashIDs>();
		_animator.SetInteger(hash.WeaponStateInt, (int)WeaponState.WS_INVALID);

		GameObject[] object_list = GameObject.FindGameObjectsWithTag(Tags.MainRole);
		foreach (GameObject go in object_list) {
			Debug.Log("objec_list = "+go.tag);
		}
	}
	
	void FixedUpdate () {
		// 根据 touchmove 算出的方向或位置 显示射击辅助线或圈
		if (is_over_skill_icon) {

		}

		int weapon_stae = _animator.GetInteger(hash.WeaponStateInt);
		switch(weapon_stae) {
			case (int)WeaponState.WS_RAISE:
				UpdateRaise();
				break;
			case (int)WeaponState.WS_SHOOT:
				UpdateShoot();
				break;
			case (int)WeaponState.WS_LOWER:
				UpdateLower();
				break;
		}
		
	}

	void UpdateRaise() {

	}

	void UpdateShoot() {
		if (!_animator.GetBool(hash.ShootBool)) {
			return;
		}

		// 转弯
		// 射击动画
		// 发射子弹
	}

	public void OnOneShootOver() {
		Debug.Log("OnOneShootOver");
		_animator.SetBool(hash.ShootBool, false);
		_animator.SetInteger(hash.WeaponStateInt, (int)WeaponState.WS_INVALID);
	}

	void UpdateLower() {

	}

	void onTouchDown(EventContext context, int index) {
		if (touchId == -1)//First touch
		{
			if ((int)WeaponType.WT_INVALID == _cur_weapon[index]) {
				// 弹出提示
				return;
			}

			InputEvent evt = (InputEvent)context.data;
			touchId = evt.touchId;

			_animator.SetInteger(hash.WeaponTypeInt, (int)WeaponType.WT_GUN);
			// 显示辅助线或圈
			// 显示武器范围


			Stage.inst.onTouchMove.Add((EventContext event_context) => { 
				this.OnTouchMove(event_context, index);
			});
			Stage.inst.onTouchEnd.Add(this.OnTouchUp);
		}
	}

	void OnTouchMove(EventContext context, int index) {
		InputEvent inputEvt = (InputEvent)context.data;
		if (touchId != -1 && inputEvt.touchId == touchId) {
			Vector2 pt = GRoot.inst.GlobalToLocal(new Vector2(inputEvt.x, inputEvt.y));

			Vector2 btn_pos = _skill[index].position;
			Vector2 btn_size = _skill[index].size;
			
			if (pt.x < btn_pos.x || pt.x > btn_pos.x + btn_size.x
				|| pt.y < btn_pos.y || pt.y > btn_pos.y + btn_size.y) {
				is_over_skill_icon = true;
			}

			// 算出方向 或 点击位置 用lineRenderer画出来
			//Debug.Log("pos = " + pt.x + " " + pt.y+" "+ btn_pos.x+" " + btn_pos.y);
		}
	}

	void OnTouchUp(EventContext context) {
		InputEvent inputEvt = (InputEvent)context.data;
		if (touchId != -1 && inputEvt.touchId == touchId) {
			Vector2 pt = GRoot.inst.GlobalToLocal(new Vector2(inputEvt.x, inputEvt.y));

			Debug.Log("OnTouchUp");
			// 是右上角取消的话，把is_over_skill_icon设为false并 取消范围，取消射击辅助 武器状态设置为WS_LOWER

			// 直接设置 rotate 转向
			// 提起武器 WS_RAISE
			_animator.SetInteger(hash.WeaponStateInt, (int)WeaponState.WS_SHOOT);
			_animator.SetBool(hash.ShootBool, true);
			// 设置要射击的方向或位置，状态设置为 WS_SHOOT 参数shoot为true
			//Debug.Log("pos = " + pt.x + " " + pt.y);

			touchId = -1;
		}
	}
}
