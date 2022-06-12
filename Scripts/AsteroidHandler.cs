using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidHandler : MonoBehaviour
{
    //FIELDS
    public static GameObject aPrefab1;
    public static GameObject aPrefab2;
    public static GameObject aPrefab3;
    public static GameObject aPrefab4;
    public Ship ship;
    public BulletHandler bulletHandler;
    public GUIHandler guiHandler;
    List<GameObject> asteroids;
    float astroTimer;
    float leftBound;
    float rightBound;
    float topBound;
    float bottomBound;

    //FIELDS FOR DEBUGGING
    //public float distanceBetween;
    //public float totalRadius;

    /// <summary>
    /// Built-in MonoBehaviour method called before Start(). Prefabs must be loaded from here in order to instantiate them with parameters.
    /// </summary>
    void Awake()
    {
        //Load up the asteroid prefabs
        aPrefab1 = Resources.Load("Asteroid1") as GameObject;
        aPrefab2 = Resources.Load("Asteroid2") as GameObject;
        aPrefab3 = Resources.Load("Asteroid3") as GameObject;
        aPrefab4 = Resources.Load("Asteroid4") as GameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Initialize asteroid list and zero necessary variables
        asteroids = new List<GameObject>();
        astroTimer = 0.0f;

        //Fetch viewport bounds
        leftBound = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        topBound = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
        bottomBound = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;

        //Start the game with three asteroids in play
        for(int i = 0; i < 3; i++)
        {
            GameObject newAsteroid = Asteroid.Initialize(ChooseAsteroidPrefab(), gameObject);
            asteroids.Add(newAsteroid);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ManageAsteroids(); //Generate asteroids
        WrapCheck(); //Check for wrapping
        ShipCollisionCheck(); //Check for collisions with ship
        BulletCollisionCheck(); //Check for collisions with bullets
        //AsteroidCollisionCheck(); //Check for collisions with other asteroids
    }

    /// <summary>
    /// Whenever less than 5 asteroids exist, generate a new asteroid. Uses a timer so that it only performs this check every 4 seconds.
    /// </summary>
    void ManageAsteroids()
    {
        //Add to the asteroid timer 
        astroTimer += Time.deltaTime;

        //After 4 seconds have passed...
        if (astroTimer > 4.0f)
        {
            //Reset the timer (subtracting the timer threshold is more accurate over time than setting it to 0)
            astroTimer -= 4.0f;

            //Check how many asteroids are present
            if(asteroids.Count < 5)
            {
                //Initialize a new asteroid and add it to the list
                GameObject newAsteroid = Asteroid.Initialize(ChooseAsteroidPrefab(), gameObject);
                asteroids.Add(newAsteroid);
            }
        }
    }

    /// <summary>
    /// Wraps asteroids to the other side of the screen after they move offscreen.
    /// </summary>
    void WrapCheck()
    {
        foreach (GameObject a in asteroids)
        {
            //Fetch the current asteroid's position
            Vector3 wrapPosition = a.GetComponent<Asteroid>().asteroidPosition;

            //If the asteroid has moved beyond the screen's view on an axis, wrap the asteroid to the opposite side
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

            a.GetComponent<Asteroid>().asteroidPosition = wrapPosition;
        }
    }

    /// <summary>
    /// Method for choosing a random asteroid prefab.
    /// </summary>
    /// <returns></returns>
    static GameObject ChooseAsteroidPrefab()
    {
        int prefabNum = Random.Range(1, 5);
        switch (prefabNum)
        {
            case 1:
                return aPrefab1;
            case 2:
                return aPrefab2;
            case 3:
                return aPrefab3;
            case 4:
                return aPrefab4;
            default:
                return aPrefab1;
        }
    }

    /// <summary>
    /// Split an asteroid into two smaller asteroids.
    /// </summary>
    /// <param name="bigAsteroid">Script component of the asteroid that is hit</param>
    void astroSplit(Asteroid bigAsteroid)
    {
        //Use a specialized Initialize method to generate two smaller asteroids in a list, then append that list to the main list of asteroids
        List<GameObject> astroFrags = Asteroid.SmallInitialize(ChooseAsteroidPrefab(), ChooseAsteroidPrefab(), bigAsteroid, gameObject);
        asteroids.AddRange(astroFrags);
    }

    /// <summary>
    /// Method for handling collisions between ASTEROIDS and SHIPS.
    /// </summary>
    void ShipCollisionCheck()
    {
        //Draw debug lines between the edges of each asteroid's collider and the ship's collider
        foreach (GameObject a in asteroids)
        {
            Debug.DrawLine(ship.GetComponent<CircleCollider2D>().ClosestPoint(new Vector2(a.GetComponent<CircleCollider2D>().transform.position.x, a.GetComponent<CircleCollider2D>().transform.position.y)), a.GetComponent<CircleCollider2D>().ClosestPoint(new Vector2(ship.shipPosition.x, ship.shipPosition.y)), Color.magenta);
        }

        //Fetch the ship's collider
        CircleCollider2D shipCollider = ship.GetComponent<CircleCollider2D>();

        //Cycle through all asteroids present
        foreach(GameObject a in asteroids)
        {
            //Fetch the current asteroid's script and collider
            Asteroid currentAsteroid = a.GetComponent<Asteroid>();
            CircleCollider2D asteroidCollider = a.GetComponent<CircleCollider2D>();

            //Compare the distance between the ship and the current asteroid to the radii of their colliders.
            //Uses the Distance Formula.
                /*NOTE: For unclear reasons, collisions occur from much farther away than intended when (Distance)^2 is compared to (Radius1+Radius2)^2, 
                 * which should be the correct evaluation. After some debugging, I tuned the second condition to (Radius1+Radius2)/16, which I found to
                 * be acceptably accurate. My guess is that there might be a discrepancy between the scales of the Distance and Radius values. */
            if((Mathf.Pow(ship.transform.position.x - a.transform.position.x, 2) + Mathf.Pow(ship.transform.position.y - a.transform.position.y, 2) < (shipCollider.radius + asteroidCollider.radius)/16) && (ship.GetComponent<CircleCollider2D>().enabled == true))
            {
                //Hurt the ship if they are colliding
                ship.Decimate();
            }
        }
    }

    /// <summary>
    /// Method for handling collisions between ASTEROIDS and BULLETS. Can only perform one removal per frame, which is more than enough.
    /// </summary>
    void BulletCollisionCheck()
    {
        //Initialize control variables
        GameObject bulletToRemove = null;
        GameObject asteroidToRemove = null;

        //Cycle through all bullets present
        foreach (GameObject b in bulletHandler.bullets)
        {
            //Fetch the current bullet's script and collider
            Bullet currentBullet = b.GetComponent<Bullet>();
            CircleCollider2D bulletCollider = b.GetComponent<CircleCollider2D>();

            //Cycle through all asteroids present
            foreach (GameObject a in asteroids)
            {
                //Fetch the current asteroid's script and collider
                Asteroid currentAsteroid = a.GetComponent<Asteroid>();
                CircleCollider2D asteroidCollider = a.GetComponent<CircleCollider2D>();

                //Compare the distance between the bullet's position and the asteroid's position to the sum radii of their colliders.
                //Uses the Distance Formula.
                    /*NOTE: For unclear reasons, collisions occur from much farther away than intended when (Distance)^2 is compared to (Radius1+Radius2)^2, 
                     * which should be the correct evaluation. After some debugging, I tuned the second condition to (Radius1+Radius2)/16, which I found to
                     * be acceptably accurate. My guess is that there might be a discrepancy between the scales of the Distance and Radius values. */
                if (Mathf.Pow(b.transform.position.x - a.transform.position.x, 2) + Mathf.Pow(b.transform.position.y - a.transform.position.y, 2) < (bulletCollider.radius + asteroidCollider.radius)/16)
                {
                    //Assign debug values; used for fine-tuning collisions
                    //distanceBetween = Mathf.Pow(b.transform.position.x - a.transform.position.x, 2) + Mathf.Pow(b.transform.position.y - a.transform.position.y, 2);
                    //totalRadius = (bulletCollider.radius + asteroidCollider.radius)/16;

                    //If a bullet and an asteroid are colliding, prepare them for removal. This has to be done outside the loop due to the nature of foreach
                    asteroidToRemove = a;
                    bulletToRemove = b;
                }
            }
        }

        //If a bullet and an asteroid are found to be colliding...
        if(bulletToRemove != null && asteroidToRemove != null)
        {
            //First check if the asteroid is a big (first-stage) asteroid
            if (asteroidToRemove.GetComponent<Asteroid>().bigAsteroid == true)
            {
                //If so, split it into two small (second-stage) asteroids
                astroSplit(asteroidToRemove.GetComponent<Asteroid>());
            }

            //Increment the player's score according to the asteroid size
            if (asteroidToRemove.GetComponent<Asteroid>().bigAsteroid == true)
            {
                guiHandler.score += 20;
            }
            else if (asteroidToRemove.GetComponent<Asteroid>().bigAsteroid == false)
            {
                guiHandler.score += 50;
            }

            //Remove the bullet and asteroid from their respective lists, then destroy their GameObjects
            bulletHandler.bullets.Remove(bulletToRemove);
            asteroids.Remove(asteroidToRemove);
            Destroy(bulletToRemove);
            Destroy(asteroidToRemove);
        }


    }

    /// <summary>
    /// Method for handling collisions between ASTEROIDS and ASTEROIDS. When two asteroids collide, resolve an elastic collision.
    /// Tried very hard to get this working, but was unsuccessful.
    /// </summary>
    void AsteroidCollisionCheck()
    {
        foreach(GameObject a1 in asteroids)
        {
            Asteroid firstAsteroid = a1.GetComponent<Asteroid>();
            CircleCollider2D firstCollider = a1.GetComponent<CircleCollider2D>();

            foreach(GameObject a2 in asteroids)
            {
                Asteroid secondAsteroid = a2.GetComponent<Asteroid>();
                CircleCollider2D secondCollider = a2.GetComponent<CircleCollider2D>();

                if(Vector3.Distance(firstAsteroid.asteroidPosition, secondAsteroid.asteroidPosition) != 0)
                {
                    //Compare the distance between the asteroids' positions to the sum radii of their colliders.
                    //Uses the Distance Formula.
                    /*NOTE: For unclear reasons, collisions occur from much farther away than intended when (Distance)^2 is compared to (Radius1+Radius2)^2, 
                     * which should be the correct evaluation. After some debugging, I tuned the second condition to (Radius1+Radius2)/16, which I found to
                     * be acceptably accurate. My guess is that there might be a discrepancy between the scales of the Distance and Radius values. */
                    if (Mathf.Pow(a1.transform.position.x - a2.transform.position.x, 2) + Mathf.Pow(a1.transform.position.y - a2.transform.position.y, 2) < (firstCollider.radius + secondCollider.radius) / 4)
                    {
                        //Resolve an Elastic Collision. Hold on tight.
                        //Set up reference values to slightly improve equation readability
                        Vector2 center1 = new Vector2(a1.transform.position.x, a1.transform.position.y);
                        Vector2 center2 = new Vector2(a2.transform.position.x, a2.transform.position.y);
                        Vector2 centerDifference1 = center1 - center2;
                        Vector2 centerDifference2 = center2 - center1;
                        Vector2 velocity1 = new Vector2(firstAsteroid.velocity.x, firstAsteroid.velocity.y);
                        Vector2 velocity2 = new Vector2(secondAsteroid.velocity.x, secondAsteroid.velocity.y);
                        Vector2 velocityDifference1 = velocity1 - velocity2;
                        Vector2 velocityDifference2 = velocity2 - velocity1;
                        float mass1 = firstAsteroid.mass;
                        float mass2 = secondAsteroid.mass;
                        float combinedMass = mass1 + mass2;

                        //Elastic collision in 2D. Vector math only; no trig needed.
                        //Equation is v'1 = v1 - 2*m2/m1+m2 * [v1-v2]·[c1-c2]/||c1-c2||^2 * (c1-c2)
                        //Where v = velocity of an asteroid, m = mass of an asteroid, and c = center of an asteroid
                        Vector2 velocityPrime1 =
                            velocity1 - ((2 * mass2) / (combinedMass)) *
                            (Vector2.Dot(velocityDifference1, centerDifference1) /
                            Mathf.Pow(Mathf.Sqrt(Mathf.Pow(centerDifference1.x, 2) + Mathf.Pow(centerDifference1.y, 2)), 2)) *
                            centerDifference1;
                        Vector2 velocityPrime2 =
                            velocity2 - ((2 * mass1) / (combinedMass)) *
                            (Vector2.Dot(velocityDifference2, centerDifference2) /
                            Mathf.Pow(Mathf.Sqrt(Mathf.Pow(centerDifference2.x, 2) + Mathf.Pow(centerDifference2.y, 2)), 2)) *
                            centerDifference2;

                        firstAsteroid.velocity = velocityPrime1;
                        secondAsteroid.velocity = velocityPrime2;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Reset asteroid values to their initial states in preparation for a game reset.
    /// </summary>
    public void Reset()
    {
        //Empty out the asteroid list and clear all references
        foreach(GameObject a in asteroids)
        {
            Destroy(a);
        }

        asteroids.Clear();

        //Reset the timer
        astroTimer = 0.0f;

        //Generate a new trio of asteroids
        for (int i = 0; i < 3; i++)
        {
            GameObject newAsteroid = Asteroid.Initialize(ChooseAsteroidPrefab(), gameObject);
            asteroids.Add(newAsteroid);
        }
    }
}
