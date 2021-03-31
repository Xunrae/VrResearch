using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class locomotionController : MonoBehaviour
{
    //rayon de teleportation
    public XRController rightTeleportRay;
    //rayon de la main gauche
    public XRController leftRay;

    public InputHelpers.Button secondaryButton;
    public InputHelpers.Button triggerButton;

    //le seuil d'activation devrait etre plus bas que son XR controller
    public float activationThreshold = 0.1f;

    //get; set; utilisé dans l'écouteur d'évènements
    public bool enableLeftRay { get; set; } = true;
    public bool enableRightRay { get; set; } = true;

    // Update is called once per frame
    void Update()
    {
        //si le rayon teleporteur main droite est enclanché
        if (rightTeleportRay)
        {
            //active le rayon si le get; set; et le bool le permettent
            rightTeleportRay.gameObject.SetActive(enableRightRay && CheckIfActivatedRight(rightTeleportRay));
        }  

        if (leftRay)
        {
            //active le rayon si le get; set; et le bool le permettent
            leftRay.gameObject.SetActive(enableLeftRay && CheckIfActivatedLeft(leftRay));
        }   
    }

    //fonction qui retourne true/false
    public bool CheckIfActivatedRight(XRController controller)
    {
        //est-ce que le bouton secondaire (B) est pesé?
        InputHelpers.IsPressed(controller.inputDevice, secondaryButton, out bool isActivated, activationThreshold);
        return isActivated;
    }

    public bool CheckIfActivatedLeft(XRController controller)
    {
        //est-ce que le bouton Trigger est pesé?
        InputHelpers.IsPressed(controller.inputDevice, triggerButton, out bool isActivated, activationThreshold);
        return isActivated;
    }
}
