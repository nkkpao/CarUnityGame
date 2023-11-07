using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    float initiallyYPos;
    bool hasInitiallyYPosBeenSet;

    public virtual void Start()
    {
        Invoke("SetInitiallyYPos", 1.5f);
    }

    public virtual void Update()
    {
        if(hasInitiallyYPosBeenSet)
        {

        }
    }
}
