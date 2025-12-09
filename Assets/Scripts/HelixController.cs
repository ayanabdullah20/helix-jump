using System.Collections.Generic;
using UnityEngine;

public class HelixController : MonoBehaviour
{
    public float rotationSpeed = .1f; // Speed of rotation
    public Vector2 startpos;
    public bool isSwiping;

    public GameObject stackprefab;
    public float gapbetweenstacks = 2f;
    private GameObject ball;
    private float loweststackY;
    private List<GameObject> stacklist = new List<GameObject>();
        //camera related
    private Camera MainCamera;
    private Vector3 cameraTargetpos;
    private float cameramovestarttime;
    public float cameramoveDuration = .5f;
    private bool iscameraMoving;

    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Player");
        spawninitialstacks();
        MainCamera = Camera.main;
    }
    void spawninitialstacks()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 spawnPos = new Vector3(0f, -i * gapbetweenstacks, 0f);
            GameObject stack = Instantiate(stackprefab, spawnPos, Quaternion.identity);
            stacklist.Add(stack);
            stack.transform.SetParent(transform);
        }
        loweststackY = -9 * gapbetweenstacks;
    }
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startpos = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                RotateHelix(touch.position.x - startpos.x);
                startpos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isSwiping = false;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            startpos = Input.mousePosition;
            isSwiping = true;
        }
        else if (Input.GetMouseButton(0) && isSwiping)
        {
            RotateHelix(Input.mousePosition.x - startpos.x);
            startpos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isSwiping = false;
        }
        checkstackclear();
        if(iscameraMoving && MainCamera)
        {
            float t = (Time.time - cameramovestarttime) / cameramoveDuration;
            if(t < 1f)
            {
                MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, cameraTargetpos, t);
            }
            else
            {
                MainCamera.transform.position = cameraTargetpos;
                iscameraMoving = false;
                Debug.Log("Camera Move Completed");
            }
        }
    }
    void checkstackclear()
    {
        if (!ball)
            return;
        for(int i = stacklist.Count - 1; i >= 0; i--)
        {
            if(stacklist[i] && ball.transform.position.y < stacklist[i].transform.position.y)
            {
                stackcleared(i);
                break;
            }
        }

    }
    void stackcleared(int index)
    {
        GameObject clearedstack = stacklist[index];
        stacklist.RemoveAt(index);
        if(clearedstack)
        {
            Destroy(clearedstack);
            Debug.Log("Stack Cleared");
        }
        loweststackY -= gapbetweenstacks;
        Vector3 spawnPos = new Vector3(0f, loweststackY, 0f);
        GameObject newstack = Instantiate(stackprefab, spawnPos, Quaternion.identity,transform);
        stacklist.Add(newstack);
        Debug.Log("New Stack Spawned at pos y : " + loweststackY);
        if (MainCamera)
        {
            cameraTargetpos = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y - gapbetweenstacks, MainCamera.transform.position.z);
            cameramovestarttime = Time.time;
            iscameraMoving = true;
        }
    }
    void RotateHelix(float swipedistance)
    {

            float rotationAmount = -swipedistance * rotationSpeed;
            transform.Rotate(0f, rotationAmount, 0f);
        
    }
}
