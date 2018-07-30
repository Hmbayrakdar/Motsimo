using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite; using System.Data;
using System.IO;

public class SizeDifferenceScript : MonoBehaviour {
	
    #region Variables

    public GameObject questionTextObject;
    public GameObject[] ShowPictureObjects;
    public Sprite[] SizeDifferenceSprites;
    public GameObject restartObject, testStartObject, goBackObject;
    public GameObject[] TestPictureObjects;
    
    private AudioClip[] IdentificationAudioClips, QuestionAudioClips, congratsAudioClips;
    private AudioSource AudioSource;
    private bool noAudioPlaying = true;

    private int PictureCounter;
    private int[] FailCounter = new int[5];
    private string TestName = "SizeDifference";
    private bool SmallPictureFlag, BigPictureFlag, isTesting;
	private string conn;
	
    #endregion
	
    #region Unity Callbacks
    
    // Use this for initialization
    void Start ()
    {
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
        
        IdentificationAudioClips =  new AudioClip[]{(AudioClip)Resources.Load("Sound/Size Difference/Identify/Büyük"),
            (AudioClip)Resources.Load("Sound/Size Difference/Identify/Küçük"),
        };
        
        QuestionAudioClips =  new AudioClip[]{(AudioClip)Resources.Load("Sound/Size Difference/Question/Hangisi büyük göster"),
            (AudioClip)Resources.Load("Sound/Size Difference/Question/Hangisi küçük göster"), 
        };
        
        congratsAudioClips = new AudioClip[]{(AudioClip)Resources.Load("Sound/Congrats/Böyle devam"),
            (AudioClip)Resources.Load("Sound/Congrats/Harika"), 
            (AudioClip)Resources.Load("Sound/Congrats/Mükemmel"), 
            (AudioClip)Resources.Load("Sound/Congrats/Süper"),
            (AudioClip)Resources.Load("Sound/Congrats/Tebrikler")
        };
        
        ShowPictures("small");
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
        if (AudioSource.isPlaying)
            yield break;
        
		if (!TestPictureObjects[i].CompareTag("trueAnswer")) {
			var number = PictureCounter - 1;
            FailCounter[number]++;
		    TestPictureObjects[i].GetComponent<Image>().color  = new Color32(255,255,225,100);
		    yield break;
		}
        
        noAudioPlaying = false;
        
        if (PictureCounter < SizeDifferenceSprites.Length)
        {
            gameObject.GetComponent<StarAnimationScript>().StarFunction();
        }
        yield return new WaitUntil(() => gameObject.GetComponent<StarAnimationScript>().getAPanelFinished() == true);

        AudioSource.clip = congratsAudioClips[UnityEngine.Random.Range(0,5)];
        AudioSource.Play();
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
            restartObject.SetActive(true);
            testStartObject.SetActive(true);   
            goBackObject.SetActive(true);
            noAudioPlaying = true;
            yield break;
        }
        
        foreach ( GameObject t in TestPictureObjects)
            t.GetComponent<Image>().color  = new Color32(255,255,225,255);

        DifferenceTest(i);
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
        if(noAudioPlaying)
            StartCoroutine(IdentifySound(key));
    }
    
    public void PlayCongrats(int i)
    {
        if(noAudioPlaying)
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

            TestPictureObjects[0].tag = "trueAnswer";
            TestPictureObjects[1].tag = "trueAnswer";
            PictureCounter = 0;
            
            DifferenceTest(-1);
        }

        if (SmallPictureFlag != true || BigPictureFlag != true) return;
        
        if (PictureCounter >= SizeDifferenceSprites.Length)
        {
            ShowPictureObjects[0].SetActive(false);
            ShowPictureObjects[1].SetActive(false);

            restartObject.SetActive(true);
            testStartObject.SetActive(true);
            goBackObject.SetActive(true);
            
            PictureCounter = 0;
        }
        
        ShowPictureObjects[0].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];
        ShowPictureObjects[1].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];
        PictureCounter++;
        SmallPictureFlag = BigPictureFlag = false;

    }

    public void DifferenceTest(int i)
    {
        var randomInteger = UnityEngine.Random.Range(0, 2);

        if (i == -1)
        {
            switch (randomInteger)
            {
                case 0:
                    TestPictureObjects[0].GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
                    TestPictureObjects[0].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];

                    TestPictureObjects[1].GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                    TestPictureObjects[1].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];
                
                    DecideQuestion(0,1);

                    break;
                case 1:
                    TestPictureObjects[0].GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                    TestPictureObjects[0].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];

                    TestPictureObjects[1].GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
                    TestPictureObjects[1].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];
                
                    DecideQuestion(1,0);
                    
                    break;
                default:
                    Debug.Log("Unexpected random integer.");
                    break;
            }

            return;
        }

        switch (randomInteger)
        {
            case 0:
                TestPictureObjects[0].GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
                TestPictureObjects[0].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];

                TestPictureObjects[1].GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                TestPictureObjects[1].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];
                
                DecideQuestion(0,1);
                break;
            case 1:
                TestPictureObjects[0].GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                TestPictureObjects[0].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];

                TestPictureObjects[1].GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
                TestPictureObjects[1].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];
                
                DecideQuestion(1,0);
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
            case 0://The question will ask the bigger picture
                
                AudioSource.clip = QuestionAudioClips[0];
                AudioSource.Play();
                
                questionTextObject.GetComponent<Text>().text = "Hangisi büyük göster.";
                
                TestPictureObjects[bigPicture].tag = "trueAnswer";
                TestPictureObjects[smallPicture].tag = "falseAnswer";
                break;
            case 1://The question will ask the smaller picture
                
                AudioSource.clip = QuestionAudioClips[1];
                AudioSource.Play();
                
                questionTextObject.GetComponent<Text>().text = "Hangisi küçük göster.";
                
                TestPictureObjects[bigPicture].tag = "falseAnswer";
                TestPictureObjects[smallPicture].tag = "trueAnswer";
                break;
            default:
                Debug.Log("Unexpected random.");
                break;
        }

        PictureCounter++;
    }

    public void GoToConceptsMenu()
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