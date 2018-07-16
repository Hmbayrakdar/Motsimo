using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudentButtonScript : MonoBehaviour {

	public void ChooseStudent()
	{
		PlayerPrefs.SetInt("StuNumber", Int32.Parse(gameObject.transform.GetChild(0).GetComponent<Text>().text));
		print("Chosen stuNo: " + PlayerPrefs.GetInt("StuNumber"));
	}
}
