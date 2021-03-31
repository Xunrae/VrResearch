using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

//script de mouvement collé au VRRig
public class climbScript : MonoBehaviour
{
    private CharacterController character;

    //la main qui fait grimper, envoyée par climbInteractable
    public static XRController climbingHand;

    //le scrit de mouvement continu du VRRig
    private continuousMovvement moveScript;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        moveScript = GetComponent<continuousMovvement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //s'il y a une main qui grimpe
        if (climbingHand)
        {
            //remet à 0 la vitesse de chute
            moveScript.fallingSpeed = 0;
            //enleve le mouvement continu
            moveScript.enabled = false;

            //activation mouvement en grimpant
            climb();
        }
        //si pas de main qui grimpe, mouvement continu activé
        else { moveScript.enabled = true; }
    }

    //mouvement en grimpant
    void climb()
    {
        //chercher la velocité de la manette qui grimpe
        InputDevices.GetDeviceAtXRNode(climbingHand.controllerNode).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity);
        //bouge le personnage avec la valeur cherchée en haut
        character.Move(transform.rotation * -velocity * Time.fixedDeltaTime);
    }
}
