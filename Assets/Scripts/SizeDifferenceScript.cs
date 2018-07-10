using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
//using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SizeDifferenceScript : MonoBehaviour {
	
    #region Variables

    public GameObject QuestionText;
    public GameObject[] ShowPictureObjects;
    public Sprite[] SizeDifferenceSprites;
    public GameObject restartObject, testStartObject, goBackObject, Point;
    public GameObject[] TestPictureObjects;

    private int PictureCounter;
    private bool SmallPictureFlag, BigPictureFlag, isTesting;
	
    #endregion
	
    #region Unity Callbacks
    
    // Use this for initialization
    void Start ()
    {
        PictureCounter = 0;
        
        SmallPictureFlag = true;
        BigPictureFlag = true;
        isTesting = false;
        
        TestPictureObjects[0].tag = "trueAnswer";
        TestPictureObjects[1].tag = "trueAnswer";
        
        ShowPictures("small");
    }
	
    // Update is called once per frame
    void Update () {
		
    }
    
    IEnumerator Help_Animation(string selected_animation)
    {
        yield return new WaitForSeconds(4);
        
        Point.SetActive(true);
        Point.GetComponent<Animation>().Play(selected_animation);

    }
    
    #endregion
    
    #region Functions

    public void RestartScene()
    {
        SceneManager.LoadScene("SizeDifference");
    }

    public void ShowPictures(String key)
    {
        switch (key)
        {
            case "small":
                SmallPictureFlag = true;
                break;
            case "big":
                BigPictureFlag = true;
                break;
            case "test":
                isTesting = true;
                break;
            default:
                Debug.Log("Unexpected function parameter.");
                break;
        }

        if (isTesting == true)
        {
            TestPictureObjects[0].SetActive(true);
            TestPictureObjects[1].SetActive(true);
            QuestionText.SetActive(true);
            goBackObject.SetActive(false);
            
            TestPictureObjects[0].tag = "trueAnswer";
            TestPictureObjects[1].tag = "trueAnswer";
            PictureCounter = 0;
            
            DifferenceTest(-1);
        }

        if (SmallPictureFlag != true || BigPictureFlag != true) return;
        
        if (PictureCounter >= SizeDifferenceSprites.Length)
        {
            ShowPictureObjects[0].SetActive(false);
            ShowPictureObjects[1].SetActive(false);

            restartObject.SetActive(true);
            testStartObject.SetActive(true);
            goBackObject.SetActive(true);
            
            PictureCounter = 0;
        }
        
        ShowPictureObjects[0].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];
        ShowPictureObjects[1].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];
        PictureCounter++;
        SmallPictureFlag = BigPictureFlag = false;

    }

    public void DifferenceTest(int i)
    {
        var randomInteger = UnityEngine.Random.Range(0, 2);

        if (i == -1)
        {
            switch (randomInteger)
            {
                case 0:
                    TestPictureObjects[0].GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
                    TestPictureObjects[0].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];

                    TestPictureObjects[1].GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                    TestPictureObjects[1].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];
                
                    DecideQuestion(0,1);

                    break;
                case 1:
                    TestPictureObjects[0].GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                    TestPictureObjects[0].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];

                    TestPictureObjects[1].GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
                    TestPictureObjects[1].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];
                
                    DecideQuestion(1,0);
                    
                    break;
                default:
                    Debug.Log("Unexpected random integer.");
                    break;
            }

            return;
        }

        if (TestPictureObjects[i].tag != "trueAnswer") return;
        Point.SetActive(false);
        
        if (PictureCounter >= SizeDifferenceSprites.Length)
        {
            TestPictureObjects[0].SetActive(false);
            TestPictureObjects[1].SetActive(false);
            QuestionText.SetActive(false);

            PictureCounter = 0;

            Point.SetActive(false); 
            StopAllCoroutines();
            restartObject.SetActive(true);
            testStartObject.SetActive(true);   
            goBackObject.SetActive(true);
        }
        
        switch (randomInteger)
        {
            case 0:
                TestPictureObjects[0].GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
                TestPictureObjects[0].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];

                TestPictureObjects[1].GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                TestPictureObjects[1].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];
                
                DecideQuestion(0,1);
                break;
            case 1:
                TestPictureObjects[0].GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                TestPictureObjects[0].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];

                TestPictureObjects[1].GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
                TestPictureObjects[1].GetComponent<Image>().sprite = SizeDifferenceSprites[PictureCounter];
                
                DecideQuestion(1,0);
                break;
            default:
                Debug.Log("Unexpected random integer.");
                break;
        }


    }

    //Decide Question takes the objects, then decides whether the question will be small or big
    private void DecideQuestion(int bigPicture, int smallPicture)
    {
        var randomInteger = UnityEngine.Random.Range(0, 2);
        
        switch (randomInteger)
        {
            case 0://The question will ask the bigger picture
                QuestionText.GetComponent<Text>().text = "Hangisi büyük göster.";
                TestPictureObjects[bigPicture].tag = "trueAnswer";
                TestPictureObjects[smallPicture].tag = "falseAnswer";
                StartCoroutine(Help_Animation("AnswerAnimation"+(bigPicture+1).ToString()));
                break;
            case 1://The question will ask the smaller picture
                QuestionText.GetComponent<Text>().text = "Hangisi küçük göster.";
                TestPictureObjects[bigPicture].tag = "falseAnswer";
                TestPictureObjects[smallPicture].tag = "trueAnswer";
                StartCoroutine(Help_Animation("AnswerAnimation"+(smallPicture+1).ToString()));
                break;
            default:
                Debug.Log("Unexpected random.");
                break;
        }

        PictureCounter++;
    }

    public void GoToConceptsMenu()
    {
        SceneManager.LoadScene("MainScene");
    }
    
    #endregion
    
}