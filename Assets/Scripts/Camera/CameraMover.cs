using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour 
{
	public Transform follow;
	public float offsetX = 5.0f;
	public float offsetY = 6.0f;
	public float offsetZ = 0f;
	public float smooth = 1.0f;
	
	private Vector3 targetPosition;	

	void FixedUpdate()
	{
		targetPosition = follow.position + new Vector3(offsetX, offsetY, offsetZ);
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smooth);
		//transform.position = targetPosition;
	}
}
