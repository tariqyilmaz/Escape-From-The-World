using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] RectTransform faderImage;

    void Start()
    {
        faderImage.gameObject.SetActive(true);
        LeanTween.alpha(faderImage, 1, 0);
        LeanTween.alpha(faderImage, 0, 0.5f).setOnComplete(() =>
        {
            faderImage.gameObject.SetActive(false);
        });
    }
    public void StartGameScene()
    {
        AudioManager.Instance.ButtonSound();
        faderImage.gameObject.SetActive(true);
        LeanTween.alpha(faderImage, 0, 0);
        LeanTween.alpha(faderImage, 1, 0.5f).setOnComplete(() =>
        {
            Time.timeScale = 1f; //Animasyon bitmeden oyunu tekrar baþlatýnca 1 olmuyor. Kontrol amaçlý tekrardan 1 yapýyoruz.
            DOTween.KillAll(); //Yeni sahne yüklenirken tweenler uyarý vermemesi için
            SceneManager.LoadScene(1);
        });
    }

    public void LoadMenuScene()
    {
        AudioManager.Instance.ButtonSound();
        faderImage.gameObject.SetActive(true);
        LeanTween.alpha(faderImage, 0, 0);
        LeanTween.alpha(faderImage, 1, 0.5f).setOnComplete(() =>
        {
            LanguageManager.Instance.isFirstScene = true;
            Time.timeScale = 1f;
            DOTween.KillAll();
            SceneManager.LoadScene(0);
        });
    }

    public void LoadGameScene()
    {
        AudioManager.Instance.ButtonSound();
        GameManager.Instance.ShowRewardedAd(); //Rewarded reklam butona atalý
        faderImage.gameObject.SetActive(true);
        LeanTween.alpha(faderImage, 0, 0);
        LeanTween.alpha(faderImage, 1, 0.5f).setOnComplete(() =>
        {
            Time.timeScale = 1f; //Animasyon bitmeden oyunu tekrar baþlatýnca 1 olmuyor. Kontrol amaçlý tekrardan 1 yapýyoruz.
            DOTween.KillAll(); //Yeni sahne yüklenirken tweenler uyarý vermemesi için
            SceneManager.LoadScene(1);
        });
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
