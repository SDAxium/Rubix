using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Boolean to track whether the cube or a section of the cube is currently rotating. 
    /// </summary>
    public static bool rotating;
    
    public GameObject colliders;

    public GameObject cube;
    
    public List<GameObject> pieces = new();
    
    private const float SnapInterval = 3.5f;
    private const float SnapRotation = 90;
    
    private int _moveCount;
    public Stack<(int, bool)> moves = new();
    
    private void Start()
    {
        /*
         * for random rotation, we want to access all of the control triggers(rename to controlSections)
         * Make a for loop that goes through all the children of the colliders gameoject and adds them to a list
         */
        for (int i = 0; i < colliders.transform.childCount; i++)
        {
            pieces.Add(colliders.transform.GetChild(i).gameObject);
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            print("Printing it all");
            PrintAllMatrices();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(RandomRotations());
        }
    }
    
    /// <summary>
    /// Gets rid of floating point inaccuracy between rubix cube manipulations by snapping all cubelets to exact
    /// positions and rotations.
    /// </summary>
    public void SnapCubelets()
    {
        for (int i = 0; i < cube.transform.childCount; i++)
        {
            var curr = cube.transform.GetChild(i).gameObject;
            var position = curr.transform.position;
            var rotation = curr.transform.eulerAngles;
            

            float snappedX = SnapInterval * Mathf.Round(position.x / SnapInterval);
            float snappedY = SnapInterval * Mathf.Round(position.y / SnapInterval);
            float snappedZ = SnapInterval * Mathf.Round(position.z / SnapInterval);
            
            
            float snappedRotX = SnapRotation * Mathf.Round(rotation.x / SnapRotation);
            float snappedRotY = SnapRotation * Mathf.Round(rotation.y / SnapRotation);
            float snappedRotZ = SnapRotation * Mathf.Round(rotation.z / SnapRotation);
            
            curr.transform.position = new Vector3(snappedX, snappedY, snappedZ);
            curr.transform.eulerAngles = new Vector3(snappedRotX, snappedRotY, snappedRotZ);
        }
    }

    /// <summary>
    /// Gets rid of floating point inaccuracy between rubix cube manipulations by snapping all cubelets to exact
    /// positions and rotations.
    /// </summary>
    /// <param name="objects">The List of GameObjects to be snapped into position</param>
    public void SnapCubelets(List<GameObject> objects)
    {
        foreach (var o in objects)
        {
            var position = o.transform.position;
            var rotation = o.transform.eulerAngles;
            

            float snappedX = SnapInterval * Mathf.Round(position.x / SnapInterval);
            float snappedY = SnapInterval * Mathf.Round(position.y / SnapInterval);
            float snappedZ = SnapInterval * Mathf.Round(position.z / SnapInterval);
            
            
            float snappedRotX = SnapRotation * Mathf.Round(rotation.x / SnapRotation);
            float snappedRotY = SnapRotation * Mathf.Round(rotation.y / SnapRotation);
            float snappedRotZ = SnapRotation * Mathf.Round(rotation.z / SnapRotation);
            
            o.transform.position = new Vector3(snappedX, snappedY, snappedZ);
            o.transform.eulerAngles = new Vector3(snappedRotX, snappedRotY, snappedRotZ);
        }
    }
    
    /// <summary>
    /// Does 50 random movements on the rubix cube, scrambling it
    /// </summary>
    /// <returns></returns>
    private IEnumerator RandomRotations()
    {
        while (_moveCount < 50)
        {
            int index = Random.Range(0, 9);
            bool clockwise = Random.value > 0.5f;

            StartCoroutine(pieces[index].GetComponent<ControlTriggers>().Rotate(clockwise));
            
            _moveCount++; 
            yield return new WaitForSeconds(0.4f);
        }
        _moveCount = 0;
    }
    
    private void PrintAllMatrices()
    {
        string inTop="", inEq="", inBot="", inFront="", inSta="", inBack="", inLeft="", inMid="", inRight="";
        for (int i = 0; i < cube.transform.childCount; i++)
        {
            var curr = cube.transform.GetChild(i).gameObject;
            var position = curr.transform.position;
            
            // Bottom layer 
            if (position.y < 3.0f) inBot += $"{curr.name} ";
            // Equator Layer
            else if (position.y is < 4.0f and > 3.0f) inEq += $"{curr.name} ";
            // Top Layer
            else if (position.y > 5.0f) inTop += $"{curr.name} ";
            
            // Adding to left
            if (position.x < 3.0f) inLeft += $"{curr.name} ";
            // Adding to middle
            else if (position.x is < 4.0f and > 3.0f) inMid += $"{curr.name} ";
            // Adding to right
            else if (position.x > 5.0f) inRight += $"{curr.name} ";
            
            // Adding to front
            if (position.z < 3.0f) inFront += $"{curr.name} ";
            // Adding to standing
            else if (position.z is < 4.0f and > 3.0f) inSta += $"{curr.name} ";
            // Adding to back
            else if (position.z > 5.0f) inBack += $"{curr.name} ";
        }
        
        print($"Bottom: {inBot}");
        print($"Equator: {inEq}");
        print($"Top: {inTop}");
        
        print($"Left: {inLeft}");
        print($"Middle: {inMid}");
        print($"Right: {inRight}");
        
        print($"Front: {inFront}");
        print($"Standing: {inSta}");
        print($"Back: {inBack}");
    }
}
