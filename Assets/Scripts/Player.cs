
using NUnit.Framework;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float bounceheight=2f;
    private Rigidbody rb;
    public Vector3 initialposition;
    private bool isGameOver = false;
    private bool isMovingDown = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialposition = transform.position;
        rb.useGravity = false;
    }
    void Update()
    {
        if(isGameOver) return;
        if (isMovingDown)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime); 
        }
        else if(transform.position.y < initialposition.y + bounceheight)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime); 
        }
        else
        {
            isMovingDown    = true;
            initialposition = transform.position;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if(isGameOver) return;
        if(collision.gameObject.CompareTag("Safe"))
        {
            Debug.Log("Safe Slice Hit");
            isMovingDown = false;
            initialposition = transform.position;
        }
        if(collision.gameObject.CompareTag("Danger"))
        {
            Debug.Log("Danger Slice Hit Game Over");
            isGameOver = true;
            rb.velocity = Vector3.zero;
        }

    }

}
