using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Doesn't work as expected
// only for y Axis and Top-Aligned elements
public class MoveToSafeArea : MonoBehaviour
{   
    public GameObject GameObject;
    public bool YAxis = true;
    public bool XAxis = false;
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject)
        {
            Rect safeArea = Screen.safeArea;
            Vector3 oldPosition = GameObject.transform.localPosition;
            GameObject.transform.localPosition = new Vector3(
                oldPosition.x,
                oldPosition.y - safeArea.yMin,
                oldPosition.z
            );
        }
    }
}
