using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    private float horizontalSpeed;

    //*****DoubleJump*****
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private GameObject doubleJumpLeft;
    [SerializeField] private GameObject doubleJumpRight;
    [SerializeField] private GameObject doubleJumpLeftWaitBar;
    [SerializeField] private GameObject doubleJumpRightWaitBar;
    [SerializeField] private RectTransform doubleJumpLeftWaitBarPicture;
    [SerializeField] private RectTransform doubleJumpRightWaitBarPicture;

    public Rigidbody2D rb;
    public float moveSpeed = 5.3f;
    public float accel = 11f;   // ivmelenme hızı
    public float decel = 15f;   // yavaşlama hızı

    float currentVel;
    float targetVel;
    bool leftPressed, rightPressed;

    float doubleJumpWaitCurrentTime = 3f; //Zıplamadan sonra beklenilecek hedef vakit
    float doubleJumpWaitTime = 0f; //Zıplamadan sonra beklenilecek anlık vakit
    Slider leftWaitBar;
    Slider rightWaitBar;
    Image leftSliderColor;
    Image rightSliderColor;
    Tween jumpButtonLeftAnim;
    Tween jumpButtonRightAnim;
    bool isCLickDoubleJump = false; //Bunu kullanmamın sebebi yaptığım fonksiyonu direkt butona attığımda sadece tıkladığımda çalışacağı için devamlı çalışmayacak. Bunu o anda düzenli çalıştırmak istediğim için updatete kontrol ettirip orda yaptırtıyorum.
    bool doubleJumpAnimStarted = false;
    bool doubleJumpReady = false; //Bara tıklandıktan ve iş bittikten sonra tekrar kırmızı ve sıfıra dönüyordu. Onu düzeltmek için bir kontrol noktası.

    Button interactableRight;
    Button interactableLeft;
    
    //Rigidbody2D rb;
    Animator playerAnim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        interactableLeft = doubleJumpLeft.GetComponent<Button>();
        interactableRight = doubleJumpRight.GetComponent<Button>();
        leftWaitBar = doubleJumpLeftWaitBar.GetComponent<Slider>();
        rightWaitBar = doubleJumpRightWaitBar.GetComponent<Slider>();
        //doubleJumpLeftWaitBarPicture = doubleJumpLeftWaitBarPicture.GetComponent<RectTransform>();

        leftSliderColor = leftWaitBar.fillRect.GetComponent<Image>();
        rightSliderColor = rightWaitBar.fillRect.GetComponent<Image>();
        playerAnim = gameObject.GetComponent<Animator>();
        StartCoroutine(BlinkRoutine());
    }

    private void Update()
    {
        // Hedef hızı belirle
        if (leftPressed) targetVel = -moveSpeed;
        else if (rightPressed) targetVel = moveSpeed;
        else targetVel = 0;

        // Hız geçişini yumuşat
        float rate = Mathf.Abs(targetVel) > 0.1f ? accel : decel;
        if (Mathf.Sign(currentVel) != Mathf.Sign(targetVel) && targetVel != 0) // Çok yavaştı biraz onun önüne geçti
            currentVel = 0; // önce hızı sıfırla
        currentVel = Mathf.MoveTowards(currentVel, targetVel, rate * Time.deltaTime);

        // Rigidbody’ye uygula
        rb.velocity = new Vector2(currentVel, rb.velocity.y);

        horizontalSpeed = Input.GetAxis("Horizontal");  //Normal oyunda kapalı olacak
        if (isCLickDoubleJump == true) DoubleJumpWaitBar();
        if (isCLickDoubleJump == false && doubleJumpAnimStarted == false)
        {
            doubleJumpAnimStarted = true;
            DoubleJumpButtonAnimation(-1, 0.30f); //(loop,time)
        }
    }
    private void FixedUpdate()
    {
        Look();
        //rb.velocity = new Vector2(horizontalSpeed * moveSpeed, rb.velocity.y); // Bunu açtığımda tuşlar geri geliyor
    }

    // Buton Eventleri
    public void OnLeftDown() => leftPressed = true;
    public void OnLeftUp() => leftPressed = false;
    public void OnRightDown() => rightPressed = true;
    public void OnRightUp() => rightPressed = false;

    //-----Hareket Bölümü------
    private void Look()
    {
        Vector2 newScale = transform.localScale;
        if (rightPressed)
        {
            newScale.x = 0.35f;
        }
        if (leftPressed)
        {
            newScale.x = -0.35f;
        }
        transform.localScale = newScale;
    }

    public void LeftButton()
    {
        horizontalSpeed = -1.1f;//-1
    }

    public void RightButton()
    {
        horizontalSpeed = 1.1f;//1
    }
    public void Stop()
    {
        horizontalSpeed = 0;
    }

    //-----Double Jump Bölümü------
    public void DoubleJump()
    {
        Vector2 jumpSpeed = rb.velocity;
        jumpSpeed.y = doubleJumpForce;
        rb.velocity = jumpSpeed;
    }

    IEnumerator waitDoubleJump() //Butona bastıktan sonra bir süre butonu inaktif ediyor.
    {
        //DoubleJumpButtonAnimation(0,0.30f);
        jumpButtonLeftAnim.Pause();
        jumpButtonRightAnim.Pause();
        doubleJumpLeftWaitBarPicture.localScale = doubleJumpRightWaitBarPicture.localScale = new Vector3(27f,27f,27f);
        isCLickDoubleJump = true;
        interactableRight.interactable = false;
        interactableLeft.interactable = false;
        yield return new WaitForSeconds(doubleJumpWaitCurrentTime); //Double Jump bekleme süresi
        doubleJumpLeftWaitBarPicture.localScale = doubleJumpRightWaitBarPicture.localScale = new Vector3(34f, 34f, 34f);
        isCLickDoubleJump = false;
        doubleJumpAnimStarted = false;
        doubleJumpReady = false; //Barın tekrar tekrar aktif olabilmesini sağlıyor
        interactableLeft.interactable = true;
        interactableRight.interactable = true;
    }

    public void wait()
    {
        StartCoroutine(waitDoubleJump());

    }

    public void DoubleJumpWaitBar()
    {
        if (doubleJumpReady) return;
        leftWaitBar.value = 0;
        rightWaitBar.value = 0;
        leftSliderColor.color = Color.red;
        rightSliderColor.color = Color.red;
        doubleJumpWaitTime += Time.deltaTime;
        leftWaitBar.value = doubleJumpWaitTime;
        rightWaitBar.value = doubleJumpWaitTime;
        if (doubleJumpWaitTime >= doubleJumpWaitCurrentTime)
        {
            leftSliderColor.color = Color.green;
            rightSliderColor.color = Color.green;
            leftWaitBar.value = leftWaitBar.maxValue;
            rightWaitBar.value = rightWaitBar.maxValue;
            doubleJumpWaitTime = 0;
            doubleJumpReady = true;
        }
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 6f));
            playerAnim.SetTrigger("Blink");
        }
    }

    public void DoubleJumpButtonAnimation(int loop,float time)
    {
        jumpButtonLeftAnim = doubleJumpLeftWaitBarPicture
            .DOScale(27f, time) //time = 0.30
            .From(34f) 
            .SetLoops(loop, LoopType.Yoyo);
        jumpButtonRightAnim = doubleJumpRightWaitBarPicture
            .DOScale(27f, time) //time = 0.30
            .From(34f) 
            .SetLoops(loop, LoopType.Yoyo);
    }
}
