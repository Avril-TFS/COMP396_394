using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public PlayerController playerController;
    public float openDoorDistance = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {
        
        if(col.gameObject.tag == "Player" && playerController.CheckHasKey())
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        transform.Translate(Vector3.down * openDoorDistance);
    }
}
