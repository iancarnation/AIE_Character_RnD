using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterMotor))]
[AddComponentMenu ("Character/InputController")]
[SerializeField]
public class InputController : MonoBehaviour {

	public CharacterMotor motor;
	private Animator anim;
	AnimatorStateInfo CurrentState;
	public HUD hudHolder = new HUD();
	bool locknGrow = false;
	public Mesh ghost;


	void OnTriggerEnter(Collider other) {
		//Destroy(other.gameObject);
		if (motor.ghostControl || locknGrow)
			return;
		if (motor.whocontroller == 1 && other.collider.tag == "Player2")
		{
			if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Punch1"))
				hudHolder.HP_2 = hudHolder.HP_2 - 4;
			else if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Punch2"))
				hudHolder.HP_2 = hudHolder.HP_2 - 8;
			else if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Kick1"))
				hudHolder.HP_2 = hudHolder.HP_2 - 1;
			else if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Kick2"))
				hudHolder.HP_2 = hudHolder.HP_2 - 2;
			//Destroy(other.gameObject);
		}
		else if (motor.whocontroller == 2 && other.collider.tag == "Player1")
		{
			if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Punch1"))
				hudHolder.HP_1 = hudHolder.HP_1 - 4;
			else if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Punch2"))
				hudHolder.HP_1 = hudHolder.HP_1 - 8;
			else if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Kick1"))
				hudHolder.HP_1 = hudHolder.HP_1 - 1;
			else if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Kick2"))
				hudHolder.HP_1 = hudHolder.HP_1 - 2;
			//Destroy(other.gameObject);
		}
	}

	public void Awake()
	{
		this.motor = (CharacterMotor)this.GetComponent(typeof(CharacterMotor));
		Animator[] tempAnim;
		tempAnim = GetComponentsInChildren<Animator>();
		foreach(Animator subAnim in tempAnim)
			this.anim = subAnim;

		CurrentState = anim.GetCurrentAnimatorStateInfo (0);
		GameObject tempHudHolder = GameObject.Find("HUD");
		hudHolder = (HUD)tempHudHolder.GetComponentInChildren<HUD>();
			//GetComponentsInChildren<HingeJoint>();
		//this.anim = (Animator)this.GetComponent(typeof(Animator));
	}
	public void Update()
	{
		Vector3 inputControl = Vector3.zero;
		if (motor.whocontroller == 1)
		{
			inputControl = new Vector3(Input.GetAxis("Strafe"), (float)0, Input.GetAxis("Vertical"));
		}
		else if (motor.whocontroller == 2)
		{
			inputControl = new Vector3(Input.GetAxis("StrafeP2"), (float)0, Input.GetAxis("VerticalP2"));
		}

		if (!locknGrow)
		{
			if (motor.ghostControl)
			{
				Vector3 vector = new Vector3(/*Input.GetAxis("Horizontal")*/0, (float)0, /*Input.GetAxis("Vertical")*/5);
				if (vector != Vector3.zero)
				{
					float num = vector.magnitude;
					vector /= num;
					num = Mathf.Min((float)1, num);
					num *= num;
					vector *= num;
				}
				this.motor.inputMoveDirection = this.transform.rotation * vector;
				bool swapCheck = (Input.GetButtonDown("Swap") && motor.whocontroller == 1) ||(Input.GetButtonDown("SwapP2") && motor.whocontroller == 2);
				if (swapCheck)
				{

					RaycastHit hit;
					Ray ray = new Ray(transform.position,transform.forward);
					//transform.
					int currentRotation = (int)transform.rotation.eulerAngles.y;

					Vector3 targetPos = transform.position;
					targetPos.z += 4;
					Vector3 fwd = transform.TransformDirection(Vector3.forward);
					//if (Physics.Raycast(transform.position, fwd, 4))
					//if (Physics.Raycast(transform.position, , out hit, 100.0F))
					
					if(Physics.Raycast(ray, out hit, 1.0f))
					{
						hit.transform.SendMessage ("isGolem",this);
					}
					if (!motor.ghostControl)
					{
						//Transform child = this.transform.GetChild(0);
						//Mesh meshInstance  = Instantiate(ghost);
						//GetComponent<MeshFilter>().mesh = meshInstance;
						//
						//child.renderer.enabled = false;
						Mesh mesh = this.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
						mesh.Clear();
						//mesh.Equals(ghost);// = ghost;
						transform.localScale = new Vector3(1,1,1);
						locknGrow = true;
					}
				}
				//this.motor.inputJump = Input.GetButton("Jump");
			}
			else
			{
				//if punch animation is playing stop the movement
				//if (anim.animation.IsPlaying("Punch1"))
				/*int hash = Animator.StringToHash ("Punch1");
				if (CurrentState.nameHash == Animator.StringToHash ("Punch1"))
				{*/
				if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Punch1"))
				{
					// Avoid any reload.
					inputControl.z = 0;
					inputControl.x = 0;
					this.motor.inputMoveDirection = Vector3.zero;
				}
				Vector3 vector = inputControl;
				if (vector != Vector3.zero)
				{
					float num = vector.magnitude;
					vector /= num;
					num = Mathf.Min((float)1, num);
					num *= num;
					vector *= num;
				}
				this.motor.inputMoveDirection = this.transform.rotation * vector;
				
				if (motor.whocontroller == 1)
					this.motor.inputJump = Input.GetButton("Jump");
				
				if (motor.whocontroller == 2)
					this.motor.inputJump = Input.GetButton("JumpP2");
			}
		}
		else if (locknGrow)
		{
			Vector3 growScale = new Vector3(0.1f,0.1f,0.1f);
			transform.localScale = transform.localScale + growScale;
			transform.position = transform.position + growScale;
			if (transform.localScale.x >= 2.5f)
				locknGrow = false;
		}
	}
}
