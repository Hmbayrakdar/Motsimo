using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;

public class VehicheSceneScript : MonoBehaviour {

    #region Variables

    public GameObject questionTextObject, ShowPictureObject, restartObject, testStartObject, goBackObject,Rakun,informationText,StarPanel ;
    public GameObject[] TestPictureObjects,Stars;
    public Sprite[] VehicheSprites;
    public Sprite[] VehicheSpritesK;
    public Sprite[] VehicheSpritesD;
    public Sprite[] VehicheSpritesH;
    public AudioSource ApplauseAudioSource;
    
    private AudioClip[] IdentificationAudioClips, QuestionAudioClips, QuestionAudioClips2, congratsAudioClips;
    private AudioSource AudioSource;
    private bool noAudioPlaying = true;
    private bool isFirstTestFinished = false;
    private bool isSecondTestFinished = false;

    private GameObject RacoonHelpObject;
    private int PictureCounter;
    private int counterP = 0;
    private int[] FailCounter = new int[5];
    private string TestName = "taşıtlar";
    private string[] Vehiches = { "Tır", "Uçak", "Otobüs", "Araba", "Gemi" };
    private string[] Vehiches2 = { "Karada","Karada","Karada","Denizde","Havada"};
    private string[] CVehiches = { "TIR", "UÇAK", "OTOBÜS", "ARABA", "GEMİ" };
    private string conn;
    private Coroutine co;
    private float[] AnswerTimes = new float[5];
    private float passedTime;
    #endregion

    #region Unity Callbacks
    // Use this for initialization
    void Start () {

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
        
        IdentificationAudioClips =  new AudioClip[]{(AudioClip)Resources.Load("Sound/Vehicles/Identify/Tır Kara"),
            (AudioClip)Resources.Load("Sound/Vehicles/Identify/Uçak Hava"), 
            (AudioClip)Resources.Load("Sound/Vehicles/Identify/Otobüs Kara"), 
            (AudioClip)Resources.Load("Sound/Vehicles/Identify/Araba Kara"),
            (AudioClip)Resources.Load("Sound/Vehicles/Identify/Gemi Deniz")
        };
        
        QuestionAudioClips2 = new AudioClip[]{(AudioClip)Resources.Load("Sound/Vehicles/SecondTestQuestions/Hangisi karada gider"),
            (AudioClip)Resources.Load("Sound/Vehicles/SecondTestQuestions/Hangisi karada gider"),
            (AudioClip)Resources.Load("Sound/Vehicles/SecondTestQuestions/Hangisi karada gider"),
            (AudioClip)Resources.Load("Sound/Vehicles/SecondTestQuestions/Hangisi denizde gider"),
            (AudioClip)Resources.Load("Sound/Vehicles/SecondTestQuestions/Hangisi havada gider"),
        };
        QuestionAudioClips =  new AudioClip[]{(AudioClip)Resources.Load("Sound/Vehicles/Question/Hangisi tır göster"),
            (AudioClip)Resources.Load("Sound/Vehicles/Question/Hangisi uçak göster"), 
            (AudioClip)Resources.Load("Sound/Vehicles/Question/Hangisi otobüs göster"), 
            (AudioClip)Resources.Load("Sound/Vehicles/Question/Hangisi araba göster"),
            (AudioClip)Resources.Load("Sound/Vehicles/Question/Hangisi gemi göster")
        };
        
        congratsAudioClips = new AudioClip[]{(AudioClip)Resources.Load("Sound/Congrats/Böyle devam"),
            (AudioClip)Resources.Load("Sound/Congrats/Harika"), 
            (AudioClip)Resources.Load("Sound/Congrats/Mükemmel"), 
            (AudioClip)Resources.Load("Sound/Congrats/Süper"),
            (AudioClip)Resources.Load("Sound/Congrats/Tebrikler")
        };

        
        ApplauseAudioSource.clip = (AudioClip) Resources.Load("Sound/applause");
        
        showVehicheImage();
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
        
        showVehicheImage();
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
        
        if (PictureCounter < VehicheSprites.Length)
        {
            gameObject.GetComponent<StarAnimationScript>().StarFunction();
        }
        yield return new WaitUntil(() => gameObject.GetComponent<StarAnimationScript>().getAPanelFinished() == true);

        AudioSource.clip = congratsAudioClips[UnityEngine.Random.Range(0,5)];
        AudioSource.Play();
        ApplauseAudioSource.Play();
        yield return new WaitForSeconds(AudioSource.clip.length);
        gameObject.GetComponent<StarAnimationScript>().deactivateAPanel();
        
        if (isFirstTestFinished && !isSecondTestFinished && PictureCounter < VehicheSprites.Length)
        {
            testVehiche2(i);
            noAudioPlaying = true;
            
            foreach ( GameObject t in TestPictureObjects)
                t.GetComponent<Image>().color  = new Color32(255,255,225,255);
            
            yield break;
        }
        
        if (PictureCounter >= VehicheSprites.Length)
        {
            if (!isFirstTestFinished && !isSecondTestFinished)
            {
                isFirstTestFinished = true;
                PictureCounter = 0;
               
                
                gameObject.GetComponent<StarAnimationScript>().StarFunction();
                
                
                yield return new WaitUntil(() => gameObject.GetComponent<StarAnimationScript>().getAPanelFinished() == true);
                AudioSource.clip = congratsAudioClips[UnityEngine.Random.Range(0,5)];
                AudioSource.Play();
                ApplauseAudioSource.Play();
                yield return new WaitForSeconds(AudioSource.clip.length);
                gameObject.GetComponent<StarAnimationScript>().deactivateAPanel();
                
                testVehiche2(-1);
                noAudioPlaying = true;
                yield break;
            }

            gameObject.GetComponent<StarAnimationScript>().StarEndAnimation.GetComponent<AudioSource>().clip =
                (AudioClip) Resources.Load("Sound/applause");
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
        
        testVehiche(i);
        
        noAudioPlaying = true;
    }
    #endregion

    #region Function
    public void RestartScene()
    {
        SceneManager.LoadScene("VehicheScene");
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

    public void showVehicheImage()
    {
        if (PictureCounter < VehicheSprites.Length)
        {
            ShowPictureObject.GetComponent<Image>().overrideSprite = VehicheSprites[PictureCounter];
            informationText.GetComponent<Text>().text = "Bunun adı \n" + CVehiches[PictureCounter] + "";
            PictureCounter++;
        }
        else
        {
            if (StarAnimationScript.counp < 4)
                testStartObject.SetActive(false);
            else
                testStartObject.SetActive(true);
            
            ShowPictureObject.SetActive(false);
            Rakun.SetActive(false);
            informationText.SetActive(false);
            
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

            isFirstTestFinished = false;
            isSecondTestFinished = false;
            StarPanel.SetActive(false);
            for(int i=0;i<StarAnimationScript.counp;i++)
                Stars[i].SetActive(false);

            PictureCounter = 0;
            testVehiche(-1);
        
      
    }


    public void testVehiche(int i)
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
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + Vehiches[PictureCounter] + " Göster";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = VehicheSprites[PictureCounter];
                    TestPictureObjects[randomInteger].tag = "trueAnswer";

                    LoadRandomColorPictureToOtherObject(1);
                    PictureCounter++;
                    break;
                case 1:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + Vehiches[PictureCounter] + " Göster";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = VehicheSprites[PictureCounter];
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
                questionTextObject.GetComponent<Text>().text = "Hangisi " + Vehiches[PictureCounter] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = VehicheSprites[PictureCounter];
                TestPictureObjects[randomInteger].tag = "trueAnswer";

                LoadRandomColorPictureToOtherObject(1);
                PictureCounter++;
                break;
            case 1:
                questionTextObject.GetComponent<Text>().text = "Hangisi " + Vehiches[PictureCounter] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = VehicheSprites[PictureCounter];
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
        var randomInteger = UnityEngine.Random.Range(0, VehicheSprites.Length);

        while (randomInteger == PictureCounter)
        {
            randomInteger = UnityEngine.Random.Range(0, VehicheSprites.Length);
        }

        TestPictureObjects[testObjectNumber].GetComponent<Image>().sprite = VehicheSprites[randomInteger];
        TestPictureObjects[testObjectNumber].tag = "falseAnswer";

        co = StartCoroutine(testObjectNumber==0 ? StartRacoonHelpCounter(1) : StartRacoonHelpCounter(0));
    }
    
    public void testVehiche2(int i)
    {
        Debug.Log(counterP);
        var randomInteger = UnityEngine.Random.Range(0, 2);
        passedTime = Time.time;

        if (i == -1)
        {
            AudioSource.clip = QuestionAudioClips2[PictureCounter];
            AudioSource.Play();
            switch (randomInteger)
            {
                case 0:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + Vehiches2[0] + " Gider";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = VehicheSpritesK[0];
                    TestPictureObjects[randomInteger].tag = "trueAnswer";
                    LoadRandomColorPictureToOtherObject2(1, 0);
                    PictureCounter++;
                    counterP++;
                    break;
                case 1:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + Vehiches2[0] + " Gider";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = VehicheSpritesK[0];
                    TestPictureObjects[randomInteger].tag = "trueAnswer";
                    LoadRandomColorPictureToOtherObject2(0, 0);
                    PictureCounter++;
                    counterP++;
                    break;
                default:
                    Debug.Log("Unexpected random integer.");
                    break;
            }

            return;
        }

        AudioSource.clip = QuestionAudioClips2[PictureCounter];
        AudioSource.Play();
        
        if (counterP <= 2)
        {
            switch (randomInteger)
            {
                case 0:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + Vehiches2[PictureCounter] + " Gider";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = VehicheSpritesK[counterP];
                    TestPictureObjects[randomInteger].tag = "trueAnswer";
                    LoadRandomColorPictureToOtherObject2(1, 0);

                    PictureCounter++;
                    counterP++;
                    break;
                case 1:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + Vehiches2[PictureCounter] + " Gider";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = VehicheSpritesK[counterP];
                    TestPictureObjects[randomInteger].tag = "trueAnswer";
                    LoadRandomColorPictureToOtherObject2(0, 0);
                    PictureCounter++;
                    counterP++;
                    break;
                default:
                    Debug.Log("Unexpected random integer.");
                    break;
            }
        }
        else if (counterP == 3)
        {
            switch (randomInteger)
            {
                case 0:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + Vehiches2[PictureCounter] + " Gider";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = VehicheSpritesD[0];
                    TestPictureObjects[randomInteger].tag = "trueAnswer";
                    LoadRandomColorPictureToOtherObject2(1, 1);

                    PictureCounter++;
                    counterP++;
                    break;
                case 1:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + Vehiches2[PictureCounter] + " Gider";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = VehicheSpritesD[0];
                    TestPictureObjects[randomInteger].tag = "trueAnswer";
                    LoadRandomColorPictureToOtherObject2(0, 1);
                    PictureCounter++;
                    counterP++;
                    break;
                default:
                    Debug.Log("Unexpected random integer.");
                    break;
            }
        }
        else if (counterP == 4)
        {
            switch (randomInteger)
            {
                case 0:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + Vehiches2[PictureCounter] + " Gider";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = VehicheSpritesH[0];
                    TestPictureObjects[randomInteger].tag = "trueAnswer";
                    LoadRandomColorPictureToOtherObject2(1, 2);
                    PictureCounter++;
                    counterP++;
                    break;
                case 1:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + Vehiches2[PictureCounter] + " Gider";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = VehicheSpritesH[0];
                    TestPictureObjects[randomInteger].tag = "trueAnswer";
                    LoadRandomColorPictureToOtherObject2(0, 2);
                    PictureCounter++;
                    counterP++;
                    break;
                default:
                    Debug.Log("Unexpected random integer.");
                    break;
            }
        }

    }
    
    private void LoadRandomColorPictureToOtherObject2(int TestObjectNumber,int TestSelect)
    {
        if (TestSelect == 0)
        {
            var randomInt = UnityEngine.Random.Range(0,2);

            switch (randomInt)
            {
                case 0:
                    TestPictureObjects[TestObjectNumber].GetComponent<Image>().sprite = VehicheSpritesH[0];
                    TestPictureObjects[TestObjectNumber].tag = "falseAnswer";
                    break;
                case 1:
                    TestPictureObjects[TestObjectNumber].GetComponent<Image>().sprite = VehicheSpritesD[0];
                    TestPictureObjects[TestObjectNumber].tag = "falseAnswer";
                    break;
                default:
                    Debug.Log("Unexpected random integer.");
                    break;
            }
        }
        if (TestSelect == 1)
        {
            var randomInt = UnityEngine.Random.Range(0, 2);
            var randomInteger = UnityEngine.Random.Range(0,3);

            switch (randomInt)
            {
                case 0:
                    TestPictureObjects[TestObjectNumber].GetComponent<Image>().sprite = VehicheSpritesK[randomInteger];
                    TestPictureObjects[TestObjectNumber].tag = "falseAnswer";
                    break;
                case 1:
                    TestPictureObjects[TestObjectNumber].GetComponent<Image>().sprite = VehicheSpritesH[0];
                    TestPictureObjects[TestObjectNumber].tag = "falseAnswer";
                    break;
                default:
                    Debug.Log("Unexpected random integer.");
                    break;
            }
        }
        if (TestSelect == 2)
        {
            var randomInt = UnityEngine.Random.Range(0, 2);
            var randomInteger = UnityEngine.Random.Range(0,3);

            switch (randomInt)
            {
                case 0:
                    TestPictureObjects[TestObjectNumber].GetComponent<Image>().sprite = VehicheSpritesK[randomInteger];
                    TestPictureObjects[TestObjectNumber].tag = "falseAnswer";
                    break;
                case 1:
                    TestPictureObjects[TestObjectNumber].GetComponent<Image>().sprite = VehicheSpritesD[0];
                    TestPictureObjects[TestObjectNumber].tag = "falseAnswer";
                    break;
                default:
                    Debug.Log("Unexpected random integer.");
                    break;
            }
        } 
        co = StartCoroutine(TestObjectNumber==0 ? StartRacoonHelpCounter(1) : StartRacoonHelpCounter(0));
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
        dbconn = (IDbConnection)new SqliteConnection("URI=file:" + conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery;

        if (isFirstTestFinished && isSecondTestFinished)
        {
            sqlQuery = "INSERT INTO Test (TestType,StuNo,q1,q2,q3,q4,q5) values ('Araç Ortamı'," + PlayerPrefs.GetInt("StuNumber") + "," + FailCounter[0] + "," + FailCounter[1] + "," + FailCounter[2] + "," + FailCounter[3] + "," + FailCounter[4] + ")";
        }
        else
            sqlQuery = "INSERT INTO Test (TestType,StuNo,q1,q2,q3,q4,q5) values ('" + TestName + "'," + PlayerPrefs.GetInt("StuNumber") + "," + FailCounter[0] + "," + FailCounter[1] + "," + FailCounter[2] + "," + FailCounter[3] + "," + FailCounter[4] + ")";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        
        IDbCommand dbcmd2 = dbconn.CreateCommand();
        if (isFirstTestFinished && isSecondTestFinished)
        {
            sqlQuery = "INSERT INTO TestTimes (TestType,StuNo,q1,q2,q3,q4,q5) values ('taşıt türü',"+PlayerPrefs.GetInt("StuNumber")+","+AnswerTimes[0]+","+AnswerTimes[1]+","+AnswerTimes[2]+","+AnswerTimes[3]+","+AnswerTimes[4]+")";
        }
        else 
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


    public void GoToMainMenu()
    {
        StarAnimationScript.counp = 0;
        StarPanel.SetActive(false);
        SceneManager.LoadScene("MainScene");
    }
    public void AddStar()
    {
        for(int i=0;i<StarAnimationScript.counp;i++)
            Stars[i].SetActive(true);
    }
    
    #endregion

}