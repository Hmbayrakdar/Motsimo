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
	
    public GameObject questionTextObject, ShowPictureObject, restartObject, testStartObject, goBackObject,Racoon, RacoonText;
    public GameObject[] TestPictureObjects;
    public Sprite[] AnimalSprites;
    
    private AudioClip[] IdentificationAudioClips, QuestionAudioClips, congratsAudioClips;
    private AudioSource AudioSource;
    private bool noAudioPlaying = true;
    

    private int PictureCounter;
    private int[] FailCounter = new int[5];
    private string TestName = "Animals";
    private string[] animals = {"Balık", "İnek", "Kedi", "Köpek", "Tavşan"};
	private string conn;
	
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

        IdentificationAudioClips =  new AudioClip[]{(AudioClip)Resources.Load("Sound/Animals/Identify/Balık"),
            (AudioClip)Resources.Load("Sound/Animals/Identify/İnek"), 
            (AudioClip)Resources.Load("Sound/Animals/Identify/Kedi"), 
            (AudioClip)Resources.Load("Sound/Animals/Identify/Köpek"),
            (AudioClip)Resources.Load("Sound/Animals/Identify/Tavşan")
        };
        
        QuestionAudioClips =  new AudioClip[]{(AudioClip)Resources.Load("Sound/Animals/Question/Hangisi balık göster"),
            (AudioClip)Resources.Load("Sound/Animals/Question/Hangisi inek göster"), 
            (AudioClip)Resources.Load("Sound/Animals/Question/Hangisi kedi göster"), 
            (AudioClip)Resources.Load("Sound/Animals/Question/Hangisi köpek göster"),
            (AudioClip)Resources.Load("Sound/Animals/Question/Hangisi tavşan göster")
        };
        
        congratsAudioClips = new AudioClip[]{(AudioClip)Resources.Load("Sound/Congrats/Böyle devam"),
            (AudioClip)Resources.Load("Sound/Congrats/Harika"), 
            (AudioClip)Resources.Load("Sound/Congrats/Mükemmel"), 
            (AudioClip)Resources.Load("Sound/Congrats/Süper"),
            (AudioClip)Resources.Load("Sound/Congrats/Tebrikler")
        };


        showAnimalImage();
        
    }
	

    IEnumerator IdentifySound()
    {
        noAudioPlaying = false;
        
        AudioSource.clip = IdentificationAudioClips[PictureCounter-1];
        AudioSource.Play();
        yield return new WaitForSeconds(AudioSource.clip.length);
        
        showAnimalImage();
        noAudioPlaying = true;
    }
    
    IEnumerator CongratsSound(int i)
    {
		if (!TestPictureObjects[i].CompareTag("trueAnswer")) {
			int number = PictureCounter - 1;
            FailCounter[number]++;
		    TestPictureObjects[i].GetComponent<Image>().color  = new Color32(255,255,225,100);
			yield break;
		}

        noAudioPlaying = false;
        
        if (PictureCounter < AnimalSprites.Length)
        {
            gameObject.GetComponent<StarAnimationScript>().StarFunction();
        }
        
        yield return new WaitUntil(() => gameObject.GetComponent<StarAnimationScript>().getAPanelFinished() == true);

        AudioSource.clip = congratsAudioClips[UnityEngine.Random.Range(0,5)];
        AudioSource.Play();
        yield return new WaitForSeconds(AudioSource.clip.length);
        gameObject.GetComponent<StarAnimationScript>().deactivateAPanel();
        
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
        foreach ( GameObject t in TestPictureObjects)
            t.GetComponent<Image>().color  = new Color32(255,255,225,255);
        
        testAnimals(i);
        noAudioPlaying = true;
    }
    
    #endregion
    
    #region Function
			
    public void RestartScene()
    {
        SceneManager.LoadScene("AnimalsScene");
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
        testAnimals(-1);
    }


    public void testAnimals(int i)
    {
        var randomInteger = UnityEngine.Random.Range(0, 2);
        
        if (i == -1)
        {
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
                    break;
                case 1:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + animals[PictureCounter] + " Göster";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = AnimalSprites[PictureCounter];
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
                questionTextObject.GetComponent<Text>().text = "Hangisi " + animals[PictureCounter] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = AnimalSprites[PictureCounter];
                TestPictureObjects[randomInteger].tag = "trueAnswer";

                LoadRandomColorPictureToOtherObject(1);
                PictureCounter++;              
                break;
            case 1:
                questionTextObject.GetComponent<Text>().text = "Hangisi " + animals[PictureCounter] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = AnimalSprites[PictureCounter];
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
        var randomInteger = UnityEngine.Random.Range(0, AnimalSprites.Length);
        
        while (randomInteger == PictureCounter)
        {
            randomInteger = UnityEngine.Random.Range(0, AnimalSprites.Length);
        }
        
        TestPictureObjects[testObjectNumber].GetComponent<Image>().sprite = AnimalSprites[randomInteger];
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