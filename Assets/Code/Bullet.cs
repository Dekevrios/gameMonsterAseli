using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletLife = 1f;
    public float bulletSpeed = 1f;
    public float rotation = 0f;
    public int damage = 10;
    public string targetTag = "Player";

    private Vector2 spawnPoint;
    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = new Vector2(transform.position.x, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > bulletLife) Destroy(this.gameObject);
        timer += Time.deltaTime;
        transform.position = Movement(timer);
    }

    private Vector2 Movement(float timer)
    {
        float x = timer * bulletSpeed * transform.right.x;
        float y = timer * bulletSpeed * transform.right.y;

        return new Vector2(x + spawnPoint.x, y + spawnPoint.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            Debug.Log("Hit " + collision.gameObject.name);
            charStats stats = collision.gameObject.GetComponent<charStats>();

            if (stats != null)
            {
                stats.TakeDamage(damage);
            }
            else
            {
                Debug.Log("No charStats component found on " + collision.gameObject.name);
            }


            Destroy(this.gameObject);
        }
    }
}
