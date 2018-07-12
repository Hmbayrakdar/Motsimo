using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Data;
using System;
using UnityEngine.UI;


public class TeacherInputScript : MonoBehaviour {

    public GameObject email;
    public GameObject password;

    private string Email;
    private string Password;


    public void Input()
    {
        string conn = "URI=file:" + Application.dataPath + "/Database.db"; //Path to database.

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();


        string sqlQuery = "SELECT Email, Password, FROM Teacher";


        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;



        Email = email.GetComponent<InputField>().text;
        Password = password.GetComponent<InputField>().text;

        if (Email==Email && Password==Password)
        {
            print("Login sucsessful");
        }
        else
        {

        }
        SceneManager.LoadScene("StudentRegisterScene");
    }

    public void Register()
    {
        SceneManager.LoadScene("TeacherRegisterScene");
    }
}
   /* public void testDB()
    {
        string conn = "URI=file:" + Application.dataPath + "/Database.db"; //Path to database.
		
		IDbConnection dbconn;
		dbconn = (IDbConnection) new SqliteConnection(conn);
		dbconn.Open(); //Open connection to the database.

		IDbCommand dbcmd = dbconn.CreateCommand();


        string sqlQuery = "SELECT Email, Password, FROM Teacher";
        //string sqlQuery = "INSERT INTO Test (TestType,StuNo,q1,q2,q3,q4,q5) values ("++","++","++","++","++","++""++")";
        //string sqlQuery3 = "INSERT INTO Teacher (Email,Password,Name,Surname) values ("++","++","++","++")";

        dbcmd.CommandText = sqlQuery;
		IDataReader reader = dbcmd.ExecuteReader();
		
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		dbconn.Close();
		dbconn = null;


    }*/
