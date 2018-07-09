using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeacherInputScript : MonoBehaviour {

    public void Input()
    {
        SceneManager.LoadScene("StudentRegisterScene");
    }
    public void Register()
    {
        SceneManager.LoadScene("TeacherRegisterScene");
    }
}
