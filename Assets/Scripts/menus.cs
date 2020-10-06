using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menus : MonoBehaviour
{
   ScreenFader scrFader = default;
    void Start()
    {
       scrFader = ScreenFader.sharedInst;
    }
      
    public void LoadGamewFader(string loadALevel)
    {
        scrFader.LoadLevel(loadALevel);
    }

    public void ExitApplication()
    {
        scrFader.ExitApp();
    }
}
