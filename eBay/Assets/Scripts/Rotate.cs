﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 speed; // RPS 
    
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(speed*Time.deltaTime * 360);
    }
}