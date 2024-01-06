using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlTriggers : MonoBehaviour
{
    private const string GameManagerTag = "GameController";
    private GameManager _gameManager;
    
    public List<GameObject> inside = new();

    public KeyCode command;

    public Vector3 axis;
    public Vector3 point;
    private bool _clockwise;
    
    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag(GameManagerTag).GetComponent<GameManager>();
        point = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!GameManager.rotating)
        { 
            _clockwise = !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        
            if (Input.GetKeyDown(command)) StartCoroutine(Rotate(_clockwise));
        }
    }

    public IEnumerator Rotate(bool c)
    {
        if(GameManager.rotating) yield break;
        
        GameManager.rotating = true;
        int angle = 0;
        float angleStep = c ? -1 : 1;
        
        while (angle < 90)
        {
            foreach (var o in inside)
            {
                o.transform.RotateAround(point, axis, angleStep);
            }
            angle++;
            yield return null;
        }
        
        GameManager.rotating = false;
        _gameManager.SnapCubelets(inside);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var go = other.gameObject;
        
        if(!inside.Contains(go) && !other.CompareTag("Color")) inside.Add(go);
    }
    
    private void OnTriggerExit(Collider other)
    {
        var go = other.gameObject;

        if (inside.Contains(go)) inside.Remove(go);
    }
}
