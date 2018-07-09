using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Random = System.Random;

public class MainScript : MonoBehaviour
{
    #region Variables
    
    public GameObject[] MainMenuElements;//Ana menü butonlarını tutmak için
    public GameObject[] ConceptsMenuElements;//Kavram menüsündeki butonları tutmak için
    public GameObject[] Images;//UI'daki ana renkleri tutan image objeleri için
    public GameObject[] Test;//Test resimlerini göstermek için
    public GameObject Point;//Yardım simgesini tutmak için
    public GameObject testImage;//Test butonunu tutmak için
    public GameObject SingleColor;//Belirlenen renkteki(kırmızı,mavi,sarı) resmi göstermek için
    public GameObject questionText;//To show the question as text on UI
    public Sprite[] RedPics;//Kırmızı resimleri tutmak için
    public Sprite[] YellowPics;//Sarı resimleri tutmak için
    public Sprite[] BluePics;//Mavi resimleri tutmak için
    public Sprite[] Colors;//Ana renklerin resimlerini tutmak için

    private List<Sprite[]> ColorImageSprites = new List<Sprite[]>();
    private int PictureCounter;//Kaçıncı resimde olduğunu belirlemek için
    private string ChosenColor;//Hangi ana rengin seçildiğini tutmak için
    private bool yellowFlag, redFlag, blueFlag;//Renklerin bakıldığını onaylamak için
    private Time AnswerTimeCheck;//Cevap süresini kontrol edip yardımcı animasyonu çağırmak için
    private int randomInt;//Test için gerekli random değerleri tutmak için
    
    #endregion
	
    #region Unity Callbacks
    
    // Use this for initialization
    void Start ()
    {
        PictureCounter = 0;
        ColorImageSprites.Add(RedPics);
        ColorImageSprites.Add(BluePics);
        ColorImageSprites.Add(YellowPics);
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
    
    #region Menu Transitions
    
    public void ConceptsMenuTransition()
    {
        //Ana menü butonlarını görünmemesi için deaktive et
        foreach (var t in MainMenuElements)
        {
            t.SetActive(false);
        }

        //Kavram menüsündeki butonları aktive et
        foreach (var t in ConceptsMenuElements)
        {
            t.SetActive(true);
        }

        foreach (var t in Images)
        {
            t.SetActive(false);
        }
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
        //Kavram menüsündeki butonları deaktive et
        foreach (var t in MainMenuElements)
        {
            t.SetActive(true);
        }

        //Ana menü butonlarını aktive et
        foreach (var t in ConceptsMenuElements)
        {
            t.SetActive(false);
        }
    }

    public void ColorMenuTransition()
    {
        //Kavram menüsündeki butonları deaktive et
        foreach (var t in ConceptsMenuElements)
        {
            t.SetActive(false);
        }
        
        //Ana renkleri gösteren butonları aktive et ve içlerine ana renkleri koy(kırmızı,mavi,yeşil)
        foreach (var t in Images)
        {
            t.SetActive(true);
        }

        for (var i = 0; i <3; i++)
        {
            Images[i].GetComponent<Image>().overrideSprite = Colors[i];
        }
        
        //Bütün renklere baktıktan sonra test simgesi aktive et
        if (redFlag == true && blueFlag == true && yellowFlag == true)
        {
            testImage.SetActive(true);
        }
    }
    
  
    //Ana renkleri tutan objelerin çağırdığı fonksiyon, parametre olarak seçilen rengi alıyor, o renk ile ilgili ilk
    //resmi gösteriyor
    public void ColorTransition(String color)
    {
        
        //Ana renkleri tutan objeleri deaktive et
        foreach (var t in Images)
        {
            t.SetActive(false);
        }
        testImage.SetActive(false);

        //Seçilen rengi kaydet
        ChosenColor = color;
        
        //Seçilen rengin resimlerini gösterecek objeyi aktive et
        SingleColor.SetActive(true);
 
        //Seçilen rengin resimleri aç ve resim sayacını 1 arttır
        if (color == "red")
        {
            SingleColor.GetComponent<Image>().overrideSprite = RedPics[PictureCounter];
            PictureCounter++;
        }
        //Seçilen rengin resimleri aç ve resim sayacını 1 arttır
        else if (color == "blue")
        {
            SingleColor.GetComponent<Image>().overrideSprite = BluePics[PictureCounter];
            PictureCounter++;
        }
        //Seçilen rengin resimleri aç ve resim sayacını 1 arttır
        else if (color == "yellow")
        {
            SingleColor.GetComponent<Image>().overrideSprite = YellowPics[PictureCounter];
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
            SingleColor.GetComponent<Image>().overrideSprite = RedPics[PictureCounter];
            PictureCounter++;
            //Resimler bitince sayacı sıfırla, resim göstere objeyi deaktive et, seçilen rengin bakıldığını onayla
            //Ana renkler menüsüne dön
            if (PictureCounter >= RedPics.Length)
            {
                PictureCounter = 0;
                SingleColor.SetActive(false);
                redFlag = true;
                ColorMenuTransition();
            }
        }
        //Seçilen renge göre sıradaki resmi göster, sayacı 1 arttır
        else if (ChosenColor == "blue")
        {
            SingleColor.GetComponent<Image>().overrideSprite = BluePics[PictureCounter];
            PictureCounter++;
            //Resimler bitince sayacı sıfırla, resim gösteren objeyi deaktive et, seçilen rengin bakıldığını onayla
            //Ana renkler menüsüne dön
            if (PictureCounter >= BluePics.Length)
            {
                PictureCounter = 0;
                SingleColor.SetActive(false);
                blueFlag = true;
                ColorMenuTransition();
            }
        }
        //Seçilen renge göre sıradaki resmi göster, sayacı 1 arttır
        else if (ChosenColor == "yellow")
        {
            SingleColor.GetComponent<Image>().overrideSprite = YellowPics[PictureCounter];
            PictureCounter++;
            //Resimler bitince sayacı sıfırla, resim göstere objeyi deaktive et, seçilen rengin bakıldığını onayla
            //Ana renkler menüsüne dön
            if (PictureCounter >= YellowPics.Length)
            {
                PictureCounter = 0;
                SingleColor.SetActive(false);
                yellowFlag = true;
                ColorMenuTransition();
            }
        }
    }

    public void ColorTestObjectsMenu()
    {
        foreach (var t in Images)
        {
            t.SetActive(false);
        }
        
        testImage.SetActive(false);
        
        Test[0].SetActive(true);
        Test[1].SetActive(true);
        questionText.SetActive(true);
        
        PictureCounter = 0;
        randomInt = UnityEngine.Random.Range(0, 2);
        Test[randomInt].tag = "trueAnswer";
        
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

        if (Test[i].tag != "trueAnswer") return;
        Point.SetActive(false);
        
        switch (randomInt)
        {
            case 1:
                Test[0].tag = "trueAnswer";
                StartCoroutine(Help_Animation("AnswerAnimation1"));
                if (PictureCounter <= 4)
                {
                    Test[0].GetComponent<Image>().sprite = ColorImageSprites[0][PictureCounter];
                    PictureCounter++;
                    LoadRandomColorPictureToOtherObject(1,0);
                    questionText.GetComponent<Text>().text = "Hangisi kırmızı göster.";
                }
                else if (PictureCounter <= 9)
                {
                    Test[0].GetComponent<Image>().sprite = ColorImageSprites[1][PictureCounter-5];
                    PictureCounter++;
                    LoadRandomColorPictureToOtherObject(1,1);
                    questionText.GetComponent<Text>().text = "Hangisi mavi göster.";
                }
                else if (PictureCounter <= 14)
                {
                    Test[0].GetComponent<Image>().sprite = ColorImageSprites[2][PictureCounter-10];
                    PictureCounter++;
                    LoadRandomColorPictureToOtherObject(1,2);
                    questionText.GetComponent<Text>().text = "Hangisi sarı göster.";
                }
                else
                {
                    Test[0].SetActive(false);
                    Test[1].SetActive(false);
                    questionText.SetActive(false);
                    ConceptsMenuTransition();
                }

                break;
            case 2:
                Test[1].tag = "trueAnswer";
                StartCoroutine(Help_Animation("AnswerAnimation2"));
                if (PictureCounter <= 4)
                {
                    Test[1].GetComponent<Image>().sprite = ColorImageSprites[0][PictureCounter];
                    PictureCounter++;
                    LoadRandomColorPictureToOtherObject(0,0);
                    questionText.GetComponent<Text>().text = "Hangisi kırmızı göster.";
                }
                else if (PictureCounter <= 9)
                {
                    Test[1].GetComponent<Image>().sprite = ColorImageSprites[1][PictureCounter-5];
                    PictureCounter++;
                    LoadRandomColorPictureToOtherObject(0,1);
                    questionText.GetComponent<Text>().text = "Hangisi mavi göster.";
                }
                else if (PictureCounter <= 14)
                {
                    Test[1].GetComponent<Image>().sprite = ColorImageSprites[2][PictureCounter-10];
                    PictureCounter++;
                    LoadRandomColorPictureToOtherObject(0,2);
                    questionText.GetComponent<Text>().text = "Hangisi sarı göster.";
                }
                else
                {
                    Test[0].SetActive(false);
                    Test[1].SetActive(false);
                    questionText.SetActive(false);
                    ConceptsMenuTransition();
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
                Test[TestObjectNumber].GetComponent<Image>().sprite =
                    ColorImageSprites[UnityEngine.Random.Range(1, 3)][UnityEngine.Random.Range(0, 5)];
                break;
            case 1:
                if (0 == UnityEngine.Random.Range(0, 2))
                {
                    Test[TestObjectNumber].GetComponent<Image>().sprite =
                        ColorImageSprites[0][UnityEngine.Random.Range(0, 5)];
                }
                else
                {
                    Test[TestObjectNumber].GetComponent<Image>().sprite =
                        ColorImageSprites[2][UnityEngine.Random.Range(0, 5)];
                }

                break;
            case 2:
                Test[TestObjectNumber].GetComponent<Image>().sprite =
                    ColorImageSprites[UnityEngine.Random.Range(0, 2)][UnityEngine.Random.Range(0, 5)];
                break;
        }

        Test[TestObjectNumber].tag = "falseAnswer";
    }
    
    #endregion
    
    
    
}