using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite; using System.Data;
using System.IO;

public class SizeDifferenceScript : MonoBehaviour
{

    #region Variables

    public GameObject questionTextObject, StarPanel;
    public GameObject[] ShowPictureObjects, Stars;
    public Sprite[] SizeDifferenceSprites;
    public GameObject restartObject, testStartObject, goBackObject;
    public GameObject[] TestPictureObjects;
    public AudioSource ApplauseAudioSource;

    public AudioClip[] IdentificationAudioClips, QuestionAudioClips, congratsAudioClips;
    private AudioSource AudioSource;
    private bool noAudioPlaying = true;
    private GameObject RacoonHelpObject;
    private int PictureCounter;
    private int[] FailCounter = new int[5];
    private string TestName = "büyük-küçük";
    private bool SmallPictureFlag, BigPictureFlag, isTesting;
    private string conn;
    private Coroutine co;
    private float[] AnswerTimes = new float[5];
    private float passedTime;

    #endregion

    #region Unity Callbacks

    // Use this for initialization
    void Start()
    {
        RacoonHelpObject = (GameObject) Instantiate(Resources.Load("RacoonHelp"));
        PictureCounter = 0;

        SmallPictureFlag = true;
        BigPictureFlag = true;
        isTesting = false;

        TestPictureObjects[0].tag = "trueAnswer";
        TestPictureObjects[1].tag = "trueAnswer";

        for (int i = 0; i < FailCounter.Length; i++)
        {
            FailCounter[i] = 0;
        }

        AudioSource = gameObject.GetComponent<AudioSource>();

        congratsAudioClips = Resources.LoadAll<AudioClip>("Sound/Congrats");
        ApplauseAudioSource.clip = (AudioClip) Resources.Load("Sound/applause");

        ShowPictures("small");
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
        RacoonHelpObject.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 0f);
    }
    
    IEnumerator WaitUntilQuestion()
    {
        yield return new WaitForSeconds(AudioSource.clip.length);
        passedTime = Time.time;
    }


    IEnumerator IdentifySound(string key)
    {
        noAudioPlaying = false;

        switch (key)
        {
            case "small":
                SmallPictureFlag = true;
                AudioSource.clip = IdentificationAudioClips[1];
                AudioSource.Play();
                break;
            case "big":
                BigPictureFlag = true;
                AudioSource.clip = IdentificationAudioClips[0];
                AudioSource.Play();
                break;
            case "test":
                isTesting = true;
                ShowPictures(key);
                noAudioPlaying = true;
                yield break;
            default:
                Debug.Log("Unexpected function parameter.");
                break;
        }


        yield return new WaitForSeconds(AudioSource.clip.length);

        ShowPictures(key);
        noAudioPlaying = true;
    }

    IEnumerator CongratsSound(int i)
    {
        if (!TestPictureObjects[i].CompareTag("trueAnswer"))
        {
            FailCounter[PictureCounter - 1]++;
            TestPictureObjects[i].GetComponent<Image>().color = new Color32(255, 255, 225, 100);

            var tempNumber = 0;
            if (i == 0)
                tempNumber = 1;
            StopCoroutine(co);
            RacoonHelpObject.GetComponent<RectTransform>()
                .SetParent(TestPictureObjects[tempNumber].GetComponent<RectTransform>());
            RacoonHelpObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100f);
            RacoonHelpObject.gameObject.SetActive(true);
            RacoonHelpObject.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 0f);

            yield break;
        }

        passedTime = Time.time - passedTime;
        AnswerTimes[PictureCounter - 1] = passedTime;

        StopCoroutine(co);
        RacoonHelpObject.SetActive(false);
        noAudioPlaying = false;

        if (PictureCounter < SizeDifferenceSprites.Length)
            gameObject.GetComponent<StarAnimationScript>().StarFunction();

        yield return new WaitUntil(() => gameObject.GetComponent<StarAnimationScript>().getAPanelFinished());

        AudioSource.clip = congratsAudioClips[UnityEngine.Random.Range(0, 5)];
        AudioSource.Play();
        ApplauseAudioSource.Play();
        yield return new WaitForSeconds(AudioSource.clip.length);

        gameObject.GetComponent<StarAnimationScript>().deactivateAPanel();

        if (PictureCounter >= SizeDifferenceSprites.Length)
        {
            isTesting = false;
            gameObject.GetComponent<StarAnimationScript>().StartAnimation();
            TestPictureObjects[0].SetActive(false);
            TestPictureObjects[1].SetActive(false);
            questionTextObject.SetActive(false);

            SendDataToDB();
            PictureCounter = 0;

            StopAllCoroutines();
            AddStar();
            StarPanel.SetActive(true);
            restartObject.SetActive(true);
            testStartObject.SetActive(true);
            goBackObject.SetActive(true);
            noAudioPlaying = true;
            yield break;
        }

        foreach (GameObject t in TestPictureObjects)
            t.GetComponent<Image>().color = new Color32(255, 255, 225, 255);

        DifferenceTest();
        noAudioPlaying = true;
    }

    #endregion

    #region Functions

    public void RestartScene()
    {
        SceneManager.LoadScene("SizeDifference");
    }




    public void PlaySound(String key)
    {
        if (noAudioPlaying)
            StartCoroutine(IdentifySound(key));
    }

    public void PlayCongrats(int i)
    {
        if (noAudioPlaying && !AudioSource.isPlaying)
            StartCoroutine(CongratsSound(i));
    }

    public void ShowPictures(String key)
    {
        if (isTesting == true)
        {
           
                TestPictureObjects[0].SetActive(true);
                TestPictureObjects[1].SetActive(true);
                questionTextObject.SetActive(true);

                goBackObject.SetActive(false);
                restartObject.SetActive(false);
                testStartObject.SetActive(false);
                StarPanel.SetActive(false);
                for (int i = 0; i < StarAnimationScript.counp; i++)
                    Stars[i].SetActive(false);
                TestPictureObjects[0].tag = "trueAnswer";
                TestPictureObjects[1].tag = "trueAnswer";
                PictureCounter = 0;

                DifferenceTest();
                return;
            
        }

        if (SmallPictureFlag != true || BigPictureFlag != true) return;

            if (PictureCounter >= SizeDifferenceSprites.Length)
            {
                if (StarAnimationScript.counp < 4)
                    testStartObject.SetActive(false);
                else
                    testStartObject.SetActive(true);
                
                ShowPictureObjects[0].SetActive(false);
                ShowPictureObjects[1].SetActive(false);

                StarPanel.SetActive(true);
                if (StarAnimationScript.counp < 5)
                    StarAnimationScript.counp++;
                AddStar();
                restartObject.SetActive(true);
                goBackObject.SetActive(true);

                PictureCounter = 0;
            }

            ShowPictureObjects[0].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];
            ShowPictureObjects[1].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];
            PictureCounter++;
            SmallPictureFlag = BigPictureFlag = false;
    }

    public void DifferenceTest()
    {
        var randomInteger = UnityEngine.Random.Range(0, 2);
        

        switch (randomInteger)
        {
            case 0:
                TestPictureObjects[0].GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
                TestPictureObjects[0].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];

                TestPictureObjects[1].GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                TestPictureObjects[1].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];

                DecideQuestion(0, 1);
                break;
            case 1:
                TestPictureObjects[0].GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                TestPictureObjects[0].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];

                TestPictureObjects[1].GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
                TestPictureObjects[1].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];

                DecideQuestion(1, 0);
                break;
            default:
                Debug.Log("Unexpected random integer.");
                break;
        }


    }

    //Decide Question takes the objects, then decides whether the question will be small or big
    private void DecideQuestion(int bigPicture, int smallPicture)
    {
        var randomInteger = UnityEngine.Random.Range(0, 2);

        switch (randomInteger)
        {
            case 0: //The question will ask the bigger picture

                AudioSource.clip = QuestionAudioClips[0];
                AudioSource.Play();
                StartCoroutine(WaitUntilQuestion());

                questionTextObject.GetComponent<Text>().text = "Hangisi büyük göster.";

                TestPictureObjects[bigPicture].tag = "trueAnswer";
                co = StartCoroutine(StartRacoonHelpCounter(bigPicture));
                TestPictureObjects[smallPicture].tag = "falseAnswer";
                break;
            case 1: //The question will ask the smaller picture

                AudioSource.clip = QuestionAudioClips[1];
                AudioSource.Play();
                StartCoroutine(WaitUntilQuestion());

                questionTextObject.GetComponent<Text>().text = "Hangisi küçük göster.";

                TestPictureObjects[bigPicture].tag = "falseAnswer";
                TestPictureObjects[smallPicture].tag = "trueAnswer";
                co = StartCoroutine(StartRacoonHelpCounter(smallPicture));
                break;
            default:
                Debug.Log("Unexpected random.");
                break;
        }

        PictureCounter++;
    }

    public void GoToConceptsMenu()
    {
        StarAnimationScript.counp = 0;
        SceneManager.LoadScene("MainScene");
    }

    public void SendDataToDB()
    {
        

        IDbConnection dbconn = connectToDB();
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "INSERT INTO Test (TestType,StuNo,q1,q2,q3,q4,q5) values ('" + TestName + "'," +
                          PlayerPrefs.GetInt("StuNumber") + "," + FailCounter[0] + "," + FailCounter[1] + "," +
                          FailCounter[2] + "," + FailCounter[3] + "," + FailCounter[4] + ")";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        IDbCommand dbcmd2 = dbconn.CreateCommand();
        sqlQuery = "INSERT INTO TestTimes (TestType,StuNo,q1,q2,q3,q4,q5) values ('" + TestName + "'," +
                   PlayerPrefs.GetInt("StuNumber") + "," + AnswerTimes[0] + "," + AnswerTimes[1] + "," +
                   AnswerTimes[2] + "," + AnswerTimes[3] + "," + AnswerTimes[4] + ")";
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
    
    private IDbConnection connectToDB()
    {
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
            conn =Application.dataPath + "/StreamingAssets/Database.db";
        }
        return (IDbConnection)new SqliteConnection("URI=file:" + conn);
    }

    public void AddStar()
    {
        for (int i = 0; i < StarAnimationScript.counp; i++)
            Stars[i].SetActive(true);
    }

    #endregion

}