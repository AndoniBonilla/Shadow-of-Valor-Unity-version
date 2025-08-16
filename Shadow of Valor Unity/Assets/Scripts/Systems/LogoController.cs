using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoController : MonoBehaviour
{
    [SerializeField] private float holdSeconds = 2.0f; // How long to show the logo.

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= holdSeconds)
        {
            goToCharacterSelect();
        }
    }

    private void goToCharacterSelect()
    {
        SceneManager.LoadScene("CharacterSelect"); // must match the scene name exactly
    }
}
