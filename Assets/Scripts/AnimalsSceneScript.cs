using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite; using System.Data;

public class AnimalsSceneScript : MonoBehaviour {

    #region Variables

    public GameObject questionTextObject, ShowPictureObject, restartObject, testStartObject, goBackObject, Point;
    public GameObject[] TestPictureObjects;
    public Sprite[] AnimalSprites;


    private int PictureCounter;
    private int[] FailCounter = new int[5];
    private string TestName = "Animals";
    private string[] animals = {"Balık", "İnek", "Kedi", "Köpek", "Tavşan"};
	private string conn;

    #endregion

    #region Unity Callbacks

    // Use this for initialization
    void Start () {

        PictureCounter = 0;
        foreach (var t in TestPictureObjects)
        {
            t.tag = "trueAnswer";
        }

        for (int i = 0; i < FailCounter.Length; i++)
        {
            FailCounter[i] = 0;
        }



        showAnimalImage();
    }

    // Update is called once per frame
    void Update ()
    {

    }

    IEnumerator Help_Animation(string selected_animation)
    {
		if (!TestPictureObjects[i].CompareTag("trueAnswer")) {
			int number = PictureCounter - 1;
            FailCounter[number]++;
			yield break;
		}


        noAudioPlaying = false;

        if (PictureCounter < AnimalSprites.Length)
        {
            gameObject.GetComponent<StarAnimationScript>().StarFunction();
        }

        yield return new WaitUntil(() => gameObject.GetComponent<StarAnimationScript>().APanel.activeSelf == false);


        AudioSource.clip = congratsAudioClips[UnityEngine.Random.Range(0,5)];
        AudioSource.Play();
        yield return new WaitForSeconds(AudioSource.clip.length);


        if (PictureCounter >= AnimalSprites.Length)
        {
            gameObject.GetComponent<StarAnimationScript>().StartAnimation();
            TestPictureObjects[0].SetActive(false);
            TestPictureObjects[1].SetActive(false);
            questionTextObject.SetActive(false);

            PictureCounter = 0;
            SendDataToDB();

            restartObject.SetActive(true);
            testStartObject.SetActive(true);
            goBackObject.SetActive(true);
            noAudioPlaying = true;
            yield break;
        }

        testAnimals(i);
        noAudioPlaying = true;
    }

    #endregion

    #region Function

    public void RestartScene()
    {
        SceneManager.LoadScene("AnimalsScene");
    }

    public void showAnimalImage()
    {
        if (PictureCounter < AnimalSprites.Length)
        {

            RacoonText.GetComponent<Text>().text = "Bu bir " + animals[PictureCounter];
            ShowPictureObject.GetComponent<Image>().overrideSprite = AnimalSprites[PictureCounter];
            PictureCounter++;
        }
        else
        {
            ShowPictureObject.SetActive(false);

            restartObject.SetActive(true);
            testStartObject.SetActive(true);
            goBackObject.SetActive(true);

        }
    }

    public void testStart()
    {
        restartObject.SetActive(false);
        testStartObject.SetActive(false);
        goBackObject.SetActive(false);

        questionTextObject.SetActive(true);
        foreach (var t in TestPictureObjects)
        {
            t.SetActive(true);
        }

        PictureCounter = 0;
        testAnimals(-1);
    }


    public void testAnimals(int i)
    {
        var randomInteger = UnityEngine.Random.Range(0, 2);

        if (i == -1)
        {
            switch (randomInteger)
            {
                case 0:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + animals[PictureCounter] + " Göster";

                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = AnimalSprites[PictureCounter];
                    TestPictureObjects[randomInteger].tag = "trueAnswer";

                    LoadRandomColorPictureToOtherObject(1);
                    PictureCounter++;
                    StartCoroutine(Help_Animation("AnswerAnimation1"));

                    break;
                case 1:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + animals[PictureCounter] + " Göster";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = AnimalSprites[PictureCounter];
                    TestPictureObjects[randomInteger].tag = "trueAnswer";

                    LoadRandomColorPictureToOtherObject(0);
                    PictureCounter++;
                    StartCoroutine(Help_Animation("AnswerAnimation2"));

                    break;
                default:
                    Debug.Log("Unexpected random integer.");
                    break;
            }

            return;
        }

        AudioSource.clip = QuestionAudioClips[PictureCounter];
        AudioSource.Play();

        switch (randomInteger)
        {
            case 0:
                questionTextObject.GetComponent<Text>().text = "Hangisi " + animals[PictureCounter] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = AnimalSprites[PictureCounter];
                TestPictureObjects[randomInteger].tag = "trueAnswer";

                LoadRandomColorPictureToOtherObject(1);
                PictureCounter++;
                StartCoroutine(Help_Animation("AnswerAnimation1"));

                break;
            case 1:
                questionTextObject.GetComponent<Text>().text = "Hangisi " + animals[PictureCounter] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = AnimalSprites[PictureCounter];
                TestPictureObjects[randomInteger].tag = "trueAnswer";

                LoadRandomColorPictureToOtherObject(0);
                PictureCounter++;
                StartCoroutine(Help_Animation("AnswerAnimation2"));

                break;
            default:
                Debug.Log("Unexpected random integer.");
                break;
        }
    }

    private void LoadRandomColorPictureToOtherObject(int TestObjectNumber)
    {
        var randomInteger = UnityEngine.Random.Range(0, AnimalSprites.Length);

        while (randomInteger == PictureCounter)
        {
            randomInteger = UnityEngine.Random.Range(0, AnimalSprites.Length);
        }

        TestPictureObjects[TestObjectNumber].GetComponent<Image>().sprite = AnimalSprites[randomInteger];
        TestPictureObjects[TestObjectNumber].tag = "falseAnswer";

    }


    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainScene");
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

        string sqlQuery = "INSERT INTO Test (TestType,StuNo,q1,q2,q3,q4,q5) values ('"+TestName+"',"+PlayerPrefs.GetInt("StuNumber")+","+FailCounter[0]+","+FailCounter[1]+","+FailCounter[2]+","+FailCounter[3]+","+FailCounter[4]+")";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }


    #endregion
}
