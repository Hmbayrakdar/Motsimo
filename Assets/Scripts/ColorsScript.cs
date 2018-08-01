using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite; using System.Data; using System.IO;
using System.Linq;

public class ColorsScript : MonoBehaviour {

	#region Variables
	
    public GameObject questionTextObject,ColorsMenuObjects, ShowPictureObject, restartObject, testStartObject, ReloadSceneObject, Racoon, RacoonText;
    public GameObject[] TestPictureObjects;
    public AudioSource ApplauseAudioSource;
    
    private AudioClip[] IdentificationAudioClips, QuestionAudioClips, congratsAudioClips;
    private AudioSource AudioSource;
    private bool noAudioPlaying = true;
    
    private Sprite[][] ColorSprites = new Sprite[3][];
    private int PictureCounter;
    private int[] FailCounter = new int[5];
    private string TestName = "Renkler";
    private string[] RacoonTextInput = {"kırmızı", "mavi", "sarı"};
	private string conn;
    private int ChosenColor;
	
    #endregion
	
    #region Unity Callbacks
	
    // Use this for initialization
    void Start ()
    {
        PictureCounter = 0;
        foreach (var t in TestPictureObjects)
        {
            t.tag = "trueAnswer";
        }

        for (int i = 0; i < FailCounter.Length; i++)
        {
            FailCounter[i] = 0;
        }
        
        AudioSource = gameObject.GetComponent<AudioSource>();
        
        ColorSprites[0] = Resources.LoadAll<Sprite>("Pictures/Kırmızı");
        ColorSprites[1] = Resources.LoadAll<Sprite>("Pictures/Mavi");
        ColorSprites[2] = Resources.LoadAll<Sprite>("Pictures/Sarı");

        IdentificationAudioClips =  new AudioClip[]{(AudioClip)Resources.Load("Sound/Colors/Identify/Kırmızı"),
            (AudioClip)Resources.Load("Sound/Colors/Identify/Mavi"), 
            (AudioClip)Resources.Load("Sound/Colors/Identify/Sarı")
        };
        
        QuestionAudioClips =  new AudioClip[]{(AudioClip)Resources.Load("Sound/Colors/Question/Hangisi kırmızı göster"),
            (AudioClip)Resources.Load("Sound/Colors/Question/Hangisi mavi göster"), 
            (AudioClip)Resources.Load("Sound/Colors/Question/Hangisi sarı göster")
        };
        
        congratsAudioClips = new AudioClip[]{(AudioClip)Resources.Load("Sound/Congrats/Böyle devam"),
            (AudioClip)Resources.Load("Sound/Congrats/Harika"), 
            (AudioClip)Resources.Load("Sound/Congrats/Mükemmel"), 
            (AudioClip)Resources.Load("Sound/Congrats/Süper"),
            (AudioClip)Resources.Load("Sound/Congrats/Tebrikler")
        };

        ApplauseAudioSource.clip = (AudioClip) Resources.Load("Sound/applause");
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
        if (AudioSource.isPlaying)
                yield break;
  
        
		if (!TestPictureObjects[i].CompareTag("trueAnswer")) {
			int number = PictureCounter - 1;
            FailCounter[number]++;
		    TestPictureObjects[i].GetComponent<Image>().color  = new Color32(255,255,225,100);
			yield break;
		}

        noAudioPlaying = false;
        
        if (PictureCounter < ColorSprites[ChosenColor].Length)
        {
            gameObject.GetComponent<StarAnimationScript>().StarFunction();
        }
        
        yield return new WaitUntil(() => gameObject.GetComponent<StarAnimationScript>().getAPanelFinished() == true);

        AudioSource.clip = congratsAudioClips[UnityEngine.Random.Range(0,5)];
        AudioSource.Play();
        ApplauseAudioSource.Play();
        yield return new WaitForSeconds(AudioSource.clip.length);
        gameObject.GetComponent<StarAnimationScript>().deactivateAPanel();
        
        if (PictureCounter >= ColorSprites[ChosenColor].Length)
        {
            gameObject.GetComponent<StarAnimationScript>().StarEndAnimation.GetComponent<AudioSource>().clip =
                (AudioClip) Resources.Load("Sound/applause");
            gameObject.GetComponent<StarAnimationScript>().StartAnimation();
            TestPictureObjects[0].SetActive(false);
            TestPictureObjects[1].SetActive(false);
            questionTextObject.SetActive(false);

            PictureCounter = 0;
            SendDataToDB();

            restartObject.SetActive(true);
            testStartObject.SetActive(true);
            ReloadSceneObject.SetActive(true);
            noAudioPlaying = true;
            yield break;
        }
        foreach ( GameObject t in TestPictureObjects)
            t.GetComponent<Image>().color  = new Color32(255,255,225,255);
        
        testAnimals(i);
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
        ChosenColor = i;
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
        if(noAudioPlaying)
            StartCoroutine(CongratsSound(i));
    }

    public void ReShowImages()
    {
        restartObject.SetActive(false);
        testStartObject.SetActive(false);
        ReloadSceneObject.SetActive(false);
        ShowPictureObject.SetActive(true);
        Racoon.SetActive(true);
        showAnimalImage();
    }
    public void showAnimalImage()
    {
        ColorsMenuObjects.SetActive(false);
        ShowPictureObject.SetActive(true);
        Racoon.SetActive(true);
        if (PictureCounter < ColorSprites[ChosenColor].Length)
        {
            
            RacoonText.GetComponent<Text>().text = "Bu renk " + RacoonTextInput[ChosenColor];
            ShowPictureObject.GetComponent<Image>().overrideSprite = ColorSprites[ChosenColor][PictureCounter];
            PictureCounter++;
        }
        else
        {
            ShowPictureObject.SetActive(false);
            Racoon.SetActive(false);
            PictureCounter = 0;
			
            restartObject.SetActive(true);
            testStartObject.SetActive(true);
            ReloadSceneObject.SetActive(true);

        }
    }

    public void testStart()
    {
        restartObject.SetActive(false);
        testStartObject.SetActive(false);
        ReloadSceneObject.SetActive(false);
        
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
            AudioSource.clip = QuestionAudioClips[ChosenColor];
            AudioSource.Play();
            switch (randomInteger)
            {
                case 0:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + RacoonTextInput[ChosenColor] + " Göster";
                    
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = ColorSprites[ChosenColor][PictureCounter];
                    TestPictureObjects[randomInteger].tag = "trueAnswer";

                    LoadRandomColorPictureToOtherObject(1);
                    PictureCounter++;                   
                    break;
                case 1:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + RacoonTextInput[ChosenColor] + " Göster";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = ColorSprites[ChosenColor][PictureCounter];
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

        AudioSource.clip = QuestionAudioClips[ChosenColor];
        AudioSource.Play();
        
        switch (randomInteger)
        {
            case 0:
                questionTextObject.GetComponent<Text>().text = "Hangisi " + RacoonTextInput[ChosenColor] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = ColorSprites[ChosenColor][PictureCounter];
                TestPictureObjects[randomInteger].tag = "trueAnswer";

                LoadRandomColorPictureToOtherObject(1);
                PictureCounter++;              
                break;
            case 1:
                questionTextObject.GetComponent<Text>().text = "Hangisi " + RacoonTextInput[ChosenColor] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = ColorSprites[ChosenColor][PictureCounter];
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
        int[] numbers = { 0, 1, 2 };
        var numToRemove = ChosenColor;
        numbers = numbers.Where(val => val != numToRemove).ToArray();
        
        var randomInteger = UnityEngine.Random.Range(0, 2);
        var tempNum = numbers[randomInteger];
        randomInteger = UnityEngine.Random.Range(0, ColorSprites[tempNum].Length);
        
        
        TestPictureObjects[testObjectNumber].GetComponent<Image>().sprite = ColorSprites[tempNum][randomInteger];
        TestPictureObjects[testObjectNumber].tag = "falseAnswer";
        
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
