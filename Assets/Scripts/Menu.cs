using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject levelSelectorPanel;
    public GameObject settingsPanel;
    public GameObject scorePanel;

    private void Start()
    {
        var audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
        audioSource.volume = SettingsUI.instance.GetVolume();
    }

    public void SetLevelSelectorPanel()
    {
        levelSelectorPanel.SetActive(true);
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }
}
