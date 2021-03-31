using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR;

//script collé sur les prefab handPresence
//sert à montrer les manettes ou mains dépendamment de la selection du joueur
public class HandPresence : MonoBehaviour
{
    //est-ce qu'on veut montrer des mains ou des manettes?
    private bool showControllers = false;

    //donne le choix du genre de matériel (hardware) à choisir
    //
    public InputDeviceCharacteristics controllerCharacteristics;

    //tous les prefabs qui peuvent etre utilisés
    public List<GameObject> controllerPrefabs;
    private InputDevice targetDevice;
    private GameObject clonedController;

    //mains
    public GameObject handPrefab;
    private GameObject clonedHand;

    private Animator handAnimator;

    // Start is called before the first frame update
    void Start()
    {
        tryInitialize();
    }

    // Update is called once per frame
    void Update()
    {
        //params : le bouton qu'on veut ecoute, output value WHERE output value = bool pour les boutons|| float (entre 0 et 1) pour les triggers || vector2 pour les analogues
        //if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue)
        //{
        // UnityEngine.Debug.Log("Bouton primaire enfoncé");
        //}



        //if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.1f)
        //{
        // UnityEngine.Debug.Log("Tigger pressed " + triggerValue);
        // }


        // if(targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue) && primary2DAxisValue != Vector2.zero)
        // {
        //UnityEngine.Debug.Log("Right Analog value" + primary2DAxisValue);
        //}


        //s'il n'y a pas de manettes au debut du jeu, on essaye de les initialiser comme suit :
        //si les manettes ne sont pas valides, essayes de les initialiser
        if (!targetDevice.isValid)
        {
            tryInitialize();
        }
        //on a des manettes, donc on choisit quelles on veut afficher
        else
        {
            //si on veut montrer les controller
            if (variablesGlobales.showControllers)
            {
                clonedHand.SetActive(false);
                clonedController.SetActive(true);
            }
            //si on montre les mains
            else
            {
                clonedHand.SetActive(true);
                clonedController.SetActive(false);
                updateHandAnimation();
            }
        }
    }
    
    //fonction 'onStart' qui initialize les mains ou manettes du joueur, selon la selections
    void tryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();

        //accéder à la manette de droite en hardcode
        //InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;s

        //headset et controllers
        //InputDevices.GetDevices(devices);

        //Input = manette droite
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);



        //debug pour voir quels hardwares sont allumés
        foreach (var item in devices)
        {
            UnityEngine.Debug.Log(item.name + item.characteristics);
        }

        //être sûr que le joueur a bel et bien une manette
        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            //prendre la bonne main pour cette manette
            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);
            if (prefab)
            {
                clonedController = Instantiate(prefab, transform);
            }
            else
            {
                UnityEngine.Debug.LogError("No corresponding model");
                clonedController = Instantiate(controllerPrefabs[0], transform);
            }

            clonedHand = Instantiate(handPrefab, transform);
            handAnimator = clonedHand.GetComponent<Animator>();
        }
    }


    //fonction qui change les animations de la main selon quels boutons sont pesés
    void updateHandAnimation()
    {
        //si on pese sur trigger, on change le float de l'animator au float retourné par la manette
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("trigger", 0);
        }

        //si on pese sur la Grip, on retourne à l'animator la valeur de la manette pour la Grip
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("grip", 0);
        }
    }
}
