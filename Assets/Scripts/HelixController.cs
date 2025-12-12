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
    private Vector3 cameraStartPos;
    private float cameramovestarttime;
    public float cameramoveDuration = .08f;
    private float currentCameraMoveDuration;
    private bool iscameraMoving;
    public Player player;

    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Player");
        spawninitialstacks();
        MainCamera = Camera.main;
        currentCameraMoveDuration = cameramoveDuration;
    }
    void spawninitialstacks()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 spawnPos = new Vector3(0f, -i * gapbetweenstacks, 0f);
            GameObject stack = Instantiate(stackprefab, spawnPos, Quaternion.identity);
            if (i == 0)
            {
                stack.GetComponent<Stack>().generateRandomStack(true);
            }
            else
            {
                stack.GetComponent<Stack>().generateRandomStack(false);
            }
            stacklist.Add(stack);
            stack.transform.SetParent(transform);
        }
        loweststackY = -9 * gapbetweenstacks;
    }
    private void Update()
    {
        if(player.isGameOver) return;
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
            float t = (Time.time - cameramovestarttime) / currentCameraMoveDuration;
            if(t < 1f)
            {
                t = Mathf.Clamp01(t);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                MainCamera.transform.position = Vector3.Lerp(cameraStartPos, cameraTargetpos, eased);
            }
            else
            {
                MainCamera.transform.position = cameraTargetpos;
                iscameraMoving = false;
                currentCameraMoveDuration = cameramoveDuration;
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
                GameManager.instance.addscore(10);
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
        newstack.GetComponent<Stack>().generateRandomStack(false);
        stacklist.Add(newstack);
        Debug.Log("New Stack Spawned at pos y : " + loweststackY);
        if (MainCamera)
        {
            // If camera already had a pending target, accumulate downwards to avoid drift
            if (iscameraMoving)
            {
                cameraTargetpos += Vector3.down * gapbetweenstacks;
                currentCameraMoveDuration = Mathf.Max(0.03f, cameramoveDuration * 0.35f);
            }
            else
            {
                cameraStartPos = MainCamera.transform.position;
                cameraTargetpos = cameraStartPos + Vector3.down * gapbetweenstacks;
                currentCameraMoveDuration = cameramoveDuration;
            }
            // Always restart interpolation from current camera position
            cameraStartPos = MainCamera.transform.position;
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
