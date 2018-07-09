using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite; using System.Data; using System;

public class AnimalsSceneScript : MonoBehaviour {

    #region Variables
	
    public GameObject questionTextObject, ShowPictureObject, restartObject, testStartObject, goBackObject, Point;
    public GameObject[] TestPictureObjects;
    public Sprite[] AnimalSprites;
    

    private int PictureCounter;
    private int[] FailCounter = new int[5];
    private string TestName = "Animals";
    private string[] animals = {"Balık", "İnek", "Kedi", "Köpek", "Tavşan"};
	
    #endregion
	
    #region Unity Callbacks
	
    // Use this for initialization
    void Start () {
		
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
        
        
        showAnimalImage();
    }
	
    // Update is called once per frame
    void Update () {
		
    }
    
    IEnumerator Help_Animation(string selected_animation)
    {
        yield return new WaitForSeconds(4);
        
        Point.SetActive(true);
        Point.GetComponent<Animation>().Play(selected_animation);

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

        if (TestPictureObjects[i].tag != "trueAnswer")
        {
            int number = PictureCounter - 1;
            FailCounter[number]++;
            return;
        }
        Point.SetActive(false);
        
        
        if (PictureCounter >= AnimalSprites.Length)
        {
            TestPictureObjects[0].SetActive(false);
            TestPictureObjects[1].SetActive(false);
            questionTextObject.SetActive(false);

            PictureCounter = 0;
            SendDataToDB();
            
            restartObject.SetActive(true);
            testStartObject.SetActive(true);   
            goBackObject.SetActive(true);
        }
        
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
        string conn = "URI=file:" + Application.dataPath + "/Database/Database.db"; //Path to database.
		
        IDbConnection dbconn;
        dbconn = (IDbConnection) new SqliteConnection(conn);
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