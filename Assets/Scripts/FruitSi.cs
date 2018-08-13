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
    public GameObject questionTextObject, ShowPictureObject, restartObject, testStartObject, goBackObject,Racoon, RacoonText,StarPanel;
    public GameObject[] TestPictureObjects,Stars;
    public Sprite[] FruitSprites;
    public AudioSource ApplauseAudioSource;
    
    private AudioClip[] IdentificationAudioClips, QuestionAudioClips, congratsAudioClips;
    private AudioSource AudioSource;
    private bool noAudioPlaying = true;
    private GameObject RacoonHelpObject;
    private int PictureCounter;
    private int[] FailCounter = new int[5];
    private string TestName = "meyveler";
    private string[] fruits = { "Muz", "Çilek", "Armut", "Elma", "Kiraz" };
    private string conn;
    private Coroutine co;
    private float[] AnswerTimes = new float[5];
    private float passedTime;

    #endregion
    // Use this for initialization
    #region Unity Callbacks
    void Start()
    { 
        RacoonHelpObject = (GameObject)Instantiate(Resources.Load("RacoonHelp"));
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
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StarAnimationScript.counp = 0;
            SceneManager.LoadScene("MainScene");
        }
    }
    
    IEnumerator StartRacoonHelpCounter(int i)
    {
        yield return new WaitForSeconds(6f);
        RacoonHelpObject.GetComponent<RectTransform>().SetParent(TestPictureObjects[i].GetComponent<RectTransform>());
        RacoonHelpObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100f);
        RacoonHelpObject.gameObject.SetActive(true);
        RacoonHelpObject.GetComponent<RectTransform>().localScale = new Vector3(1.0f,1.0f,0f);
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
            FailCounter[PictureCounter-1]++;
            TestPictureObjects[i].GetComponent<Image>().color  = new Color32(255,255,225,100);
            
            var tempNumber = 0;
            if (i == 0)
                tempNumber = 1;
            StopCoroutine(co);
            RacoonHelpObject.GetComponent<RectTransform>().SetParent(TestPictureObjects[tempNumber].GetComponent<RectTransform>());
            RacoonHelpObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100f);
            RacoonHelpObject.gameObject.SetActive(true);
            RacoonHelpObject.GetComponent<RectTransform>().localScale = new Vector3(1.0f,1.0f,0f);
            
            yield break;
        }
        passedTime = Time.time - passedTime;
        AnswerTimes[PictureCounter-1] = passedTime;
        
        StopCoroutine(co);
        RacoonHelpObject.SetActive(false);
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
            AddStar();
            StarPanel.SetActive(true);
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
            if (StarAnimationScript.counp < 4)
                testStartObject.SetActive(false);
            else
                testStartObject.SetActive(true);
            
            ShowPictureObject.SetActive(false);
            Racoon.SetActive(false);
            StarPanel.SetActive(true);
            if (StarAnimationScript.counp < 5)
                StarAnimationScript.counp++;
            AddStar();
            restartObject.SetActive(true);
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

            StarPanel.SetActive(false);
            for (int i = 0; i < StarAnimationScript.counp; i++)
                Stars[i].SetActive(false);
            
            PictureCounter = 0;
            testFruits(-1);
    }


    public void testFruits(int i)
    {
        var randomInteger = UnityEngine.Random.Range(0, 2);
        passedTime = Time.time;

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



    private void LoadRandomColorPictureToOtherObject(int testObjectNumber)
    {
        var randomInteger = UnityEngine.Random.Range(0, FruitSprites.Length);

        while (randomInteger == PictureCounter)
        {
            randomInteger = UnityEngine.Random.Range(0, FruitSprites.Length);
        }

        TestPictureObjects[testObjectNumber].GetComponent<Image>().sprite = FruitSprites[randomInteger];
        TestPictureObjects[testObjectNumber].tag = "falseAnswer";

        co = StartCoroutine(testObjectNumber==0 ? StartRacoonHelpCounter(1) : StartRacoonHelpCounter(0));
    }


    public void GoToMainMenu()
    {
        StarAnimationScript.counp = 0;
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
        
        IDbCommand dbcmd2 = dbconn.CreateCommand();
        sqlQuery = "INSERT INTO TestTimes (TestType,StuNo,q1,q2,q3,q4,q5) values ('"+TestName+"',"+PlayerPrefs.GetInt("StuNumber")+","+AnswerTimes[0]+","+AnswerTimes[1]+","+AnswerTimes[2]+","+AnswerTimes[3]+","+AnswerTimes[4]+")";
        dbcmd2.CommandText = sqlQuery;
        reader = dbcmd2.ExecuteReader();
        
        dbcmd2.Dispose();
        dbcmd2 = null;
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }
    
    public void AddStar()
    {
        for (int i = 0; i < StarAnimationScript.counp; i++)
            Stars[i].SetActive(true);
    }

    #endregion
}