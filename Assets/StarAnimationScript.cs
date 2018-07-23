using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarAnimationScript : MonoBehaviour
{

    public GameObject StarEndAnmiaton, APanel;

    private int starCounter = 0;
    private GameObject[] allChildren;

    public void StartAnimation()
    {
        StarEndAnmiaton.SetActive(true);
    }

    public void EndAnimation()
    {
        gameObject.SetActive(false);
    }

    public void StarFunction(GameObject obj)
    {
        obj.SetActive(false);
        starCounter++;
        if (starCounter >= 4)
        {
            APanel.SetActive(false);
            starCounter = 0;
        }
            
    }
	
    public void StarFunction()
    {
        APanel.SetActive(true);
        ActivatePanelsChildren();
    }

    public void ActivatePanelsChildren()
    {
        allChildren = new GameObject[APanel.transform.childCount];
        var i = 0;
        foreach (Transform child in APanel.transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }
        
        foreach (GameObject child in allChildren)
        {
            child.SetActive(true);
        }
    }
	
}