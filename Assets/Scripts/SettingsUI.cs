using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public static SettingsUI instance;

    [SerializeField] GameObject panel;
    [SerializeField] Slider volumeSlider;
    [SerializeField] TMP_InputField playerNameField;
    [SerializeField] InputActionAsset inputActionAsset;
    [SerializeField] TMP_Dropdown controllerDropdown;

    List<InputControlScheme> inputControlSchemes = new List<InputControlScheme>();

    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
        GetControlSchemes();
        audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
    }

    //------Pseudo------
    
    //Allow to set user pseudo in user preference
    public void SetPseudo(string playerName)
    {
        if (playerName != null)
        {
            PlayerPrefs.SetString("playerName", playerName);
        }
    }

    //Get stored parameter pseudo in user preference
    public string GetPseudo()
    {
        return PlayerPrefs.GetString("playerName", "Anonymous");
    }


    //Allow to get current user's pseudo in order to insert it in playerNameField
    public void GetCurrentPseudoInputText()
    {
        playerNameField.text = GetPseudo();
    }

    //------Volume------

    public float GetVolume()
    {
        return PlayerPrefs.GetFloat("volume", audioSource.volume);
    }

    public void SetVolume()
    {
        audioSource.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
    }

    public void GetCurrentVolumeSlider()
    {
        volumeSlider.value = GetVolume();
    }

    //------Controller------

    //Allow to get control scheme list like (ZQSD or Row left,right,up,down) which is stored in inputActionAsset
    public void GetControlSchemes()
    {
        inputControlSchemes = inputActionAsset.controlSchemes.ToList();
        foreach (var controlScheme in inputControlSchemes)
        {
            controllerDropdown.options.Add(new TMP_Dropdown.OptionData(controlScheme.name));
        }
    }

    //Allow to set control scheme in user store preference
    public void SetControlScheme(int indexControlScheme)
    {
        PlayerPrefs.SetString("currentControlScheme", inputControlSchemes[indexControlScheme].name);
    }

    //Get stored parameter currentControlScheme
    public string GetCurrentControlScheme()
    {
        return PlayerPrefs.GetString("currentControlScheme");
    }

    //Get parameter choosen by user
    public void GetCurrentControlSchemeDropdown()
    {
        controllerDropdown.value = inputControlSchemes[0].name == GetCurrentControlScheme() ? 0 : 1;
    }

    //-------Other-----

    //Allow to reset scores
    public void ResetScores()
    {
        HighScoreHandler.instance.ResetScores();
    }

    //Allow to show panel
    public void ShowPanel()
    {
        panel.SetActive(true);
        GetCurrentPseudoInputText();
        GetCurrentControlSchemeDropdown();
        GetCurrentVolumeSlider();
    }

    //Allow to close panel
    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}
