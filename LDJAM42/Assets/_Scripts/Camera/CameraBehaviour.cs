﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public float startHeight = 10f;
    public float zoomSpeed = 2f;
    public float movementSpeed = 0.5f;
    public float rotationSpeed = 1f;

    public bool rotation = true;

    private World world;
    private bool rightButton, middleButton, rButton;
    private float xMovement, yMovement, wZoom;

    private Vector3 startPosition, startRotEuler;
    private Quaternion startRotation;
    

	// Use this for initialization
	void Start ()
    {
        world = FindObjectOfType<WorldCreation>().GetWorld();
        startPosition = new Vector3(world.sizeX / 2, startHeight, -(world.sizeZ/2));
        startRotEuler = new Vector3(55, 0, 0);
        startRotation = Quaternion.Euler(startRotEuler);
        resetCamera();
	}

    public void SetInput(bool right, bool middle, float x, float y, float w, bool bR)
    {
        rightButton = right;
        middleButton = middle;

        xMovement = x;
        yMovement = y;
        wZoom = w;

        rButton = bR;
    }

    void Update ()
    {
        if (wZoom != 0f)
            Zoom(wZoom);

        if (rightButton)
            Movement(xMovement, yMovement);
        else if (middleButton && rotation)
            Rotation(xMovement, yMovement);

        if (rButton)
            resetCameraRotation();

	}

    private void Zoom(float z)
    {
        this.transform.position += this.transform.forward * z * zoomSpeed;
    }

    private void Movement(float x, float y)
    {
        this.transform.position -= Vector3.right * x * movementSpeed;
        this.transform.position -= Vector3.forward * y * movementSpeed;
    }

    private void Rotation(float x, float y)
    {
        this.transform.Rotate(this.transform.right, -y * rotationSpeed);
        this.transform.Rotate(Vector3.up, -x * rotationSpeed);
    }

    private void resetCamera()
    {
        this.transform.position = startPosition;
        resetCameraRotation();
    }

    private void resetCameraRotation()
    {
        this.transform.rotation = startRotation;
    }
}
