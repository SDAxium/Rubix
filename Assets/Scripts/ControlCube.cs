using System.Collections;
using UnityEngine;

public class ControlCube : MonoBehaviour
{
    private const string GameManagerTag = "GameController";
    private GameManager _gameManager;
    
    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag(GameManagerTag).GetComponent<GameManager>();
    }
    
    private void Update()
    {
        if (!GameManager.rotating)
        {
            if(!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                HandleRotation(KeyCode.X,true,Vector3.right);
                HandleRotation(KeyCode.Y,true,Vector3.up);
                HandleRotation(KeyCode.Z,true,Vector3.forward);
            }
            else
            {
                HandleRotation(KeyCode.X,false,Vector3.right);
                HandleRotation(KeyCode.Y,false,Vector3.up);
                HandleRotation(KeyCode.Z,false,Vector3.forward);
            }
        }
    }

    private void HandleRotation(KeyCode key, bool clockwise, Vector3 axis)
    {
        if (!Input.GetKeyDown(key)) return;
        StartCoroutine(Rotate(axis,clockwise));
        _gameManager.SnapCubelets();
    }

    /// <summary>
    /// Rotates the cube 90 degrees along a given axis and direction
    /// </summary>
    /// <param name="axis">The axis to rotate around</param>
    /// <param name="clockwise">Whether to rotate the cube in the clockwise or counterclockwise direction</param>
    /// <returns></returns>
    private IEnumerator Rotate(Vector3 axis, bool clockwise)
    {
        if(GameManager.rotating) yield break;
        GameManager.rotating = true;
        int angle = 0;
        float angleStep = clockwise ? -1 : 1;

        while (angle < 90)
        {
            _gameManager.cube.transform.Rotate(axis,angleStep,Space.World);
            angle++;
            yield return null;
        }
        
        GameManager.rotating = false;
        _gameManager.SnapCubelets();
    }
}
