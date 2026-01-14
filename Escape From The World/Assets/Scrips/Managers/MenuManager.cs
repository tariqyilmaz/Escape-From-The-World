using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public Button gameModeButton;
    public TMP_Text gameModeText;
    public bool isTouchScreen = false; //TouchScreen:true TouchButton:false
    public GameObject buttonMode;
    public GameObject screenMode;
    public GameObject languageTab;
    public GameObject settingsPanel;
    public Image currentFlag;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Dinamik Frame Rate - Yuksek refresh rate ekranlarda (120Hz vb.)
        // frame pacing sorunlarini onlemek icin ekranin destekledigi hizi kullan
        int screenRefreshRate = (int)Screen.currentResolution.refreshRateRatio.value;
        // Minimum 60 FPS, maksimum ekranin destekledigi
        Application.targetFrameRate = Mathf.Clamp(screenRefreshRate, 60, 120);
    }

    void Start()
    {
        gameModeText = gameModeButton.GetComponentInChildren<TMP_Text>();

        if (!PlayerPrefs.HasKey("gameMode"))
        {
            PlayerPrefs.SetInt("gameMode", 0);
            Load();
        }
        else
        {
            PlayerPrefs.GetInt("gameMode");
            Load();
        }
        if (PlayerPrefs.GetInt("gameMode") == 0)
        {
            buttonMode.SetActive(true);
            screenMode.SetActive(false);
        }
        else
        {
            buttonMode.SetActive(false);
            screenMode.SetActive(true);
        }
        settingsPanel.SetActive(false);
    }

    //-----Language Garanti Kısmı-----
    public void OpenLanguageTab()
    {
        if (languageTab != null)
        {
            languageTab.SetActive(true);
        }
    }

    public void selectEnglishA()
    {
        LanguageManager.Instance.selectEnglish();
    }

    public void selectTurkishA()
    {
        LanguageManager.Instance.selectTurkish();
    }

    public void selectGermanA()
    {
        LanguageManager.Instance.selectGerman();
    }

    public void selectSpanishA()
    {
        LanguageManager.Instance.selectSpanish();
    }

    //-----OYUN MODU BELİRLEME-----

    public void SelectButtonMode()
    {
        if (isTouchScreen)
        {
            buttonMode.SetActive(true);
            screenMode.SetActive(false);
            isTouchScreen = false;
            Save();
        }
    }
    public void SelectScreenMode()
    {
        if (!isTouchScreen)
        {
            buttonMode.SetActive(false);
            screenMode.SetActive(true);
            isTouchScreen = true;
            Save();
        }
    }

    private void Save()
    {
        PlayerPrefs.SetInt("gameMode", isTouchScreen ? 1 : 0);
    }
    private void Load()
    {
        isTouchScreen = PlayerPrefs.GetInt("gameMode") == 1;
        if (isTouchScreen)
        {
            gameModeText.text = "Touch Screen";
        }
        else
        {
            gameModeText.text = "Touch Button";
        }
    }

    public void enterSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }
    
    public void quitSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }
}

