using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script installé sur cubeActiveDistance
public class simpleParticleToggle : MonoBehaviour
{
    public GameObject particles;
    private bool isOn = false;

    //fonction qui active ou desactive l'objet "particles"
   public void toggleOnOff()
    {
        isOn = !isOn;

        if (isOn)
        {
            particles.SetActive(true);
        }
        else
        {
            particles.SetActive(false);
        }
    }
}
