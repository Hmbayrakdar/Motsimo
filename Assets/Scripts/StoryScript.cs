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

public class StoryScript : MonoBehaviour {

    #region functions

    

   
    public void goToLearningFoodScene()
    {
        SceneManager.LoadScene("LearningFoodScene");
    }

    public void goToLearningToyScene()
    {
        SceneManager.LoadScene("LearningToyScene");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    public void goToMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
    
    #endregion
  
}
