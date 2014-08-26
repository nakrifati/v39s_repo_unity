using UnityEngine;
using System.Collections;

public class DeadTimer : MonoBehaviour {
	public float Timer=5;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	Timer -= Time.deltaTime;
		
		if (Timer <=0)
		{
		GameObject.Destroy(gameObject);
		}
	}
}
