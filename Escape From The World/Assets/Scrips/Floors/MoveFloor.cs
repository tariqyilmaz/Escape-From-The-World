using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveFloor : MonoBehaviour
{
    [SerializeField]private float jumpForce;
    private Vector2 pointA; //MoveFloor'un gittiði yerler
    private Vector2 pointB; //MoveFloor'un gittiði yerler
    public static float floorSpeed;
    public bool isRight = false;
    private int isOneJump = 1; //Spawn için oluþturduðumuz sayýyý kontrol etmek için oluþturdum. 1 yazdýðým için bir kere dokunduðunda objeyi saymýþ oluyor.
    Animator anim;

    private void Start()
    {
        int moveLocation;
        int randomMoveDirection = Random.Range(0, 2); //Baþlatma yönleri her zaman ayný yönde olmasýn diye
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
                Vector2 jumpSpeed = rb.velocity;
                jumpSpeed.y = jumpForce;
                rb.velocity = jumpSpeed;
                isOneJump -= 1;
            }
            anim.SetBool("isTrigger", true); //Yok olma animasyonunu çaðýrdý
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
