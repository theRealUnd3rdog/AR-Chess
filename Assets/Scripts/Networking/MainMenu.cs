using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android) 
        {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                
                Application.Quit();
            }
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
