using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StarNewGame()
    {
        SceneManager.LoadScene("SelectChar");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
