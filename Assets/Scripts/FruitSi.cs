using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;

public class FruitSi : MonoBehaviour
{
    #region Variables
    public GameObject questionTextObject, ShowPictureObject, restartObject, testStartObject, goBackObject,Racoon, RacoonText;
    public GameObject[] TestPictureObjects;
    public Sprite[] FruitSprites;
    public AudioSource ApplauseAudioSource;
    
    private AudioClip[] IdentificationAudioClips, QuestionAudioClips, congratsAudioClips;
    private AudioSource AudioSource;
    private bool noAudioPlaying = true;

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

        for (var i = 0; i < FailCounter.Length; i++)
        {
            FailCounter[i] = 0;
        }
        
        AudioSource = gameObject.GetComponent<AudioSource>();
        
        IdentificationAudioClips =  new AudioClip[]{(AudioClip)Resources.Load("Sound/Fruits/Identify/Muz"),
            (AudioClip)Resources.Load("Sound/Fruits/Identify/Çilek"), 
            (AudioClip)Resources.Load("Sound/Fruits/Identify/Armut"), 
            (AudioClip)Resources.Load("Sound/Fruits/Identify/Elma"),
            (AudioClip)Resources.Load("Sound/Fruits/Identify/Kiraz")
        };
        
        QuestionAudioClips =  new AudioClip[]{(AudioClip)Resources.Load("Sound/Fruits/Question/Hangisi muz göster"),
            (AudioClip)Resources.Load("Sound/Fruits/Question/Hangisi çilek göster"), 
            (AudioClip)Resources.Load("Sound/Fruits/Question/Hangisi armut göster"), 
            (AudioClip)Resources.Load("Sound/Fruits/Question/Hangisi elma göster"),
            (AudioClip)Resources.Load("Sound/Fruits/Question/Hangisi kiraz göster")
        };
        
        congratsAudioClips = new AudioClip[]{(AudioClip)Resources.Load("Sound/Congrats/Böyle devam"),
            (AudioClip)Resources.Load("Sound/Congrats/Harika"), 
            (AudioClip)Resources.Load("Sound/Congrats/Mükemmel"), 
            (AudioClip)Resources.Load("Sound/Congrats/Süper"),
            (AudioClip)Resources.Load("Sound/Congrats/Tebrikler")
        };

        ApplauseAudioSource.clip = (AudioClip) Resources.Load("Sound/applause");

        showFruitsImage();
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
        if (AudioSource.isPlaying)
            yield break;
        
        if (!TestPictureObjects[i].CompareTag("trueAnswer"))
        {
            int number = PictureCounter - 1;
            FailCounter[number]++;
            TestPictureObjects[i].GetComponent<Image>().color  = new Color32(255,255,225,100);
            yield break;
        }
        
        noAudioPlaying = false;

        if (PictureCounter < FruitSprites.Length)
        {
            gameObject.GetComponent<StarAnimationScript>().StarFunction();
        }
        yield return new WaitUntil(() => gameObject.GetComponent<StarAnimationScript>().getAPanelFinished() == true);
        
        AudioSource.clip = congratsAudioClips[UnityEngine.Random.Range(0,5)];
        AudioSource.Play();
        ApplauseAudioSource.Play();
        yield return new WaitForSeconds(AudioSource.clip.length);
        gameObject.GetComponent<StarAnimationScript>().deactivateAPanel();
        
        
        if (PictureCounter >= FruitSprites.Length)
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
        foreach ( GameObject t in TestPictureObjects)
            t.GetComponent<Image>().color  = new Color32(255,255,225,255);
        
        testFruits(i);
        noAudioPlaying = true;
    }
    
    #endregion
    
    #region Function

    public void RestartScene()
    {
        SceneManager.LoadScene("FruitsScene");
    }
    
    public void PlaySound()
    {
        if(noAudioPlaying)
            StartCoroutine(IdentifySound());
    }
    
    public void PlayCongrats(int i)
    {
        if(noAudioPlaying)
            StartCoroutine(CongratsSound(i));
    }
    
    

    public void showFruitsImage()
    {
        if (PictureCounter < FruitSprites.Length)
        {
            RacoonText.GetComponent<Text>().text = "Bu bir " + fruits[PictureCounter];
            ShowPictureObject.GetComponent<Image>().overrideSprite = FruitSprites[PictureCounter];
            PictureCounter++;
        }
        else
        {
            ShowPictureObject.SetActive(false);
            Racoon.SetActive(false);

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