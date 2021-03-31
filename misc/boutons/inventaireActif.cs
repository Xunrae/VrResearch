using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script collé au bouton Inventaire
public class inventaireActif : MonoBehaviour
{
    public GameObject invGauche;
    public GameObject invDroit;
    private bool estActif = false;

    //fonction qui affiche l'inventaire
    public void changerInv()
    {
        //estActif devient son inverse
        estActif = !estActif;

        //active ou desactive les inventaires
        invDroit.SetActive(estActif);
        invGauche.SetActive(estActif);
    }
}
