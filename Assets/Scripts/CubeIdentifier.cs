/* 8/27/2020 */
/* B. Rudolph */

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CubeIdentifier : MonoBehaviour
{
    public enum cubeTypes { safe, mine }
    private const float mouseHoverCubeGrowFactor = 0.2f;

    [Header("--- Main Values --- ")]
    public int test = 0;
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
    public GameObject mineObject;

    IDictionary<int, Color> sidesTouchingMinesColorDict = new Dictionary<int, Color>();

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        sidesTouchingMinesColorDict.Add(1, new Color(0, 0, 255));
        sidesTouchingMinesColorDict.Add(2, new Color(0, 123, 0));
        sidesTouchingMinesColorDict.Add(3, new Color(255, 0, 0));
        sidesTouchingMinesColorDict.Add(4, new Color(0, 0, 123));
        sidesTouchingMinesColorDict.Add(5, new Color(123, 0, 0));
        sidesTouchingMinesColorDict.Add(6, new Color(0, 123, 123));
        sidesTouchingMinesColorDict.Add(7, new Color(0, 0, 0));
        sidesTouchingMinesColorDict.Add(8, new Color(123, 123, 123));

        // FIXME - Colors through 26!!

        mineObject.SetActive(false);
    }

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
        if (InputAndCameraManager.Instance.getCanRevealOrFlagCubes())
            transform.localScale += new Vector3(mouseHoverCubeGrowFactor,
                                                mouseHoverCubeGrowFactor,
                                                mouseHoverCubeGrowFactor);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnMouseExit()
    {
        if (InputAndCameraManager.Instance.getCanRevealOrFlagCubes())
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
    public void revealMine()
    {
        if (cubeType != cubeTypes.mine) return;

        mineObject.SetActive(true);

        // Change our material to transparent to see the number
        GetComponent<Renderer>().material = transparentMat;
    }

    /// <summary>
    /// 
    /// </summary>
    public void showSidesTouchingMinesText()
    {
        textOfsidesTouchingMines.text = sidesTouchingMines.ToString();
        textOfsidesTouchingMines.enabled = true;

        // First set our text color correctly
        if (sidesTouchingMinesColorDict.ContainsKey(sidesTouchingMines))
        {
            textOfsidesTouchingMines.color = sidesTouchingMinesColorDict[sidesTouchingMines];
        }

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
    public bool reverseMineFlagOnOrOff()
    {
        parentOfMineXFlags.SetActive(!parentOfMineXFlags.activeSelf);
        return parentOfMineXFlags.activeSelf;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool getIsMineFlagged()
    {
        return parentOfMineXFlags.activeSelf;
    }
}
