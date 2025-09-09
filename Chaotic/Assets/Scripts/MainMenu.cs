using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject controllerScreen;
    public void StartGame()
    {
        controllerScreen.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ClickStart()
    {
        SceneManager.LoadScene("GroundFloor");
    }
     
}
