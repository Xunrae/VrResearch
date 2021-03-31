using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script collé sur les armes à feu
public class spawnBalle : MonoBehaviour
{
    //vitesse de la balle
    public float speed = 10;
    //prefab de la balle
    public GameObject bullet;
    //transform du barril pour projection
    public Transform barrel;


    //fonction qui crée une balle et la projette
    public void shoot()
    {
        //cloner une balle
        GameObject cloneBullet = Instantiate(bullet, barrel.position, barrel.rotation);
        //la mettre active
        cloneBullet.SetActive(true);

        //donner à la balle une velocité dans la direction du transform du barril
        cloneBullet.GetComponent<Rigidbody>().velocity = speed * barrel.forward;

        //detruit la balle dans 2s
        Destroy(cloneBullet, 2f);

    }
}
