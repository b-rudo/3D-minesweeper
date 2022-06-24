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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="centerCubeReference"></param>
    public void setStartingCameraPos(GameObject centerCubeRef)
    {
        centerCubeReference = centerCubeRef;

        transform.position = new Vector3(centerCubeReference.transform.position.x,
                                         centerCubeReference.transform.position.y,
                                         transform.position.z);
        transform.LookAt(centerCubeReference.transform);

        /* Now that our camera is in its proper starting pos, we can calculate
         * our max camera zoom levels */
        maxCameraZoomInZVal = transform.position.z + 5;
        maxCameraZoomOutZVal = transform.position.z - 15;
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
