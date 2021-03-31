using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

//script collé sur le gameObject gestureSpawn
public class objectSpawner : MonoBehaviour
{
    //liste d'objets qui doivent etre instantiés
    public List<GameObject> objects;

    //la main gauche
    public GameObject mainGauche;
    
    //l'instance de l'item à cloner
    private GameObject cloneItem;

    //prefab boule de feu
    public GameObject fireball;
    //l'instance de la boule
    private GameObject cloneFireball;


    //fonction param {string} : verifie quel objet dans sa liste correspond
    public void spawn(string objectName)
    {
        //pour chaque objet dans la liste
        foreach (var item in objects)
        {
            //si l'objet de la liste a le meme nom que le {string} de la fonction
            if (objectName == item.name)
            {
                //créer une instance de cet objet
                cloneItem = Instantiate(item, mainGauche.transform.position, mainGauche.transform.rotation);
                cloneItem.SetActive(true);

                //créer une instance de la boule de feu si le string recu est "o"
                if(objectName == "o")
                {
                    cloneFireball = Instantiate(fireball, mainGauche.transform.position, mainGauche.transform.rotation);
                    cloneFireball.SetActive(true);
                }
            }
        }
    }
}
