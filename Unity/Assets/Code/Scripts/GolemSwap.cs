using UnityEngine;
using System.Collections;

public class GolemSwap : MonoBehaviour {

	int currentHP = 100;
	bool killOff = false;
	// Use this for initialization
	void Start () {
	
	}
	void isGolem (InputController script) {
		//script.climbTree = true;
		//int bob = 4;
		script.motor.ghostControl = false;

		if (script.motor.whocontroller == 1)
			script.hudHolder.HP_1 = currentHP;
		else
			script.hudHolder.HP_2 = currentHP;
		killOff = true;
	}
	// Update is called once per frame
	void Update () {
	
		if (killOff)
			//Destroy(this);
			Destroy(this.gameObject);
	}
}
