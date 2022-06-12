using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHandler : MonoBehaviour
{
    //FIELDS
    public static GameObject bPrefab;
    public Ship ship;
    public List<GameObject> bullets;
    float leftBound;
    float rightBound;
    float topBound;
    float bottomBound;

    /// <summary>
    /// Built-in MonoBehaviour method. Prefab must be loaded from here.
    /// </summary>
    void Awake()
    {
        bPrefab = Resources.Load("Bullet") as GameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Initialize bullet list
        bullets = new List<GameObject>();

        //Fetch viewport bounds
        leftBound = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        topBound = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
        bottomBound = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;
    }

    // Update is called once per frame
    void Update()
    {
        Fire();
        WrapCheck();
        SelfDestruct();
    }

    /// <summary>
    /// Fire a new bullet from the ship.
    /// </summary>
    void Fire()
    {
        //Create a new bullet when the space button is pressed, as long as the player is alive and there are less than 3 bullets already in the scene
        if (Input.GetKeyDown(KeyCode.Space) == true && bullets.Count < 3 && ship.alive == true)
        {
            GameObject newBullet = Bullet.Initialize(bPrefab, ship, gameObject);
            bullets.Add(newBullet);
        }
    }

    /// <summary>
    /// Method for wrapping bullets across the screen.
    /// </summary>
    void WrapCheck()
    {
        foreach(GameObject b in bullets)
        {
            //Fetch the current bullet's position
            Vector3 wrapPosition = b.GetComponent<Bullet>().bulletPosition;

            //If the bullet has moved beyond the screen's view on an axis, wrap the bullet to the opposite side
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

            b.GetComponent<Bullet>().bulletPosition = wrapPosition;
        }
    }

    /// <summary>
    /// Method for checking and removing bullets that have traveled a sufficient distance.
    /// Only one bullet can be removed per frame, but this is not an issue as more than one bullet cannot be fired in a single frame.
    /// </summary>
    void SelfDestruct()
    {
        GameObject bulletToRemove = null;
        for(int i = 0; i < bullets.Count; i++)
        {
            if (bullets[i].GetComponent<Bullet>().displacement > 0.3f)
            {
                bulletToRemove = bullets[i];
                break;
            }
        }

        if(bulletToRemove != null)
        {
            bullets.Remove(bulletToRemove);
            Destroy(bulletToRemove);
        }
    }

    /// <summary>
    /// Destroy all bullets in preparation for a game reset.
    /// </summary>
    public void Reset()
    {
        //Empty out the bullet list and clear all references
        foreach (GameObject b in bullets)
        {
            Destroy(b);
        }

        bullets.Clear();
    }
}
