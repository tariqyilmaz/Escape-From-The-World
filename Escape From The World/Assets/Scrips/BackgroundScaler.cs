using UnityEngine;

/// <summary>
/// Arkaplan sprite'ını kameranın görüş alanına göre otomatik ölçeklendirir.
/// Bu script'i arkaplan sprite'ınızın olduğu GameObject'e ekleyin.
/// </summary>
public class BackgroundScaler : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Kullanılacak kamera. Boş bırakılırsa Main Camera kullanılır.")]
    [SerializeField] private Camera targetCamera;
    
    [Tooltip("Sprite'ın ekranı tamamen kaplaması için fazladan ölçekleme")]
    [SerializeField] private float extraScale = 1.05f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
        
        if (spriteRenderer == null)
        {
            Debug.LogError("BackgroundScaler: SpriteRenderer bulunamadı!");
            return;
        }
        
        ScaleToFitScreen();
    }

    private void Start()
    {
        // Start'ta da çağırarak kameranın tam olarak yüklenmesini garantileyelim
        ScaleToFitScreen();
    }

    /// <summary>
    /// Sprite'ı ekrana sığacak şekilde ölçeklendirir
    /// </summary>
    public void ScaleToFitScreen()
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null || targetCamera == null)
        {
            return;
        }

        // Kameranın görüş alanı boyutlarını hesapla
        float cameraHeight = targetCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * targetCamera.aspect;

        // Sprite'ın orijinal boyutlarını al
        Sprite sprite = spriteRenderer.sprite;
        float spriteWidth = sprite.bounds.size.x;
        float spriteHeight = sprite.bounds.size.y;

        // Ekranı tamamen kaplamak için gereken ölçeği hesapla
        float scaleX = cameraWidth / spriteWidth;
        float scaleY = cameraHeight / spriteHeight;

        // En büyük ölçeği kullan (ekranı tamamen kaplamak için)
        float scale = Mathf.Max(scaleX, scaleY) * extraScale;

        // Ölçeği uygula
        transform.localScale = new Vector3(scale, scale, 1f);

        // Sprite'ı kameranın merkezine konumlandır
        Vector3 cameraPosition = targetCamera.transform.position;
        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, transform.position.z);
    }

    // Editörde test etmek için
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            ScaleToFitScreen();
        }
    }
}
