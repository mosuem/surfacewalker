using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static float speed = 1f;
    public Text countText;
    public Text winText;
    public Vector3 direction;
    public GameObject bullet;
    public Transform bulletTransform;
    private Rigidbody rb;
    private int count;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        ShowCount();
        winText.text = "";
        bulletTransform = this.transform;
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Fire();
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            speed *= 2f;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            speed *= 0.5f;
        }
    }

    public bool isMoved = true;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMoved && Wrapper.isEuclidean)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            // Debug.DrawRay (transform.position, direction, Color.red);
            rb.AddForce((moveVertical * direction + moveHorizontal * Vector3.Cross(direction, Vector3.down)) * speed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            Duplicator.tileChanged = other.gameObject;
            ++count;
            ShowCount();
            if (count == 10)
            {
                //				winText.text = "You Win";
            }
        }
    }

    void ShowCount()
    {
        string bla = "Count: ";
        countText.text = bla + count.ToString();
    }

    void Fire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet1 = (GameObject)Instantiate(bullet, bulletTransform.position + direction * 2f, bulletTransform.rotation);
        bullet1.name = char.ToString(this.gameObject.name[this.gameObject.name.Length - 1]);
        if (bullet1.name == "r")
        {
            bullet1.name = "8";
        }
        // Add velocity to the bullet
        bullet1.GetComponent<Rigidbody>().velocity = direction * 6;

        // Destroy the bullet after 2 seconds
        //		Destroy (bullet1, 2.0f);
    }

}

