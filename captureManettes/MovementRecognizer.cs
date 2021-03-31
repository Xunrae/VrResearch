using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using PDollarGestureRecognizer;
using System.IO;
using UnityEngine.Events;

public class MovementRecognizer : MonoBehaviour
{
    //la manette qui doit etre activée
    public XRNode inputSource;
    //le bouton qui doit etre pesé
    public InputHelpers.Button inputButton;
    //seuil d'activation
    public float inputTreshold = 0.1f;
    //source de mouvement (maingauche.transform)
    public Transform moveSource;

    //distance minimale pour le prochain point à lister
    public float newPositionTresholdDistance = 0.05f;

    //prefab de cube (debug, pas utilisé)
    public GameObject cubePrefab;

    //le tracé créé par la main
        //le parent = la main
    public GameObject trailParent;
        //le prefab
    public GameObject trail;
        //un clone pas encore créé
    private GameObject trailClone;

    //mode de creation d'un glyphe
    public bool creationMode = true;
    //le nom du glyphe
    public string newGestureName;

    //seuil de reconnaissance du glyphe
    public float recognitionThreshold = 0.85f;

    //utilisation de Serializable pour pouvoir essayer de lire un glyph donné par le joueur
        //traduire une structure ou un objet en un format qui peut etre sauvegardé
    [System.Serializable]
    //assigner qu'on veut implementer un UnityEvent qui prend un String
    public class UnityStringEvent : UnityEvent<string>{}
    //nommer cet evenement OnRecognized
    public UnityStringEvent OnRecognized;

    //liste de gestes possibles
    private List<Gesture> trainingSet = new List<Gesture>();

    //bool pour definir si un geste est commencé
    private bool inMotion = false;

    //liste de positions en vector3 qui contiendra chaque point
    private List<Vector3> positionList = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        //gestureFiles est un Array qui contient tous les fichiers dans le dossier choisi (qui finissent en .xml)
        string[] gestureFiles = Directory.GetFiles(Application.dataPath+"/gestures/", "*.xml");

        //pour chacun de ces documents
        foreach (var item in gestureFiles)
        {
            //ajoute ces fichiers à la liste de glyphes connus/ qui peuvent faire quelque chose
            trainingSet.Add(GestureIO.ReadGestureFromFile(item));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //est-ce que le jouer pese sur le bouton choisi? (Y)
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), inputButton, out bool isPressed, inputTreshold);

        //le mouvement commence
        if(!inMotion && isPressed)
        {
            startMove();
        }
        //le joueur a fini le mouvement
        else if(inMotion && !isPressed)
        {
            endMove();
        }
        //rafraichit le mouvement
        else if(inMotion && isPressed)
        {
            updateMove();
        }
    }

    //fonction qui commence la lecture du glyphe donné par le joueur
    void startMove()
    {
        //on est en mouvement
        inMotion = true;
        //vider la liste de positions remplie par le joueur
        positionList.Clear();
        
        //commencer par ajouter un point à la position de la main
        positionList.Add(moveSource.position);

       // if (cubePrefab)
       // {
        //    Destroy(Instantiate(cubePrefab, moveSource.position, Quaternion.identity), 3f);

        //}

        //si le prefab trail existe
        if (trail)
        {
            //instantier un clone du tracé à la position de la main
            trailClone = Instantiate(trail, moveSource.position, Quaternion.identity);
            //changer le parent du tracé pour la main
            trailClone.transform.parent = trailParent.transform;
            //la position du tracé est la meme que celle de la main
            trailClone.transform.position = trailParent.transform.position;
        }
    }

    //fonction qui termine le mouvement et essaie de reconnaître ou créer le glyphe donné par le joueur
    void endMove()
    {
        //on termine le mouvement
        inMotion = false;

        //creer un tableau qui a la grandeur du tableau de points créé en start+update
        Point[] pointsArray = new Point[positionList.Count];

        //pour chaque point de la liste de positions
        for (int i = 0; i < positionList.Count; i++)
        {
            //le point dans l'ecran devient la position en i traduite du monde vers l'ecran
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(positionList[i]);

            //on ne veut garder que le x et y de ce point pour la reconnaissance
            pointsArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
        }

        //le mouvement donné = aux points décrits dans la boucle ci-haut
        Gesture newGesture = new Gesture(pointsArray);

        //si on est en mode de creation de glyphe
        if (creationMode)
        {
            //le nom du glyph devient celui choisi dans la console
            newGesture.Name = newGestureName;
            //rajouter ce glyph à la liste de glyphes connus
            trainingSet.Add(newGesture);

            //créer un fichier xml qui aura le nom indiqué et qui contiendra la liste de points
            string fileName = Application.dataPath + "/gestures/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointsArray, newGestureName, fileName);
        }

        //si on n'est pas en mode creatif
        else
        {
            //besoin de >1 pour faire le calcul, sinon crash (on peut pas calculer le chemin entre 2 points s'il n'y a pas 2 points!)
            if (pointsArray.Length > 1)
            {
                //Provient du code PDollar
                //fonction Classify de PointCloudRecognizer -> retourne la class du glyphe qui match le plus entre la gesture du joueur et celles dans la liste
                       //transferer la liste en array avec toArray()
                Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());

                //si le score de notre resultat est acceptable
                if (result.Score > recognitionThreshold)
                {
                    //invoquer la fonction implémentée dans l'inspecteur de Unity
                    OnRecognized.Invoke(result.GestureClass);
                }
            }
        }

        //detruit le tracé apres 3s
        Destroy(trailClone,3f);
        //enlève le parent du tracé
        trailClone.transform.parent = null;
    }

    //fonction qui raffraichit le geste
    void updateMove()
    {
        //aller chercher la derniere position créée dans la liste
        Vector3 lastPosition = positionList[positionList.Count - 1];

        //si la distance depasse le seuil de distance donné
        if(Vector3.Distance(moveSource.position, lastPosition) > newPositionTresholdDistance)
        {
            //rajoute ce point dans la liste
            positionList.Add(moveSource.position);

           // if (cubePrefab)
           // {
             //   Destroy(Instantiate(cubePrefab, moveSource.position, Quaternion.identity), 3f);

           // }
        }
    }
}
