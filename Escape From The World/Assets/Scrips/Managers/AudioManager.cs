using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public enum FloorName { NULL, FLOOR, JUMPERFLOOR, MOVEFLOOR }
    public FloorName floorName = FloorName.FLOOR;

    [SerializeField] private AudioSource gameMusic;
    [SerializeField] private AudioSource gameMusicLoop;

    //[SerializeField] private AudioSource floorSound;
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private AudioSource buttonSound;
    [SerializeField] private AudioSource floorSound;
    [SerializeField] private AudioSource jumperFloorSound;

    [SerializeField] private Image soundOn;
    [SerializeField] private Image soundOff;
    [SerializeField] private Image musicOn;
    [SerializeField] private Image musicOff;
    private bool isSound = true;
    private bool isMusic = true;
    private bool isFirstScene = true; //Ses tuþlarýnýn çalýþmasý için kontrolcü

    public int interstitialAdCounter = 2; // Bunlarý burada tanýmlamak zorunda kaldým çünkü Gamemanager yeniden baþladýðý için sayaçlar da yeniden baþlýyordu sürekli
    public int rewardedAdCounter = 5;
    bool isMusicFinish;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if(!PlayerPrefs.HasKey("isSound") && !PlayerPrefs.HasKey("isMusic"))
        {
            PlayerPrefs.SetInt("isSound", 1);
            PlayerPrefs.SetInt("isMusic", 1);
            Load();
            UpdateButtonIcon();
        }
        else
        {
            Load();
            UpdateButtonIcon();
        }
    }

    private void Update()
    {
        if (!gameMusic.isPlaying && !isMusicFinish)
        {
            gameMusicLoop.Play();
            isMusicFinish = true;
        }
    }

    public void SoundButton()
    {
        if (isSound)
        {
            soundOn.enabled = false;
            soundOff.enabled = true;
            isSound = false;
            floorSound.volume = 0f;
            deathSound.volume = 0f;
            buttonSound.volume = 0f;
            floorSound.volume = 0f;
            jumperFloorSound.volume = 0f;
            //Sesler buraya eklenecek
        }
        else
        {
            soundOn.enabled = true;
            soundOff.enabled = false;
            isSound = true;
            floorSound.volume = 0.2f;
            deathSound.volume = 1f;
            buttonSound.volume = 0.5f;
            jumperFloorSound.volume = 1f;
        }
        Save();
    }

    public void MusicButton()
    {
        if (isMusic)
        {
            musicOn.enabled = false;
            musicOff.enabled = true;
            isMusic= false;
            gameMusic.volume = 0f;
            gameMusicLoop.volume = 0f;
            //Sesler buraya eklenecek
        }
        else
        {
            musicOn.enabled = true;
            musicOff.enabled = false;
            isMusic = true;
            gameMusic.volume = 0.3f;
            gameMusicLoop.volume = 0.3f;
            //Müzikler buraya eklenecek
        }
        Save();
    }

    private void UpdateButtonIcon()
    {
        if (isSound)
        {
            soundOn.enabled = true;
            soundOff.enabled = false;
            floorSound.volume = 0.2f;
            deathSound.volume = 1f;
            buttonSound.volume = 0.5f;
            jumperFloorSound.volume = 1f;
        }
        else
        {
            soundOn.enabled = false;
            soundOff.enabled = true;
            floorSound.volume = 0f;
            deathSound.volume = 0f;
            buttonSound.volume = 0f;
            floorSound.volume = 0f;
            jumperFloorSound.volume = 0f;
        }

        if (isMusic)
        {
            musicOn.enabled = true;
            musicOff.enabled = false;
            gameMusic.volume = 0.3f;
            gameMusicLoop.volume = 0.3f;
        }
        else
        {
            musicOn.enabled = false;
            musicOff.enabled = true;
            gameMusic.volume = 0f;
            gameMusicLoop.volume = 0f;
        }
    }

    private void Save()
    {
        PlayerPrefs.SetInt("isSound", isSound ? 1 : 0);
        PlayerPrefs.SetInt("isMusic", isMusic ? 1 : 0);
    }
    private void Load()
    {
        isSound = PlayerPrefs.GetInt("isSound") == 1;
        isMusic = PlayerPrefs.GetInt("isMusic") == 1;

    }

    public void FloorSound()
    {
        switch (floorName)
        {
            case FloorName.FLOOR:
                floorSound.Play();
                break;
            case FloorName.MOVEFLOOR:
                floorSound.Play();
                break;
            case FloorName.JUMPERFLOOR:
                jumperFloorSound.Play();
                break;
        }
    }

    public void DeathSound()
    {
        deathSound.Play();
    }

    public void ButtonSound()
    {
        buttonSound.Play();
    }

    public void JumperFloorSound()
    {
        jumperFloorSound.Play();
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isFirstScene) //Ses tuþlarýnýn çalýþmasý için kontrolcü
        {
            isFirstScene = false;
            return;
        }
        // Yeni sahnede UI’leri tekrar baðlamayý dene
        soundOn = GameObject.Find("SoundOn")?.GetComponent<Image>();
        soundOff = GameObject.Find("SoundOff")?.GetComponent<Image>();
        musicOn = GameObject.Find("MusicOn")?.GetComponent<Image>();
        musicOff = GameObject.Find("MusicOff")?.GetComponent<Image>();

        //BURADAN DEVAM ---> RESTARTTAN SONRA ÝCONLAR GÝBÝ BUTONLARDA KAYBOLUYOR. ONLARI GERÝ GETÝRÝCEZ 
        Button soundBtn = GameObject.Find("SoundButtons")?.GetComponent<Button>();
        Button musicBtn = GameObject.Find("MusicButtons")?.GetComponent<Button>();

        if (soundBtn != null)
        {
            soundBtn.onClick.RemoveAllListeners();
            soundBtn.onClick.AddListener(SoundButton);
        }

        if (musicBtn != null)
        {
            musicBtn.onClick.RemoveAllListeners();
            musicBtn.onClick.AddListener(MusicButton);
        }

            if (soundOn != null && soundOff != null && musicOn != null && musicOff != null)
        {
            UpdateButtonIcon();
        }
    }
}
