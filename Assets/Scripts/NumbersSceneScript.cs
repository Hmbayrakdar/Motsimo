using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite; using System.Data;
using System.IO;

public class NumbersSceneScript : MonoBehaviour {

	#region Variables

    public GameObject QuestionText, ShowPictureObject, restartObject, testStartObject, goBackObject, Point;
    public GameObject[] TestPictureObjects;

    private int PictureCounter,randomInt;
	private int[] FailCounter = new int[9];
	private string TestName = "Numbers";
	private string[] NumbersInTextForm = {"Bir", "İki", "Üç", "Dört", "Beş","Altı", "Yedi", "Sekiz", "Dokuz"};
	private string conn;

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

	    for (var i = 0; i < FailCounter.Length; i++)
	    {
		    FailCounter[i] = 0;
	    }
    }

    // Update is called once per frame
    void Update ()
    {

    }

	IEnumerator Help_Animation(string selected_animation)
	{
		yield return new WaitForSeconds(4);

		AudioSource.clip = IdentificationAudioClips[PictureCounter-1];
		AudioSource.Play();
		yield return new WaitForSeconds(AudioSource.clip.length);

		showNumbers();
		noAudioPlaying = true;
	}

	IEnumerator CongratsSound(int i)
	{
		if (!TestPictureObjects[i].CompareTag("trueAnswer")) {
			var number = PictureCounter - 1;
			FailCounter[number]++;
			yield break;
		}

		if (PictureCounter < 9)
		{
			gameObject.GetComponent<StarAnimationScript>().StarFunction();
		}

		yield return new WaitUntil(() => gameObject.GetComponent<StarAnimationScript>().APanel.activeSelf == false);

		noAudioPlaying = false;

		AudioSource.clip = congratsAudioClips[UnityEngine.Random.Range(0,5)];
		AudioSource.Play();

		yield return new WaitForSeconds(AudioSource.clip.length);

		if (PictureCounter >= 9)
		{
			gameObject.GetComponent<StarAnimationScript>().StartAnimation();

			questionTextObject.SetActive(false);
			foreach(var t in TestPictureObjects)
				t.SetActive(false);

			SendDataToDB();
			restartObject.SetActive(true);
			testStartObject.SetActive(true);
			goBackObject.SetActive(true);
			questionTextObject.SetActive(false);
			noAudioPlaying = true;
			yield break;
		}

		testNumbers(i);
		noAudioPlaying = true;
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

		AudioSource.clip = QuestionAudioClips[PictureCounter];
		AudioSource.Play();


		QuestionText.GetComponent<Text>().text = "Hangisi " + NumbersInTextForm[PictureCounter] +" göster.";
		PictureCounter++;
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
				StartCoroutine(Help_Animation("AnswerAnimation1"));
				break;
			case 1:
				TestPictureObjects[randomInt].tag = "trueAnswer";
				TestPictureObjects[randomInt].GetComponent<Text>().text = PictureCounter.ToString();

				TestPictureObjects[0].GetComponent<Text>().text = falseAnswer.ToString();
				TestPictureObjects[0].tag = "falseAnswer";
				StartCoroutine(Help_Animation("AnswerAnimation2"));
				break;
			default:
				Debug.Log("Unexpected randomint");
				break;
		}
	}

	public void SendDataToDB()
	{
		//Path to database.
        if (Application.platform == RuntimePlatform.Android)
        {
			conn = Application.persistentDataPath + "/Database.db";

			if(!File.Exists(conn)){
				WWW loadDB = new WWW("jar:file://" + Application.dataPath+ "!/assets/Database.db");

			while(!loadDB.isDone){}

				File.WriteAllBytes(conn,loadDB.bytes);
			}

        }
        else
        {
            // WINDOWS
			conn =Application.dataPath + "/StreamingAssets/Database.db";
        }

		IDbConnection dbconn;
        dbconn = (IDbConnection) new SqliteConnection("URI=file:" + conn);

		dbconn.Open(); //Open connection to the database.

		IDbCommand dbcmd = dbconn.CreateCommand();

		string sqlQuery = "INSERT INTO Test (TestType,StuNo,q1,q2,q3,q4,q5,q6,q7,q8,q9) values ('"+TestName+"',"+PlayerPrefs.GetInt("StuNumber")+","+FailCounter[0]+","+FailCounter[1]+","+FailCounter[2]+","+FailCounter[3]+","+FailCounter[4]+","+FailCounter[5]+","+FailCounter[6]+","+FailCounter[7]+","+FailCounter[8]+")";

		dbcmd.CommandText = sqlQuery;
		IDataReader reader = dbcmd.ExecuteReader();

		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		dbconn.Close();
		dbconn = null;
	}



    public void GoToConceptsMenu()
    {
        SceneManager.LoadScene("MainScene");
    }

    #endregion
}
