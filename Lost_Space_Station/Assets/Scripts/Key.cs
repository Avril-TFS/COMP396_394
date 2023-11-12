using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public float rotationSpeed = 10.0f;

    public bool isPickedUp = false;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player" && !isPickedUp)
        {
            isPickedUp = true;

            Destroy(gameObject);

            col.GetComponent<PlayerController>().HasKey();
        }
    }
}
