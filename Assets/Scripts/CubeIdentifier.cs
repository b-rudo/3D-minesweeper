/* 8/27/2020 */
/* B. Rudolph */

using UnityEngine;

public class CubeIdentifier : MonoBehaviour
{
    public enum cubeTypes { safe, mine }

    public int cubeXIndex = 0;
    public int cubeYIndex = 0;
    public int cubeZIndex = 0;
    public int sidesTouchingMines = 0;
    public cubeTypes cubeType;
}
