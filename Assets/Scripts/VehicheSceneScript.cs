using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class VehicheSceneScript : MonoBehaviour {

    #region Variables

    public GameObject questionTextObject, ShowPictureObject, restartObject, testStartObject, goBackObject,Point,Rakun,SpeechBubble,informationText;
    public GameObject[] TestPictureObjects;
    public Sprite[] VehicheSprites;

    private int PictureCounter;
    private int[] FailCounter = new int[5];
    private string TestName = "Vehiches";
    private string[] Vehiches = { "Tır", "Uçak", "Otobüs", "Araba", "Gemi" };
    private string[] CVehiches = { "TIR", "UÇAK", "OTOBÜS", "ARABA", "GEMİ" };
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
        showVehicheImage();
       


    }

    // Update is called once per frame
    void Update () {
		
	}
    #endregion

    #region Function
    public void RestartScene()
    {
        SceneManager.LoadScene("VehicheScene");
    }
    IEnumerator Help_Animation(string selected_animation)
    {
        yield return new WaitForSeconds(4);

        Point.SetActive(true);
        Point.GetComponent<Animation>().Play(selected_animation);

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
            ShowPictureObject.SetActive(false);
            Rakun.SetActive(false);
            SpeechBubble.SetActive(false);
            informationText.SetActive(false);

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
        testVehiche(-1);
    }


    public void testVehiche(int i)
    {
        var randomInteger = UnityEngine.Random.Range(0, 2);

        if (i == -1)
        {
            switch (randomInteger)
            {
                case 0:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + Vehiches[PictureCounter] + " Göster";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = VehicheSprites[PictureCounter];
                    TestPictureObjects[randomInteger].tag = "trueAnswer";

                    LoadRandomColorPictureToOtherObject(1);
                    PictureCounter++;
                    StartCoroutine(Help_Animation("AnswerAnimation1"));

                    break;
                case 1:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + Vehiches[PictureCounter] + " Göster";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = VehicheSprites[PictureCounter];
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

        if (PictureCounter >= VehicheSprites.Length)
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
                questionTextObject.GetComponent<Text>().text = "Hangisi " + Vehiches[PictureCounter] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = VehicheSprites[PictureCounter];
                TestPictureObjects[randomInteger].tag = "trueAnswer";

                LoadRandomColorPictureToOtherObject(1);
                PictureCounter++;
                StartCoroutine(Help_Animation("AnswerAnimation1"));

                break;
            case 1:
                questionTextObject.GetComponent<Text>().text = "Hangisi " + Vehiches[PictureCounter] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = VehicheSprites[PictureCounter];
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
        var randomInteger = UnityEngine.Random.Range(0, VehicheSprites.Length);

        while (randomInteger == PictureCounter)
        {
            randomInteger = UnityEngine.Random.Range(0, VehicheSprites.Length);
        }

        TestPictureObjects[TestObjectNumber].GetComponent<Image>().sprite = VehicheSprites[randomInteger];
        TestPictureObjects[TestObjectNumber].tag = "falseAnswer";

    }
    public void SendDataToDB()
    {
        string conn = "URI=file:" + Application.dataPath + "/Database/Database.db"; //Path to database.

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
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


    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainScene");
    }
    #endregion
}
