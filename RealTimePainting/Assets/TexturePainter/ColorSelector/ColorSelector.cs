/// summary>
/// CodeArtist.mx 2018
/// To get the selector color, call in any method: ColorSelector.GetColor();
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ColorSelector : MonoBehaviour , IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private RawImage selectorImage;

    [SerializeField]
    private Image outerCursor;

    [SerializeField]
    private Image innerCursor;

    [SerializeField]
    private Image finalColorImage;

    private RectTransform rectTransform;
    private Vector2 innerDelta;
    private Color finalColor;
    private Color selectedColor;
    private float selectorAngle;

    private enum SelectorState { OuterColor,InnerColor,None}
    private SelectorState selectorState;

    private void Start ()
    {     
        selectedColor = Color.red;
		
        selectorAngle = 0.0f;
        innerDelta = Vector2.zero;
        finalColorImage.color=finalColor;
        rectTransform = transform as RectTransform;
        selectorState = SelectorState.None;
        SelectInnerColor(Vector2.zero);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateSelector(eventData);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateSelector(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        selectorState = SelectorState.None;        
    }
    private void UpdateSelector(PointerEventData eventData)
    {
      
        Vector2 localPosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPosition);
        localPosition *= 1.0f / (rectTransform.sizeDelta.x * 0.5f);
        if (selectorState == SelectorState.None)
        {
            float dist = Vector2.Distance(Vector2.zero, localPosition) ;
            if (dist <= 0.8f)
            {
                selectorState = SelectorState.InnerColor;
            }
            else
            {
                selectorState = SelectorState.OuterColor;
            }
        }
        switch (selectorState)
        {
            case SelectorState.InnerColor:
                SelectInnerColor(localPosition);
                break;
            case SelectorState.OuterColor:
                SelectOuterColor(localPosition);
                break;
        }
    }
	private void SelectInnerColor(Vector2 delta)
    {
		float v=0.0f, w=0.0f, u=0.0f;
		Barycentric (delta,ref v,ref w,ref u);
        
        /*v = Mathf.Clamp(v, 0.15f,v);
        w = Mathf.Clamp(w, -0.15f,w);
        u = Mathf.Clamp(u, -0.15f,u);*/
        if (v >= 0.15f && w >= -0.15f && u >= -0.15f)
        {
            Debug.Log(delta.magnitude);
            Vector3 colorVector = new Vector3 (selectedColor.r, selectedColor.g, selectedColor.b);
			Vector3 finalColorVector = v * colorVector + u * new Vector3 (0.0f, 0.0f, 0.0f) + w * new Vector3 (1.0f, 1.0f, 1.0f);
			finalColor = new Color (finalColorVector.x, finalColorVector.y, finalColorVector.z);
            finalColorImage.color=finalColor;
            innerDelta=delta;
            Vector2 a = new Vector2(0.0f, 0.5f);
            Vector2 b = new Vector2(-0.58f, -0.58f);
            Vector2 c = new Vector2(0.58f, -0.58f);

            innerCursor.transform.localPosition = delta * rectTransform.sizeDelta.x*0.5f;//ClosesPointOnTriangle(a, b, c, delta) * rectTransform.sizeDelta.x * 0.5f; //
            innerDelta = delta;		
        }

	}
    private Vector3 ClampPosToCircle(Vector3 pos)
    {
		Vector3 newPos = Vector3.zero;
		float dist = 0.9f;
		float angle = Mathf.Atan2(pos.x, pos.y);// * 180 / Mathf.PI;

		newPos.x = dist * Mathf.Sin( angle ) ;
		newPos.y = dist * Mathf.Cos( angle ) ;
		newPos.z = pos.z;
		return newPos;
	}


	void Barycentric(Vector2 point,ref float u,ref float v,ref float w)
    {
		Vector2 a = new Vector2 (0.0f, 0.5f);
		Vector2 b = new Vector2 (-0.58f, -0.58f);
		Vector2 c = new Vector2 (0.58f, -0.58f);

		Vector2 v0 = b - a, v1 = c - a, v2 = point - a;
		float d00 = Vector2.Dot(v0, v0);
		float d01 = Vector2.Dot(v0, v1);
		float d11 = Vector2.Dot(v1, v1);
		float d20 = Vector2.Dot(v2, v0);
		float d21 = Vector2.Dot(v2, v1);
		float denom = d00 * d11 - d01 * d01;
		v = (d11 * d20 - d01 * d21) / denom;
		w = (d00 * d21 - d01 * d20) / denom;
		u = 1.0f - v - w;
	}


	private void SelectOuterColor(Vector2 delta)
    {
		float angle= Mathf.Atan2(delta.x, delta.y);
        float angleGrad = angle * Mathf.Rad2Deg;
        if (angleGrad < 0.0f)
        { 
			angleGrad=360+angleGrad;
        }
        selectorAngle =angleGrad/360;
		selectedColor=HSVToRGB(selectorAngle,1.0f,1.0f);        
		selectorImage.GetComponent<CanvasRenderer>().GetMaterial().SetColor("_Color",selectedColor);
		outerCursor.transform.localPosition = ClampPosToCircle (delta)*(rectTransform.sizeDelta.x*0.5f);
		SelectInnerColor (innerDelta);
	}
	public static Color HSVToRGB(float H, float S, float V)
	{
		if (S == 0f)
			return new Color(V,V,V);
		else if (V == 0f)
			return Color.black;
		else
		{
			Color col = Color.black;
			float Hval = H * 6f;
			int sel = Mathf.FloorToInt(Hval);
			float mod = Hval - sel;
			float v1 = V * (1f - S);
			float v2 = V * (1f - S * mod);
			float v3 = V * (1f - S * (1f - mod));
			switch (sel + 1)
			{
			case 0:
				col.r = V;
				col.g = v1;
				col.b = v2;
				break;
			case 1:
				col.r = V;
				col.g = v3;
				col.b = v1;
				break;
			case 2:
				col.r = v2;
				col.g = V;
				col.b = v1;
				break;
			case 3:
				col.r = v1;
				col.g = V;
				col.b = v3;
				break;
			case 4:
				col.r = v1;
				col.g = v2;
				col.b = V;
				break;
			case 5:
				col.r = v3;
				col.g = v1;
				col.b = V;
				break;
			case 6:
				col.r = V;
				col.g = v1;
				col.b = v2;
				break;
			case 7:
				col.r = V;
				col.g = v3;
				col.b = v1;
				break;
			}
			col.r = Mathf.Clamp(col.r, 0f, 1f);
			col.g = Mathf.Clamp(col.g, 0f, 1f);
			col.b = Mathf.Clamp(col.b, 0f, 1f);
			return col;
		}
	}

    private Vector3 ClosesPointOnTriangle( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p)
    {
        Vector3 result= p;
        result = Vector3.Project(p0 - p1, (Vector3.zero - p0).normalized);
      /*  p = NearestPointOnLine(p0, (p0 - p1), p);
        p = NearestPointOnLine(p1, (p1 - p2), p);
        p = NearestPointOnLine(p2, (p2 - p0), p);*/
       return result;
    }
    public static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
    {
        lineDir.Normalize();//this needs to be a unit vector
        var v = pnt - linePnt;
        var d = Vector3.Dot(v, lineDir);
        return linePnt + lineDir * d;
    }
    /// <summary>
    /// Returns the selected color.
    /// </summary>
    /// <returns>The Selected Color</returns>
	public Color GetColor()
    {
		return finalColor;
	}
}
