using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlTriggers : MonoBehaviour
{
    public List<GameObject> inside = new List<GameObject>();

    public KeyCode command;
    /*
     * TOP, EQUATOR, BOTTOM
     * point = new Vector3(3.5f, 7, 3.5f);
     * axis = Vector3.up;
     *
     * LEFT MIDDLE RIGHT
     * point = new Vector3(3.5f, 0, 0);
     * axis = Vector3.right;
     *
     * FRONT STANDING BACK
     * point = new Vector3(0, 0, 3.5f);
     * axis = Vector3.forward;
     */

    public Vector3 axis;
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float angleStep = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? -90f : 90f;
        
        if (Input.GetKeyDown(command))
        {
            foreach (var o in inside)
            {
                o.transform.RotateAround(this.transform.position, axis, angleStep);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var go = other.gameObject;
        
        if(!inside.Contains(go)) inside.Add(go);
    }

    private void OnTriggerExit(Collider other)
    {
        var go = other.gameObject;

        if (inside.Contains(go)) inside.Remove(go);
    }
}
