using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Math = System.Math;

public class FloorGenerator : MonoBehaviour
{
    [Header("Objects")] //Floorlarý buraya yazýyoruz
    [SerializeField]private GameObject floor;
    [SerializeField]private GameObject moveFloor;

    [Header("Others")]
    public float floorWidth;
    public float minimumy, maximumy;
    public static int numberTargerFloor = 0; //Yeni floor spawnlamak için kontrol saðlýyor
    Vector3 spawnLocation = new Vector3();

    int previousFloorX = 0;

    private void Start()
    {
        if (gameObject.tag != "MenuStartFloor") //Menü sahnesinde floor oluþturmamasý için
        {
            spawnFloor(8);
        }      
    }

    private void FixedUpdate()
    {
        if (numberTargerFloor == 5)
        {
            numberTargerFloor = 0;
            spawnFloor(5);
        }
    }

    private void spawnFloor(int numberFloor)
    {
        for (int i = 0; i < numberFloor; i++)
        {
            spawnLocation.y += Random.Range(minimumy, maximumy); //Zemin yüksekliklerini belirliyor
            spawnLocation.x = Random.Range(-5,5);

            if (Math.Abs(spawnLocation.x) + Math.Abs(previousFloorX) > 8) //Zeminler arasý mesafenin çok olmamasý için kontrol
            {
                if (Math.Sign(previousFloorX) == 1)
                {
                    spawnLocation.x = -4;
                }
                else
                {
                    spawnLocation.x = +4;
                }
                previousFloorX = (int)spawnLocation.x;
            }
            else
            {
                previousFloorX = (int)spawnLocation.x;
            }

            if (GameManager.Instance.score >= 8)
            {
                int selectObject = Random.Range(0,5); //Gelen sayý hangi objenin spawnlanacaðýný belirleyecek 

                switch (selectObject) //Birden fazla objeyi spawnlama iþlemi yapýlacak
                {
                    case 0:
                        Instantiate(floor, spawnLocation, Quaternion.identity);
                        break;
                    case 1:
                        Instantiate(floor, spawnLocation, Quaternion.identity);
                        break;
                    case 2:
                        Instantiate(floor, spawnLocation, Quaternion.identity);
                        break;
                    case 3:
                        Instantiate(floor, spawnLocation, Quaternion.identity);
                        break;
                    case 4:
                        Instantiate(moveFloor, spawnLocation, Quaternion.identity);
                        break;
                }        
            }
            else
            {
                Instantiate(floor, spawnLocation, Quaternion.identity);
            }           
        }
    }
}
