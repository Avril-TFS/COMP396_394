using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : MonoBehaviour
{
    public float rotationSpeed = 10.0f;
    public int healing = 50;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            Destroy(gameObject);
            
            PlayerController playerController = col.GetComponent<PlayerController>();
            if(playerController != null)
            {
                playerController.Heal(healing);
            }
        }
    }
}
