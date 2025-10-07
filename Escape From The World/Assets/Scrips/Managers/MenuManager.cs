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
    public GameObject languageTab;
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
        float refreshRate = (float)Screen.currentResolution.refreshRateRatio.value;
        Application.targetFrameRate = Mathf.RoundToInt(refreshRate);
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
    }

    //-----Language Garanti Kýsmý-----
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

    //-----OYUN MODU BELÝRLEME-----

    public void SelectGameMode()
    {
        if (isTouchScreen)
        {
            isTouchScreen = false;
            gameModeText.text = "Touch Button";
            Save();
        }
        else
        {
            isTouchScreen = true;
            gameModeText.text = "Touch Screen";
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
}
