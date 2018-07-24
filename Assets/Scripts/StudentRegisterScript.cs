using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;


public class StudentRegisterScript : MonoBehaviour
{
    public GameObject StudentRegister;
    public GameObject StudentSelect,StuName,StuSurname,StuNo,warning, StuNoButtonPrefab;

    private GameObject GridWithRows;

    private string[] StuNames,StuSurnames;
    private int[] StudentNumbers;
    private int numberOfResults = 0;
    private string conn;
    //public GameObject[] StuNoButttons;
    private List<GameObject> StuNoButttons = new List<GameObject>();

    void Start()
    {
        GridWithRows = GameObject.Find("GridWithRows");
        getData();
        Display();
        PlayerPrefs.SetInt("StuNumber",-1);
    }

    public void Play()
    {
        if (PlayerPrefs.GetInt("StuNumber") > 0)
        {
            StudentSelect.SetActive(true);
            StudentRegister.SetActive(false);
            SceneManager.LoadScene("MainScene");
        }
    }
    public void NewStudent()
    {
        StudentSelect.SetActive(false);
        StudentRegister.SetActive(true);
    }
    public void GoBackToSelect()
    {
        SceneManager.LoadScene("StudentRegisterScene");
    }

    public void GoToTeacherStatisticsScene()
    {
        SceneManager.LoadScene("TeacherStatistics");
    }


    public void logOut()
    {
        SceneManager.LoadScene("TeacherInputScene");
    }

    public void RegisterStudent()
    {
        //Path to database.
        if (Application.platform == RuntimePlatform.Android)
        {
            conn = Application.persistentDataPath + "/Database.db";

            if (!File.Exists(conn))
            {
                WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/Database.db");

                while (!loadDB.isDone)
                {
                }

                File.WriteAllBytes(conn, loadDB.bytes);
            }

        }
        else
        {
            // WINDOWS
            conn = Application.dataPath + "/StreamingAssets/Database.db";
        }

        try
        {
            IDbConnection dbconn;
            dbconn = (IDbConnection) new SqliteConnection("URI=file:" + conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();



            string sqlQuery = "INSERT INTO Student values ('" + StuNo.GetComponent<InputField>().text + "','" +
                              StuName.GetComponent<InputField>().text + "','" +
                              StuSurname.GetComponent<InputField>().text + "','" +
                              PlayerPrefs.GetString("TeacherEmail") + "')";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }
        catch (Exception e)
        {
            warning.SetActive(true);
            //warning.transform.GetChild(0).GetComponent<Text>().text = e.Source + e.Message;
            warning.transform.GetChild(0).GetComponent<Text>().text = "Kayıt başarısız. Başka bir numara ile deneyin. Problem devam ederse destek isteyin.";
            return;
        }

        warning.SetActive(true);
        warning.transform.GetChild(0).GetComponent<Text>().text ="Kayıt başarılı!";
    }

    public void getData()
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
        dbconn = (IDbConnection)new SqliteConnection("URI=file:" + conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        IDbCommand dbcmd2 = dbconn.CreateCommand();

        string sqlQuery = "SELECT COUNT(*) FROM Student WHERE Teacher = '"+PlayerPrefs.GetString("TeacherEmail")+"'";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        reader.Read();

        numberOfResults = reader.GetInt32(0);

        StuNames = new string[numberOfResults];
        StudentNumbers = new int[numberOfResults];
        StuSurnames = new string[numberOfResults];
        //StuNoButttons = new GameObject[numberOfResults];

        sqlQuery = "SELECT * FROM Student WHERE Teacher = '"+PlayerPrefs.GetString("TeacherEmail")+"' ORDER BY StuNo ASC";


        dbcmd2.CommandText = sqlQuery;
        reader = dbcmd2.ExecuteReader();

        var counter = 0;


        while(reader.Read())
        {
            StudentNumbers[counter] = reader.GetInt32(0);
            StuNames[counter] = reader.GetString(1);
            StuSurnames[counter] = reader.GetString(2);

            counter++;
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbcmd2.Dispose();
        dbcmd2 = null;
        dbconn.Close();
        dbconn = null;
    }

    public void Display()
    {
        var i = 0;

        for (i = 0; i < StudentNumbers.Length; i++)
        {
            GameObject NewRow = (GameObject) Instantiate(Resources.Load("RowWithColumns"), GridWithRows.transform);

            GameObject NewStuNo = Instantiate(StuNoButtonPrefab, NewRow.transform) as GameObject;
            NewStuNo.transform.GetChild(0).GetComponent<Text>().text = StudentNumbers[i].ToString();
            StuNoButttons.Add(NewStuNo);

            GameObject NewStuNameObject = (GameObject) Instantiate(Resources.Load("TestType"), NewRow.transform);
            NewStuNameObject.transform.GetChild(0).GetComponent<Text>().text = StuNames[i];

            GameObject NewStuSurNameObject = (GameObject) Instantiate(Resources.Load("TestType"), NewRow.transform);
            NewStuSurNameObject.transform.GetChild(0).GetComponent<Text>().text = StuNames[i];
        }
    }

    public void CheckStuNoButtons(GameObject obj)
    {
        GameObject[] NewStuNoButton = GameObject.FindGameObjectsWithTag("Pressed");

        foreach (GameObject t in NewStuNoButton)
        {
            t.tag = "Unpressed";
            t.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("StudentRegister/ButtonUnpressed");
        }

        PlayerPrefs.SetInt("StuNumber", Int32.Parse(obj.transform.GetChild(0).GetComponent<Text>().text));
        print("Chosen stuNo: " + PlayerPrefs.GetInt("StuNumber"));
        obj.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("StudentRegister/ButtonPressed");
        obj.tag = "Pressed";
    }


}
