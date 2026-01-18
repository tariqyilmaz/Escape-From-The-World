using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveFloor : MonoBehaviour
{
    [SerializeField]private float jumpForce;
    private Vector2 pointA; //MoveFloor'un gitti�i yerler
    private Vector2 pointB; //MoveFloor'un gitti�i yerler
    public static float floorSpeed;
    public bool isRight = false;
    private int isOneJump = 1; //Spawn i�in olu�turdu�umuz say�y� kontrol etmek i�in olu�turdum. 1 yazd���m i�in bir kere dokundu�unda objeyi saym�� oluyor.
    Animator anim;

    private void Start()
    {
        int moveLocation;
        int randomMoveDirection = Random.Range(0, 2); //Ba�latma y�nleri her zaman ayn� y�nde olmas�n diye
        if (randomMoveDirection == 0)
        {
            moveLocation = 5;
        }
        else
        {
            moveLocation = -5;
        }
        pointA = new Vector2(moveLocation,transform.position.y);
        pointB = new Vector2(-moveLocation, transform.position.y);
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        MoveXFloor();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y < 0)
        {
            AudioManager.Instance.floorName = AudioManager.FloorName.MOVEFLOOR;
            AudioManager.Instance.FloorSound();
            GameManager.Instance.blinkPointLight(7f, 0.2f);

            Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 jumpSpeed = rb.linearVelocity;
                jumpSpeed.y = jumpForce;
                rb.linearVelocity = jumpSpeed;
                isOneJump -= 1;
            }
            anim.SetBool("isTrigger", true); //Yok olma animasyonunu �a��rd�
            Destroy(gameObject, 1.1f);         //Yok etti
        }

        if (isOneJump == 0)
        {
            int randomScore = Random.Range(1, 4); //Puan
            GameManager.Instance.score+=randomScore;
            GameManager.Instance.scoreText.text = GameManager.Instance.score.ToString();
            FloorGenerator.numberTargerFloor++;
        }
    }

    private void MoveXFloor()
    {
        if (isRight == false)
        {
            transform.position = Vector2.MoveTowards(transform.position, pointA, floorSpeed * Time.deltaTime);
            if (transform.position.x == pointA.x)
            {
                isRight = true;
            }
        }
        else if (isRight == true) {

            transform.position = Vector2.MoveTowards(transform.position, pointB, floorSpeed * Time.deltaTime);
            if (transform.position.x == pointB.x)
            {
                isRight = false;
            }
        }

        if (GameManager.Instance.score >= 10)
        {
            floorSpeed = 2;
        }
    }
}
