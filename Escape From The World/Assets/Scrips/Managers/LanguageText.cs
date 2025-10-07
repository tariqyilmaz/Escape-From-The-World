using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LanguageText : MonoBehaviour
{
    // Inspector'da çevirisini istediðiniz anahtarý buraya yazýn (Örn: "key_start_button")
    public string localizationKey;

    private TextMeshProUGUI textComponent;

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        // Dil yöneticisinin olayýna abone ol: Dil deðiþtiðinde UpdateText metodu çalýþsýn
        UpdateText();
        LanguageManager.Instance.OnLanguageChanged += UpdateText;
    }

    private void OnDestroy()
    {
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.OnLanguageChanged -= UpdateText;
        }
    }

    public void UpdateText()
    {
        if (string.IsNullOrEmpty(localizationKey)) return;

        // Çeviriyi Singleton Instance üzerinden al
        string newText = LanguageManager.Instance.GetTranslation(localizationKey); //GetTranslation'dan gelen stringi alýyor ve newTexte eþitliyor

        // Metin bileþenini güncelle
        if (textComponent != null)
        {
            textComponent.text = newText; //new texti bastýrýyor
        }
    }
}