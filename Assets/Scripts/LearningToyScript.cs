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



public class LearningToyScript : MonoBehaviour {

    #region Variables

    public GameObject ToyTextObject, ShowToyPictureObject, restartObject, goBackObject;
    public Sprite[] StoryToySprites;
    public GameObject[] StoryToyPictureObjects;

    private AudioClip[] StoryToyAudioClips;
    private AudioSource AudioSource;

    private bool noAudioPlaying = true;

    private int PictureCounter;
    private string[] toysStory = { "Teneffüste arkadaşlarımla oyun oynarım", "Arkadaşlarımla oyun oynarken çok eğlenirim", "Oyun oynarken oyuncaklarımı arkadaşlarımla paylaşmam gerekir", "Eğer oyuncaklarımı paylaşırsam arkadaşlarım çok mutlu olur", "Oyuncaklarımı arkadaşlarımla paylaşmaya çalışacağım" };

    #endregion




    void Start () {

    

        AudioSource = gameObject.GetComponent<AudioSource>();

        StoryToyAudioClips = new AudioClip[]{(AudioClip)Resources.Load("Sound/Story/Toy/Oyuncak1"),
            (AudioClip)Resources.Load("Sound/Story/Toy/Oyuncak2"),
            (AudioClip)Resources.Load("Sound/Story/Toy/Oyuncak3"),
            (AudioClip)Resources.Load("Sound/Story/Toy/Oyuncak4"),
            (AudioClip)Resources.Load("Sound/Story/Toy/Oyuncak5")
        };

        StartCoroutine(ToySound());

    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("StoryScene");
        }
    }


    
    IEnumerator ToySound()
    {
        noAudioPlaying = false;
        
        if (PictureCounter < StoryToySprites.Length)
        {

            ToyTextObject.GetComponent<Text>().text = toysStory[PictureCounter];
            ShowToyPictureObject.GetComponent<Image>().overrideSprite = StoryToySprites[PictureCounter];
            PictureCounter++;
            AudioSource.clip = StoryToyAudioClips[PictureCounter-1];
            AudioSource.Play();
            yield return new WaitForSeconds(AudioSource.clip.length);
        }
        else
        {
            ShowToyPictureObject.SetActive(false);
            restartObject.SetActive(true);
            goBackObject.SetActive(true);
            ToyTextObject.SetActive(false);
        }
        
        noAudioPlaying = true;
        
       
    } 
    

    #region Function



    public void RestartScene()
    {
        SceneManager.LoadScene("LearningToyScene");
    }




     public void PlaySound()
{
    if (noAudioPlaying)
        StartCoroutine(ToySound());
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("StoryScene");
    }

    #endregion

}
