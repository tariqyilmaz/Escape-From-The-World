using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager Instance;

    public float score;
    public GameObject startFloor;
    public enum GameStage { ONE, TWO, POWERUP, INPOWERUP }
    public GameStage stage = GameStage.ONE;

    bool exitCoroutine = false; //Kontrol

    //Deletor
    public GameObject deletor;
    Transform deletorCurrentY;
    float deletorMaxY = -30f;
    //Player
    public GameObject player;
    Transform playerCurrentY;
    Rigidbody2D playerRb;
    //Camera
    Rigidbody2D cameraRb;
    public CinemachineVirtualCamera virtualCam;

    //Canvas
    public RectTransform gameOverPanel;
    public GameObject touchScreen;
    public GameObject touchButton;
    public GameObject jumpRightButton; //Bunun sebebi iki kontrolcüde de ayný buton var ve tekrardan hepsini atamamak için ortak kullanýyoruz
    public Button mainMenuButton;
    public Button startOverButton;
    public Text scoreText;
    public Text bestScore;
    public TMP_Text bestScoreText;
    string yellowHexCode = "#FFDE00";
    Color colorFromHex;
    Animator bestScoreAnim;

    //EnvironmentLight
    Animator anim;
    [SerializeField] private GameObject environmentLight;

    //Sound
    public static bool isDeathSound = true; //Bu ölüm sesinin sadece bir kez oynatýlmasý için bir kontrolcü

    //General
    
    public bool isDeath = false;
    public bool isDeathController = false;
    public bool isAdController = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerCurrentY = player.GetComponent<Transform>();
        deletorCurrentY = deletor.GetComponent<Transform>();
        playerRb = player.GetComponent<Rigidbody2D>();
        cameraRb = player.GetComponent<Rigidbody2D>();
        anim = environmentLight.GetComponent<Animator>();
        bestScoreAnim = bestScoreText.GetComponent<Animator>();
        DOTween.SetTweensCapacity(1250,1250); //Verilen uyarý üzerine eklendi
        bestScore.text = PlayerPrefs.GetFloat(nameof(score)).ToString(); //Kontrol amaçlý burada tekrar eþitledik
        isDeathSound = true; //Ölüm sesinin bir kez çalmasý için kontrolcü

        if (MenuManager.Instance.isTouchScreen)
        {
            touchScreen.SetActive(true);
            touchButton.SetActive(false);
        }
        else
        {
            touchScreen.SetActive(false);
            touchButton.SetActive(true);
        }
        StartCoroutine(destroyStartFloor());
    }

    private void OnEnable()
    {
        // Olay dinleyicisini ekle
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        DeletorMax();
        switch (stage)
        {
            case GameStage.ONE:
                MoveFloor.floorSpeed = 2f;
                break;
            case GameStage.TWO:
                MoveFloor.floorSpeed = 3f;
                break;
            case GameStage.POWERUP:
                PowerUp();
                stage = GameStage.INPOWERUP; // Tekrar update den dolayý sürekli sürekli buraya girmesin diye
                break;
        }

        if (stage != GameStage.POWERUP && stage != GameStage.INPOWERUP)
        {
            Stage();
        }
    }

    //-----StartFloordan Kurtulma-----

    IEnumerator destroyStartFloor()
    {
        SpriteRenderer sr = startFloor.GetComponent<SpriteRenderer>();
        BoxCollider2D collider = startFloor.GetComponent<BoxCollider2D>();

          yield return new WaitForSeconds(3f);
          sr.enabled = false;
          collider.enabled = false;
          exitCoroutine = true;
                                    
    }

    //-----Oyunu Dondurma-----

    private void DeletorMax()
    {
        if (deletorMaxY<deletorCurrentY.position.y)
        {
            deletorMaxY = deletorCurrentY.position.y;
        }
        if (playerCurrentY.position.y<deletorMaxY)
        {                       
            if (!isDeathController) //isDeathController sürekli buraya girmesin diye var
            {
                StartCoroutine(StopGame());
                if (isDeathSound == true)
                {
                    anim.SetBool("isDeathAnim", true);
                    AudioManager.Instance.DeathSound();
                    isDeathSound = false;
                }
            }          
        }
    }
    IEnumerator StopGame()
    {
        virtualCam.Follow = null; //cinemachine ile baðlantýsýný kesiyor
        yield return new WaitForSeconds(0.1f);
        cameraRb.constraints = RigidbodyConstraints2D.FreezePosition; //Kamera Pozisyonunu donduruyor
        touchButton.SetActive(false);
        touchScreen.SetActive(false);
        jumpRightButton.SetActive(false);
        StartCoroutine(GameOverAnimation());
        yield return new WaitForSeconds(12f);
        playerRb.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    IEnumerator GameOverAnimation()
    {
        if (!PlayerPrefs.HasKey(nameof(score)) || PlayerPrefs.GetFloat(nameof(score))<score)
        {
            PlayerPrefs.SetFloat(nameof(score), score);
            bestScoreAnim.SetBool("bestScore", true);
            if (UnityEngine.ColorUtility.TryParseHtmlString(yellowHexCode, out colorFromHex))
            {
                scoreText.color = colorFromHex;
            }
        }        

        bestScore.text = PlayerPrefs.GetFloat(nameof(score)).ToString(); //güncel bestscore u yazdýrdýk
        gameOverPanel.DOAnchorPos(new Vector3(-195, -58, 0), 0.3f).SetEase(Ease.InBounce); //0.1 yapýyorum çünkü telefonlarda çok yavaþ (0.4f)   
        TMP_Text mainMenuText = mainMenuButton.GetComponentInChildren<TMP_Text>();
        TMP_Text startOverText = startOverButton.GetComponentInChildren<TMP_Text>();
        yield return new WaitForSeconds(2.5f);
        isDeath = true;
        if (isDeath && !isAdController)
        {
            isAdController = true;
            ShowAd();
        }
        mainMenuButton.image.DOFade(1f, 0f);
        mainMenuText.DOFade(1f, 1f); //1f
        startOverButton.image.DOFade(1f, 0f);
        startOverText.DOFade(1f, 1f); //1f
        
        

        isDeathController = true; //Sürekli updatete bu kod çalýþmasýn diye
    }


    //-----POWER UP-----

    private void PowerUp()
    {
        Time.timeScale = 1.2f;        
        StartCoroutine(WaitPowerUpStage());
    }

    IEnumerator WaitPowerUpStage()
    {
        anim.SetBool("isStart", true);
        yield return new WaitForSeconds(12f+(score/100));
        Time.timeScale = 1f;
        anim.SetBool("End", true);
        anim.SetBool("isStart", false);
        Stage();
        yield return new WaitForSeconds(1f);
        anim.SetBool("End", false);     
    }

    //-----STAGE BELÝRLEME-----

    private void Stage()
    {       
        if (300 < score && stage != GameStage.TWO)
        {
            stage = GameStage.TWO;
        }
        else
        {
            stage = GameStage.ONE;
        }
    }

    //-----Reklam Çalýþtýrma-----
    private void ShowAd()
    {
        AudioManager.Instance.interstitialAdCounter -= 1;
        Debug.Log(AudioManager.Instance.interstitialAdCounter);
        AudioManager.Instance.rewardedAdCounter -= 1;
        if (AudioManager.Instance.interstitialAdCounter == 0)
        {           
            AdManager.Instance.ShowIntertitialAd();
            AudioManager.Instance.interstitialAdCounter = 2;
        }      
    }

    public void ShowRewardedAd()
    {
        if (AudioManager.Instance.rewardedAdCounter == 0)
        {
            AdManager.Instance.ShowRewardedAd();
            AudioManager.Instance.rewardedAdCounter = 5;
            AudioManager.Instance.interstitialAdCounter = 2;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0 || scene.buildIndex == 1)
        {
            isDeath = false;
            isDeathController = false;
            isAdController = false;

        }
    }
}
