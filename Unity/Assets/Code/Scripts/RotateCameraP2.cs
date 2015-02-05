using UnityEngine;
using System.Collections;

public class RotateCameraP2 : MonoBehaviour {
	
	public float sensitivityX = 5F;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Keypad4) == true)
		{
			
			transform.RotateAround(transform.position, Vector3.up, -sensitivityX);
			//print( transform.parent.position.x.ToString());
			//	transform.Rotate(0,transform.rotation.x + sensitivityX,0);
		}
		if (Input.GetKey(KeyCode.Keypad6) == true)
		{
			
			transform.RotateAround(transform.position, Vector3.up, sensitivityX);
			//	transform.Rotate(0,transform.rotation.x + sensitivityX,0);
		}
	}
}
