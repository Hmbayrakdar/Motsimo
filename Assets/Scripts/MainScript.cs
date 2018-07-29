using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Random = System.Random;
using Mono.Data.Sqlite; using System.Data;
using System.Runtime.InteropServices;
using System.IO;

public class MainScript : MonoBehaviour
{
    #region Variables
    
    public GameObject[] MainMenuElements,ColorsMenuElements,TestPictureObjects;
    public GameObject testStartObject,ShowPictureObject,questionTextObject,Racoon, RacoonText,MenuPanel;
    public Sprite[] RedPics, YellowPics, BluePics, Colors;

    
    private AudioClip[] IdentificationAudioClips, QuestionAudioClips, congratsAudioClips;
    private AudioSource AudioSource;
    private bool noAudioPlaying = true;

    private List<Sprite[]> ColorImageSprites = new List<Sprite[]>();
    private int PictureCounter, randomInt;
    private string ChosenColor,conn;
    private bool yellowFlag, redFlag, blueFlag;
    private int[] FailCounter = new int[5];
    private string TestName = "Color";
    private string[] RacoonColors = {"Kırmızı", "Mavi", "Sarı"};
    
    
    #endregion
	
    #region Unity Callbacks
    
    // Use this for initialization
    void Start ()
    {
        PictureCounter = 0;
        ColorImageSprites.Add(RedPics);
        ColorImageSprites.Add(BluePics);
        ColorImageSprites.Add(YellowPics);
        
        for (int i = 0; i < FailCounter.Length; i++)
        {
            FailCounter[i] = 0;
        }
        
        AudioSource = gameObject.GetComponent<AudioSource>();

        IdentificationAudioClips =  new AudioClip[]{(AudioClip)Resources.Load("Sound/Colors/Identify/Kırmızı"),
            (AudioClip)Resources.Load("Sound/Colors/Identify/Mavi"), 
            (AudioClip)Resources.Load("Sound/Colors/Identify/Sarı")
        };
        
        QuestionAudioClips =  new AudioClip[]{(AudioClip)Resources.Load("Sound/Colors/Question/Hangisi kırmızı göster"),
            (AudioClip)Resources.Load("Sound/Colors/Question/Hangisi mavi göster"), 
            (AudioClip)Resources.Load("Sound/Colors/Question/Hangisi sarı göster")
        };
        
        congratsAudioClips = new AudioClip[]{(AudioClip)Resources.Load("Sound/Congrats/Böyle devam"),
            (AudioClip)Resources.Load("Sound/Congrats/Harika"), 
            (AudioClip)Resources.Load("Sound/Congrats/Mükemmel"), 
            (AudioClip)Resources.Load("Sound/Congrats/Süper"),
            (AudioClip)Resources.Load("Sound/Congrats/Tebrikler")
        };
    }
	
    // Update is called once per frame
    void Update () {
		
    }
    
    IEnumerator IdentifySound()
    {
        noAudioPlaying = false;
        var integer =0;

        switch (ChosenColor)
        {
                case "red":
                    integer = 0;
                    break;
                case "blue" :
                    integer = 1;
                    break;
                case "yellow" :
                    integer = 2;
                    break;
                default:
                    break;
        }
        
        AudioSource.clip = IdentificationAudioClips[integer];
        AudioSource.Play();
        yield return new WaitForSeconds(AudioSource.clip.length);
        
        nextColorPicture();
        noAudioPlaying = true;
    }
    
    IEnumerator CongratsSound(int i)
    {
		if (!TestPictureObjects[i].CompareTag("trueAnswer")) {
			var firstColorTreshold = 6;
            var secondColorTreshold = 11;
            int number = 0;
            if (PictureCounter <= 5)
            {
                number = PictureCounter - 1;
            }
            else if (PictureCounter <= 10)
            {
                number = PictureCounter - firstColorTreshold;
            }
            else if (PictureCounter <= 15)
            {
                number = PictureCounter - secondColorTreshold;
            }
            
            Debug.Log(number + " PictureCounter: " + PictureCounter);
            
            FailCounter[number]++;
		    TestPictureObjects[i].GetComponent<Image>().color  = new Color32(255,255,225,100);
            yield break;
		}
        
        noAudioPlaying = false;
        
        if (PictureCounter <= 14)
        {
            gameObject.GetComponent<StarAnimationScript>().StarFunction();
        }
        yield return new WaitUntil(() => gameObject.GetComponent<StarAnimationScript>().getAPanelFinished() == true);

        AudioSource.clip = congratsAudioClips[UnityEngine.Random.Range(0,5)];
        AudioSource.Play();
        yield return new WaitForSeconds(AudioSource.clip.length);
        gameObject.GetComponent<StarAnimationScript>().deactivateAPanel();
        
        if (PictureCounter > 14)
        {
            gameObject.GetComponent<StarAnimationScript>().StartAnimation();
            TestPictureObjects[0].SetActive(false);
            TestPictureObjects[1].SetActive(false);
            questionTextObject.SetActive(false);
            ColorMenuTransition();
            noAudioPlaying = true;
            PictureCounter = 0;
            yield break;
        }
        
        foreach ( GameObject t in TestPictureObjects)
            t.GetComponent<Image>().color  = new Color32(255,255,225,255);
        
        ColorTestTransition(i);
        noAudioPlaying = true;
    }

    #endregion
    
    #region Menu Transitions

    public void goToTeacherLogin()
    {
        SceneManager.LoadScene("TeacherInputScene");
    }

    public void goToSizeDifferenceScene()
    {
        SceneManager.LoadScene("SizeDifference");
    }

    public void goToNumbersScene()
    {
        SceneManager.LoadScene("NumbersScene");
    }

    public void goToAnimalsScene()
    {
        SceneManager.LoadScene("AnimalsScene");
    }
    public void goToVehicheScene()
    {
        SceneManager.LoadScene("VehicheScene");
    }
    public void goToFruitScene()
    {
        SceneManager.LoadScene("FruitsScene");
    }


    public void MainMenuTransition()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void ColorMenuTransition()
    {
        Racoon.SetActive(false);
        //Kavram menüsündeki butonları deaktive et
        MenuPanel.SetActive(false);
        
        //Ana renkleri gösteren butonları aktive et ve içlerine ana renkleri koy(kırmızı,mavi,yeşil)
        foreach (var t in ColorsMenuElements)
        {
            t.SetActive(true);
        }

        for (var i = 0; i <3; i++)
        {
            ColorsMenuElements[i].GetComponent<Image>().overrideSprite = Colors[i];
        }
        
        //Bütün renklere baktıktan sonra test simgesi aktive et
        if (redFlag == true && blueFlag == true && yellowFlag == true)
        {
            testStartObject.SetActive(true);
        }
    }
    
    public void PlaySound()
    {
        if(noAudioPlaying)
            StartCoroutine(IdentifySound());
    }
    
    public void PlayCongrats(int i)
    {
        if(noAudioPlaying)
            StartCoroutine(CongratsSound(i));
    }
    
    //Ana renkleri tutan objelerin çağırdığı fonksiyon, parametre olarak seçilen rengi alıyor, o renk ile ilgili ilk
    //resmi gösteriyor
    public void ColorTransition(String color)
    {
        
        //Ana renkleri tutan objeleri deaktive et
        foreach (var t in ColorsMenuElements)
        {
            t.SetActive(false);
        }
        testStartObject.SetActive(false);

        //Seçilen rengi kaydet
        ChosenColor = color;
        
        //Seçilen rengin resimlerini gösterecek objeyi aktive et
        ShowPictureObject.SetActive(true);
        Racoon.SetActive(true);
 
        //Seçilen rengin resimleri aç ve resim sayacını 1 arttır
        if (color == "red")
        {
            RacoonText.GetComponent<Text>().text = "Bunun rengi " + RacoonColors[0];
            ShowPictureObject.GetComponent<Image>().overrideSprite = RedPics[PictureCounter];
            PictureCounter++;
        }
        //Seçilen rengin resimleri aç ve resim sayacını 1 arttır
        else if (color == "blue")
        {
            RacoonText.GetComponent<Text>().text = "Bunun rengi " + RacoonColors[1];
            ShowPictureObject.GetComponent<Image>().overrideSprite = BluePics[PictureCounter];
            PictureCounter++;
        }
        //Seçilen rengin resimleri aç ve resim sayacını 1 arttır
        else if (color == "yellow")
        {
            RacoonText.GetComponent<Text>().text = "Bunun rengi " + RacoonColors[2];
            ShowPictureObject.GetComponent<Image>().overrideSprite = YellowPics[PictureCounter];
            PictureCounter++;
        }
    }
    
    

    //Seçilen renge göre resim gösteren objenin çağırdığı fonksiyon, sıradaki resmi gösteriyor ve resimler bitince,
    //Ana renk menüsüne geri dönüyor
    public void nextColorPicture()
    {
        //Seçilen renge göre sıradaki resmi göster, sayacı 1 arttır
        if (ChosenColor == "red")
        {
            RacoonText.GetComponent<Text>().text = "Bunun rengi " + RacoonColors[0];
            ShowPictureObject.GetComponent<Image>().overrideSprite = RedPics[PictureCounter];
            PictureCounter++;
            //Resimler bitince sayacı sıfırla, resim göstere objeyi deaktive et, seçilen rengin bakıldığını onayla
            //Ana renkler menüsüne dön
            if (PictureCounter >= RedPics.Length)
            {
                PictureCounter = 0;
                ShowPictureObject.SetActive(false);
                redFlag = true;
                ColorMenuTransition();
            }
        }
        //Seçilen renge göre sıradaki resmi göster, sayacı 1 arttır
        else if (ChosenColor == "blue")
        {
            RacoonText.GetComponent<Text>().text = "Bunun rengi " + RacoonColors[1];
            ShowPictureObject.GetComponent<Image>().overrideSprite = BluePics[PictureCounter];
            PictureCounter++;
            //Resimler bitince sayacı sıfırla, resim gösteren objeyi deaktive et, seçilen rengin bakıldığını onayla
            //Ana renkler menüsüne dön
            if (PictureCounter >= BluePics.Length)
            {
                PictureCounter = 0;
                ShowPictureObject.SetActive(false);
                blueFlag = true;
                ColorMenuTransition();
            }
        }
        //Seçilen renge göre sıradaki resmi göster, sayacı 1 arttır
        else if (ChosenColor == "yellow")
        {
            RacoonText.GetComponent<Text>().text = "Bunun rengi " + RacoonColors[2];
            ShowPictureObject.GetComponent<Image>().overrideSprite = YellowPics[PictureCounter];
            PictureCounter++;
            //Resimler bitince sayacı sıfırla, resim göstere objeyi deaktive et, seçilen rengin bakıldığını onayla
            //Ana renkler menüsüne dön
            if (PictureCounter >= YellowPics.Length)
            {
                PictureCounter = 0;
                ShowPictureObject.SetActive(false);
                yellowFlag = true;
                ColorMenuTransition();
            }
        }
    }

    public void ColorTestObjectsMenu()
    {
        foreach (var t in ColorsMenuElements)
        {
            t.SetActive(false);
        }
        
        testStartObject.SetActive(false);
        
        TestPictureObjects[0].SetActive(true);
        TestPictureObjects[1].SetActive(true);
        questionTextObject.SetActive(true);
        
        PictureCounter = 0;
        randomInt = UnityEngine.Random.Range(0, 2);
        TestPictureObjects[randomInt].tag = "trueAnswer";
        
        ColorTestTransition(randomInt);
    }
    
    #endregion

    #region Help animation(Not finished)

    /*IEnumerator HelpAnimation_1()
    {
        yield return new WaitForSeconds(4);
        Point.SetActive(true);
        //Point.GetComponent<Animation>().Play("AnimationAnswer1");
        Point.transform.position =
            Vector3.MoveTowards(Point.transform.position, KavramMenuElements[0].transform.position, 5);
        Point.
        Debug.Log("Hi");
    }*/
    
    #endregion
    
    #region Test for Colors
    
    //Test simgesini gösteren objenin çağırdığı fonksyion, testi başlatır
    public void ColorTestTransition(int i)
    {
        randomInt = UnityEngine.Random.Range(1, 3);

        

        if (PictureCounter == 5 || PictureCounter == 10 || PictureCounter == 15)
        {
            SendDataToDB();
            for (int k = 0; k < FailCounter.Length; k++)
            {
                FailCounter[k] = 0;
            }
        }
        
        switch (randomInt)
        {
            case 1:
                TestPictureObjects[0].tag = "trueAnswer";
                if (PictureCounter <= 4)
                {
                    TestPictureObjects[0].GetComponent<Image>().sprite = ColorImageSprites[0][PictureCounter];
                    PictureCounter++;
                    LoadRandomColorPictureToOtherObject(1,0);
                    questionTextObject.GetComponent<Text>().text = "Hangisi kırmızı göster.";
					AudioSource.clip = QuestionAudioClips[0];
        			AudioSource.Play();
                    TestName = "ColorRed";
                }
                
                else if (PictureCounter <= 9)
                {
                    TestPictureObjects[0].GetComponent<Image>().sprite = ColorImageSprites[1][PictureCounter-5];
                    PictureCounter++;
                    LoadRandomColorPictureToOtherObject(1,1);
                    questionTextObject.GetComponent<Text>().text = "Hangisi mavi göster.";
					AudioSource.clip = QuestionAudioClips[1];
        			AudioSource.Play();
                    TestName = "ColorBlue";
                }
                
                else if (PictureCounter <= 14)
                {
                    TestPictureObjects[0].GetComponent<Image>().sprite = ColorImageSprites[2][PictureCounter-10];
                    PictureCounter++;
                    LoadRandomColorPictureToOtherObject(1,2);
                    questionTextObject.GetComponent<Text>().text = "Hangisi sarı göster.";
					AudioSource.clip = QuestionAudioClips[2];
        			AudioSource.Play();
                    TestName = "ColorYellow";
                }
                else
                {
                    TestPictureObjects[0].SetActive(false);
                    TestPictureObjects[1].SetActive(false);
                    questionTextObject.SetActive(false);
                    ColorMenuTransition();
                }

                break;
            case 2:
                TestPictureObjects[1].tag = "trueAnswer";
                if (PictureCounter <= 4)
                {
                    TestPictureObjects[1].GetComponent<Image>().sprite = ColorImageSprites[0][PictureCounter];
                    PictureCounter++;
                    LoadRandomColorPictureToOtherObject(0,0);
                    questionTextObject.GetComponent<Text>().text = "Hangisi kırmızı göster.";
					AudioSource.clip = QuestionAudioClips[0];
        			AudioSource.Play();
                }
                else if (PictureCounter <= 9)
                {
                    TestPictureObjects[1].GetComponent<Image>().sprite = ColorImageSprites[1][PictureCounter-5];
                    PictureCounter++;
                    LoadRandomColorPictureToOtherObject(0,1);
                    questionTextObject.GetComponent<Text>().text = "Hangisi mavi göster.";
					AudioSource.clip = QuestionAudioClips[1];
        			AudioSource.Play();
                }
                else if (PictureCounter <= 14)
                {
                    TestPictureObjects[1].GetComponent<Image>().sprite = ColorImageSprites[2][PictureCounter-10];
                    PictureCounter++;
                    LoadRandomColorPictureToOtherObject(0,2);
                    questionTextObject.GetComponent<Text>().text = "Hangisi sarı göster.";
					AudioSource.clip = QuestionAudioClips[2];
        			AudioSource.Play();
                }
                else
                {
                    TestPictureObjects[0].SetActive(false);
                    TestPictureObjects[1].SetActive(false);
                    questionTextObject.SetActive(false);
                    ColorMenuTransition();
                }

                break;
            default:
                Debug.Log("Unexpected random integer at color transition.");
                break;
        }


    }

    private void LoadRandomColorPictureToOtherObject(int TestObjectNumber, int ChosenColor)
    {
        switch (ChosenColor)
        {
            case 0:
                TestPictureObjects[TestObjectNumber].GetComponent<Image>().sprite =
                    ColorImageSprites[UnityEngine.Random.Range(1, 3)][UnityEngine.Random.Range(0, 5)];
                break;
            case 1:
                if (0 == UnityEngine.Random.Range(0, 2))
                {
                    TestPictureObjects[TestObjectNumber].GetComponent<Image>().sprite =
                        ColorImageSprites[0][UnityEngine.Random.Range(0, 5)];
                }
                else
                {
                    TestPictureObjects[TestObjectNumber].GetComponent<Image>().sprite =
                        ColorImageSprites[2][UnityEngine.Random.Range(0, 5)];
                }

                break;
            case 2:
                TestPictureObjects[TestObjectNumber].GetComponent<Image>().sprite =
                    ColorImageSprites[UnityEngine.Random.Range(0, 2)][UnityEngine.Random.Range(0, 5)];
                break;
        }

        TestPictureObjects[TestObjectNumber].tag = "falseAnswer";
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

		IDbConnection dbconn;
        dbconn = (IDbConnection) new SqliteConnection("URI=file:" + conn);

        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();
		
        string sqlQuery = "INSERT INTO Test (TestType,StuNo,q1,q2,q3,q4,q5) values ('"+TestName+"',"+PlayerPrefs.GetInt("StuNumber")+","+FailCounter[0]+","+FailCounter[1]+","+FailCounter[2]+","+FailCounter[3]+","+FailCounter[4]+")";
		
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
		
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }
    
    #endregion
    
    
    
}