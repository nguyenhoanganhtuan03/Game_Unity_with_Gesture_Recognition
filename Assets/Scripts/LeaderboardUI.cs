using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] GameObject highScoreUIElementPrefab;
    [SerializeField] Transform elementWrapper;

    List<GameObject> uiElements = new List<GameObject>();

    private void OnEnable()
    {
        HighScoreHandler.onHighScoreListChanged += UpdateUI;
    }

    private void OnDisable()
    {
        HighScoreHandler.onHighScoreListChanged -= UpdateUI;
    }

    public void ShowPanel()
    {
        panel.SetActive(true);
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
    }

    public void UpdateUI(HighScoreElements highScoreElements)
    {
        for (int i = 0; i < highScoreElements.highScoresList.Count; i++)
        {
            HighScoreElement el = highScoreElements.highScoresList[i];
            if (el.score > 0)
            {
                if (i >= uiElements.Count)
                {
                    var cloneHighScoreElement = Instantiate(highScoreUIElementPrefab, Vector3.zero, Quaternion.identity);
                    cloneHighScoreElement.transform.SetParent(elementWrapper);
                    uiElements.Add(cloneHighScoreElement);
                }
                var texts = uiElements[i].GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = el.playerName;
                texts[1].text = el.score.ToString();
                texts[2].text = el.levelSelection.ToString();

            }
        }
    }

}
