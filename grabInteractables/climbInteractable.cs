using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//script collé aux objets qui peuvent etre "pris" pour monter
public class climbInteractable : XRBaseInteractable
{

    //réécrire la fonction OnSelectEntered de XRBaseInteractable
    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        base.OnSelectEntered(interactor);

        // si interactor = XRDirectInteractor = une manette
        if(interactor is XRDirectInteractor) { 
            //envoyer cette manette à climbScript
            climbScript.climbingHand = interactor.GetComponent<XRController>();
        }
    }


    //réécrire la fonction OnSelectExited de XRBaseInteractable
        //donc quand on lâche l'objet
    protected override void OnSelectExited(XRBaseInteractor interactor)
    {
        base.OnSelectExited(interactor);

        if(interactor is XRDirectInteractor)
        {
            //s'il y a une main qui fait grimper ET que le nom de la main qui grimpe est celle de l'interactor
            if(climbScript.climbingHand && climbScript.climbingHand.name == interactor.name)
            {
                //la main du script devient null == mouvement continu actif
                climbScript.climbingHand = null;
            }
        }
    }
}
