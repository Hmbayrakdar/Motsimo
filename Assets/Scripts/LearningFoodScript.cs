using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Random = System.Random;
using Mono.Data.Sqlite;
using System.Data;
using System.Runtime.InteropServices;
using System.IO;
public class LearningFoodScript : MonoBehaviour {

    #region Variables

    public GameObject FoodTextObject, ShowFoodPictureObject, restartObject, goBackObject;
    public Sprite[] StoryFoodSprites;

    private AudioClip[] StoryFoodAudioClips;
    private AudioSource AudioSource;
    private bool noAudioPlaying = true;

    private int PictureCounter;
    private string[] foodsStory = { "Karnım acıktığında yemek yerim", "Karnım acıktığında mutfağa gidip yemek yemem gerekir", "Yemek yerken çok mutlu olurum", "Eğer yemek yersem annem çok mutlu olur", "Karnım acıktığında yemek yemeye çalışacağım" };

    #endregion
    
    void Start () {
     

        AudioSource = gameObject.GetComponent<AudioSource>();

        StoryFoodAudioClips = new AudioClip[]{(AudioClip)Resources.Load("Sound/Story/Food/Yemek1"),
            (AudioClip)Resources.Load("Sound/Story/Food/Yemek2"),
            (AudioClip)Resources.Load("Sound/Story/Food/Yemek3"),
            (AudioClip)Resources.Load("Sound/Story/Food/Yemek4"),
            (AudioClip)Resources.Load("Sound/Story/Food/Yemek5")
        };

        StartCoroutine(FoodSound());
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("StoryScene");
        }
    }


    IEnumerator FoodSound()
    {
        noAudioPlaying = false;

        if (PictureCounter < StoryFoodSprites.Length)
        {

            FoodTextObject.GetComponent<Text>().text = foodsStory[PictureCounter];
            ShowFoodPictureObject.GetComponent<Image>().overrideSprite = StoryFoodSprites[PictureCounter];
            PictureCounter++;
            AudioSource.clip = StoryFoodAudioClips[PictureCounter - 1];
            AudioSource.Play();
            yield return new WaitForSeconds(AudioSource.clip.length);
        }
        else
        {
            ShowFoodPictureObject.SetActive(false);
            restartObject.SetActive(true);
            goBackObject.SetActive(true);
            FoodTextObject.SetActive(false);
        }
        
        noAudioPlaying = true;
    }

    #region Function

    public void RestartScene()
    {
        SceneManager.LoadScene("LearningFoodScene");
    }

    public void PlaySound()
  {
      if (noAudioPlaying)
          StartCoroutine(FoodSound());
  }


    public void GoToMainMenu()
    {
        SceneManager.LoadScene("StoryScene");
    }


    #endregion

}
