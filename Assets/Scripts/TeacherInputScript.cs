using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class TeacherInputScript : MonoBehaviour {


    public void Input()
    {
        //TouchScreenKeyboard.Open();
        SceneManager.LoadScene("StudentRegisterScene");
    }
    public void Register()
    {
        SceneManager.LoadScene("TeacherRegisterScene");
    }
    public void testDB()
    {
        string conn = "URI=file:" + Application.dataPath + "/Database.db"; //Path to database.
		
		IDbConnection dbconn;
		dbconn = (IDbConnection) new SqliteConnection(conn);
		dbconn.Open(); //Open connection to the database.

		IDbCommand dbcmd = dbconn.CreateCommand();
		string sqlQuery = "SELECT Email,Password FROM Teacher";
		//string sqlQuery = "INSERT INTO Test (TestType,StuNo,q1,q2,q3,q4,q5) values ("++","++","++","++","++","++""++")";
		//string sqlQuery2 = "SELECT StuNo, Name, Surname, Teacher FROM Student";
		//string sqlQuery3 = "INSERT INTO Teacher (Email,Password,Name,Surname) values ("++","++","++","++")";
		
		dbcmd.CommandText = sqlQuery;
		IDataReader reader = dbcmd.ExecuteReader();
		
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		dbconn.Close();
		dbconn = null;


    }
}
