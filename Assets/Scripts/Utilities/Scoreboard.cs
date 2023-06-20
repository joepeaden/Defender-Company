using UnityEngine;
using UnityEngine.Events; 

public class Scoreboard : MonoBehaviour
{
    public static UnityEvent<int> OnScoreUpdated = new UnityEvent<int>();

    public static int totalScore { get; private set; }

    private void Start()
    {
        Enemy.OnEnemyKilled.AddListener(AddPoints);
        GameManager.OnGameOver.AddListener(ResetPoints);
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyKilled.RemoveListener(AddPoints);
        GameManager.OnGameOver.RemoveListener(ResetPoints);
    }

    private static void AddPoints(int points)
    {
        totalScore += points;
        OnScoreUpdated.Invoke(totalScore);
    }

    /// <summary>
    /// Attempts to deduct the points, but if totalScore is less than 0, returns false and does not proceed.
    /// </summary>
    /// <param name="points"></param>
    /// <returns>False if totalPoints - points < 0, true otherwise.</returns>
    public static bool TryRemovePoints(int points)
    {
        if (totalScore - points >= 0)
        {
            totalScore -= points;
            OnScoreUpdated.Invoke(totalScore);
            return true;
        }

        return false;
    }

    private void ResetPoints()
    {
        totalScore = 0;
    }
}
