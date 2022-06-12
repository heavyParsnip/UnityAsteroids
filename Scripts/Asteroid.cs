using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    //FIELDS
    public Vector3 asteroidPosition;
    public Vector3 direction;
    public Vector3 velocity;
    public float angleOfRotation;
    public float mass;
    public bool bigAsteroid;
    public bool asteroidCollided;
    public bool shipCollided;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Float();
    }

    /// <summary>
    /// Move the asteroid based on its velocity.
    /// </summary>
    void Float()
    {
        //Clamp at max velocity just in case an asteroid is somehow going too fast
        velocity = Vector3.ClampMagnitude(velocity, 0.08f);

        asteroidPosition += velocity;

        //Update world position
        direction = Quaternion.Euler(0, 0, gameObject.transform.rotation.z) * direction;
        transform.position = asteroidPosition;
    }

    /// <summary>
    /// Static method for instantiating an asteroid with parameters.
    /// </summary>
    /// <param name="asteroidPrefab">Prefab to instantiate the asteroid from</param>
    /// <param name="newParent">Parent of the new asteroid</param>
    /// <returns></returns>
    public static GameObject Initialize(Object asteroidPrefab, GameObject newParent)
    {
        //Generate a random position on screen
        float randX = Random.Range(0.0f, 0.8f);
        float randY = Random.Range(0.0f, 0.8f);

        //Use a logical range to keep asteroids from spawning too close to the center of the screen
        if (randX > 0.4f)
        {
            randX += 0.2f;
        }
        if (randY > 0.4f)
        {
            randY += 0.2f;
        }
        Vector3 newViewportPos = new Vector3(randX, randY, 0);

        //Instantiate a new asteroid and initialize its values
        GameObject asteroid = Instantiate(asteroidPrefab, Camera.main.ViewportToWorldPoint(newViewportPos), Quaternion.Euler(0, 0, Random.Range(-180, 180)), newParent.transform) as GameObject;

        asteroid.GetComponent<Asteroid>().asteroidPosition = new Vector3(asteroid.transform.position.x, asteroid.transform.position.y, 0);
        asteroid.GetComponent<Asteroid>().direction = Quaternion.Euler(0,0,asteroid.transform.rotation.z) * new Vector3(0, 1, 0);
        asteroid.GetComponent<Asteroid>().velocity = new Vector3(Random.Range(-0.07f, 0.07f), Random.Range(-0.07f, 0.07f));
        asteroid.GetComponent<Asteroid>().angleOfRotation = asteroid.transform.rotation.eulerAngles.z;
        asteroid.GetComponent<Asteroid>().bigAsteroid = true;
        asteroid.GetComponent<Asteroid>().mass = 1f;

        return asteroid;
    }

    /// <summary>
    /// Instantiate two asteroids for when a big asteroid is hit.
    /// </summary>
    /// <param name="asteroidPrefab1">First prefab</param>
    /// <param name="asteroidPrefab2">Second prefab</param>
    /// <param name="parentAsteroid">Script of the asteroid that was hit</param>
    /// <param name="newParent">Parent of the new asteroids</param>
    /// <returns></returns>
    public static List<GameObject> SmallInitialize(Object asteroidPrefab1, Object asteroidPrefab2, Asteroid parentAsteroid, GameObject newParent)
    {
        List<GameObject> asteroidPair = new List<GameObject>();
        GameObject frag1 = Instantiate(asteroidPrefab1, parentAsteroid.asteroidPosition + Vector3.Scale(new Vector3(0.55f, 0.55f, 0), parentAsteroid.direction), Quaternion.Euler(0, 0, Random.Range(-180,180)), newParent.transform) as GameObject;
        GameObject frag2 = Instantiate(asteroidPrefab2, parentAsteroid.asteroidPosition + Vector3.Scale(new Vector3(-0.55f, -0.55f, 0), parentAsteroid.direction), Quaternion.Euler(0, 0, Random.Range(-180,180)), newParent.transform) as GameObject;

        frag1.transform.localScale *= 0.5f;
        frag1.GetComponent<Asteroid>().asteroidPosition = new Vector3(frag1.transform.position.x, frag1.transform.position.y, 0);
        frag1.GetComponent<Asteroid>().direction = Quaternion.Euler(0, 0, frag1.transform.rotation.z) * new Vector3(0, 1, 0);
        frag1.GetComponent<Asteroid>().velocity = new Vector3(AsteroidGaussian(0f, 1.2f), AsteroidGaussian(0f, 1.2f));
        frag1.GetComponent<Asteroid>().angleOfRotation = frag1.transform.rotation.eulerAngles.z;
        frag1.GetComponent<Asteroid>().bigAsteroid = false;
        frag1.GetComponent<Asteroid>().mass = 0.5f;

        frag2.transform.localScale *= 0.5f;
        frag2.GetComponent<Asteroid>().asteroidPosition = new Vector3(frag2.transform.position.x, frag2.transform.position.y, 0);
        frag2.GetComponent<Asteroid>().direction = Quaternion.Euler(0, 0, frag2.transform.rotation.z) * new Vector3(0, 1, 0);
        frag2.GetComponent<Asteroid>().velocity = new Vector3(AsteroidGaussian(0f, 1.2f), AsteroidGaussian(0f, 1.2f));
        frag2.GetComponent<Asteroid>().angleOfRotation = frag2.transform.rotation.eulerAngles.z;
        frag2.GetComponent<Asteroid>().bigAsteroid = false;
        frag2.GetComponent<Asteroid>().mass = 0.5f;

        asteroidPair.Add(frag1);
        asteroidPair.Add(frag2);

        return asteroidPair;
    }

    /// <summary>
    /// Gaussian distribution method provided by slides.
    /// </summary>
    /// <param name="mean">Average value. Values are focused around this value.</param>
    /// <param name="stdDev">Standard deviation. Higher values create wider distributions.</param>
    /// <returns></returns>
    public static float AsteroidGaussian(float mean, float stdDev) 
    { 
        float val1 = Random.Range(0f, 1f); 
        float val2 = Random.Range(0f, 1f); 
        float gaussValue = Mathf.Sqrt(-2.0f * Mathf.Log(val1)) * Mathf.Sin(2.0f * Mathf.PI * val2); 
        return mean + stdDev * gaussValue; 
    }

    /// <summary>
    /// Special ClampMagnitude method that takes a minimum and maximum magnitude.
    /// </summary>
    /// <param name="vector">Vector to clamp</param>
    /// <param name="min">Minimum magnitude</param>
    /// <param name="max">Maximum magnitude</param>
    /// <returns></returns>
    static Vector3 ClampMagnitude(Vector3 vector, float min, float max)
    {
        double squareMag = vector.sqrMagnitude;
        if (squareMag > max * max)
        {
            return vector.normalized * max;
        }
        else if (squareMag < min * min)
        {
            return vector.normalized * min;
        }
        return vector;
    }
}