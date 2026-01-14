using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;

public class Floor : MonoBehaviour
{
    [SerializeField]private float jumpForce;
    public FloorGenerator floorGenerator;
    private int isOneJump = 1; //Spawn için oluþturduðumuz sayýyý kontrol etmek için oluþturdum. 1 yazdýðým için bir kere dokunduðunda objeyi saymýþ oluyor.
    Animator anim;
    private enum Floors {FLOOR, JUMPERFLOOR, POWERUPFLOOR}
    private Floors floorType = Floors.FLOOR;
    float[] pitchValues = { 0.97f, 1f, 1f, 1f, 1.05f }; //Ses deðerleri için aralýk
    public static float randomPitch;

    string hexCode = "#FFBE00";
    Light2D spotlight2D;
    Color colorFromHex;


    private void Start()
    {
        anim = GetComponent<Animator>();
        spotlight2D = GetComponentInChildren<Light2D>();
        randomPitch = pitchValues[Random.Range(0, pitchValues.Length)]; //Rasgele bir ses araýðý seçimi
        
        int selectFloor = Random.Range(0,36);
        if (selectFloor<6 && gameObject.tag != "MenuStartFloor")
        {
            floorType = Floors.JUMPERFLOOR;
            jumpForce = 25;//22
            anim.SetBool("extraJumper", true);
        }
        else if (selectFloor == 6 && GameManager.Instance.stage != GameManager.GameStage.POWERUP && GameManager.Instance.score>75)
        {
            floorType = Floors.POWERUPFLOOR;
            anim.SetBool("powerUp", true);
            jumpForce = 25;//22
            if (ColorUtility.TryParseHtmlString(hexCode, out colorFromHex))
            {
                spotlight2D.color = colorFromHex;
            }
        }
        else if (gameObject.tag == "MenuStartFloor")
        {
            jumpForce = 14;//12
        }
        else
        {
            floorType = Floors.FLOOR;
            jumpForce = 16.5f;//15
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y < 0)
        {
            //O platforma ilk iniþini yaptýðý an
            if (floorType == Floors.POWERUPFLOOR && GameManager.Instance.stage != GameManager.GameStage.INPOWERUP)
            {
                GameManager.Instance.stage = GameManager.GameStage.POWERUP; //PowerUp Bölümüne geçiþ
            }
            if (gameObject.tag != "MenuStartFloor")
            {
                if (floorType == Floors.JUMPERFLOOR)
                {
                    GameManager.Instance.blinkPointLight(12f, 0.35f);
                }
                else if (GameManager.Instance.stage == GameManager.GameStage.POWERUP)
                {
                    GameManager.Instance.blinkPointLight(11f, 0.25f);
                }
                else
                {
                    GameManager.Instance.blinkPointLight(6f, 0.15f);
                }
                
            }
            
            //Ses için obje zemin kontrolü
            if (floorType == Floors.JUMPERFLOOR) 
            {
                AudioManager.Instance.floorName = AudioManager.FloorName.JUMPERFLOOR;
            }
            else
            {
                AudioManager.Instance.floorName = AudioManager.FloorName.FLOOR;
            }
            AudioManager.Instance.FloorSound();

            Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 jumpSpeed = rb.velocity;
                jumpSpeed.y = jumpForce;
                rb.velocity = jumpSpeed;
                isOneJump -=1;
                
            }
            if (gameObject.tag == "Floor")
            {
                anim.SetBool("extraJumper", false); //ekstra jump animasyonunu iptal ediyoruz
                anim.SetBool("powerUp", false); //powerUp animasyonunu iptal ediyoruz
                anim.SetBool("isTrigger", true); //Yok olma animasyonunu çaðýrdý
                Destroy(gameObject, 1.1f);         //Yok etti
            }
        }
        if (isOneJump == 0 && gameObject.tag != "MenuStartFloor")
        {
            int randomScore = Random.Range(1, 3); //Puan
            int randomExtraJumpScore = Random.Range(3, 6); //Jumperlardan gelen puan
            if (floorType == Floors.JUMPERFLOOR) {            
                GameManager.Instance.score += randomExtraJumpScore;
                GameManager.Instance.scoreText.text = GameManager.Instance.score.ToString();
            }
            else if (floorType == Floors.POWERUPFLOOR)
            {
                GameManager.Instance.score += 10;
            }
            else
            {
                GameManager.Instance.score+= randomScore;
                GameManager.Instance.scoreText.text = GameManager.Instance.score.ToString();
            }
            FloorGenerator.numberTargerFloor++;
        }              
    }
}
