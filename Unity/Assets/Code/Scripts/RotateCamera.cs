using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour {
	
	public float sensitivityX = 5F;
	public int whocontroller = 1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (whocontroller == 1)
		{
			if (Input.GetKey(KeyCode.A) == true || Input.GetAxis("Horizontal") < 0)
			{

				transform.RotateAround(transform.position, Vector3.up, -sensitivityX);
				//print( transform.parent.position.x.ToString());
				//	transform.Rotate(0,transform.rotation.x + sensitivityX,0);
			}
			if (Input.GetKey(KeyCode.D) == true || Input.GetAxis("Horizontal") > 0)
			{
				
				transform.RotateAround(transform.position, Vector3.up, sensitivityX);
				//	transform.Rotate(0,transform.rotation.x + sensitivityX,0);
			}
		}
		else if (whocontroller == 2)
		{
			if (Input.GetKey(KeyCode.Keypad4) == true || Input.GetAxis("HorizontalP2") < 0)
			{
				
				transform.RotateAround(transform.position, Vector3.up, -sensitivityX);
				//print( transform.parent.position.x.ToString());
				//	transform.Rotate(0,transform.rotation.x + sensitivityX,0);
			}
			if (Input.GetKey(KeyCode.Keypad6) == true || Input.GetAxis("HorizontalP2") < 0)
			{
				
				transform.RotateAround(transform.position, Vector3.up, sensitivityX);
				//	transform.Rotate(0,transform.rotation.x + sensitivityX,0);
			}
		}
	}
}
