using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    private SpawnManager spawnManager;
    [SerializeField] float rotationSpeedMenu = 10;
    public float rotationSpeed = 40;
    public Joystick joystick;

    // Start is called before the first frame update
    void Awake()
    {
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnManager.isGameActive)
        {
            //Camera rotates upon a fixed point by horizontal inputs.
            float horizontalInput = joystick.Horizontal;
            transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(Vector3.up * rotationSpeedMenu * Time.deltaTime);
        }
    }
}
