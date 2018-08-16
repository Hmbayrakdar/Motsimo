using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite; using System.Data; using System.IO;
using System.Linq;

public class ColorsScript : MonoBehaviour {

    #region Variables
	
    public GameObject questionTextObject,ColorsMenuObjects, ShowPictureObject, restartObject, testStartObject, ReloadSceneObject, Racoon, RacoonText,StarPanel ;
    public GameObject[] TestPictureObjects,Stars;
    public AudioSource ApplauseAudioSource;
    
    public AudioClip[] IdentificationAudioClips, QuestionAudioClips, congratsAudioClips;
    private AudioSource AudioSource;
    private bool noAudioPlaying = true;
    private GameObject RacoonHelpObject;
    private Sprite[][] ColorSprites = new Sprite[3][];
    private int[] mixTestColorCounter = new int[3];
    private int[] FailCounter = new int[5];
    List<int[]> mixTestFailCounter = new List<int[]>(); 
    List<float[]> mixTestAnswerTimes = new List<float[]>(); 
    private string[] TestNames = {"kırmızı", "mavi", "sarı"};
    private string[] RacoonTextInput = {"kırmızı", "mavi", "sarı"};
    private string conn;
    private int ChosenColor;
    private Coroutine co;
    private float[] AnswerTimes = new float[5];
    private float passedTime;
    private bool isMixTestFinished = false , isMixTestActive = false;
	
    #endregion
	
    #region Unity Callbacks
	
    // Use this for initialization
    void Start ()
    {
        RacoonHelpObject = (GameObject)Instantiate(Resources.Load("RacoonHelp"));
        foreach (var t in TestPictureObjects)
        {
            t.tag = "trueAnswer";
        }

        for (int i = 0; i < FailCounter.Length; i++)
        {
            FailCounter[i] = 0;
        }
        
        for (int i = 0; i < 3; i++)
        {
            mixTestColorCounter[i] = 0; 
        }
        
        for (int i = 0; i < 3; i++)
        {
            mixTestFailCounter.Add(new int[5]);
            mixTestAnswerTimes.Add(new float[5]);
            for (int j = 0; j < 5; j++)
            {
                mixTestFailCounter[i][j] = 0;
                mixTestAnswerTimes[i][j] = 0;
            }
        }

        AudioSource = gameObject.GetComponent<AudioSource>();
        
        ColorSprites[0] = Resources.LoadAll<Sprite>("Pictures/Kırmızı");
        ColorSprites[1] = Resources.LoadAll<Sprite>("Pictures/Mavi");
        ColorSprites[2] = Resources.LoadAll<Sprite>("Pictures/Sarı");

        congratsAudioClips = Resources.LoadAll<AudioClip>("Sound/Congrats");
        ApplauseAudioSource.clip = (AudioClip) Resources.Load("Sound/applause");
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

    IEnumerator WaitUntilQuestion()
    {
        yield return new WaitForSeconds(AudioSource.clip.length);
        passedTime = Time.time;
    }

    IEnumerator IdentifySound()
    {
        noAudioPlaying = false;
        
        AudioSource.clip = IdentificationAudioClips[ChosenColor];
        AudioSource.Play();
        yield return new WaitForSeconds(AudioSource.clip.length);
        
        showAnimalImage();
        noAudioPlaying = true;
    }
    
    IEnumerator CongratsSound(int i)
    {
        if (!TestPictureObjects[i].CompareTag("trueAnswer")) {
            FailCounter[mixTestColorCounter[ChosenColor]-1]++;
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
        AnswerTimes[mixTestColorCounter[ChosenColor]-1] = passedTime;
        
        StopCoroutine(co);
        RacoonHelpObject.SetActive(false);
        noAudioPlaying = false;
        
        if (mixTestColorCounter[ChosenColor] < ColorSprites[ChosenColor].Length)
            gameObject.GetComponent<StarAnimationScript>().StarFunction();

        yield return new WaitUntil(() => gameObject.GetComponent<StarAnimationScript>().getAPanelFinished() == true);

        AudioSource.clip = congratsAudioClips[UnityEngine.Random.Range(0,5)];
        AudioSource.Play();
        ApplauseAudioSource.Play();
        yield return new WaitForSeconds(AudioSource.clip.length);
        gameObject.GetComponent<StarAnimationScript>().deactivateAPanel();

        if (mixTestColorCounter[ChosenColor] >= ColorSprites[ChosenColor].Length)
        {
            gameObject.GetComponent<StarAnimationScript>().StarEndAnimation.GetComponent<AudioSource>().clip =
                (AudioClip) Resources.Load("Sound/applause");
            gameObject.GetComponent<StarAnimationScript>().StartAnimation();
            TestPictureObjects[0].SetActive(false);
            TestPictureObjects[1].SetActive(false);
            questionTextObject.SetActive(false);

            mixTestColorCounter[ChosenColor] = 0;
            SendDataToDB();

            AddStar();
            StarPanel.SetActive(true);
            restartObject.SetActive(true);
            testStartObject.SetActive(true);
            ReloadSceneObject.SetActive(true);
            noAudioPlaying = true;
            isMixTestFinished = false;
            isMixTestActive = false;
            yield break;
        }
        
        foreach ( GameObject t in TestPictureObjects)
            t.GetComponent<Image>().color  = new Color32(255,255,225,255);
        
        testAnimals();
        
        noAudioPlaying = true;
    }
    
    IEnumerator CongratsSoundMixTest(int i)
    {
        if (!TestPictureObjects[i].CompareTag("trueAnswer")) {
            TestPictureObjects[i].GetComponent<Image>().color  = new Color32(255,255,225,100);

            mixTestFailCounter[ChosenColor][mixTestColorCounter[ChosenColor]-1]++;
		    
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
        mixTestAnswerTimes[ChosenColor][mixTestColorCounter[ChosenColor]-1] = passedTime;
        
        StopCoroutine(co);
        RacoonHelpObject.SetActive(false);
        noAudioPlaying = false;
        
        if (mixTestColorCounter[ChosenColor] <= ColorSprites[ChosenColor].Length)
            gameObject.GetComponent<StarAnimationScript>().StarFunction();
        
        yield return new WaitUntil(() => gameObject.GetComponent<StarAnimationScript>().getAPanelFinished() == true);

        AudioSource.clip = congratsAudioClips[UnityEngine.Random.Range(0,5)];
        AudioSource.Play();
        ApplauseAudioSource.Play();
        yield return new WaitForSeconds(AudioSource.clip.length);
        gameObject.GetComponent<StarAnimationScript>().deactivateAPanel();
        
        if (mixTestColorCounter[0] == 5 && mixTestColorCounter[1] == 5 && mixTestColorCounter[2] == 5)
        {
            isMixTestFinished = true;
            isMixTestActive = false;
        }

        if (isMixTestFinished)
        {
            gameObject.GetComponent<StarAnimationScript>().StarEndAnimation.GetComponent<AudioSource>().clip =
                (AudioClip) Resources.Load("Sound/applause");
            gameObject.GetComponent<StarAnimationScript>().StartAnimation();
            TestPictureObjects[0].SetActive(false);
            TestPictureObjects[1].SetActive(false);
            questionTextObject.SetActive(false);

            for (int j = 0; j < 3; j++)
            {
                SendDataToDBforMixTest(mixTestFailCounter[i], mixTestAnswerTimes[i],i);
            }

            ColorsMenuObjects.SetActive(true);
            noAudioPlaying = true;
            isMixTestFinished = false;
            isMixTestActive = false;
            yield break;
        }
        
        foreach ( GameObject t in TestPictureObjects)
            t.GetComponent<Image>().color  = new Color32(255,255,225,255);
        
        mixTest();
        noAudioPlaying = true;
    }
    
    #endregion
    
    #region Function
			
    public void RestartScene()
    {
        SceneManager.LoadScene("ColorsScene");
    }
    

    public void PlaySound(int i)
    {
        StarAnimationScript.counp = 0;
        ChosenColor = i;
        mixTestColorCounter[ChosenColor] = 0;
        if(noAudioPlaying)
            StartCoroutine(IdentifySound());
    }

    public void NextImage()
    {
        if(noAudioPlaying)
            StartCoroutine(IdentifySound());
    }
    
    public void PlayCongrats(int i)
    {
        if (!noAudioPlaying || AudioSource.isPlaying) return;
        if(isMixTestActive)
            StartCoroutine(CongratsSoundMixTest(i));
        else
            StartCoroutine(CongratsSound(i));
    }

    public void ReShowImages()
    {
        StarPanel.SetActive(false);
        for (int i = 0; i < StarAnimationScript.counp; i++)
            Stars[i].SetActive(false);
        restartObject.SetActive(false);
        testStartObject.SetActive(false);
        ReloadSceneObject.SetActive(false);
        ShowPictureObject.SetActive(true);
        Racoon.SetActive(true);
        showAnimalImage();
    }
    
    private void showAnimalImage()
    {
        ColorsMenuObjects.SetActive(false);
        ShowPictureObject.SetActive(true);
        Racoon.SetActive(true);
        if (mixTestColorCounter[ChosenColor] < ColorSprites[ChosenColor].Length)
        {
            
            RacoonText.GetComponent<Text>().text = "Bu renk " + RacoonTextInput[ChosenColor];
            ShowPictureObject.GetComponent<Image>().overrideSprite = ColorSprites[ChosenColor][mixTestColorCounter[ChosenColor]];
            mixTestColorCounter[ChosenColor]++;
        }
        else
        {
            if (StarAnimationScript.counp < 4)
                testStartObject.SetActive(false);
            else
                testStartObject.SetActive(true);
            
            ShowPictureObject.SetActive(false);
            Racoon.SetActive(false);
            mixTestColorCounter[ChosenColor] = 0;
            StarPanel.SetActive(true);
            if (StarAnimationScript.counp < 5)
                StarAnimationScript.counp++;
            AddStar();
            restartObject.SetActive(true);

            ReloadSceneObject.SetActive(true);

        }
    }

    private void testStart()
    {
        
        StarAnimationScript.counp = 0;
        restartObject.SetActive(false);
        testStartObject.SetActive(false);
        ReloadSceneObject.SetActive(false);
        
        questionTextObject.SetActive(true);
        foreach (var t in TestPictureObjects)
        {
            t.SetActive(true);
        }
        StarPanel.SetActive(false);
        foreach (var t in Stars)
        {
            t.SetActive(false);
        }

        mixTestColorCounter[ChosenColor] = 0;
        testAnimals();
        
    }
    
    private void mixTestStart()
    {
        ColorsMenuObjects.SetActive(false);
        
        questionTextObject.SetActive(true);
        foreach (var t in TestPictureObjects)
        {
            t.SetActive(true);
        }
        
        for (int i = 0; i < 3; i++)
        {
            mixTestColorCounter[i] = 0; 
        }
        
        isMixTestActive = true;
        StarPanel.SetActive(false);
        for (int i = 0; i < StarAnimationScript.counp; i++)
            Stars[i].SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                mixTestFailCounter[i][j] = 0;
                mixTestAnswerTimes[i][j] = 0;
            }
        }
        
        mixTest();
    }


    private void testAnimals()
    {
        var randomInteger = UnityEngine.Random.Range(0, 2);
        
        AudioSource.clip = QuestionAudioClips[ChosenColor];
        AudioSource.Play();
        StartCoroutine(WaitUntilQuestion());
        
        switch (randomInteger)
        {
            case 0:
                questionTextObject.GetComponent<Text>().text = "Hangisi " + RacoonTextInput[ChosenColor] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = ColorSprites[ChosenColor][mixTestColorCounter[ChosenColor]];
                TestPictureObjects[randomInteger].tag = "trueAnswer";

                LoadRandomColorPictureToOtherObject(1);
                mixTestColorCounter[ChosenColor]++;              
                break;
            case 1:
                questionTextObject.GetComponent<Text>().text = "Hangisi " + RacoonTextInput[ChosenColor] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = ColorSprites[ChosenColor][mixTestColorCounter[ChosenColor]];
                TestPictureObjects[randomInteger].tag = "trueAnswer";

                LoadRandomColorPictureToOtherObject(0);
                mixTestColorCounter[ChosenColor]++;

                break;
            default:
                Debug.Log("Unexpected random integer.");
                break;
        }
    }

    private void mixTest()
    {
        ChosenColor = UnityEngine.Random.Range(0, 3);
        while (mixTestColorCounter[ChosenColor] == 5)
            ChosenColor = UnityEngine.Random.Range(0, 3);
        
        print( "Kırmızı : " + mixTestColorCounter[0] + " Mavi: " + mixTestColorCounter[1] + " Sarı: " + mixTestColorCounter[2]);


        var randomInteger = UnityEngine.Random.Range(0, 2);
        AudioSource.clip = QuestionAudioClips[ChosenColor];
        AudioSource.Play();
        StartCoroutine(WaitUntilQuestion());
        
        switch (randomInteger)
        {
            case 0:
                questionTextObject.GetComponent<Text>().text = "Hangisi " + RacoonTextInput[ChosenColor] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = ColorSprites[ChosenColor][mixTestColorCounter[ChosenColor]];
                TestPictureObjects[randomInteger].tag = "trueAnswer";

                LoadRandomColorPictureToOtherObject(1);
                mixTestColorCounter[ChosenColor]++;
                break;
            case 1:
                questionTextObject.GetComponent<Text>().text = "Hangisi " + RacoonTextInput[ChosenColor] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = ColorSprites[ChosenColor][mixTestColorCounter[ChosenColor]];
                TestPictureObjects[randomInteger].tag = "trueAnswer";

                LoadRandomColorPictureToOtherObject(0);
                mixTestColorCounter[ChosenColor]++;
                break;
            default:
                Debug.Log("Unexpected random integer.");
                break;
        }
    }

    
    
    private void LoadRandomColorPictureToOtherObject(int testObjectNumber)
    {
        int[] numbers = { 0, 1, 2 };
        var numToRemove = ChosenColor;
        numbers = numbers.Where(val => val != numToRemove).ToArray();
        
        var randomInteger = UnityEngine.Random.Range(0, 2);
        var tempNum = numbers[randomInteger];
        randomInteger = UnityEngine.Random.Range(0, ColorSprites[tempNum].Length);
        
        
        TestPictureObjects[testObjectNumber].GetComponent<Image>().sprite = ColorSprites[tempNum][randomInteger];
        TestPictureObjects[testObjectNumber].tag = "falseAnswer";
        
        co = StartCoroutine(testObjectNumber==0 ? StartRacoonHelpCounter(1) : StartRacoonHelpCounter(0));
    }
    
	
    public void GoToMainMenu()
    {
        StarAnimationScript.counp = 0;
        SceneManager.LoadScene("MainScene");
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
    

    private void SendDataToDB()
    {
        IDbConnection dbconn = connectToDB();
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();
		
        string sqlQuery = "INSERT INTO Test (TestType,StuNo,q1,q2,q3,q4,q5) values ('"+TestNames[ChosenColor]+"',"+PlayerPrefs.GetInt("StuNumber")+","+FailCounter[0]+","+FailCounter[1]+","+FailCounter[2]+","+FailCounter[3]+","+FailCounter[4]+")";
		
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        
        IDbCommand dbcmd2 = dbconn.CreateCommand();
        sqlQuery = "INSERT INTO TestTimes (TestType,StuNo,q1,q2,q3,q4,q5) values ('"+TestNames[ChosenColor]+"',"+PlayerPrefs.GetInt("StuNumber")+","+AnswerTimes[0]+","+AnswerTimes[1]+","+AnswerTimes[2]+","+AnswerTimes[3]+","+AnswerTimes[4]+")";
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
    
    private void SendDataToDBforMixTest(int[] failCounter, float[] answerTimes, int chosenColor)
    {
        IDbConnection dbconn = connectToDB();
        dbconn = (IDbConnection) new SqliteConnection("URI=file:" + conn);

        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();
		
        string sqlQuery = "INSERT INTO Test (TestType,StuNo,q1,q2,q3,q4,q5) values ('"+TestNames[chosenColor]+"',"+PlayerPrefs.GetInt("StuNumber")+","+failCounter[0]+","+failCounter[1]+","+failCounter[2]+","+failCounter[3]+","+failCounter[4]+")";
		
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        
        IDbCommand dbcmd2 = dbconn.CreateCommand();
        sqlQuery = "INSERT INTO TestTimes (TestType,StuNo,q1,q2,q3,q4,q5) values ('"+TestNames[chosenColor]+"',"+PlayerPrefs.GetInt("StuNumber")+","+answerTimes[0]+","+answerTimes[1]+","+answerTimes[2]+","+answerTimes[3]+","+answerTimes[4]+")";
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