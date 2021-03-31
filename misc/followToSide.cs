using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


//script collé aux spots de l'inventaire (à travailler)
public class followToSide : MonoBehaviour
{
    //cible pour la position
    public Transform target;
    //la position à prendre par rapport à la camera
    public Vector3 offset;

    void FixedUpdate()
    {
        //position du gameObject = celle par rapport  à la cible
        transform.position = target.position + Vector3.up * offset.y 
            + Vector3.ProjectOnPlane(target.right, Vector3.up).normalized * offset.x 
            + Vector3.ProjectOnPlane(target.forward, Vector3.up).normalized * offset.z;

        //prendre seulement la rotation en y de la cible
        transform.eulerAngles = new Vector3(0, target.eulerAngles.y, 0);
    }
}
