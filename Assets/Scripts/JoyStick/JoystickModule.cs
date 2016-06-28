using FairyGUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class JoystickModule : EventDispatcher
{
    bool is_show_touch_rect = true;
    public bool can_frame_touch { get; set; }

	float _InitX;
	float _InitY;
	float _startStageX;
	float _startStageY;
	float _lastStageX;
	float _lastStageY;
	GButton _button;
    GComponent _frame;
    GGraph  _touchArea;
	GObject _thumb;
	GObject _center;
    Controller _frame_ctrler;
	int touchId;
	Tweener _tweener;

	public EventListener onMove { get; private set; }
	public EventListener onEnd { get; private set; }

	public int radius { get; set; }
    public int frame_radius { get; set; }

	public float rotation { get; set; }
	public Vector3 targetDir { get; set; }
	public Player.Locomotion	locomotion { get; set; }

	public JoystickModule(GComponent mainView)
	{
		onMove = new EventListener(this, "onMove");
		onEnd = new EventListener(this, "onEnd");

		_button = mainView.GetChild("joystick").asButton;
		_button.changeStateOnClick = false;
		_thumb = _button.GetChild("thumb");
		_touchArea = mainView.GetChild("joystick_touch").asGraph;
        if (is_show_touch_rect) {
            _touchArea.DrawRect(_touchArea.width, _touchArea.height, 1, Color.gray, Color.clear);
        }
        _center = mainView.GetChild("joystick_center");
        _frame = mainView.GetChild("joystick_frame").asCom;
        _frame.visible = false;
        //GComponent frameView = UIPackage.CreateObject("Joystick", "frame").asCom;
        _frame_ctrler = _frame.GetController("button");

        _InitX = _center.x + _center.width / 2;
		_InitY = _center.y + _center.height / 2;
		touchId = -1;
		radius = 60;
        frame_radius = 106;
		locomotion = Player.Locomotion.Idle;

		can_frame_touch = true;
        _touchArea.onTouchBegin.Add(this.onTouchDown);
	}

	public void Trigger(EventContext context)
	{
		onTouchDown(context);
	}

	private void onTouchDown(EventContext context)
	{
		if (touchId == -1)//First touch
		{
			InputEvent evt = (InputEvent)context.data;
			touchId = evt.touchId;

			if (_tweener != null)
			{
				_tweener.Kill();
				_tweener = null;
			}

			Vector2 pt = GRoot.inst.GlobalToLocal(new Vector2(evt.x, evt.y));
			float bx = pt.x;
			float by = pt.y;
			_button.selected = true;

			if (bx < 0)
				bx = 0;
			else if (bx > _touchArea.width)
				bx = _touchArea.width;

			if (by > GRoot.inst.height)
				by = GRoot.inst.height;
			else if (by < _touchArea.y)
				by = _touchArea.y;

			_lastStageX = bx;
			_lastStageY = by;
			_startStageX = bx;
			_startStageY = by;

            _frame.visible = true;
            _frame.x = bx - _frame.width / 2;
            _frame.y = by - _frame.height / 2;

            _center.visible = true;
			_center.x = bx - _center.width / 2;
			_center.y = by - _center.height / 2;
			_button.x = bx - _button.width / 2;
			_button.y = by - _button.height / 2;

			float deltaX = bx - _InitX;
			float deltaY = by - _InitY;
			float degrees = Mathf.Atan2(deltaY, deltaX) * 180 / Mathf.PI;
			_thumb.rotation = degrees + 90;
			rotation = degrees;
			targetDir = new Vector3(deltaY, 0, deltaX);
			locomotion = Player.Locomotion.walk;

			Stage.inst.onTouchMove.Add(this.OnTouchMove);
			Stage.inst.onTouchEnd.Add(this.OnTouchUp);
		}
	}

	private void OnTouchUp(EventContext context)
	{
		InputEvent inputEvt = (InputEvent)context.data;
		if (touchId != -1 && inputEvt.touchId == touchId)
		{
            touchId = -1;
			_thumb.rotation = _thumb.rotation + 180;
			_center.visible = false;
            _frame.visible = false;
            _tweener = _button.TweenMove(new Vector2(_InitX - _button.width / 2, _InitY - _button.height / 2), 0.3f).OnComplete(() =>
			{
				_tweener = null;
				_button.selected = false;
				_thumb.rotation = 0;
				_center.visible = true;
				_center.x = _InitX - _center.width / 2;
				_center.y = _InitY - _center.height / 2;
			}
			);
			locomotion = Player.Locomotion.Idle;

			Stage.inst.onTouchMove.Remove(this.OnTouchMove);
			Stage.inst.onTouchEnd.Remove(this.OnTouchUp);

			this.onEnd.Call();
		}
	}

	private void OnTouchMove(EventContext context)
	{
		InputEvent evt = (InputEvent)context.data;
		if (touchId != -1 && evt.touchId == touchId)
		{
			Vector2 pt = GRoot.inst.GlobalToLocal(new Vector2(evt.x, evt.y));
			float bx = pt.x;
			float by = pt.y;

            float offsetX = bx - _startStageX;
            float offsetY = by - _startStageY;
            float offsetX_backup = offsetX;
            float offsetY_backup = offsetY;

            float rad = Mathf.Atan2(offsetY, offsetX);
            float degree = rad * 180 / Mathf.PI;
            _thumb.rotation = degree + 90;
			rotation = degree;
			targetDir = new Vector3(offsetY, 0, offsetX);

			bool is_over = false;
            // 控制内摇杆不出范围
            float maxX = radius * Mathf.Cos(rad);
            float maxY = radius * Mathf.Sin(rad);
            if (Mathf.Abs(offsetX) > Mathf.Abs(maxX)) {
                offsetX = maxX;
                is_over = true;
            }
            if (Mathf.Abs(offsetY) > Mathf.Abs(maxY)) {
                offsetY = maxY;
                is_over = true;
            }
            if (!is_over) {
                can_frame_touch = true;
			}
            // 控制外层变色
            is_over = false;
            maxX = frame_radius * Mathf.Cos(rad);
            maxY = frame_radius * Mathf.Sin(rad);
            if (Mathf.Abs(offsetX_backup) > Mathf.Abs(maxX)) {
                is_over = true;
            }
            if (Mathf.Abs(offsetY_backup) > Mathf.Abs(maxY)) {
                is_over = true;
            }
            if (can_frame_touch && is_over) {
                this.SetFrameState(FrameState.Touch);
			} else {
                this.SetFrameState(FrameState.NotTouch);
            }

            float buttonX = _startStageX + offsetX;
            float buttonY = _startStageY + offsetY;
            if (buttonX < 0)
                buttonX = 0;
            if (buttonY > GRoot.inst.height)
                buttonY = GRoot.inst.height;

            _button.x = buttonX - _button.width / 2;
            _button.y = buttonY - _button.height / 2;

            this.onMove.Call(degree);
		}
	}

    public enum FrameState {
        NotTouch = 0,
        Touch = 1,
    };
    public void SetFrameState(FrameState state) 
    {
        _frame_ctrler.SetSelectedIndex((int)state);
		switch(state) {
			case FrameState.NotTouch:
				locomotion = Player.Locomotion.walk;
				break;
			case FrameState.Touch:
				locomotion = Player.Locomotion.run;
				break;	
		}
    }
}