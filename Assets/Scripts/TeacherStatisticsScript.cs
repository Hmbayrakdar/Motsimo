using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeacherStatisticsScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnGUI() {
		GUILayout.BeginHorizontal("box",GUILayout.Width(1000),GUILayout.Height(200));
		GUILayout.Button("I'm the first button");
		GUILayout.Button("I'm in the middle");
		GUILayout.Button("I'm to the right");
		GUILayout.EndHorizontal();
	}
}
