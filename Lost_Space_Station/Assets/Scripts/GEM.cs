using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GEM : MonoBehaviour
{

    public float rotationSpeed = 10.0f;

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {

            SceneManager.LoadScene("Victory");

        }
    }
}
