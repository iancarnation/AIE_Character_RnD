using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Collections.Generic;


public class comboo : MonoBehaviour {
	XDocument xDoc;

	float previousButtonDownTime;

	
	public int whocontroller = 1;

	public List<char> combomove;
	public List<string> sMoveList;
	public List<string> sReleaseMoveList;

	private Animator anim;
	float fAnimationLength;
	AnimatorStateInfo CurrentState;

	int iPunchHash;
	int iKickHash;
	int iWalkHash;



	// Use this for initialization
	void Start () {
		combomove = new List<char> ();
		sMoveList = new List<string> ();
		sReleaseMoveList = new List<string>();
		
		xDoc =  XDocument.Load ("./Assets/ComboLoaded.xml");
		LoadXmltoList ();
		resetList ();
		

		anim = GetComponent<Animator>();
		CurrentState = anim.GetCurrentAnimatorStateInfo (0);

		iPunchHash = Animator.StringToHash ("Punch1");
		iKickHash = Animator.StringToHash ("Kick1");
		iWalkHash = Animator.StringToHash ("Walking");



	}

	void resetList()
	{
		sReleaseMoveList.Clear ();
		//sReleaseMoveList = sMoveList;
		foreach (string List in sMoveList) 
		{
			sReleaseMoveList.Add(List);
			
			
		}

	}

	void LoadXmltoList()
	{
		XElement root = xDoc.Document.Element("ComboList");
		XElement Element = root.Element("Strikes");
		
		XAttribute Attribute = Element.Attribute("Combo0");
		XAttribute NextAttribute = Element.Attribute("Combo0").NextAttribute;
		XAttribute LastAttribute = Element.LastAttribute;

		while (NextAttribute != LastAttribute)
		{
			sMoveList.Add((string)Attribute);
			Attribute = NextAttribute;
				NextAttribute = NextAttribute.NextAttribute;
			if (NextAttribute == LastAttribute)
			{
				sMoveList.Add((string)LastAttribute);
				
			}
		}




	}


	char ButtonCheck(){
		char coutput = 'E';
		if (whocontroller == 1)
		{
			if (/*Input.GetKeyDown(KeyCode.J)||*/Input.GetButtonDown("Punch")) {
				coutput = 'P';
				
			}
			if (/*Input.GetKeyDown(KeyCode.L)||*/ Input.GetButtonDown("Kick")){
				coutput = 'K';
			}
		}
		else if (whocontroller == 2)
		{
			if (/*Input.GetKeyDown(KeyCode.J)||*/Input.GetButtonDown("PunchP2")) {
				coutput = 'P';
				
			}
			if (/*Input.GetKeyDown(KeyCode.L)||*/ Input.GetButtonDown("KickP2")){
				coutput = 'K';
			}
		}

		return coutput;
	}



	bool timelapsecheck(){
		float fTimelapse = Time.time - previousButtonDownTime;
		if (fTimelapse <= (fAnimationLength)) {
			return true;
		}

		return false;

	}


	// Update is called once per frame
	void Update () {
		char button = ButtonCheck ();
		if (combomove.Count == 0 && button!= 'E') {
			combomove.Add(button);
			previousButtonDownTime = Time.time;
			AnimateCheck(button);
			Debug.Log (button);

		}
		else if (combomove.Count != 0&&button != 'E' && timelapsecheck() && MoreInputPossCheck() && NextInputPossible(button) && sMoveList.Count != 0 ) { 
			previousButtonDownTime = Time.time; 
			Debug.Log (button);
			combomove.Add(button);
			AnimateCheck(button);
		}
		else if(!timelapsecheck() && combomove.Count != 0){
			resetList();
			Debug.Log(sMoveList);
			AnimateCheck('E');
			combomove.Clear();	
		}
		if (whocontroller == 1)
		{
			if (Input.GetAxis("Vertical") != 0)
			{
				//anim.SetTrigger(iWalkHash);
				//animation.Play("Walking");
				anim.SetBool("Walking",true);
			}
			else
			{
				anim.SetBool("Walking",false);
			}
			if (Input.GetButton("Jump"))
			{
				anim.SetBool("Jump",true);
			}
			else
			{
				anim.SetBool("Jump",false);
			}
		}
		if (whocontroller == 2)
		{
			if (Input.GetAxis("VerticalP2") != 0)
			{
				//anim.SetTrigger(iWalkHash);
				//animation.Play("Walking");
				anim.SetBool("Walking",true);
			}
			else
			{
				anim.SetBool("Walking",false);
			}
			if (Input.GetButton("JumpP2"))
			{
				anim.SetBool("Jump",true);
			}
			else
			{
				anim.SetBool("Jump",false);
			}
		}
	}
	//Runs when object is destroyed
	void OnDestroy(){
		xDoc = null;
	}

	bool NextInputPossible(char cButtons)
	{
		int i = combomove.Count;
		i = i - 1;
		
		foreach(string checkmovelist in sReleaseMoveList)
		{
			char[] mychar = checkmovelist.ToCharArray();
			if(mychar[i +1] == '0' && sReleaseMoveList.Count == 1)
			{
				return false;
			}

			else if(mychar[i+1] == cButtons)
			{
				return true;
			}


		}
		return false;


	}




	bool MoreInputPossCheck()
	{
		if (combomove.Count == 0) {
			return true;
				}
		List <string> removeList = new List<string>();
		foreach (string combocheck in sReleaseMoveList)
		{
			int i= 0;
			char[] myChar = combocheck.ToCharArray();
			while(i < combomove.Count)
			{
				if (myChar[i] != combomove[i])
				{
					removeList.Add(combocheck);
					break;
				}
				i++;
			}
		}
		
		foreach (string remove in removeList)
		{
			sReleaseMoveList.Remove(remove);
		}
		if(sReleaseMoveList.Count >= 1)
		{
			return true;

		}
		else {
			return false;
		}
	}
	void AnimateCheck(char button)
	{
		anim.SetBool("Idle",false);
		anim.SetBool("Kick",false);
		anim.SetBool ("True", false);
		anim.SetBool ("Punch", false);
		int count = combomove.Count();

		if( button == 'P')
		{
			anim.SetBool("Punch",true);
			CurrentState = anim.GetCurrentAnimatorStateInfo (0);
			//-488670092
			if(count > 1 && CurrentState.nameHash == -488670092){
					anim.SetBool("True",true);
					anim.SetBool("Punch",false);
			}
			fAnimationLength = getAnimClip();

		}
		else if(button == 'K')
		{

			anim.SetBool("Kick",true);
			CurrentState = anim.GetCurrentAnimatorStateInfo (0);
			//-488670092
			if(count > 1 && CurrentState.nameHash == -1222267295){
				anim.SetBool("True",true);
				anim.SetBool("Kick",false);
			}
			fAnimationLength = getAnimClip();

		}
		else 
		{

			anim.SetBool("Idle",true);
			anim.SetBool("Punch",false);
			anim.SetBool("Kick",false);
		}





	}
	float getAnimClip()
	{
		float length;
		UnityEditorInternal.AnimatorController ac = anim.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
		UnityEditorInternal.StateMachine sm = ac.GetLayer(0).stateMachine;

		UnityEditorInternal.State state = sm.GetState (0);
		AnimationClip clip = state.GetMotion () as AnimationClip;

		length = clip.length;

		return length;

	}




}






















