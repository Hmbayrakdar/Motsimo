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