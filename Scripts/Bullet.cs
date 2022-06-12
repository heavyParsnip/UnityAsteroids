using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //FIELDS
    public Vector3 bulletOrigin;
    public Vector3 bulletPosition;
    public Vector3 direction;
    public Vector3 velocity;
    public Vector3 acceleration;
    public float displacement;

    // Start is called before the first frame update
    void Start()
    {
        displacement = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Fired();
    }

    /// <summary>
    /// Handles the movement of a bullet; a blend of Ship's Drive() and SetTransform(). Also tracks displacement.
    /// </summary>
    void Fired()
    {
        acceleration = 1f * direction;
        velocity += acceleration * Time.deltaTime;

        //Clamp at max velocity
        velocity = Vector3.ClampMagnitude(velocity, 0.2f);

        bulletPosition += velocity;
        displacement += velocity.magnitude * Time.deltaTime;

        //Update world position
        transform.position = bulletPosition;
    }

    /// <summary>
    /// Instantiate a new bullet with parameters.
    /// </summary>
    /// <param name="bulletPrefab">Prefab to instantiate the bullet from</param>
    /// <param name="playerShip">The ship</param>
    /// <param name="newParent">Parent of the new bullet</param>
    /// <returns></returns>
    public static GameObject Initialize(Object bulletPrefab, Ship playerShip, GameObject newParent)
    {
        GameObject bullet = Instantiate(bulletPrefab, playerShip.shipPosition + Vector3.Scale(new Vector3(0.5f, 0.5f, 0), playerShip.direction), Quaternion.Euler(0, 0, playerShip.angleOfRotation), newParent.transform) as GameObject;
        bullet.GetComponent<Bullet>().bulletOrigin = bullet.transform.position;
        bullet.GetComponent<Bullet>().bulletPosition = bullet.transform.position;
        bullet.GetComponent<Bullet>().direction = playerShip.direction;

        return bullet;
    }
}
