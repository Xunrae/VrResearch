using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//script collé au GameObject DragAnchor
public class dragInteractable : XRBaseInteractable
{

    //réécrire la fonction OnHoverEntered de XRBaseInteractable
    protected override void OnHoverEntered(XRBaseInteractor interactor)
    {
        base.OnHoverEntered(interactor);

        //XRDirectInteractor = une manette
        if (interactor is XRDirectInteractor)
        {
            //la main qui tire du script dragMove devient la manette en question
            dragMove.draggingHand = interactor.GetComponent<XRController>();
        }
    }


    //réécrire la fonction OnHoverExited de XRBaseInteractable
    protected override void OnSelectExited(XRBaseInteractor interactor)
    {
        base.OnHoverExited(interactor);

        //si c'est une manette
        if (interactor is XRDirectInteractor)
        {
            //s'il y a une main qui tire ET que cette main a le meme nom que l'interacteur courant
            if (dragMove.draggingHand && dragMove.draggingHand.name == interactor.name)
            {
                //draggingHand = null == mouvement continu activé
                dragMove.draggingHand = null;
            }
        }
    }
}