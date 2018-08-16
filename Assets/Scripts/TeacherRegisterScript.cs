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
#region Variables

    public GameObject Email1, Password1, Name1, Surname1, warning;

    private string conn,Email, Password, Name, Surname;
    
    #endregion
    
    #region functions

    public void TeacherRegisterChoice()
    {
        SceneManager.LoadScene("StudentRegisterScene");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainScene");
        }
    }
    private bool CheckInputs()
    {
        Email = Email1.GetComponent<InputField>().text;
        Password = Password1.GetComponent<InputField>().text;
        Name = Name1.GetComponent<InputField>().text;
        Surname = Surname1.GetComponent<InputField>().text;

        if (Email == "" || Password == "" || Name == "" || Surname == "")
        {
            warning.transform.GetChild(0).GetComponent<Text>().text = "Lütfen boşlukları doldurun.";
            return false;
        }
        
        return true;
    }

    public void SendDataToDB()
    {
        if (!CheckInputs())
        {
            warning.SetActive(true);
            return;
        }
        
        try{
            IDbConnection dbconn = connectToDB();
            dbconn.Open(); //Open connection to the database.

            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = "INSERT INTO Teacher (Email,Password,Name,Surname) values ('"+Email+"','"+Password+"','"+Name+"','"+Surname+"')";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }catch(Exception e){
            print(e.StackTrace);
            warning.SetActive(true);
            //warning.transform.GetChild(0).GetComponent<Text>().text = e.Source + e.Message;
            warning.transform.GetChild(0).GetComponent<Text>().text = "Kayıt başarısız. Başka bir E-mail ile deneyin. Problem devam ederse destek isteyin.";
            return;
        }
	    
        warning.SetActive(true);
        warning.transform.GetChild(0).GetComponent<Text>().text ="Kayıt başarılı!";


        //SceneManager.LoadScene("StudentRegisterScene");
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

    public void goBackToTeacherLoginScreen()
    {
        SceneManager.LoadScene("TeacherInputScene");
    }
    
    #endregion

}