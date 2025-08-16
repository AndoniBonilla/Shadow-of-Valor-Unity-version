using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public Character player1;
    public Character player2;

    public void OnKO(Character loser)
    {
        // TEMP: log the KO so Character.cs compiles and runs
        Character winner = (loser == player1) ? player2 : player1;
        Debug.Log($"KO! Winner: {(winner != null ? winner.name : "Unknown")}");
        // Later: add round timer, wins, reset, etc.
    }
}
