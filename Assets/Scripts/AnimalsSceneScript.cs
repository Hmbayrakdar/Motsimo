using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnimalsSceneScript : MonoBehaviour {

    #region Variables
	
    public GameObject questionTextObject, ShowPictureObject, restartObject, testStartObject, goBackObject;
    public GameObject[] TestPictureObjects;
    public Sprite[] AnimalSprites;

    private int PictureCounter;
    private string[] animals = {"Balık", "İnek", "Kedi", "Köpek", "Tavşan"};
	
    #endregion
	
    #region Unity Callbacks
	
    // Use this for initialization
    void Start () {
		
        PictureCounter = 0;
        foreach (var t in TestPictureObjects)
        {
            t.tag = "trueAnswer";
        }
        showAnimalImage();
    }
	
    // Update is called once per frame
    void Update () {
		
    }
	
    #endregion
    
    #region Function
			
    public void RestartScene()
    {
        SceneManager.LoadScene("AnimalsScene");
    }
	
    public void showAnimalImage()
    {
        if (PictureCounter < AnimalSprites.Length)
        {
            ShowPictureObject.GetComponent<Image>().overrideSprite = AnimalSprites[PictureCounter];
            PictureCounter++;
        }
        else
        {
            ShowPictureObject.SetActive(false);
			
            restartObject.SetActive(true);
            testStartObject.SetActive(true);
            goBackObject.SetActive(true);
            
        }
    }

    public void testStart()
    {
        restartObject.SetActive(false);
        testStartObject.SetActive(false);
        goBackObject.SetActive(false);
        
        questionTextObject.SetActive(true);
        foreach (var t in TestPictureObjects)
        {
            t.SetActive(true);
        }

        PictureCounter = 0;
        testAnimals(-1);
    }


    public void testAnimals(int i)
    {
        var randomInteger = UnityEngine.Random.Range(0, 2);
        
        if (i == -1)
        {
            switch (randomInteger)
            {
                case 0:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + animals[PictureCounter] + " Göster";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = AnimalSprites[PictureCounter];
                    TestPictureObjects[randomInteger].tag = "trueAnswer";

                    LoadRandomColorPictureToOtherObject(1);
                    PictureCounter++;
                    
                    break;
                case 1:
                    questionTextObject.GetComponent<Text>().text = "Hangisi " + animals[PictureCounter] + " Göster";
                    TestPictureObjects[randomInteger].GetComponent<Image>().sprite = AnimalSprites[PictureCounter];
                    TestPictureObjects[randomInteger].tag = "trueAnswer";

                    LoadRandomColorPictureToOtherObject(0);
                    PictureCounter++;
                    
                    break;
                default:
                    Debug.Log("Unexpected random integer.");
                    break;
            }

            return;
        }

        if (TestPictureObjects[i].tag != "trueAnswer") return;
        
        if (PictureCounter >= AnimalSprites.Length)
        {
            TestPictureObjects[0].SetActive(false);
            TestPictureObjects[1].SetActive(false);
            questionTextObject.SetActive(false);

            PictureCounter = 0;
            
            restartObject.SetActive(true);
            testStartObject.SetActive(true);   
            goBackObject.SetActive(true);
        }
        
        switch (randomInteger)
        {
            case 0:
                questionTextObject.GetComponent<Text>().text = "Hangisi " + animals[PictureCounter] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = AnimalSprites[PictureCounter];
                TestPictureObjects[randomInteger].tag = "trueAnswer";

                LoadRandomColorPictureToOtherObject(1);
                PictureCounter++;
                
                break;
            case 1:
                questionTextObject.GetComponent<Text>().text = "Hangisi " + animals[PictureCounter] + " Göster";
                TestPictureObjects[randomInteger].GetComponent<Image>().sprite = AnimalSprites[PictureCounter];
                TestPictureObjects[randomInteger].tag = "trueAnswer";

                LoadRandomColorPictureToOtherObject(0);
                PictureCounter++;
                
                break;
            default:
                Debug.Log("Unexpected random integer.");
                break;
        }
    }
    
    private void LoadRandomColorPictureToOtherObject(int TestObjectNumber)
    {
        var randomInteger = UnityEngine.Random.Range(0, AnimalSprites.Length);
        
        while (randomInteger == PictureCounter)
        {
            randomInteger = UnityEngine.Random.Range(0, AnimalSprites.Length);
        }
        
        TestPictureObjects[TestObjectNumber].GetComponent<Image>().sprite = AnimalSprites[randomInteger];
        TestPictureObjects[TestObjectNumber].tag = "falseAnswer";
        
    }
    
	
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainScene");
    }
	

    #endregion
}