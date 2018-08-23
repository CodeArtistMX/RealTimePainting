using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BrushToggle : MonoBehaviour {

    [SerializeField]
    private Sprite Brush;

    [SerializeField]
    private Sprite Cursor;
    
    public void OnValueChanged(bool value)
    {
        if (value)
        {
            TexturePainter.Instance.SetNewBrush(Brush, Cursor);
        }
    }
}
