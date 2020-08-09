using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {

        public bool isGameOver;
        void Update()
        {
            if (PlayerStats.Lives <= 0)
            {
                if (!isGameOver)
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    EndGame();
            }
       
        }

        void EndGame()
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            Debug.Log("Game Over");
            isGameOver = true;
        }
    }
}
