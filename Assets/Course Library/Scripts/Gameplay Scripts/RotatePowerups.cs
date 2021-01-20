using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePowerups : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 7.5f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}