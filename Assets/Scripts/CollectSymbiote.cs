using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectSymbiote : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}