using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterMotor))]
[AddComponentMenu ("Character/InputControllerP2")]
public class InputControllerP2 : MonoBehaviour {

	private CharacterMotor motor;
	public void Awake()
	{
		this.motor = (CharacterMotor)this.GetComponent(typeof(CharacterMotor));
	}
	public void Update()
	{
		Vector3 vector = new Vector3(Input.GetAxis("HorizontalP2"), (float)0, Input.GetAxis("VerticalP2"));
		if (vector != Vector3.zero)
		{
			float num = vector.magnitude;
			vector /= num;
			num = Mathf.Min((float)1, num);
			num *= num;
			vector *= num;
		}
		this.motor.inputMoveDirection = this.transform.rotation * vector;
		this.motor.inputJump = Input.GetButton("JumpP2");
	}
}
