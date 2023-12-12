using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryCamera : MonoBehaviour
{
    float speed = 5.0f;

    public Vector3 start = new Vector3(0f,-45f,0f);
    public Vector3 end = new Vector3(0f, 45f, 0f);

    private bool rotateFinished = true;

    // Start is called before the first frame update
    void Start()
    {
        //transform.rotation = Quaternion.Euler(startRotation);
    }

    // Update is called once per frame
    void Update()
    {
        float rotate = speed * Time.deltaTime;

        Vector3 swapDirection = rotateFinished ? end : start;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(swapDirection), rotate);

        if(Quaternion.Euler(swapDirection) == transform.rotation)
        {
            rotateFinished = !rotateFinished;
        }
    }
}
