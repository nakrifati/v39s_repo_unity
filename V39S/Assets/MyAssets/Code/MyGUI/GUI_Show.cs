using UnityEngine;
using System.Collections;

public class GUI_Show : MonoBehaviour {

	public bool triggered = false;

	// Use this for initialization
	void Start () {
		switch (triggered) {
			
			case false:
				gameObject.SetActive(false);
			break;
			
			case true:
				gameObject.SetActive(true);
			break;
		}
		
	}


	public void Show_Button(){
		switch (triggered) {

		case false:
			gameObject.SetActive(true);
			triggered = true;
			break;

		case true:
			gameObject.SetActive(false);
			triggered = false;
			break;
		}

		
		
	}
}
