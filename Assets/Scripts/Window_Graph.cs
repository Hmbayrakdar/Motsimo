using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite; using System.Data;
using System.IO;
using UnityEngine.SceneManagement;

public class Window_Graph : MonoBehaviour {
    #region variables
    
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    public GameObject text;
    private List<GameObject> gameObjectList = new List<GameObject>();
    private string conn;
    private List<float> valueList = new List<float>() ;
    private List<string> barList = new List<string>();
    
    #endregion
    
    #region functions
    
    public void OnEnable() {

        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        
        valueList.Clear();
        barList.Clear();

        
        if (PlayerPrefs.GetString("TestTypeForBarGraph") == "Hepsi")
        {
            getDataAllTestTypes();  
        }
        else
        { 
            getDataSpecifiedTestTypes();
        }

        ShowGraph(valueList, -1, barList, (float _f) => "" + Mathf.RoundToInt(_f));
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainScene");
        }
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

    private void getDataAllTestTypes( )
    {
        IDbConnection dbconn = connectToDB();
        
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery;
        if (PlayerPrefs.GetInt("SearchTypeForBarGraph") == 1)
        {
            sqlQuery =
                "SELECT  Count(TestType),TestType,avg (q1),avg (q2),avg (q3),avg (q4),avg (q5),avg (q6),avg (q7),avg (q8),avg (q9),avg (q10) FROM Test where StuNo =" +
                PlayerPrefs.GetInt("StuNumberForBarGraph") + " group by TestType order by StuNo asc;";
            
            text.GetComponent<Text>().text = PlayerPrefs.GetInt("StuNumberForBarGraph") +
                                             " numaralı öğrencinin farklı test türlerinde yaptığı ortalama yanlış sayıları";
        }
        else
        {
            sqlQuery =
                "SELECT  Count(TestType),TestType,avg (q1),avg (q2),avg (q3),avg (q4),avg (q5),avg (q6),avg (q7),avg (q8),avg (q9),avg (q10) FROM TestTimes where StuNo =" +
                PlayerPrefs.GetInt("StuNumberForBarGraph") + " group by TestType order by StuNo asc;";
            
            text.GetComponent<Text>().text = PlayerPrefs.GetInt("StuNumberForBarGraph") +
                                             " numaralı öğrencinin farklı test türlerinde saniye olarak ortalama cevaplama süreleri";
        }

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        string testType = "";
        float avg = 0;
        float sum = 0;
        float count = 0;
        
        while (reader.Read())
        {
            for (var i = 2; i <= 11; i++)
            {
                var tempNumber = reader.GetFloat(i);
                if (tempNumber < 0) continue;
                sum = sum + tempNumber;
                count++;
            }
            testType = reader.GetString(1);

            avg = sum / count;
            
            addToLists(avg,testType);
            
            sum = count = 0;
            

        }
        
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    private void getDataSpecifiedTestTypes( )
    {
        IDbConnection dbconn = connectToDB();
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery;
        if (PlayerPrefs.GetInt("SearchTypeForBarGraph") == 1)
        {
            sqlQuery =
                "SELECT  avg (q1),avg (q2),avg (q3),avg (q4),avg (q5),avg (q6),avg (q7),avg (q8),avg (q9),avg (q10) FROM Test where StuNo =" +
                PlayerPrefs.GetInt("StuNumberForBarGraph") + " and TestType ='" +
                PlayerPrefs.GetString("TestTypeForBarGraph") + "'";
            
            text.GetComponent<Text>().text = PlayerPrefs.GetInt("StuNumberForBarGraph") + " numaralı öğrencinin " +
                                             PlayerPrefs.GetString("TestTypeForBarGraph") +
                                             " testinde her sorudaki yanlış ortalamaları";
        }
        else
        {
            sqlQuery =
                "SELECT  avg (q1),avg (q2),avg (q3),avg (q4),avg (q5),avg (q6),avg (q7),avg (q8),avg (q9),avg (q10) FROM TestTimes where StuNo =" +
                PlayerPrefs.GetInt("StuNumberForBarGraph") + " and TestType ='" +
                PlayerPrefs.GetString("TestTypeForBarGraph") + "'";
            
            text.GetComponent<Text>().text = PlayerPrefs.GetInt("StuNumberForBarGraph") + " numaralı öğrencinin " +
                                             PlayerPrefs.GetString("TestTypeForBarGraph") +
                                             " testinde saniye olarak her sorudaki cevaplama süreleri";
        }

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        reader.Read();
        
        
        for (var i = 0; i <= 9; i++)
        {
            var tempNumber = reader.GetFloat(i);
            if (tempNumber < 0) continue;
            
            addToLists(tempNumber,"Soru "+ (i+1));
        }
        
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    private void addToLists(float avg,string testType)
    {
        if (avg < 0)
            return;
        valueList.Add(avg);
        barList.Add(testType);

    }

    private void ShowGraph(List<float> valueList, int maxVisibleValueAmount = -1, List<string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null) {
        
        

        if (maxVisibleValueAmount <= 0) {
            maxVisibleValueAmount = valueList.Count;
        }

        foreach (GameObject gameObject in gameObjectList) {
            Destroy(gameObject);
        }
        gameObjectList.Clear();
        
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        float yMaximum = valueList[0];
        float yMinimum = valueList[0];
        
        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++) {
            float value = valueList[i];
            if (value > yMaximum) {
                yMaximum = value;
            }
            if (value < yMinimum) {
                yMinimum = value;
            }
        }

        float yDifference = yMaximum - yMinimum;
        if (yDifference <= 0) {
            yDifference = 5f;
        }
        yMaximum = yMaximum + (yDifference * 0.2f);
        yMinimum = yMinimum - (yDifference * 0.2f);

        yMinimum = 0f; // Start the graph at zero

        float xSize = graphWidth / (maxVisibleValueAmount + 1);

        int xIndex = 0;

        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++) {
            float xPosition = xSize + xIndex * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            GameObject barGameObject = CreateBar(new Vector2(xPosition, yPosition), xSize * .9f);
            gameObjectList.Add(barGameObject);

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -15f);
            if (getAxisLabelX != null) labelX.GetComponent<Text>().text = getAxisLabelX[i];
            gameObjectList.Add(labelX.gameObject);

            xIndex++;
        }

        int separatorCount = 10;
        for (int i = 0; i <= separatorCount; i++) {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-60f, normalizedValue * graphHeight);
            labelY.GetComponent<Text>().text = Truncate((yMinimum + (normalizedValue * (yMaximum - yMinimum))),2).ToString();
            gameObjectList.Add(labelY.gameObject);
        }
    }
    
    private float Truncate( float value, int digits)
    {
        double mult = Math.Pow(10.0, digits);
        double result = Math.Truncate( mult * value ) / mult;
        return (float) result;
    }

    private GameObject CreateBar(Vector2 graphPosition, float barWidth) {
        GameObject gameObject = new GameObject("bar", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0f);
        rectTransform.sizeDelta = new Vector2(barWidth, graphPosition.y);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(.5f, 0f);
        return gameObject;
    }
    
    #endregion

}