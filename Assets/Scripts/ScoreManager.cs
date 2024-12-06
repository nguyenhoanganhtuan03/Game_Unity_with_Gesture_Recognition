using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public bool highScoreSaved = false;

    [SerializeField] private TextMeshProUGUI scoreTxt;
    [SerializeField] private TextMeshProUGUI highScoreTxt;

    float score = 0;
    int pointIncreasedPerSecond = 10;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreTxt.text = score.ToString();
        //highScoreTxt.text = highScore.ToString();
    }

    private void Update()
    {
        if (Time.timeScale != 0)
        {
            AddPoints(pointIncreasedPerSecond * Time.fixedDeltaTime);
        }
        else
        {
            if ((GameManager.playerIsDied || GameManager.playerReachedFinishLine) && !highScoreSaved)
            {
                var highScore = HighScoreHandler.instance.GetTemporaryHighScore();
                highScoreTxt.text = highScore.ToString();
                if (score > highScore)
                {
                    highScore = (int)score;
                    HighScoreElement highScoreElement = new HighScoreElement
                    {
                        // TODO : Faire une comparaison pour voir si le pseudo est renseigné
                        playerName = SettingsUI.instance.GetPseudo(),
                        score = highScore,
                        levelSelection = LevelSelection.currentLevel

                    };
                    HighScoreHandler.instance.SaveHighScore(highScoreElement);
                    highScoreSaved = true;

                }
            }
        }
    }

    public void AddPoints(float pointsNumber)
    {
        score += pointsNumber;
        scoreTxt.text = ((int)score).ToString();
    }
}
