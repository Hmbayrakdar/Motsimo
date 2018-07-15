using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class StudentRegisterScript : MonoBehaviour
{
    public GameObject StudentRegister;
    public GameObject StudentSelect,StuName,StuSurname,StuNo;
	
	private GameObject GridWithRows;

	private string[] StuNames,StuSurnames;
	private int[] StudentNumbers;
	private List<int[]> Questions = new List<int[]>();
	private int numberOfResults = 0;

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
        StudentSelect.SetActive(true);
        StudentRegister.SetActive(false);
    }

	public void GoToTeacherStatisticsScene()
	{
		SceneManager.LoadScene("TeacherStatistics");
	}


	public void logOut()
	{
		SceneManager.LoadScene("TeacherInputScene");
	}
	
	public void  RegisterStudent()
	{
		string conn = "URI=file:" + Application.dataPath + "/Database/Database.db"; //Path to database.

		IDbConnection dbconn;
		dbconn = (IDbConnection)new SqliteConnection(conn);
		dbconn.Open(); //Open connection to the database.

		IDbCommand dbcmd = dbconn.CreateCommand();
        
        

		string sqlQuery = "INSERT INTO Student values ('"+StuNo.GetComponent<InputField>().text+"','"+StuName.GetComponent<InputField>().text+"','"+StuSurname.GetComponent<InputField>().text+"','"+PlayerPrefs.GetString("TeacherEmail")+"')";
		dbcmd.CommandText = sqlQuery;
		IDataReader reader = dbcmd.ExecuteReader();

		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		dbconn.Close();
		dbconn = null;
	}
	
	private void getData()
	{
		string conn = "URI=file:" + Application.dataPath + "/Database/Database.db"; //Path to database.
		
		IDbConnection dbconn;
		dbconn = (IDbConnection) new SqliteConnection(conn);
		dbconn.Open(); //Open connection to the database.
		IDbCommand dbcmd = dbconn.CreateCommand();
		IDbCommand dbcmd2 = dbconn.CreateCommand();

		string sqlQuery = "SELECT COUNT(*) FROM Student";
	    
		dbcmd.CommandText = sqlQuery;
		IDataReader reader = dbcmd.ExecuteReader();

		reader.Read();

		numberOfResults = reader.GetInt32(0);

		StuNames = new string[numberOfResults];
		StudentNumbers = new int[numberOfResults];
		StuSurnames = new string[numberOfResults];
        
		sqlQuery = "SELECT * FROM Student";
	    

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
	
	private void Display()
	{
		var i = 0;

		for (i = 0; i < StudentNumbers.Length; i++)
		{
			GameObject NewRow = (GameObject) Instantiate(Resources.Load("RowWithColumns"), GridWithRows.transform);
			
			GameObject NewStuNo = (GameObject) Instantiate(Resources.Load("StudentRegister/StuNoButton"), NewRow.transform);
			NewStuNo.transform.GetChild(0).GetComponent<Text>().text = StudentNumbers[i].ToString();
			
			GameObject NewStuNameObject = (GameObject) Instantiate(Resources.Load("TestType"), NewRow.transform);
			NewStuNameObject.transform.GetChild(0).GetComponent<Text>().text = StuNames[i];
			
			GameObject NewStuSurNameObject = (GameObject) Instantiate(Resources.Load("TestType"), NewRow.transform);
			NewStuSurNameObject.transform.GetChild(0).GetComponent<Text>().text = StuNames[i];
		}
	}
    

}

