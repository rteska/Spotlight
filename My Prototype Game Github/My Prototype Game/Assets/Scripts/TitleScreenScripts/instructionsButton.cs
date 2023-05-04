using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class instructionsButton : MonoBehaviour
{
    public Canvas titleScreen;
    public Canvas instructionsScreen;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void swapCanvas()
    {
        titleScreen.enabled = false;
        instructionsScreen.enabled = true;
    }
    
}
