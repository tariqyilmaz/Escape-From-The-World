using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class DeletorCommand : MonoBehaviour
{
    float deletorMaxHeight = -30;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor") || collision.CompareTag("MoveFloor")) //Buraya zemin türlerinin hepsi yazýlmalý
        {
            FloorGenerator.numberTargerFloor++;
            Destroy(collision.gameObject);
        }
    }

    private void Update()
    {
        isDeath();
    }

    private void isDeath()
    {
        if (gameObject.transform.position.y>deletorMaxHeight)
        {
           deletorMaxHeight = gameObject.transform.position.y;
        }      
        
    }
}
