using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdjust : MonoBehaviour
{
    [Header("Keeps the main window of the scene to the Right-Side of the screen.")]
    /// <summary>
    /// This script is just to keep the main window of the scene to the Right-Side of the screen.
    /// This can be removed without harm.
    /// </summary>
    [SerializeField]
    private int leftSidePixels = 150;
    [SerializeField]
    private Camera adjustedCamera;
    void Update ()
    {
        float margin = (Screen.width - leftSidePixels) / (float)Screen.width;
        adjustedCamera.rect = new Rect(1.0f-margin, 0.0f, margin, 1.0f);
    }
}
