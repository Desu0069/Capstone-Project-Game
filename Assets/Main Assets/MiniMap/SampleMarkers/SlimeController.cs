using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    #region Varibles
    public float moveSpeed;
    public float moveTime = 1f;
    public float waitTime = 3f;
    public float reloadTime = 2f;

    float moveCounter;
    float waitCounter;
    bool moving;
    bool reloading;
    public Rigidbody rb;
    Vector3 moveDirection;
    GameObject player;
    #endregion

    void Start()
    {
      //  rb = GetComponent<Rigidbody>();
        // waitCounter = waitTime;
        // moveCounter = moveTime;
        // player = GameObject.FindGameObjectWithTag("Player");
        waitCounter = Random.Range(waitTime * 0.75f, waitTime * 1.25f);
        moveCounter = Random.Range(moveTime * 0.75f, moveTime * 1.25f);
    }

    void Update()
    {
        if (moving)
        {
            moveCounter -= Time.deltaTime;
            rb.linearVelocity = moveDirection;
            if (moveCounter < 0f)
            {
                moving = false;
                // waitCounter = waitTime;
                waitCounter = Random.Range(waitTime * 0.75f, waitTime * 1.25f);
            }
        }
        else
        {
            waitCounter -= Time.deltaTime;
            rb.linearVelocity = Vector3.zero;
            if (waitCounter < 0f)
            {
                moving = true;
                // moveCounter = moveTime;
                moveCounter = Random.Range(moveTime * 0.75f, moveTime * 1.25f);
                moveDirection = new Vector3(Random.Range(-1f, 1f) * moveSpeed, Random.Range(-1f, 1f) * moveSpeed, 0f);
            }
        }

        if (reloading)
        {
            reloadTime -= Time.deltaTime;
            if (reloadTime < 0f)
            {
               // GameManager.LoadGame(player.transform);
                player.SetActive(true);
                reloading = false;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // if (other.gameObject.tag == "Player")
        // {
        //    other.gameObject.SetActive(false);
        //    reloading = true;
        //    player = other.gameObject;
        // }
    }
}
