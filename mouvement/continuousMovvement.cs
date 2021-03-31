using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class continuousMovvement : MonoBehaviour
{
    //vitesse appliquée au joueur
    public float speed = 1;
    //La manette qui va avoir le mouvement lié dessus
    public XRNode inputSource;
    //valeur retournée par l'analogue
    private Vector2 inputAxis;
    //controlleur du personnage (propriété propre à XR)
    private CharacterController character;
    //Le Rig en tant que tel du composant sur lequel est lié ce script
    private XRRig rig;

    //Vraie gravité physique
    public float gravity = -9.81f;
    //vitesse pour quand le joueur tombe
    public float fallingSpeed;
    //Layer du/des sols
    public LayerMask groundLayer;
    //float ajustable pour la hauteur du headset
    public float additionalHeight = 0.2f;

    //Pour savoir si Trigger est appuyé
    public InputHelpers.Button triggerButton;
    //seuil devrait être un peu en dessous de sa valeur dans XR Controller
    public float activationThreshold = 0.1f;
    //le rayon en tant que tel
    public XRController leftRay;

    // Start is called before the first frame update
    void Start()
    {
        //chercher le composant de characterController
        character = GetComponent<CharacterController>();
        //chercher le rig
        rig = GetComponent<XRRig>();
    }

    // Update is called once per frame
    void Update()
    {
        //quelle manette retourne le input
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        //si l'analogue retourne des valeurs, les mettre dans inputAxis
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

    }
    private void FixedUpdate()
    {
        //Mode de vision qui donne mal au coeur => à éviter mais disponible
            //le quaternion representant la rotation en y du headsset
            //Quaternion headYaw = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);

            //multiplier quaternion obtenu avec le vecteur
            //Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);


        //appel à la fonction qui deplace le joueur selon le deplacement du casque
        capsuleFollowHeadset();

        //chercher la rotation du Rig (rotJoueur)
        Quaternion rotJoueur = Quaternion.Euler(0, gameObject.transform.eulerAngles.y, 0);
        //multiplier la rotation par la direction renvoyée par le controlleur
        Vector3 direction = rotJoueur * new Vector3(inputAxis.x , 0, inputAxis.y);

        //Si le rayon de gauche N'EST PAS enclanché
        if (!CheckIfActivated(leftRay))
        {
            //Bouge le personnage dans la direction renvoyée par l'analogue
            character.Move(direction * Time.fixedDeltaTime * speed);
        }

        //gravite
        //est-ce que le personnage tout au sol?
        bool isOnGround = CheckIfGrounded();
        if (isOnGround)
        {
            //si oui, pas de falling speed
            fallingSpeed = 0;
        }
        //sinon, on applique la gravité par incrémentation avec le temps
        else {fallingSpeed += gravity * Time.fixedDeltaTime;}

        //on applique ensuite cette vitesse de tombée au Rig
        character.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);
    }

    //are we on the ground?
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

    //Verifie si le bouton Trigger de la manette est activé
    public bool CheckIfActivated(XRController controller)
    {
        //si le bouton trigger est appuyé sur la manette, retourne True
        InputHelpers.IsPressed(controller.inputDevice, triggerButton, out bool isActivated, activationThreshold);
        return isActivated;
    }

    //Fonction qui fait suivre le Rig avec le mouvement du headset (SUPER IMPORTANT)
    void capsuleFollowHeadset()
    {
        //la hauteur du personnage = la hauteur du rig par rapport au sol
        character.height = rig.cameraInRigSpaceHeight + additionalHeight;

        //InverseTransformPoint transforme la position du monde vers la position locale, donc à l'interieur du rig
            //on prend la valeur de la caméra (le headset)
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.cameraGameObject.transform.position);

        //le centre du personnage devient les paramètres retournés, character.skinWidth est une valeur pour empecher les bugs et doit être présente en y
        character.center = new Vector3(capsuleCenter.x, character.height / 2 + character.skinWidth, capsuleCenter.z);
    }
}
