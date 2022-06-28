/* 8/27/2020 */
/* B. Rudolph */

using TMPro;
using UnityEngine;

public class CubeIdentifier : MonoBehaviour
{
    public enum cubeTypes { safe, mine }
    private const float mouseHoverCubeGrowFactor = 0.2f;

    [Header("--- Main Values --- ")]
    public int cubeXIndex = 0;
    public int cubeYIndex = 0;
    public int cubeZIndex = 0;
    public int sidesTouchingMines = 0;
    public cubeTypes cubeType;

    [Header("--- References --- ")]
    public TextMeshPro textOfsidesTouchingMines;
    public GameObject parentOfMineXFlags;   
    public Material transparentMat;
    public GameObject cubeCrumblePSobj;
    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        textOfsidesTouchingMines.enabled = false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        if (textOfsidesTouchingMines.enabled)
        {
            textOfsidesTouchingMines.transform.rotation = Quaternion.LookRotation(textOfsidesTouchingMines.transform.position - Camera.main.transform.position);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnMouseEnter()
    {
        transform.localScale += new Vector3(mouseHoverCubeGrowFactor,
                                            mouseHoverCubeGrowFactor,
                                            mouseHoverCubeGrowFactor);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnMouseExit()
    {
        transform.localScale -= new Vector3(mouseHoverCubeGrowFactor,
                                            mouseHoverCubeGrowFactor,
                                            mouseHoverCubeGrowFactor);
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

        // If we were flagged make sure to turn it off
        parentOfMineXFlags.SetActive(false);

        /* Also turn off our collider so players can "click through" the
         * transparency */
        GetComponent<BoxCollider>().enabled = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void reverseMineFlagOnOrOff()
    {
        parentOfMineXFlags.SetActive(!parentOfMineXFlags.activeSelf);
    }

    /// <summary>
    /// 
    /// </summary>
    public void totallyRemoveCube()
    {
        cubeCrumblePSobj.SetActive(true);
    }
}
