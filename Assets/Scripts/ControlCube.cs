using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;
using Random = UnityEngine.Random;

public class ControlCube : MonoBehaviour
{
    public GameObject cube;
    public GameObject center;
    public Material highlightMat;
    public Material defaultMat;

    private string[] sections = { "top", "equator", "bottom", "left", "middle", "right", "front", "stand", "back" };

    public GameObject[,] bottom = new GameObject[3,3];
    public GameObject[,] equator = new GameObject[3,3];
    public GameObject[,] top = new GameObject[3,3];

    public GameObject[,] left = new GameObject[3,3];
    public GameObject[,] middle = new GameObject[3,3];
    public GameObject[,] right = new GameObject[3,3];

    public GameObject[,] front = new GameObject[3,3];
    public GameObject[,] standing = new GameObject[3,3];
    public GameObject[,] back = new GameObject[3,3];

    private List<GameObject[,]> pieces = new();
    
    public Color[,] topColor = new Color[3,3];
    public Color[,] botColor = new Color[3,3];
    public Color[,] leftColor = new Color[3,3];
    public Color[,] rightColor = new Color[3,3];
    public Color[,] frontColor = new Color[3,3];
    public Color[,] backColor = new Color[3,3];
    

    private bool rotating;
    private int moveCount;
    private Stack<(int, int)> moves = new();
    
    // Start is called before the first frame update
    void Start()
    {
        equator[1, 1] = center;
        middle[1, 1] = center;
        standing[1, 1] = center;
        UpdateLists();
        
        pieces.Add(bottom);
        pieces.Add(equator);
        pieces.Add(top);
        pieces.Add(left);
        pieces.Add(middle);
        pieces.Add(right);
        pieces.Add(front);
        pieces.Add(standing);
        pieces.Add(back);
    }

    // Update is called once per frame
    void Update()
    {
        if (!rotating)
        {
            if(!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                HandleRotation(KeyCode.X,true,RotateX);
                HandleRotation(KeyCode.Y,true,RotateY);
                HandleRotation(KeyCode.Z,true,RotateZ);
                // HandleRotation(KeyCode.T,true,RotateTop);
            }
            else
            {
                HandleRotation(KeyCode.X,false,RotateX);
                HandleRotation(KeyCode.Y,false,RotateY);
                HandleRotation(KeyCode.Z,false,RotateZ);
                // HandleRotation(KeyCode.T,false,RotateTop);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                RotateTop(true);
                SnapCubelets();
            }
        }
        

        if (Input.GetKeyDown(KeyCode.I))
        {
            print("Printing it all");
            PrintAllMatrices();
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            for (int i = 0; i < top.GetLength(0); i++)
            {
                for (int j = 0; j < top.GetLength(1); j++)
                {
                    top[i, j].gameObject.GetComponent<Renderer>().material = defaultMat;
                }
            }
        }
    }

    private void HandleRotation(KeyCode key, bool clockwise, Func<bool, IEnumerator> rotateMethod)
    {
        if (!Input.GetKeyDown(key)) return;
        rotating = true;
        StartCoroutine(rotateMethod(clockwise));
        SnapCubelets();
    }
    
    private void ClearMatrix(GameObject[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix[i, j] = null;
            } 
        }
    }

    private void SnapCubelets()
    {
        float snapInterval = 3.5f;
        float snapRotation = 90f;
        for (int i = 0; i < cube.transform.childCount; i++)
        {
            var curr = cube.transform.GetChild(i).gameObject;
            var position = curr.transform.position;
            var rotation = curr.transform.eulerAngles;
            

            float snappedX = snapInterval * Mathf.Round(position.x / snapInterval);
            float snappedY = snapInterval * Mathf.Round(position.y / snapInterval);
            float snappedZ = snapInterval * Mathf.Round(position.z / snapInterval);
            
            
            float snappedRotX = snapRotation * Mathf.Round(rotation.x / snapRotation);
            float snappedRotY = snapRotation * Mathf.Round(rotation.y / snapRotation);
            float snappedRotZ = snapRotation * Mathf.Round(rotation.z / snapRotation);
            
            curr.transform.position = new Vector3(snappedX, snappedY, snappedZ);
            curr.transform.eulerAngles = new Vector3(snappedRotX, snappedRotY, snappedRotZ);
        }
    }
    
    /// <summary>
    /// Rotates the cube 90 degrees along the X-Axis
    /// </summary>
    /// <param name="clockwise">Whether to rotate the cube in the clockwise or counterclockwise direction</param>
    /// <returns></returns>
    private IEnumerator RotateX(bool clockwise)
    {
        int angle = 0;
        int angleStep = clockwise ? -1 : 1;

        while (angle < 90)
        {
            cube.transform.Rotate(angleStep,0,0, Space.World);
            angle++;
            yield return null;
        }
        
        rotating = false;
        SnapCubelets();
        UpdateLists();
    }
    
    /// <summary>
    /// Rotates the cube 90 degrees along the Y-Axis
    /// </summary>
    /// <param name="clockwise">Whether to rotate the cube in the clockwise or counterclockwise direction</param>
    /// <returns></returns>
    private IEnumerator RotateY(bool clockwise)
    {
        int angle = 0;
        int angleStep = clockwise ? -1 : 1;

        while (angle < 90)
        {
            cube.transform.Rotate(0,angleStep,0, Space.World);
            angle++;
            yield return null;
        }
        rotating = false;
        SnapCubelets();
        UpdateLists();
    }
    
    /// <summary>
    /// Rotates the cube 90 degrees along the Z-Axis
    /// </summary>
    /// <param name="clockwise">Whether to rotate the cube in the clockwise or counterclockwise direction</param>
    /// <returns></returns>
    private IEnumerator RotateZ(bool clockwise)
    {
        int angle = 0;
        int angleStep = clockwise ? -1 : 1;

        while (angle < 90)
        {
            cube.transform.Rotate(0,0,angleStep, Space.World);
            angle++;
            yield return null;
        }
        rotating = false;
        SnapCubelets();
        UpdateLists();
    }

    private IEnumerator RotateTop(bool clockwise)
    {
        
        int angle = 0;
        int angleStep = clockwise ? -1 : 1;
        
        Vector3 point = new Vector3(3.5f, 7, 3.5f);
        Vector3 axis = Vector3.up;
        
        
        while (angle < 90)
        {
            for (int i = 0; i < top.GetLength(0); i++)
            {
                for (int j = 0; j < top.GetLength(1); j++)
                {
                    GameObject cubelet = top[i, j];
                    print($"moving {cubelet.name}");
                    cubelet.transform.RotateAround(point,axis,angleStep);
                }
            }
            angle++;
            yield return null;
        }
        
        rotating = false;
        UpdateLists();
    }
    
    private IEnumerator RotateEquator(bool clockwise)
    {
        /*
         * LEFT MIDDLE RIGHT
         * point = new Vector3(7, 3.5f, 3.5f);
         * axis = Vector3.right;
         *
         * FRONT STANDING BACK
         * point = new Vector3(3.5f, 3.5f, 7);
         * axis = Vector3.forward;
         */
        int angle = 0;
        int angleStep = clockwise ? -1 : 1;
        
        Vector3 point = new Vector3(3.5f, 3.5f, 3.5f);
        Vector3 axis = Vector3.up;
        while (angle < 90)
        {
            foreach (GameObject obj in equator)
            {
                obj.transform.RotateAround(point, axis, angleStep);
            }
            angle++;
            yield return null;
        }
        
        UpdateLists();
        rotating = false;
    }
    
    private IEnumerator RotateBottom(bool clockwise)
    {
        int angle = 0;
        int angleStep = clockwise ? -1 : 1;
        
        Vector3 point = new Vector3(3.5f, 0, 3.5f);
        Vector3 axis = Vector3.up;
        while (angle < 90)
        {
            foreach (GameObject obj in bottom)
            {
                obj.transform.RotateAround(point, axis, angleStep);
            }
            angle++;
            yield return null;
        }
        
        UpdateLists();
        rotating = false;
    }
    
    /// <summary>
    /// Iterate through every cubelet and add them to their respective arrays. 
    /// <br></br>
    /// <br></br>
    /// This method originally checked the exact position of each gameobject to see what sections they fell into but 
    /// floating point inaccuracy, or what I assume to be floating point inaccuracy, caused errors. It instead now
    /// checks in a certain range to assign cubelets to sections.
    /// </summary>
    private void UpdateLists()
    {
        for (int i = 0; i < cube.transform.childCount; i++)
        {
            var curr = cube.transform.GetChild(i).gameObject;
            var position = curr.transform.position;

            // Left Layer
            if (position.x <= 3.0f) 
                left[(int)(position.y / 3.5f), (int)(position.z / 3.5f)] = curr;
            // Middle Layer
            else if (position.x is < 4.0f and > 3.0f) 
                middle[(int)(position.y / 3.5f), (int)(position.z / 3.5f)] = curr;
            // Right Layer
            else if (position.x > 5.0f) 
                right[(int)(position.y / 3.5f), (int)(position.z / 3.5f)] = curr;
            
            // Bottom layer 
            if (position.y < 3.0f) 
                bottom[(int)(position.x / 3.5f), (int)(position.z / 3.5f)] = curr;
            // Equator Layer
            else if (position.y is < 4.0f and > 3.0f) 
                equator[(int)(position.x / 3.5f), (int)(position.z / 3.5f)] = curr;
            // Top Layer
            else if (position.y > 5.0f) 
                top[(int)(position.x / 3.5f), (int)(position.z / 3.5f)] = curr;
            
            // Front Layer
            if (position.z < 3.0f) 
                front[(int)(position.x / 3.5f), (int)(position.y / 3.5f)] = curr;
            // Standing Layer
            else if (position.z is < 4.0f and > 3.0f) 
                standing[(int)(position.x / 3.5f), (int)(position.y / 3.5f)] = curr;
            // Back Layer
            else if (position.z > 5.0f) 
                back[(int)(position.x / 3.5f), (int)(position.y / 3.5f)] = curr;
        }
    }
    
    private void Rotate90Clockwise(GameObject[,] a)
    {
        int n = a.GetLength(0);
        
        // Traverse each cycle
        for (int i = 0; i < n / 2; i++)
        {
            for (int j = i; j < n - i - 1; j++)
            {
                // Swap elements of each cycle
                // in clockwise direction
                var temp = a[i, j];
                a[i, j] = a[n - 1 - j, i];
                a[n - 1 - j, i] = a[n - 1 - i, n - 1 - j];
                a[n - 1 - i, n - 1 - j] = a[j, n - 1 - i];
                a[j, n - 1 - i] = temp;
            }
        }
    }
    
    private static void Rotate90CounterClockwise(GameObject[,] arr)
    {
        int n = arr.GetLength(0);
        for (int i = 0; i < n; i++)
        {
            for (int j = i; j < n; j++) {
                (arr[j, i], arr[i, j]) = (arr[i, j], arr[j, i]);
            }
        }
        
        for (int i = 0; i <n; i++)
        {
            for (int j = 0, k = n - 1; j < k; j++, k--) {
                (arr[j, i], arr[k, i]) = (arr[k, i], arr[j, i]);
            }
        }
    }
    
    /// <summary>
    /// Rotates an array of colors 90 degrees. Probably no longer needed because ray-casts will be used on all sides to
    /// see if there are any completed sides. 
    /// </summary>
    /// <param name="a">The array of colors</param>
    static void Rotate90Clockwise(Color[,] a)
    {
        int n = a.GetLength(0);
        // Traverse each cycle
        for (int i = 0; i < n / 2; i++)
        {
            for (int j = i; j < n - i - 1; j++)
            {
 
                // Swap elements of each cycle
                // in clockwise direction
                var temp = a[i, j];
                a[i, j] = a[n - 1 - j, i];
                a[n - 1 - j, i] = a[n - 1 - i, n - 1 - j];
                a[n - 1 - i, n - 1 - j] = a[j, n - 1 - i];
                a[j, n - 1 - i] = temp;
            }
        }
    }
    
    private IEnumerator DoRotation(bool clockwise)
    {
        /*
         * TOP, EQUATOR, BOTTOM
         * point = new Vector3(0, 3.5f, 0);
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
        int index = Random.Range(0, 9);
        int angle = 0;
        Vector3 point, axis;
        int angleStep = clockwise ? 1 : -1;
        
        point = new Vector3(0, 0, 3.5f);
        axis = Vector3.forward;
        while (angle < 90)
        {
            foreach (GameObject obj in back)
            {
                obj.transform.RotateAround(point, axis, angleStep);
            }
            angle++;
            yield return 0;
        }
    }

    private IEnumerator RandomRotations()
    {
        while (moveCount < 50)
        {
            print($"Move count: {moveCount}");
            int index = Random.Range(0, 9);
            bool clockwise = Random.value > 0.5f;
            int angle = 0;
            Vector3 point, axis;

            int angleStep = clockwise ? 1 : -1;

            // Pick Point based on piece chosen
            // Pick Axis based on piece chosen 
            // Pick Angle based on clockwise or not
            switch (index)
            {
                case 0:
                case 1:
                case 2:
                    point = new Vector3(0, 3.5f, 0);
                    axis = Vector3.up;
                    break;
                case 3: 
                case 4:
                case 5:
                    point = new Vector3(3.5f, 0, 0);
                    axis = Vector3.right;
                    break;
                default:
                    point = new Vector3(0, 0, 3.5f);
                    axis = Vector3.forward;
                    break;
            }
            
            while (angle < 90)
            {
                foreach (GameObject obj in pieces[index])
                {
                    obj.transform.RotateAround(point, axis, angleStep);
                }
                angle++;
                yield return 0;
            }

            // if (angle >= 90) UpdateLists();
            
            moveCount++; 
            yield return new WaitForSeconds(1f);
        }

        moveCount = 0;
        rotating = false;
    }
    
    private void PrintMatrix(GameObject[,] matrix)
    {
        var result = "";
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            { 
                result += $"{matrix[i, j].name} ";
            }
            result += "\n";
        }
        print(result);
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
    
    private void PrintAllMatricesFaux()
    {
        string res = "";
        for (int i = 0; i < top.GetLength(0); i++)
        {
            for (int j = 0; j < top.GetLength(1); j++)
            {
                res += $"{top[i, j].name} ";
            }
        }

        print(res);
    }
}
