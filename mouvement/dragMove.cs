using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

//script collé au VRRig
public class dragMove : MonoBehaviour
{
    //composant characterController
    private CharacterController character;
    //la main qui tire
    public static XRController draggingHand;
    public XRController rightHand;
    public XRController leftHand;

    //le script de mouvement continu
    private continuousMovvement moveScript;

    //le point d'ancrage d'ou la personne va tirer
    public GameObject dragAnchor;
    //le bouton qui sera pesé (primary dans mon cas)
    public InputHelpers.Button primaryButton;

    //le seuil d'activation
    public float activationThreshold = 0.1f;

    //bools pour savoir quel bouton primaire est actif
    public bool boutonPrimaireActifDroit;
    public bool boutonPrimaireActifGauche;

    //la velocité du geste de tirer
    private Vector3 dragVelocity;
    //vitesse de deplacement
    public float dragSpeed = 1.4f;

    //layer pour tester qu'on soit au sol
    public LayerMask groundLayer;
        
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        moveScript = GetComponent<continuousMovvement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //verifier quel bouton est actif
        boutonPrimaireActifDroit = CheckIfPrimaryActive(rightHand);
        boutonPrimaireActifGauche = CheckIfPrimaryActive(leftHand);

        //si on a activé la main droite ET qu'on est au sol OU en train de voler
       if (boutonPrimaireActifDroit && (CheckIfGrounded() || variablesGlobales.voler == true))
        {
            //la main droite devient la main qui tire
            draggingHand = rightHand;
            //desactive le script de mouvement continu
            moveScript.enabled = false;
            //active fonction drag
            drag();
        }
        //si la manette de gauche est activée  ET qu'on est au sol OU en train de voler
        else if (boutonPrimaireActifGauche && (CheckIfGrounded() || variablesGlobales.voler == true))
        {
            //la main droite devient la main qui tire
            draggingHand = leftHand;
            //desactive le script de mouvement continu
            moveScript.enabled = false;
            //active fonction drag
            drag();
        }
       //si aucune main n'est activée, on laisse le script de mouvement continu
        else { moveScript.enabled = true; }
        

    }

    //fonction de mouvement en se tirant
    void drag()
    {
        //activer le point d'ancrage
        dragAnchor.SetActive(true);

        //chercher valeur de position de la manette
        InputDevices.GetDeviceAtXRNode(draggingHand.controllerNode).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
        //position du point d'ancrage devient position de la manette
        dragAnchor.transform.position = position;

        //chercher la velocité de la manette
        InputDevices.GetDeviceAtXRNode(draggingHand.controllerNode).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity);

        //si on ne vole pas
        if(variablesGlobales.voler == false)
        {
            //enlever la velocité en Y
            dragVelocity = new Vector3(velocity.x, 0, velocity.z);
        }
        //si on vole
        else
        {
            //on prend la velocité au complet
            dragVelocity = velocity;
            //et dire au script de mouvement qu'on n'est pas en train de tomber
            moveScript.fallingSpeed = 0;
        }

        //bouger le VRRig en avec dragVelocity
        character.Move(transform.rotation * -dragVelocity * Time.fixedDeltaTime * dragSpeed);
    }

    //fonction qui teste si le bouton primaire est activé sur la manette donnée
    // param {XRController} : une manette
    public bool CheckIfPrimaryActive(XRController controller)
    {
        //est-ce que le bouton primaire A est pesé?
        InputHelpers.IsPressed(controller.inputDevice, primaryButton, out bool isActivated, activationThreshold);
        return isActivated;
    }

    //fonction pour verifier qu'on est bien au sol
    bool CheckIfGrounded()
    {
        //rayStart = point central du personnage
        Vector3 rayStart = transform.TransformPoint(character.center);
        //on veut que ce soit légerement plus bas que le perso
        float rayLength = character.center.y + 0.01f;
        //raycast en sphere, en dessous du perso
        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);

        //si ca touche la layer Ground, retourne True
        return hasHit;
    }
}
