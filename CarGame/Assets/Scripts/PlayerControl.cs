using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    public float drivingSpeed = 10;
    public float rotationSpeed = 60;
    public float verticalMove, horizontalMove;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        verticalMove = Input.GetAxis("Vertical") * drivingSpeed * Time.deltaTime;
        transform.Rotate(0, horizontalMove, 0);
        transform.Translate(0, 0, verticalMove);
    }
}
