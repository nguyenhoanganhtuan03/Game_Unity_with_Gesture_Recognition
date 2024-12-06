using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int restEsquive;
    public static bool playerIsDied;
    public static bool playerReachedFinishLine;

    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject finishUI;

    // Start is called before the first frame update
    void Start()
    {
        restEsquive = 1;
        playerIsDied = false;
        playerReachedFinishLine = false;
        InputSystem.EnableDevice(Keyboard.current);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsDied)
        {
            StopGame();
            gameOverUI.SetActive(true);
        }
        else if (playerReachedFinishLine)
        {
            StopGame();
            finishUI.SetActive(true);
        }
    }

    private void StopGame()
    {
        Time.timeScale = 0;
        InputSystem.DisableDevice(Keyboard.current);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        restEsquive = 1;
        playerIsDied = false;
        playerReachedFinishLine = false;
        InputSystem.EnableDevice(Keyboard.current);
    }

    public void Retry(GameObject panelUI)
    {
        panelUI.SetActive(false);
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit(GameObject panelUI)
    {
        ///Revoir pour la réinitialisation de  la vie pour l'appliquer dans le script du joueur
        panelUI.SetActive(false);
        HighScoreHandler.instance.ResetTemporaryHighScore();
        Resume();
        SceneManager.LoadScene("Menu");
    }


}
