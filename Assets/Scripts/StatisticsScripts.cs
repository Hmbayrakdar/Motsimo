using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite; using System.Data;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class StatisticsScripts : MonoBehaviour {
    #region Variables

    private GameObject GridWithRows;
    private GameObject StartingRow;

    private string[] Testnames;
    private int[] StudentNumbers;
    private List<int[]> Questions = new List<int[]>();
    private int numberOfResults = 0;
    private string conn;
    public GameObject NormalSearchCanvas;
    public GameObject SpecializedSearchCanvas;
    public GameObject StudentList;
    public GameObject TestTypesList;
    public GameObject SearchTypeList,warning;
    private List<int> StuNoDropdownList = new List<int>();
    private List<string> TestTypeDropdownList = new List<string>();
    private List<string> SearchTypeDropdownList = new List<string>();
		
    #endregion

    #region Unity Callbacks
	
    // Use this for initialization
    private void OnEnable()
    {
        SearchTypeDropdownList.Add("Arama Tipi");
        SearchTypeDropdownList.Add("Yanlış Ortalaması");
        SearchTypeDropdownList.Add("Cevaplama süresi");
        SearchTypeList.GetComponent<Dropdown>().AddOptions(SearchTypeDropdownList);
        //PlayerPrefs.SetString("TeacherEmail","hamzamelih61@hotmail.com");
        GridWithRows = GameObject.Find("GridWithRows");
        StartingRow = GameObject.Find("StartingRow");
        getData();
        Display();
        FillDropDownLists();
        
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainScene");
        }
    }
	
    #endregion
	
    #region Functions

    public void goBackToStudentSelectScene()
    {
        SceneManager.LoadScene("StudentRegisterScene");
    }

    private void getData()
    {
        
        IDbConnection dbconn = connectToDB();
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        IDbCommand dbcmd2 = dbconn.CreateCommand();

        string sqlQuery = "SELECT COUNT(*),StuNo,TestType FROM Test WHERE StuNo IN (SELECT StuNo FROM Student WHERE Teacher = '"+PlayerPrefs.GetString("TeacherEmail")+"')";
	    
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        reader.Read();

        numberOfResults = reader.GetInt32(0);

        Testnames = new string[numberOfResults];
        StudentNumbers = new int[numberOfResults];

        sqlQuery = "SELECT * FROM Test WHERE StuNo IN (SELECT StuNo FROM Student WHERE Teacher = '"+PlayerPrefs.GetString("TeacherEmail")+"') ORDER BY StuNo ASC";
	    

        dbcmd2.CommandText = sqlQuery;
        reader = dbcmd2.ExecuteReader();

        var counter = 0;

	    
        while(reader.Read())
        {
            var readerFieldCount = reader.FieldCount;

            Testnames[counter] = reader.GetString(0);
            StudentNumbers[counter] = reader.GetInt32(1);

            int[] TempQuestionArray = new int[(readerFieldCount-2)];

            for (var i = 2; i < readerFieldCount; i++)
            {
			    
                TempQuestionArray[(i-2)] = reader.GetInt32(i);
			    
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

    private void Display()
    {
        var i = 0;
        var MaxNumberOfQuestions = 0;
        for (i = 0; i < numberOfResults; i++)
        {
            if (Questions[i].Length>= MaxNumberOfQuestions)
            {
                MaxNumberOfQuestions = Questions[i].Length;
            }
        }

        for (i = 0; i < MaxNumberOfQuestions; i++)
        {
            GameObject NewObj = (GameObject) Instantiate(Resources.Load("Question"), StartingRow.transform);
            NewObj.transform.GetChild(0).GetComponent<Text>().text = "Soru" + (i + 1);

        }
		
        for (i = 0; i < Testnames.Length; i++)
        {
            GameObject NewRow = (GameObject) Instantiate(Resources.Load("RowWithColumns"), GridWithRows.transform);
			
            GameObject NewTestTypeObject = (GameObject) Instantiate(Resources.Load("TestType"), NewRow.transform);
            NewTestTypeObject.transform.GetChild(0).GetComponent<Text>().text = Testnames[i];
			
            GameObject NewStuNo = (GameObject) Instantiate(Resources.Load("StuNo"), NewRow.transform);
            NewStuNo.transform.GetChild(0).GetComponent<Text>().text = StudentNumbers[i].ToString();

            for (var j = 0; j < Questions[i].Length; j++)
            {
                if(Questions[i][j] == -1) continue;
                GameObject NewQuestionObject = (GameObject) Instantiate(Resources.Load("Question"), NewRow.transform);
                NewQuestionObject.transform.GetChild(0).GetComponent<Text>().text = Questions[i][j].ToString();
            }
        }
    }

    public void SpecializedSearch()
    {
        if (StudentList.GetComponent<Dropdown>().value == 0 || TestTypesList.GetComponent<Dropdown>().value == 0 ||
            SearchTypeList.GetComponent<Dropdown>().value == 0)
        {
            warning.transform.GetChild(0).GetComponent<Text>().text = "Ayrıntılı arama için bütün seçenekleri seçmelisiniz.";
            warning.SetActive(true);
            return;
        }
        
        
           
        PlayerPrefs.SetInt("StuNumberForBarGraph",Int32.Parse(StudentList.transform.GetChild(0).GetComponent<Text>().text));
        PlayerPrefs.SetString("TestTypeForBarGraph",TestTypesList.transform.GetChild(0).GetComponent<Text>().text);
        PlayerPrefs.SetInt("SearchTypeForBarGraph",SearchTypeList.GetComponent<Dropdown>().value);
        print(PlayerPrefs.GetInt("SearchTypeForBarGraph"));
        NormalSearchCanvas.SetActive(false);
        SpecializedSearchCanvas.SetActive(true);
        
        
    }

    public void GeneralSearch()
    {
        NormalSearchCanvas.SetActive(true);
        SpecializedSearchCanvas.SetActive(false);
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

    private void FillDropDownLists()
    {
        IDbConnection dbconn = connectToDB();
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        IDbCommand dbcmd2 = dbconn.CreateCommand();

        string sqlQuery = "SELECT StuNo FROM Test WHERE StuNo IN (SELECT StuNo FROM Student WHERE Teacher = '"+PlayerPrefs.GetString("TeacherEmail")+"') group by StuNo";
	    
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        List<string> StuNoList = new List<string>();
        StuNoList.Add("Öğrenci No");
        
        while (reader.Read())
        {
            print(reader.GetInt32(0));
            int tempVar = reader.GetInt32(0);
            StuNoList.Add(tempVar.ToString());
            StuNoDropdownList.Add(reader.GetInt32(0));
        }

        StudentList.GetComponent<Dropdown>().AddOptions(StuNoList);
        
        sqlQuery = "SELECT TestType FROM Test WHERE StuNo IN (SELECT StuNo FROM Student WHERE Teacher = '"+PlayerPrefs.GetString("TeacherEmail")+"') group by TestType";
	    

        dbcmd2.CommandText = sqlQuery;
        reader = dbcmd2.ExecuteReader();
        
        TestTypeDropdownList.Add("TestTürü");
        TestTypeDropdownList.Add("Hepsi");
        
        while(reader.Read())
        {
            TestTypeDropdownList.Add(reader.GetString(0));
        }
        
        TestTypesList.GetComponent<Dropdown>().AddOptions(TestTypeDropdownList);
	
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbcmd2.Dispose();
        dbcmd2 = null;
        dbconn.Close();
        dbconn = null;
    }
    #endregion
}