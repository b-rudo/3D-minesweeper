/* 8/27/2020 */
/* B. Rudolph */

using TMPro;
using UnityEngine;

public class CubeIdentifier : MonoBehaviour
{
    public enum cubeTypes { safe, mine }

    [Header("--- Main Values --- ")]
    public int cubeXIndex = 0;
    public int cubeYIndex = 0;
    public int cubeZIndex = 0;
    public int sidesTouchingMines = 0;
    public cubeTypes cubeType;

    [Header("--- References --- ")]
    public TextMeshPro textOfsidesTouchingMines;
    public Material transparentMat;
    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        textOfsidesTouchingMines.enabled = false;
    }

    private void Update()
    {
        if (textOfsidesTouchingMines.enabled)
        {
            textOfsidesTouchingMines.transform.rotation = Quaternion.LookRotation(textOfsidesTouchingMines.transform.position - Camera.main.transform.position);
        }
    }

    /* ************************************************************************
     *                          PUBLIC FUNCTIONS
     * ***********************************************************************/

    /// <summary>
    /// 
    /// </summary>
    public void showSidesTouchingMinesText()
    {
        textOfsidesTouchingMines.text = sidesTouchingMines.ToString();
        textOfsidesTouchingMines.enabled = true;

        // Change our material to transparent to see the number
        GetComponent<Renderer>().material = transparentMat;
    }
}
