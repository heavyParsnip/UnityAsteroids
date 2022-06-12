using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    //FIELDS
    public Vector3 shipPosition;
    public Vector3 direction;
    public Vector3 appliedDirection;
    public Vector3 velocity;
    public Vector3 acceleration;
    public float accelRate;
    public float angleOfRotation;
    public float appliedAngle;
    public float maxVelocity;
    public bool alive;
    public int livesLeft;
    public float invulnTimer;
    float leftBound;
    float rightBound;
    float topBound;
    float bottomBound;

    // Start is called before the first frame update
    void Start()
    {
        //Zero all necessary fields with default values
        shipPosition = transform.position;
        direction = new Vector3(0, 1, 0);
        velocity = Vector3.zero;
        alive = true;
        livesLeft = 3;
        invulnTimer = 0;

        //Fetch viewport bounds
        leftBound = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        topBound = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
        bottomBound = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;
    }

    // Update is called once per frame
    void Update()
    {
        RotateShip();       //Rotate direction first
        Drive();            //Determine where to drive
        AccelerateShip();   //Update ship's acceleration
        WrapCheck();        //Check for wrapping!
        SetTransform();     //Update ship's position based on drive
        InvulnExpire();
    }

    /// <summary>
    /// Update the ship's location and rotation based on the changes that occur in Drive()
    /// </summary>
    void SetTransform()
    {
        //Update graphic rotation, then update world position
        transform.rotation = Quaternion.Euler(0, 0, angleOfRotation);
        transform.position = shipPosition;
    }

    /// <summary>
    /// Move the ship in response to player input and simulate physics.
    /// </summary>
    void Drive()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            appliedDirection = direction;
            appliedAngle = angleOfRotation;
        }

        acceleration = accelRate * appliedDirection;
        velocity += acceleration * Time.deltaTime;

        //Clamp at max velocity
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity);

        shipPosition += velocity;
    }

    /// <summary>
    /// Rotate the ship in response to player input.
    /// </summary>
    void RotateShip()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            angleOfRotation += 3; //Increase rotation angle of image
            direction = Quaternion.Euler(0, 0, 3) * direction; //Change direction of movement
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            angleOfRotation -= 3; //Decrease rotation angle of image
            direction = Quaternion.Euler(0, 0, -3) * direction; //Change direction of movement
        }
    }

    /// <summary>
    /// Accelerate and decelerate the ship in response to player input.
    /// </summary>
    void AccelerateShip()
    {
        if (Input.GetKey(KeyCode.UpArrow) && accelRate < 0.5f)
        {
            accelRate += 0.25f;
            if (accelRate > 0.5f)
            {
                accelRate = 0.5f;
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow) && accelRate > -0.5f)
        {
            accelRate -= 0.25f;
            if(accelRate < -0.5f)
            {
                accelRate = -0.5f;
            }
        }
        else if(Input.GetKey(KeyCode.UpArrow) == false && Input.GetKey(KeyCode.DownArrow) == false)
        {
            velocity *= 0.9f;
            accelRate *= 0.9f;
            if(velocity.sqrMagnitude < 0.00001f)
            {
                velocity = Vector3.zero;
                accelRate = 0;
            }
        }
    }

    /// <summary>
    /// Wrap the ship across the screen.
    /// </summary>
    void WrapCheck()
    {
        //Fetch the ship's position
        Vector3 wrapPosition = shipPosition;

        //If the ship has moved beyond the screen's view on an axis, send the ship to the opposite side
        if (wrapPosition.x < leftBound - 0.5f)
        {
            wrapPosition.x = rightBound + 0.5f;
        }
        if (wrapPosition.x > rightBound + 0.5f)
        {
            wrapPosition.x = leftBound - 0.5f;
        }
        if (wrapPosition.y > topBound + 0.5f)
        {
            wrapPosition.y = bottomBound - 0.5f;
        }
        if (wrapPosition.y < bottomBound - 0.5f)
        {
            wrapPosition.y = topBound + 0.5f;
        }

        shipPosition = wrapPosition;
    }

    /// <summary>
    /// Method for removing invulnerability after it has expired. Invulnerability lasts for three seconds after getting hit.
    /// </summary>
    void InvulnExpire()
    {
        if(gameObject.GetComponent<CircleCollider2D>().enabled == false)
        {
            invulnTimer += Time.deltaTime;
            if(invulnTimer > 3.0f)
            {
                invulnTimer -= 3.0f;
                gameObject.GetComponent<CircleCollider2D>().enabled = true;
                gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    /// <summary>
    /// Method called when the ship gets hit. Resolves with either losing a life or ending the game.
    /// </summary>
    public void Decimate()
    {
        if(livesLeft > 1)
        {
            //Subtract a life, move the player back to the starting position, turn on invulnerability, and provide a visual indicator for the invulnerability.
            livesLeft -= 1;
            transform.position = Vector3.zero;
            shipPosition = Vector3.zero;
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
        }
        else if(livesLeft == 1)
        {
            //Subtract a life and remove player control.
            livesLeft -= 1;
            alive = false;
            gameObject.SetActive(false);
            gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
        }
    }

    /// <summary>
    /// Reset ship values to their initial states in preparation for a game reset.
    /// </summary>
    public void Reset()
    {
        //Reset all ship values to their initial states
        transform.position = Vector3.zero;
        shipPosition = transform.position;
        direction = new Vector3(0, 1, 0);
        velocity = Vector3.zero;
        alive = true;
        livesLeft = 3;
        invulnTimer = 0;
    }
}
