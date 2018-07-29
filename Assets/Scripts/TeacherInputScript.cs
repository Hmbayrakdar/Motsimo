using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using UnityEngine.UI;


public class TeacherInputScript : MonoBehaviour
{

    public GameObject email;
    public GameObject password;
	public GameObject warning;

    private string Email;
    private string Password;

    private string Email1;
    private string Password1;

    private string conn;



    public void Input()
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

		try{
        IDbConnection dbconn;
        dbconn = (IDbConnection) new SqliteConnection("URI=file:" + conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT Email, Password FROM Teacher";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
	        
            Email1 = reader.GetString(0);
            Password1 = reader.GetString(1);
			}

			reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
		}catch(Exception e) {
			warning.SetActive(true);
			warning.transform.GetChild(0).GetComponent<Text>().text = e.Source + e.Message;
			return;
        }

        Email = email.GetComponent<InputField>().text;
        Password = password.GetComponent<InputField>().text;
	    print(" entered " + Email + Password);

        if (Email == Email1 && Password == Password1)
        {
            print("Login successful");
            PlayerPrefs.SetString("TeacherEmail", Email);
            SceneManager.LoadScene("StudentRegisterScene");
        }
        else
        {
			warning.SetActive(true);
			warning.transform.GetChild(0).GetComponent<Text>().text = "Giriş Başarısız";
            print("Giriş Başarısız");
        }

    }

    public void Register()
    {
        SceneManager.LoadScene("TeacherRegisterScene");
    }

	public void goBack()
	{
		SceneManager.LoadScene("MainScene");
	}
}