using UnityEngine;
using System.Collections;

public class HandControlScript : MonoBehaviour {
	[SerializeField]
	int whocontroller = 1;
	// Use this for initialization
	void Start () {
	
	}
	void OnTriggerEnter(Collider other) {
		//Destroy(other.gameObject);
		if (whocontroller == 1 && other.collider.tag == "Player2")
		{
			Destroy(other.gameObject);
		}
		else if (whocontroller == 2 && other.collider.tag == "Player1")
		{
			Destroy(other.gameObject);
		}
	}
	// Update is called once per frame
	//void Update () {
	
	//}
}
