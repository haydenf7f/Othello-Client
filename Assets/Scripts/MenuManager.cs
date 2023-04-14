using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public string newGameScene;


    [Header("Audio Settings")]
    [SerializeField] private float defaultVolume = 0.5f;
    [SerializeField] private TMP_Text masterVolumeTextValue = null;
    [SerializeField] private Slider masterVolumeSlider = null;

    [SerializeField] private GameObject confirmationPrompt = null;


    [Header("Graphics Settings")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;
    [SerializeField] private float defaultBrightness = 1;
    private int _qualityLevel;
    private bool _isFullScreen;
    private float _brightnessLevel;


    [Space(10)]
    [SerializeField] private TMP_Dropdown qualityDropdown = null;
    [SerializeField] private Toggle fullScreenToggle = null;
    

    [Header("Resolutions Dropdown")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    [Space(10)]
    [Header("Multiplayer")]
    public static MenuManager instance;
    public InputField usernameField;


    private void Awake() 
    {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void ConnectToServer() 
    {
        usernameField.interactable = false;
        Client.instance.ConnectToServer();
    }

    // public void Disconnect() 
    // {
    //     Client.instance.Disconnect();
    // }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void NewGame()
    {
        SceneManager.LoadScene(newGameScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetMasterVolume(float masterVolume)
    {
        AudioListener.volume = masterVolume;
        masterVolumeTextValue.text = Mathf.RoundToInt(masterVolume * 100).ToString();
    }

    public void VolumeApply() 
    {
        // Debug.Log("AudioListener.volume: " + AudioListener.volume);
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        // Debug.Log("masterVolume: " + PlayerPrefs.GetFloat("masterVolume"));
        StartCoroutine(ConfirmationBox());
    }

    public void SetBrightness(float brightness) 
    {
        _brightnessLevel = brightness;
        brightnessTextValue.text = brightness.ToString("0.0");
    }

    public void SetFullScreen(bool isFullScreen) 
    {
        _isFullScreen = isFullScreen;
    }

    public void SetQuality(int qualityIndex) 
    {
        _qualityLevel = qualityIndex;
    }

    public void GraphicsApply() 
    {
        PlayerPrefs.SetInt("qualityLevel", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        PlayerPrefs.SetInt("isFullScreen", (_isFullScreen ? 1 : 0));
        Screen.fullScreen = _isFullScreen;

        PlayerPrefs.SetFloat("brightnessLevel", _brightnessLevel);

        StartCoroutine(ConfirmationBox());
    }

    public void ResetButton(string MenuType) {
        if (MenuType == "Graphics") {
            // Reset brightness
            brightnessSlider.value = defaultBrightness;
            brightnessTextValue.text = defaultBrightness.ToString("0.0");

            // Reset fullscreen toggle
            fullScreenToggle.isOn = false;
            Screen.fullScreen = false;

            // Reset quality dropdown
            qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);

            // Reset resolution dropdown
            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            resolutionDropdown.value = resolutions.Length;

            GraphicsApply();
        }

        if (MenuType == "Audio") {
            AudioListener.volume = defaultVolume;
            masterVolumeSlider.value = defaultVolume;
            masterVolumeTextValue.text = Mathf.RoundToInt(defaultVolume * 100).ToString();
            VolumeApply();
        }
        // if (MenuType == "Gameplay") {}

    }

    public IEnumerator ConfirmationBox() {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }
}
