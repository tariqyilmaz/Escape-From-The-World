using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Yükleme işlemlerini yöneten singleton manager.
/// Async sahne yükleme ve progress tracking sağlar.
/// </summary>
public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance { get; private set; }

    [Header("Loading Settings")]
    [SerializeField] private float minimumLoadTime = 1.5f; // Minimum yükleme süresi (UX için)
    [SerializeField] private int loadingSceneIndex = 2; // Loading sahnesinin build index'i

    // Events
    public event Action<float> OnProgressChanged;
    public event Action OnLoadingComplete;

    // Yüklenecek sahne index'i
    private int targetSceneIndex;
    private bool isLoading = false;

    // Progress tracking
    private float currentProgress = 0f;
    public float CurrentProgress => currentProgress;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Yükleme ekranı ile sahne yükler
    /// </summary>
    /// <param name="sceneIndex">Yüklenecek sahnenin build index'i</param>
    public void LoadSceneWithLoadingScreen(int sceneIndex)
    {
        if (isLoading) return;

        targetSceneIndex = sceneIndex;
        isLoading = true;
        currentProgress = 0f;

        // Önce loading sahnesine geç
        SceneManager.LoadScene(loadingSceneIndex);

        // Loading sahnesi yüklenince asıl yüklemeyi başlat
        SceneManager.sceneLoaded += OnLoadingSceneLoaded;
    }

    private void OnLoadingSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == loadingSceneIndex)
        {
            SceneManager.sceneLoaded -= OnLoadingSceneLoaded;
            StartCoroutine(LoadTargetSceneAsync());
        }
    }

    private IEnumerator LoadTargetSceneAsync()
    {
        float elapsedTime = 0f;

        // Hedef sahneyi async olarak yükle
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetSceneIndex);
        asyncOperation.allowSceneActivation = false;

        // Yükleme döngüsü
        while (!asyncOperation.isDone)
        {
            elapsedTime += Time.deltaTime;

            // AsyncOperation.progress 0.9'a kadar gider, son 0.1 activation için
            float sceneProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            // Minimum süre ile birleştir (smooth animasyon için)
            float timeProgress = Mathf.Clamp01(elapsedTime / minimumLoadTime);

            // İkisinin minimumunu al (gerçekçi ilerleme)
            currentProgress = Mathf.Min(sceneProgress, timeProgress);

            // Event'i tetikle
            OnProgressChanged?.Invoke(currentProgress);

            // Yükleme tamamlandı mı kontrol et
            if (asyncOperation.progress >= 0.9f && elapsedTime >= minimumLoadTime)
            {
                // %100 göster
                currentProgress = 1f;
                OnProgressChanged?.Invoke(currentProgress);

                // Kısa bekleme
                yield return new WaitForSeconds(0.3f);

                // Yükleme tamamlandı event'i
                OnLoadingComplete?.Invoke();

                // Sahneyi aktif et
                isLoading = false;
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    /// <summary>
    /// Doğrudan sahne yükler (yükleme ekranı olmadan)
    /// </summary>
    public void LoadSceneDirect(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
