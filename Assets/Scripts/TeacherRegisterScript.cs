﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Data;
using System;
using UnityEngine.UI;
using Mono.Data.Sqlite;



public class TeacherRegisterScript : MonoBehaviour
{
    public GameObject Email, Password, Name, Surname;
    public void TeacherRegisterChoice()
    {
        SceneManager.LoadScene("StudentRegisterScene");
    }

    public void SendDataToDB()
    {
        string conn = "URI=file:" + Application.dataPath + "/Database/Database.db"; //Path to database.

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "INSERT INTO Teacher (Email,Password,Name,Surname) values ("+Email+","+Password+","+Name+","+Surname+")";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;


        SceneManager.LoadScene("StudentRegisterScene");
    }

}