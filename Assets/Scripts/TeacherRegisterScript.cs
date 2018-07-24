using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Data;
using System;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.IO;

public class TeacherRegisterScript : MonoBehaviour
{
    public GameObject Email1, Password1, Name1, Surname1, warning;

    private string conn;

    public void TeacherRegisterChoice()
    {
        SceneManager.LoadScene("StudentRegisterScene");
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

        try{
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection("URI=file:" + conn);
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = "INSERT INTO Teacher (Email,Password,Name,Surname) values ('"+Email1.GetComponent<InputField>().text+"','"+Password1.GetComponent<InputField>().text+"','"+Name1.GetComponent<InputField>().text+"','"+Surname1.GetComponent<InputField>().text+"')";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }catch(Exception e){
            warning.SetActive(true);
            //warning.transform.GetChild(0).GetComponent<Text>().text = e.Source + e.Message;
            warning.transform.GetChild(0).GetComponent<Text>().text = "Kayıt başarısız. Başka bir E-mail ile deneyin. Problem devam ederse destek isteyin.";
            return;
        }

        warning.SetActive(true);
        warning.transform.GetChild(0).GetComponent<Text>().text ="Kayıt başarılı!";


        //SceneManager.LoadScene("StudentRegisterScene");
    }

}
