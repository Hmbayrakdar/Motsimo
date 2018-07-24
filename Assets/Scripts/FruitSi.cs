using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
//using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;

public class FruitSi : MonoBehaviour
{
    #region Variables
    public GameObject questionTextObject, ShowPictureObject, restartObject, testStartObject, goBackObject, Point;
    public GameObject[] TestPictureObjects;
    public Sprite[] FruitSprites;

    private int PictureCounter;
    private int[] FailCounter = new int[5];
    private string TestName = "Fruits";
    private string[] fruits = { "Muz", "Çilek", "Armut", "Elma", "Kiraz" };
	private string conn;

    #endregion
    // Use this for initialization
    #region Unity Callbacks
    void Start()
    {
        PictureCounter = 0;
        foreach (var t in TestPictureObjects)
        {
            t.tag = "trueAnswer";
        }
        FailCounter[0] = 0;
        FailCounter[1] = 0;
        FailCounter[2] = 0;
        FailCounter[3] = 0;
        FailCounter[4] = 0;

        showFruitsImage();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Identifnd()
    {

        yield return new WaitForSeconds(5);

        print("hello");
    }

    IEnumerator IdentifySound()
    {
        noAudioPlaying = false;

        AudioSource.clip = IdentificationAudioClips[PictureCounter-1];
        AudioSource.Play();
        yield return new WaitForSeconds(AudioSource.clip.length);

        showFruitsImage();
        noAudioPlaying = true;
    }

    IEnumerator CongratsSound(int i)
    {
        if (!TestPictureObjects[i].CompareTag("trueAnswer"))
        {
            int number = PictureCounter - 1;
            FailCounter[number]++;
            yield break;
        }

        if (PictureCounter < FruitSprites.Length)
        {
            gameObject.GetComponent<StarAnimationScript>().StarFunction();
        }
        yield return new WaitUntil(() => gameObject.GetComponent<StarAnimationScript>().APanel.activeSelf == false);

        noAudioPlaying = false;

        AudioSource.clip = congratsAudioClips[UnityEngine.Random.Range(0,5)];
        AudioSource.Play();
        yield return new WaitForSeconds(AudioSource.clip.length);


        if (PictureCounter >= FruitSprites.Length)
        {
            gameObject.GetComponent<StarAnimationScript>().StartAnimation();
            TestPictureObjects[0].SetActive(false);
            TestPictureObjects[1].SetActive(false);
            questionTextObject.SetActive(false);

        Point.SetActive(true);
        Point.GetComponent<Animation>().Play(selected_animation);

            restartObject.SetActive(true);
            testStartObject.SetActive(true);
            goBackObject.SetActive(true);
            noAudioPlaying = true;
            yield break;
        }

        testFruits(i);
        noAudioPlaying = true;
    }
    #endregion
    #region Function

    public void RestartScene()
    {
        SceneManager.LoadScene("FruitsScene");
    }

    public void showFruitsImage()
    {
        if (PictureCounter < FruitSprites.Length)
        {
            ShowPictureObject.GetComponent<Image>().overrideSprite = FruitSprites[PictureCounter];
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
        testFruits(-1);
    }


    public void testFruits(int i)
    {
        var randomInteger = UnityEngine.Random.Range(0, 2);

        if (i == -1)
        {
            switch (randomInteger)
            {
                case 0:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + fruits[PictureCounter] + " Göster";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = FruitSprites[PictureCounter];
                    TestPictureObjects[randomInteger].tag = "trueAnswer";

                    LoadRandomColorPictureToOtherObject(1);
                    PictureCounter++;
                    StartCoroutine(Help_Animation("AnswerAnimation1"));

                    break;
                case 1:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + fruits[PictureCounter] + " Göster";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = FruitSprites[PictureCounter];
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
                questionTextObject.GetComponent<Text>().text = "Hangisi " + fruits[PictureCounter] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = FruitSprites[PictureCounter];
                TestPictureObjects[randomInteger].tag = "trueAnswer";

                LoadRandomColorPictureToOtherObject(1);
                PictureCounter++;

                break;
            case 1:
                questionTextObject.GetComponent<Text>().text = "Hangisi " + fruits[PictureCounter] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = FruitSprites[PictureCounter];
                TestPictureObjects[randomInteger].tag = "trueAnswer";

                LoadRandomColorPictureToOtherObject(0);
                PictureCounter++;

                break;
            default:
                Debug.Log("Unexpected random integer.");
                break;
        }
    }



    private void LoadRandomColorPictureToOtherObject(int TestObjectNumber)
    {
        var randomInteger = UnityEngine.Random.Range(0, FruitSprites.Length);

        while (randomInteger == PictureCounter)
        {
            randomInteger = UnityEngine.Random.Range(0, FruitSprites.Length);
        }

        TestPictureObjects[TestObjectNumber].GetComponent<Image>().sprite = FruitSprites[randomInteger];
        TestPictureObjects[TestObjectNumber].tag = "falseAnswer";

    }


    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainScene");
    }


    public void SendDataToDB()
    {
        for (var i = 0; i < FailCounter.Length; i++)
        {
            print(i + " " + FailCounter[i] );
        }
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

        string sqlQuery = "INSERT INTO Test (TestType,StuNo,q1,q2,q3,q4,q5) values ('" + TestName + "'," + PlayerPrefs.GetInt("StuNumber") + "," + FailCounter[0] + "," + FailCounter[1] + "," + FailCounter[2] + "," + FailCounter[3] + "," + FailCounter[4] + ")";

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
