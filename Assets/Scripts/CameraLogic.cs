/* 8/28/2020 */
/* B. Rudolph */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : Singleton<CameraLogic>
{

    private GameObject centerCubeReference = null;

    private float maxCameraZoomInZVal;
    private float maxCameraZoomOutZVal;

    /// <summary>
    /// 
    /// </summary>
    public void onReturnToMenuAfterGame()
    {
        transform.position += new Vector3(-3, 0, 0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="centerCubeReference"></param>
    public void setStartingCameraPos(GameObject centerCubeRef, int numRowsCols, float cubeLengthWidthHeight, float interCubeSpacing)
    {
        centerCubeReference = centerCubeRef;

        int halfNumRowsCols = (numRowsCols / 2);

        float finalZVal = centerCubeRef.transform.position.z - (halfNumRowsCols * cubeLengthWidthHeight * (interCubeSpacing * halfNumRowsCols)) - (3.5f * halfNumRowsCols);

        transform.position = new Vector3(centerCubeReference.transform.position.x,
                                         centerCubeReference.transform.position.y,
                                         finalZVal);
        transform.LookAt(centerCubeReference.transform);

        /* Now that our camera is in its proper starting pos, we can calculate
         * our max camera zoom levels */
        maxCameraZoomInZVal = centerCubeRef.transform.position.z - (cubeLengthWidthHeight + 2);
        maxCameraZoomOutZVal = transform.position.z - 10;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inOrOut"></param>
    /// <returns></returns>
    public bool attemptZoomInOrOut(char inOrOut)
    {
        float addToZVal = 0.5f;

        if (inOrOut == '-' && transform.position.z >= maxCameraZoomOutZVal)
            addToZVal *= -1;
        else if (inOrOut == '+' && transform.position.z <= maxCameraZoomInZVal)
            ;
        else
            return false;
        
        transform.position = new Vector3(transform.position.x,
                                         transform.position.y,
                                         transform.position.z + addToZVal);

        return true;
    }
}
