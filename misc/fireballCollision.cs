using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fonction collée aux objets qui doivent être détruits en collision avec le layer "ground"
public class fireballCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //sur collision avec sol, detruire objet
        if(collision.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            Destroy(gameObject);
        }
    }
}
