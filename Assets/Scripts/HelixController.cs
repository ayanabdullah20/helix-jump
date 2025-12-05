using UnityEngine;

public class HelixController : MonoBehaviour
{
    public float rotationSpeed = 100f; // Speed of rotation
    public Vector2 startpos;
    public bool isSwiping;

    private void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                startpos = touch.position;
                isSwiping = true;
            }
            else if(touch.phase == TouchPhase.Moved && isSwiping)
            {
                RotateHelix(touch.position.x - startpos.x);
                startpos = touch.position;
            }
            else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isSwiping = false;
            }
        }
        else if(Input.GetMouseButtonDown(0))
        {
            startpos = Input.mousePosition;
            isSwiping = true;
        }
        else if(Input.GetMouseButton(0) && isSwiping)
        {
            RotateHelix(Input.mousePosition.x - startpos.x);
            startpos = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            isSwiping = false;
        }
    }
    void RotateHelix(float swipedistance)
    {
        if(Mathf.Abs(swipedistance) > 20f)
        {
            float rotationAmount = (swipedistance > 0  ? -1f:1f )* rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, rotationAmount, 0f);
        }
    }
}
