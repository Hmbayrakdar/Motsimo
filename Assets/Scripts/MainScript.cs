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
    
    public GameObject MenuPanel;

    public AudioSource backGroundMusic;
    
    static bool AudioBegin = false;
    public GameObject loggedInText;

    #endregion
    
    #region Unity Callbacks

    void Awake()
    {
        loggedInText.GetComponent<Text>().text =
           "Öğretmen: " + PlayerPrefs.GetString("TeacherEmail") + " Öğrenci No: " + PlayerPrefs.GetInt("StuNumber");

        if (PlayerPrefs.GetInt("IsLoggedIn") == 1)
            loggedInText.SetActive(true);
        else if(PlayerPrefs.GetInt("IsLoggedIn") == 0)
            loggedInText.SetActive(false);
        
    }

    void Start()
    {
        
        if (!AudioBegin) 
        {
            backGroundMusic.clip = (AudioClip) Resources.Load("Sound/backGroundMusic");
            DontDestroyOnLoad(backGroundMusic);
            backGroundMusic.Play();
            AudioBegin = true;
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            if(Input.GetKeyDown(KeyCode.Escape))
                
                Application.Quit();
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }
    
    
    #endregion

    #region Menu Transitions

    public void goToTeacherLogin()
    {
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            SceneManager.LoadScene("TeacherInputScene");
        }
        else
            SceneManager.LoadScene("StudentRegisterScene");
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
    
    public void goToColorsScene()
    {
        SceneManager.LoadScene("ColorsScene");
    }

    public void MainMenuTransition()
    {
        SceneManager.LoadScene("MainScene");
    }

    #endregion

    
    
}