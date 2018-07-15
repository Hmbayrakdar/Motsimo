using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class test : MonoBehaviour {
    private string[] Testnames;
    private string[] StudentNumbers;
    private List<int[]> Questions = new List<int[]>();
    private int numberOfResults = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    private void getData()
    {
        string conn = "URI=file:" + Application.dataPath + "/Database/Database.db"; //Path to database.

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        IDbCommand dbcmd2 = dbconn.CreateCommand();

        string sqlQuery = "SELECT COUNT(*) FROM Test";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        reader.Read();

        numberOfResults = reader.GetInt32(0);

        Testnames = new string[numberOfResults];
        StudentNumbers = new string[numberOfResults];

        sqlQuery = "SELECT TestType,StuNo,ifnull(q1,-1)q1,ifnull(q2,-1)q2,ifnull(q3,-1)q3,ifnull(q4,-1)q4,ifnull(q5,-1)q5,ifnull(q6,-1)q6,ifnull(q7,-1)q7,ifnull(q8,-1)q8,ifnull(q9,-1)q9,ifnull(q10,-1)q10 FROM Test";


        dbcmd2.CommandText = sqlQuery;
        reader = dbcmd2.ExecuteReader();

        var counter = 0;


        while (reader.Read())
        {
            var readerFieldCount = reader.FieldCount;

            Testnames[counter] = reader.GetString(0);
            StudentNumbers[counter] = reader.GetString(1);

            int[] TempQuestionArray = new int[(readerFieldCount - 2)];

            for (var i = 2; i < readerFieldCount; i++)
            {
                if (reader.GetInt32(i) != -1)
                {
                    TempQuestionArray[(i - 2)] = reader.GetInt32(i);
                }
            }
            Questions.Add(TempQuestionArray);

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
}
