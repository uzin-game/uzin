using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;


public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    private InputAction PauseAction;
    public GameObject pauseMenuUI;
    public GameObject player;
    
    void Start()
    {
        pauseMenuUI.SetActive(false);
        PauseAction = InputSystem.actions.FindAction("Pause");
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseAction.triggered)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void QuitGame()
    {
        Application.Quit();
        //EditorApplication.isPlaying = false;
    }

    public void Respawn()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        player.transform.position = new Vector3(10, 10, 0);
    }
}
