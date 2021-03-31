using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


//À noter que ce script herite de la classe XRGrabInteractable, il contient donc tous ses paramètres
public class XROffsetGrabInteractable : XRGrabInteractable
{
    //position initiale de l'objet
    public Vector3 initialAttachLocalPos;
    //rotation initiale de l'objet
    public Quaternion initialAttachLocalRot;


    // Start is called before the first frame update
    void Start()
    {
        //crée un attachpoint si il n'y en a pas
        if (!attachTransform)
        {
            //crée un nouveau pivot à l'objet
            GameObject grab = new GameObject("Grab pivot");
            //change le parent de l'objet au rig du joueur, perd la position par rapport au monde (remplacé par celle par rapport au joueur)
            grab.transform.SetParent(transform, false);

            //paramètre de XRGrabInteractable, au lieu d'être le centre de la main c'est l'objet créé précédemment
            attachTransform = grab.transform;
        }

        //les positions initiales sont déterminées
        initialAttachLocalPos = attachTransform.localPosition;
        initialAttachLocalRot = attachTransform.localRotation;
    }


    //override la fonction OnSelectEntering de XRBaseInteractor dans XRGrabInteractable
    protected override void OnSelectEntering(XRBaseInteractor interactor)
    {
        //si l'interacteur est la main
        if(interactor is XRDirectInteractor)
        {
            //les points d'attache sont transférés à la main
            attachTransform.position = interactor.transform.position;
            attachTransform.rotation = interactor.transform.rotation;
        }
        else
        {
            //sinon, on garde les valeurs initiales
            attachTransform.position = initialAttachLocalPos;
            attachTransform.rotation = initialAttachLocalRot;
        }

        //valeur de retour originale de la fonction
        base.OnSelectEntering(interactor);
    }
}
