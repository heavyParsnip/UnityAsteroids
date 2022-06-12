using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIHandler : MonoBehaviour
{
    public int score;
    public Ship ship;
    public BulletHandler bulletHandler;
    public AsteroidHandler asteroidHandler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Draw the GUI.
    /// </summary>
    void OnGUI()
    {
        //Create two new font styles
        GUIStyle newLabel = new GUIStyle(GUI.skin.GetStyle("label"));
        newLabel.fontSize = 18;
        newLabel.normal.textColor = Color.white;

        GUIStyle gameOver = new GUIStyle(GUI.skin.GetStyle("label"));
        gameOver.fontSize = 40;
        gameOver.normal.textColor = Color.white;

        //Draw the score and remaining lives
        GUI.Label(new Rect(10, 10, 200, 100), $"SCORE: {score} \nLIVES: {ship.livesLeft}", newLabel);

        //Show the Game Over text when the player dies.
        if(ship.livesLeft == 0)
        {
            GUI.Label(new Rect(Screen.width/2 - Screen.width/8, Screen.height/3, 200, 200), "GAME OVER", gameOver);
        }
    }

    //Reset the game to its initial state.
    void Reset()
    {
        //Reinitialize values and call all reset methods
        score = 0;

        ship.Reset();
        asteroidHandler.Reset();
        bulletHandler.Reset();
    }
}
