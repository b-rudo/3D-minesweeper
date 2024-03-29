﻿/* 8/28/2020 */
/* B. Rudolph */
/* Code modified from: 
 https://answers.unity.com/questions/1257281/how-to-rotate-camera-orbit-around-a-game-object-on.html 
 */
using UnityEngine;

public class DragToRotate : Singleton<DragToRotate>
{
    [Header("*** MUST BE ATTACHED TO CUBE GRID HOLDER PARENT")]

    public float distance = 2.0f;
    public float xSpeed = 30.0f;
    public float ySpeed = 30.0f;
    public float yMinLimit = -90f;
    public float yMaxLimit = 90f;
    public float distanceMin = 10f;
    public float distanceMax = 10f;
    public float smoothTime = 20f;

    private float rotationYAxis = 0.0f;
    private float rotationXAxis = 0.0f;
    private float velocityX = 0.0f;
    private float velocityY = 0.0f;

    private GameObject centerCubeReference = null;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationYAxis = angles.y;
        rotationXAxis = angles.x;
    }

    /// <summary>
    /// 
    /// </summary>
    void LateUpdate()
    {
        if (InputAndCameraManager.Instance.getPlayerInputAllowed())
        {
            if (centerCubeReference)
            {
                if (Input.GetMouseButton(0))
                {
                    velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.02f;
                    velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
                }
                rotationYAxis += velocityX;
                rotationXAxis -= velocityY;
                //rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
                Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
                Quaternion rotation = toRotation;

                transform.rotation = rotation;
                velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
                velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
            }
        }
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    public void setCenterCubeReference(GameObject obj)
    {
        centerCubeReference = obj;
    }
}