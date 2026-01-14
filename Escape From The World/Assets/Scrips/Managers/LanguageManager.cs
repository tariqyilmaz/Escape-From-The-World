using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    // Start is called before the first frame update
    public enum Languages { TURKISH, ENGLISH, GERMAN, SPANISH } 
    public Languages currentLanguage = Languages.ENGLISH;

    public Sprite turkishFlag;
    public Sprite englishFlag;
    public Sprite germanFlag;
    public Sprite spanishFlag;
    Image currentFlaga;
    GameObject languageTaba;

    public event System.Action OnLanguageChanged;
    private Dictionary<string, Dictionary<string, string>> allTranslations = new Dictionary<string, Dictionary<string, string>>();

    public bool isFirstScene = true; //Language tuþlarýnýn çalýþmasý için kontrolcü

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllTranslations();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey("selectedLanguage"))
        {
            PlayerPrefs.SetInt("selectedLanguage", 1);
            Load();
        }
        else
        {
            Load();
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; //Dont Destroy on load olduðu için eþleþtirmeler kayboluyor. Onu engellemek için tekrar eþliyoruz
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetLanguage(Languages newLanguage)
    {
        currentLanguage = newLanguage;
        UpdateLanguageState();
        Save();
    }
    private void Save()
    {
        PlayerPrefs.SetInt("selectedLanguage", (int)currentLanguage);
        PlayerPrefs.Save(); //Ön bellekleten genel belleðe kaydeder
    }
    private void Load()
    {
        int savedInt = PlayerPrefs.GetInt("selectedLanguage", (int)Languages.ENGLISH); // Languages.ENGLISH): selectedLanguage'te bir veri yoksa dönecek deðer

        currentLanguage = (Languages)savedInt;
        UpdateLanguageState();
    }

    public void selectEnglish()
    {
        SetLanguage(Languages.ENGLISH);
        languageTaba.SetActive(false);
    }

    public void selectTurkish()
    {
        SetLanguage(Languages.TURKISH);
        languageTaba.SetActive(false);
    }

    public void selectGerman()
    {
        SetLanguage(Languages.GERMAN);
        languageTaba.SetActive(false);
    }

    public void selectSpanish()
    {
        SetLanguage(Languages.SPANISH);
        languageTaba.SetActive(false);
    }

    private void UpdateLanguageState()
    {
        if (currentFlaga == null) return; // Null check to prevent NullReferenceException
        
        switch (currentLanguage)
        {
            case Languages.TURKISH:
                currentFlaga.sprite = turkishFlag;
                break;
            case Languages.ENGLISH:
                currentFlaga.sprite = englishFlag;
                break;
            case Languages.GERMAN:
                currentFlaga.sprite = germanFlag;
                break;
            case Languages.SPANISH:
                currentFlaga.sprite = spanishFlag;
                break;
        }

        if (OnLanguageChanged != null)
        {
            OnLanguageChanged.Invoke(); //Tüm abonelere dil deðiþikliðini bildirir
        }
    }

    public string GetTranslation(string key)
    {
        string languageKey = currentLanguage.ToString();

        if (allTranslations.ContainsKey(languageKey) && allTranslations[languageKey].ContainsKey(key)) //Dýþ dictionary(TURKISH,ENGLISH...) ve iç dicitonary(keyler) var mý kontrol ediyor
        {
            return allTranslations[languageKey][key];
        }
        Debug.LogError($"Çeviri anahtarý bulunamadý: '{key}' ({languageKey})");
        return $"!{key}!";
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isFirstScene)
        {
            currentFlaga = GameObject.Find("LanguageFlag")?.GetComponent<Image>();
            languageTaba = GameObject.Find("LanguageTab");
            if (languageTaba == null && MenuManager.Instance != null)
            {
                languageTaba = MenuManager.Instance.languageTab;
            }
            Button flagButton = GameObject.Find("LanguageFlag")?.GetComponent<Button>();
            if (flagButton != null)
            {
                flagButton.onClick.RemoveAllListeners();
                flagButton.onClick.AddListener(MenuManager.Instance.OpenLanguageTab);
            }

            if (currentFlaga != null)
            {
                switch (currentLanguage)
                {
                    case Languages.TURKISH:
                        currentFlaga.sprite = turkishFlag;
                        break;
                    case Languages.ENGLISH:
                        currentFlaga.sprite = englishFlag;
                        break;
                    case Languages.GERMAN:
                        currentFlaga.sprite = germanFlag;
                        break;
                    case Languages.SPANISH:
                        currentFlaga.sprite = spanishFlag;
                        break;
                }
            }
            isFirstScene = false;
            return;
        }
    }

    public void RegisterUIElements(Image flagImage, GameObject langTab, Button flagBtn)
    {
        currentFlaga = flagImage;
        languageTaba = langTab;

        if (flagBtn != null)
        {
            flagBtn.onClick.RemoveAllListeners();
            // MenuManager'ýn instance olduðundan emin olarak dinleyici ekle
            if (MenuManager.Instance != null)
                flagBtn.onClick.AddListener(MenuManager.Instance.OpenLanguageTab);
        }

        UpdateLanguageState(); // Yeni gelen objenin görselini hemen güncelle
    }

    #region Dictionary
    private void LoadAllTranslations()
    {
        // NOT: Anahtar olarak enum adlarýný (TURKISH, ENGLISH vb.) kullanýyoruz.

        // --- TÜRKÇE METÝNLER (Anahtar: TURKISH) ---
        var trTexts = new Dictionary<string, string>
        {
            {"key_start_button", "BASLA"},
            {"key_game_mode", "Oyun Modu"},
            {"key_main_menu", "ANA MENU"},
            {"key_restart", "TEKRAR OYNA"},
            {"key_game_over", "OYUN BÝTTÝ"},
            {"key_best_score", "EN ÝYÝ PUAN"},
            {"key_score", "PUAN"},
            {"key_settings", "AYARLAR"},
            {"key_settings_sound", "Ses"},
            {"key_settings_music", "Muzik"},
            {"key_settings_language", "Dil"},
            {"key_settings_tutorial", "Ogretici"},
            {"key_settings_back", "Geri"},
        };
        allTranslations.Add(Languages.TURKISH.ToString(), trTexts); //TURKISH dicitonary'sine  trTexts dictionarysini ekliyor

        // --- ÝNGÝLÝZCE METÝNLER (Anahtar: ENGLISH) ---
        var enTexts = new Dictionary<string, string>
        {
            {"key_start_button", "START"},
            {"key_game_mode", "Game Mode"},
            {"key_main_menu", "MAIN MENU"},
            {"key_restart", "RESTART"},
            {"key_game_over", "GAME OVER"},
            {"key_best_score", "BEST SCORE"},
            {"key_score", "SCORE"},
            {"key_settings", "SETTINGS"},
            {"key_settings_sound", "Sound"},
            {"key_settings_music", "Music"},
            {"key_settings_language", "Language"},
            {"key_settings_tutorial", "Tutorial"},
            {"key_settings_back", "Back"},
        };
        allTranslations.Add(Languages.ENGLISH.ToString(), enTexts);

        // --- ALMANCA METÝNLER (Anahtar: GERMAN) ---
        var deTexts = new Dictionary<string, string>
        {
            {"key_start_button", "STARTEN"},
            {"key_game_mode", "Spielmodus"},
            {"key_main_menu", "HAUPTMENU"},
            {"key_restart", "ERNEUT SPIELEN"},
            {"key_game_over", "SPIEL VORBEI"},
            {"key_best_score", "ESTER PUNKTESTAND"},
            {"key_score", "PUNKTE"},
            {"key_settings", "EINSTELLUNGEN"},
            {"key_settings_sound", "Klang"},
            {"key_settings_music", "Musik"},
            {"key_settings_language", "Sprache"},
            {"key_settings_tutorial", "Tutorial"},
            {"key_settings_back", "Zuruck"},
        };
        allTranslations.Add(Languages.GERMAN.ToString(), deTexts);

        // --- ÝSPANYOLCA METÝNLER (Anahtar: SPANISH) ---
        var esTexts = new Dictionary<string, string>
        {
            {"key_start_button", "EMPEZAR"},
            {"key_game_mode", "Modo de Juego"},
            {"key_main_menu", "MENÚ PRINCIPAL"},
            {"key_restart", "JUGAR DE NUEVO"},
            {"key_game_over", "FIN DEL JUEGO"},
            {"key_best_score", "MEJOR PUNTUACIÓN"},
            {"key_score", "PUNTOS"},
            {"key_settings", "AJUSTES"},
            {"key_settings_sound", "Sonido"},
            {"key_settings_music", "Musica"},
            {"key_settings_language", "Idioma"},
            {"key_settings_tutorial", "Tutorial"},
            {"key_settings_back", "Atras"},
        };
        allTranslations.Add(Languages.SPANISH.ToString(), esTexts);
    }
    #endregion
}
