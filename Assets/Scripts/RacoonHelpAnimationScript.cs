using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RacoonHelpAnimationScript : MonoBehaviour {
    #region Variables

    

    
    private Sprite[] RacoonSprites;

    private Image ImageComp;

    private bool flag;
    
    #endregion

    #region Unity callbacks

    

    
    // Use this for initialization
    private void Awake()
    {
        ImageComp = gameObject.GetComponent<Image>();
        RacoonSprites = Resources.LoadAll<Sprite>("Pictures/RacoonSprites");
    }

    private void OnEnable()
    {
        flag = true;
        
        StartCoroutine(StartAnimation());
    }

    private void OnDisable()
    {
        flag = false;
    }

    IEnumerator StartAnimation()
    {
        var i = 0;
        while (flag)
        {

            if (i < RacoonSprites.Length)
            {
                ImageComp.overrideSprite = RacoonSprites[i];
                yield return new WaitForSeconds(0.05f);
                i++;
            }
            else if (i >= RacoonSprites.Length)
            {
                i = 0;
            }
        }
    }

    private void OnDestroy()
    {
        flag = false;
    }
    #endregion
}