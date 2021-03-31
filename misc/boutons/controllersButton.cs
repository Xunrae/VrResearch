using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script du bouton qui change l'apparence des mains
//change la variable globale pour les mains
public class controllersButton : MonoBehaviour
{
    public void toggleControllers()
    {
        variablesGlobales.showControllers = !variablesGlobales.showControllers;
    }
}
