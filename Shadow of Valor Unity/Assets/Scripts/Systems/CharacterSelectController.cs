using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectController : MonoBehaviour
{
    [Header("Character Roster")]
    public GameObject[] characterPrefabs; // Assign in Inspector

    private int p1Index = 0;
    private int p2Index = 0;

    private bool p1Ready = false;
    private bool p2Ready = false;

    private void Update()
    {
        handlePlayer1Input();
        handlePlayer2Input();

        if (p1Ready && p2Ready)
        {
            loadFightScene();
        }
    }

    private void handlePlayer1Input()
    {
        if (p1Ready) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            p1Index = (p1Index - 1 + characterPrefabs.Length) % characterPrefabs.Length;
            Debug.Log("P1 selected " + characterPrefabs[p1Index].name);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            p1Index = (p1Index + 1) % characterPrefabs.Length;
            Debug.Log("P1 selected " + characterPrefabs[p1Index].name);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            p1Ready = true;
            Debug.Log("P1 locked in " + characterPrefabs[p1Index].name);
        }
    }

    private void handlePlayer2Input()
    {
        if (p2Ready) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            p2Index = (p2Index - 1 + characterPrefabs.Length) % characterPrefabs.Length;
            Debug.Log("P2 selected " + characterPrefabs[p2Index].name);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            p2Index = (p2Index + 1) % characterPrefabs.Length;
            Debug.Log("P2 selected " + characterPrefabs[p2Index].name);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            p2Ready = true;
            Debug.Log("P2 locked in " + characterPrefabs[p2Index].name);
        }
    }

    private void loadFightScene()
    {
        Debug.Log("Both players ready! Loading Fight scene...");
        SceneManager.LoadScene("Fight"); // We'll create Fight.unity later
    }
}
