using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAnimationController : MonoBehaviour
{
    public enum ItemType
    {
        Generic, Door
    }

    [Header("Puzzle settings")]
    public ItemType currentItemType;
    [SerializeField] private bool inverseRotation;
    [SerializeField, Range(0f, 360.0f)] private float rotationLimit = 100f;
    int rotationValue;


    [Header("General settings")]
    [SerializeField, Range(0f, 180.0f)] private float degreesPerSecond = 75f;
    [SerializeField, Range(0f, 5f)] private float verticalLength = 0.75f;
    [SerializeField, Range(0f, 5f)] private float verticalSpeed = 0.5f;

    Vector3 startPosition;


    void Start()
    {
        if (inverseRotation == true)
        {
            rotationValue = -1;
            transform.Rotate(new Vector3(0, degreesPerSecond * rotationValue * Time.deltaTime, 0), Space.World);
        }
        else rotationValue = 1;

        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
    }

    private void Animate()
    {
        if (currentItemType == ItemType.Generic)
            transform.Rotate(new Vector3(0, degreesPerSecond * rotationValue * Time.unscaledDeltaTime, 0), Space.World);
        else
        {
            if (inverseRotation)
            {
                //Debug.Log(Mathf.Round(Mathf.Abs(transform.localEulerAngles.y)));
                if (Mathf.Round(Mathf.Abs(transform.localEulerAngles.y)) > 360 - rotationLimit)
                    transform.Rotate(new Vector3(0, degreesPerSecond * rotationValue * Time.unscaledDeltaTime, 0), Space.World);
            }
            else
            {
                //Debug.Log(Mathf.Round(Mathf.Abs(transform.localEulerAngles.y)));
                if (Mathf.Round(Mathf.Abs(transform.localEulerAngles.y)) < rotationLimit)
                {
                    transform.Rotate(new Vector3(0, degreesPerSecond * rotationValue * Time.unscaledDeltaTime, 0), Space.World);
                    //Debug.Log("rotating");
                }
            }

            return;
        }


        float verticalY = Mathf.PingPong(verticalSpeed * Time.time, verticalLength);
        transform.position = new Vector3(transform.position.x, startPosition.y + verticalY, transform.position.z);
    }
}
