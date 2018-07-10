using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeacherRegisterScript : MonoBehaviour
{
    public GameObject Email, Password, name, surname;
    public void TeacherRegisterChoice()
    {
        //string tempEmail = Email.GetComponent<InputField>().text;
        SceneManager.LoadScene("StudentRegisterScene");
    }

   
}