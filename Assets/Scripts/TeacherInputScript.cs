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
            string dbPath = System.IO.Path.Combine (Application.persistentDataPath, "Database.db");
            var dbTemplatePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Database.db");
 
            if (!System.IO.File.Exists(dbPath)) {
                
                // Must use WWW for streaming asset
                WWW reader1 = new WWW(dbTemplatePath);
                while ( !reader1.isDone) {}
                System.IO.File.WriteAllBytes(dbPath, reader1.bytes);
                     
            }

            conn = dbPath;

        }
        else
        {
            // WINDOWS
            conn = "URI=file:" + Application.dataPath + "/Database/Database.db";
        }


        IDbConnection dbconn;
        dbconn = (IDbConnection) new SqliteConnection(conn);
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



        Email = email.GetComponent<InputField>().text;
        Password = password.GetComponent<InputField>().text;

        if (Email == Email1 && Password == Password1)
        {
            print("Login sucsessful");
            PlayerPrefs.SetString("TeacherEmail", Email);
            SceneManager.LoadScene("StudentRegisterScene");
        }
        else
        {
            print("Giriş Başarısız");
        }

    }

    public void Register()
    {
        SceneManager.LoadScene("TeacherRegisterScene");
    }
}