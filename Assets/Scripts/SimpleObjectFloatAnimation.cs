/* 7/13/2021 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Attaching this script will simply make the GameObject "float" up and down. */

/* Script adapted from "Floater v0.0.2" by Donovan Keith under the 
 * [MIT License](https://opensource.org/licenses/MIT) */

public class SimpleObjectFloatAnimation : MonoBehaviour
{

    public float frequency = 1f;
    public float amplitude = 0.5f;

    private Vector3 startingPos = new Vector3();
    private Vector3 tempPos = new Vector3();

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Float up/down with a Sin()
        tempPos = startingPos;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }
}
