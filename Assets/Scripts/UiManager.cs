using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UiManager : MonoBehaviour
{
    public GameObject keyInfoImage;

    public void ToggleKeyInfoImage()
    {
        if (keyInfoImage != null)
        {
            bool isActive = keyInfoImage.activeSelf;
            keyInfoImage.SetActive(!isActive);

            if (keyInfoImage.activeSelf)
            {
                Time.timeScale = 0f; 
            }
            else
            {
                Time.timeScale = 1f; 
            }
        }

    }
    void Update()
    {
        if (Time.timeScale == 0f && keyInfoImage != null && keyInfoImage.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            { 
            ToggleKeyInfoImage();
            }
        }
    }

}
