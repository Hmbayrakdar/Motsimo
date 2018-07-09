using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
//using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;

public class NumbersSceneScript : MonoBehaviour {

	#region Variables

    public GameObject QuestionText, ShowPictureObject, restartObject, testStartObject, goBackObject;
    public GameObject[] TestPictureObjects;

    private int PictureCounter,randomInt;
	
    #endregion
	
    #region Unity Callbacks
    
    // Use this for initialization
    void Start ()
    {
	    PictureCounter = 1;
	    foreach (var t in TestPictureObjects)
	    {
		    t.tag = "trueAnswer";
	    }
    }
	
    // Update is called once per frame
    void Update () {
		
    }
    
    #endregion
    
    #region Functions

    public void RestartScene()
    {
        SceneManager.LoadScene("NumbersScene");
    }

	public void showNumbers()
	{
		if (PictureCounter < 9)
		{
			PictureCounter++;
			ShowPictureObject.GetComponent<Text>().text = PictureCounter.ToString();
		}
		else
		{
			ShowPictureObject.SetActive(false);
			
			restartObject.SetActive(true);
			testStartObject.SetActive(true);
			goBackObject.SetActive(true);
			PictureCounter = 0;
		}
	}

	public void startTest()
	{
		restartObject.SetActive(false);
		testStartObject.SetActive(false);
		goBackObject.SetActive(false);

		foreach (var t in TestPictureObjects)
		{
			t.SetActive(true);
		}
		
		testNumbers(UnityEngine.Random.Range(0,2));
	}

	public void testNumbers(int i)
	{
		goBackObject.SetActive(false);
		QuestionText.SetActive(true);

		if (PictureCounter >= 9)
		{
			QuestionText.SetActive(false);
			foreach(var t in TestPictureObjects)
				t.SetActive(false);
			
			restartObject.SetActive(true);
			testStartObject.SetActive(true);
			goBackObject.SetActive(true);
			QuestionText.SetActive(false);
		}
		
		if (!TestPictureObjects[i].CompareTag("trueAnswer")) return;

		PictureCounter++;
		QuestionText.GetComponent<Text>().text = "Hangisi " + PictureCounter +" göster.";
		randomInt = UnityEngine.Random.Range(1, 10);
		
		while (randomInt == PictureCounter)
		{
			randomInt = UnityEngine.Random.Range(1, 10);
		}

		var falseAnswer = randomInt;

		randomInt = UnityEngine.Random.Range(0, 2);

		switch (randomInt)
		{
			case 0:
				TestPictureObjects[randomInt].tag = "trueAnswer";
				TestPictureObjects[randomInt].GetComponent<Text>().text = PictureCounter.ToString();
				
				TestPictureObjects[1].GetComponent<Text>().text = falseAnswer.ToString();
				TestPictureObjects[1].tag = "falseAnswer";
				break;
			case 1:
				TestPictureObjects[randomInt].tag = "trueAnswer";
				TestPictureObjects[randomInt].GetComponent<Text>().text = PictureCounter.ToString();
				
				TestPictureObjects[0].GetComponent<Text>().text = falseAnswer.ToString();
				TestPictureObjects[0].tag = "falseAnswer";
				break;
			default:
				Debug.Log("Unexpected randomint");
				break;
		}
	}
	


    public void GoToConceptsMenu()
    {
        SceneManager.LoadScene("MainScene");
    }
    
    #endregion
}
