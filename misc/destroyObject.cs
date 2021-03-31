using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script collé aux objets qui doivent être detruits
public class destroyObject : MonoBehaviour
{
    //temps avant destruction
    public float temps = 0f;

    //fonction qui detruit le gameObject sur lequel le script en collé
    public void destroyItem()
    {
        Destroy(gameObject, temps);
    }
}
