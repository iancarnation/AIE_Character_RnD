using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUD : MonoBehaviour {


	private static HUD _instance = new HUD();
	public static HUD Instance { get { return _instance; } }


	public bool hpDebug;


	public RectTransform[] hpTrans;
	public Image[] hpBar;
	public Text[] hpText;

	float cachedY;
	float[] minX;
	float[] maxX;

	[SerializeField]
	public int[] currentHP;

	int maxHP = 100;

	void Start() {

		minX = new float[2];
		maxX = new float[2];
		currentHP = new int[2];

		cachedY = hpTrans[0].position.y;

		currentHP[0] = maxHP;
		currentHP [1] = maxHP;

		// set min and max sizes of hp bars relative to max hp
		maxX[0] = hpTrans[0].position.x;
		minX[0] = hpTrans[0].position.x - hpTrans[0].rect.width;

		maxX[1] = hpTrans[1].position.x;
		minX[1] = hpTrans[1].position.x + hpTrans[1].rect.width;


	}

	public int HP_1 {
		get{ return currentHP[0]; }
		set{ currentHP[0] = value;
			UpdateHP(); }
	}

	public int HP_2 {
		get{ return currentHP[1]; }
		set{ currentHP[1] = value;
			UpdateHP(); }
	}


	float TranslateValues(int hp, int min, int max, float posMin, float posMax) {

		// formula to convert the min and max hp values into transofrm X positions on the hp bar
		return (hp - min) * (posMax - posMin) / (max - min) + posMin;
	}
	

	void UpdateHP() {

		for(int i=0; i<2; i++) {
			if(currentHP[i] < 0)
				currentHP[i] = 0;
			else if(currentHP[i] > maxHP)
				currentHP[i] = maxHP;
		}


		// MOVE POSITION
		for(int i=0; i<2; i++)
		{
			float currentXValue = TranslateValues (currentHP[i], 0, 100, minX[i], maxX[i]);
			hpTrans[i].position = new Vector3 (currentXValue, cachedY, 0);
		}


		// CHANGE COLOR
		for(int i=0; i<2; i++)
		{
			if(currentHP[i] > maxHP/2)
			{
				hpBar[i].color = new Color32((byte)TranslateValues(currentHP[i], maxHP/2, maxHP, 255, 0), 255,0,255);

			} else {
				hpBar[i].color = new Color32(255, (byte)TranslateValues(currentHP[i], 0, maxHP/2, 0, 255),0,255);
			}
		}
	}

	void UpdateScreen() {
	}


	void Update() {

		/*if (Input.GetKey (KeyCode.A)) {
			HP_1 -= 1;
		}
		if (Input.GetKey (KeyCode.D)) {
			HP_1 += 1;
		}

		if (Input.GetKey (KeyCode.LeftArrow)) {
			HP_2 -= 1;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			HP_2 += 1;
		}*/

		//UpdateScreen();
		//UpdateHP ();
		if(hpDebug) {
			Debug.Log ("HP 1: " + currentHP[0]);
			Debug.Log ("HP 2: " + currentHP[1]);
		}

		hpText[0].text = currentHP[0].ToString() + " / 100";
		hpText[1].text = currentHP[1].ToString() + " / 100";



	}
	
	void Awake() {
		_instance = this;
		DontDestroyOnLoad (this.gameObject);
	}
}
