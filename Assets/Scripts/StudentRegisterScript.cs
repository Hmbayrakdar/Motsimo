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
    #region Variables
  
    public GameObject StudentRegister;
    public GameObject StudentSelect,StuName,StuSurname,StuNo,warning, StuNoButtonPrefab;
	
    private GameObject GridWithRows;

    private string[] StuNames,StuSurnames;
    private int[] StudentNumbers;
    private int numberOfResults = 0;
    private string conn;
    public ToggleGroup StudentToggles;
    
    #endregion

    #region functions

    void Start()
    {
        GridWithRows = GameObject.Find("GridWithRows");
        getData();
        Display();
        PlayerPrefs.SetInt("StuNumber",-1);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainScene");
        }
    }
	
    public void Play()
    {
        if (PlayerPrefs.GetInt("StuNumber") <= 0) return;
        StudentSelect.SetActive(true);
        StudentRegister.SetActive(false);
        SceneManager.LoadScene("MainScene");
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
        PlayerPrefs.SetInt("IsLoggedIn",0);
        SceneManager.LoadScene("TeacherInputScene");
    }
    
    private bool CheckInputs()
    {
        string studentName = StuName.GetComponent<InputField>().text;
        string studentNumber = StuNo.GetComponent<InputField>().text;
        string studentSurname = StuSurname.GetComponent<InputField>().text;

        if (studentName != "" && studentNumber != "" && studentSurname != "") return true;
        warning.transform.GetChild(0).GetComponent<Text>().text = "Lütfen boşlukları doldurun.";
        return false;

    }


    public void RegisterStudent()
    {
        if (!CheckInputs())
        {
            warning.SetActive(true);
            return;
        }
        

        try
        {
            IDbConnection dbconn = connectToDB();
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
            print(e.StackTrace);
            warning.SetActive(true);
            warning.transform.GetChild(0).GetComponent<Text>().text = "Kayıt başarısız. Başka bir numara ile deneyin. Problem devam ederse destek isteyin.";
            return;
        }
        
        warning.SetActive(true);
        warning.transform.GetChild(0).GetComponent<Text>().text ="Kayıt başarılı!";
    }

    private void getData()
    {
        IDbConnection dbconn = connectToDB();
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
	
    private void Display()
    {
        var i = 0;

        for (i = 0; i < StudentNumbers.Length; i++)
        {
            GameObject NewRow = (GameObject) Instantiate(Resources.Load("RowWithColumns"), GridWithRows.transform);
            
            GameObject NewToggle = Instantiate(Resources.Load("StudentRegister/Toggle"), NewRow.transform) as GameObject;
            NewToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate
            {
                CheckStuNoButtons(NewToggle);
            });
            NewToggle.GetComponent<Toggle>().group = StudentToggles;
			
            GameObject NewStuNo = Instantiate(StuNoButtonPrefab, NewRow.transform) as GameObject;
            NewStuNo.transform.GetChild(0).GetComponent<Text>().text = StudentNumbers[i].ToString();
			
            GameObject NewStuNameObject = (GameObject) Instantiate(Resources.Load("TestType"), NewRow.transform);
            NewStuNameObject.transform.GetChild(0).GetComponent<Text>().text = StuNames[i];
			
            GameObject NewStuSurNameObject = (GameObject) Instantiate(Resources.Load("TestType"), NewRow.transform);
            NewStuSurNameObject.transform.GetChild(0).GetComponent<Text>().text = StuNames[i];
        }
    }

    public void CheckStuNoButtons(GameObject obj)
    {
        var chosenStuNo = obj.transform.parent.GetChild(1).GetChild(0).GetComponent<Text>().text;

        if (obj.GetComponent<Toggle>().isOn)
        {
            PlayerPrefs.SetInt("StuNumber", Int32.Parse(chosenStuNo));
        }
    }
    
    #endregion
    

}