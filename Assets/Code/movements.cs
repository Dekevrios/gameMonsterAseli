using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class movements : MonoBehaviour
{
    public float speed = 5f;
    Rigidbody2D rb;
    float moveHorizontal;
    float moveVertical;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveVertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        rb.velocity = movement * speed;

        
    }
}
