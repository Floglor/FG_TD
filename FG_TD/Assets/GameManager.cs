using UnityEngine;

public class GameManager : MonoBehaviour
{

    public bool isGameOver;
    void Update()
    {
        if (PlayerStats.Lives <= 0)
        {
            if (!isGameOver)
            EndGame();
        }
       
    }

    void EndGame()
    {
        Debug.Log("Game Over");
        isGameOver = true;
    }
}
