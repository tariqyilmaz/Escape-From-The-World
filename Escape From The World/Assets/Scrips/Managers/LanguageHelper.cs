using UnityEngine;
using UnityEngine.UI;

public class LanguageHelper : MonoBehaviour
{
    public Image flagImage;
    public GameObject langTab;
    public Button flagButton;

    void Start()
    {
        // Sahne açýldýðýnda LanguageManager'a "Ben buradayým, beni kullan" der
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.RegisterUIElements(flagImage, langTab, flagButton);
        }
    }
}