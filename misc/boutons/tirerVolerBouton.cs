using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script collé au bouton tirer/voler
public class tirerVolerBouton : MonoBehaviour
{

    //fonction qui change la variable globale
    public void tirerVoler()
    {
        variablesGlobales.voler = !variablesGlobales.voler;
    }
}
