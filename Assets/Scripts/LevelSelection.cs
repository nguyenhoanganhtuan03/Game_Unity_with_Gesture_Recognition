using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    public static LevelSelector currentLevel;

    private void Start()
    {

    }
    public void SetEasyMode()
    {
        currentLevel = LevelSelector.Easy;
        gameObject.SetActive(false);
        SceneManager.LoadScene("Easy");
    }

    public void SetMediumMode()
    {
        currentLevel = LevelSelector.Medium;
        gameObject.SetActive(false);
        SceneManager.LoadScene("Medium");
    }

    public void SetHardMode()
    {
        currentLevel = LevelSelector.Hard;
        gameObject.SetActive(false);
        SceneManager.LoadScene("Hard");
    }

    public void SetInfiniteMode()
    {
        currentLevel = LevelSelector.Infinite;
        gameObject.SetActive(false);
        SceneManager.LoadScene("Infinite");
    }

    public void ReturnBack()
    {
        gameObject.SetActive(false);
    }
}

public enum LevelSelector
{
    Easy,
    Medium,
    Hard,
    Infinite
}
