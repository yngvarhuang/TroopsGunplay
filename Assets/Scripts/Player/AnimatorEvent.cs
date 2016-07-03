using UnityEngine;
using System.Collections;

public class AnimatorEvent : MonoBehaviour {
	SkillViewMain skill_view;
	// Use this for initialization
	void Start () {
		skill_view = GameObject.FindWithTag(Tags.UIPanel).GetComponent<SkillViewMain>();
	}
	
	void OnOneShootOver() {
		skill_view.OnOneShootOver();
	}
}
