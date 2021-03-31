using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//script collé aux 2e poignées des objets à 2 mains
public class twoHandGrabInteractable : XRGrabInteractable
{
    //nouvelle liste de simpleInteractables
    public List<XRSimpleInteractable> secondHandGrabPoints = new List<XRSimpleInteractable>();
    
    //la 2e main qui ira sur l'objet
    private XRBaseInteractor secondInteractor;

    //rotation initial de l'objet
    private Quaternion attachInitialLocalRot;
    
    //enumeration des types de rotation pour la main
    public enum TwoHandRotationType { None,First,Second};
    public TwoHandRotationType twoHandRotationType;

    public bool snapToSecondHand = true;

    // Start is called before the first frame update
    void Start()
    {
        //va donner un eventListener sur selectEntered et Exited du 2e point de poigne
        foreach (var item in secondHandGrabPoints)
        {
            item.onSelectEntered.AddListener(OnSecondHandGrab);
            item.onSelectExited.AddListener(OnSecondHandRelease);
        }

    }


    //réécris ProcessInteractable de XRGrabInteractable
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        //s'il y a 2 manettes qui interagissent
        if(secondInteractor && selectingInteractor)
        {
            //la rotation de l'objet devient celle définie par la fonction
            selectingInteractor.attachTransform.rotation = GetTwoHandRotation();
        }

        base.ProcessInteractable(updatePhase);
    }

    //rotation de l'objet à deux mains
    private Quaternion GetTwoHandRotation()
    {
        Quaternion targetRot;

        //aucune rotation
        if (twoHandRotationType == TwoHandRotationType.None)
        {
            // quaternion du point entre les deux manettes
            targetRot = Quaternion.LookRotation(secondInteractor.attachTransform.position - selectingInteractor.attachTransform.position);
        }
        //rotation par la main droite
        else if (twoHandRotationType == TwoHandRotationType.First)
        {
            //quaternion entre les deux manettes ET prend la rotation de la manette droite
            targetRot = Quaternion.LookRotation(secondInteractor.attachTransform.position - selectingInteractor.attachTransform.position, selectingInteractor.attachTransform.up);
        }
        //rotation par la main gauche
        else
        {
            //quaternion entre les deux manettes ET prend la rotation de la manette gauche
            targetRot = Quaternion.LookRotation(secondInteractor.attachTransform.position - selectingInteractor.attachTransform.position, secondInteractor.attachTransform.up);
        }
        //renvoie targetRot
        return targetRot;
    }

    //fonctions envoyées dans L'eventListener
    //quand on prend l'objet
    public void OnSecondHandGrab(XRBaseInteractor interactor)
    {
        //2e interacteur = main qui interagit
        secondInteractor = interactor;
    }
    //quand on le lache
    public void OnSecondHandRelease(XRBaseInteractor interactor)
    {
        //2e interacteur = aucun
        secondInteractor = null;
    }

    //réécrire OnSelectEntering donc avec la 1ere main
    protected override void OnSelectEntering(XRBaseInteractor interactor)
    {
        base.OnSelectEntering(interactor);

        //la rotation locale de l'objet = celle de la manette
        attachInitialLocalRot = interactor.attachTransform.localRotation;
    }

    //réécrire OnSelectExiting donc avec la 1ere main
    protected override void OnSelectExiting(XRBaseInteractor interactor)
    {
        base.OnSelectExiting(interactor);
        //lâche aussi avec la 2e main
        secondInteractor = null;

        //l'objet reprend sa rotation initiale
        interactor.attachTransform.localRotation = attachInitialLocalRot;
    }


    //réécrire IsSelectableBy donc avec la 1ere main
    public override bool IsSelectableBy(XRBaseInteractor interactor)
    {
        //est-ce qu'il y a une main principale ET que la manette qui interagit n'est pas la manette principale
        bool isGrabbed = selectingInteractor && !interactor.Equals(selectingInteractor);
        return base.IsSelectableBy(interactor) && !isGrabbed;
    }
}
