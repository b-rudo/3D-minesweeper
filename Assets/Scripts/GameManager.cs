/* 8/27/2020 */
/* B. Rudolph */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Set in inspector
    [Header("--- Prefab References --- ")]
    public GameObject basicCubePrefab;

    [Header("--- Obj Script References --- ")]
    public InputAndCameraManager inputCamManager;

    [Header("--- Dynamic Variables --- ")]
    public int numRowsCols = 5;
    public float mineQuantityVal = 0.2065f;

    // Private vars
    private float cubeLengthWidthHeight = 0;
    private int numMines;
    private bool minesHaveBeenPlanted = false;
    private GameObject cubeGridHolder;
    private GameObject mineHolder;
    private GameObject centerGridCube;
    private GameObject[,,] correspondingCubeGrid3DArray;

    // Start is called before the first frame update
    private void Start()
    {
        // Init variables
        cubeLengthWidthHeight = basicCubePrefab.transform.localScale.x;
        correspondingCubeGrid3DArray = new GameObject[numRowsCols, numRowsCols, numRowsCols];

        /* numMines is the total number of cubes times the special mine
         * quantity val */
        numMines = (int)((numRowsCols * numRowsCols * numRowsCols) * mineQuantityVal);

        // Set up our game
        initialSetup();
    }

    /// <summary>
    /// 
    /// </summary>
    private void initialSetup()
    {
        // Create a holder gameobject for our cube grid
        cubeGridHolder = new GameObject();
        cubeGridHolder.name = "Cube Grid Holder";

        mineHolder = new GameObject();
        mineHolder.name = "Mine Holder";
        mineHolder.transform.SetParent(cubeGridHolder.transform);

        buildCubeGrid();

        // Once initial setup is finished, start the game
        inputCamManager.setPlayerInputAllowed(true);
    }

    /// <summary>
    /// 
    /// </summary>
    private void buildCubeGrid()
    {
        float originalSpawnX = 1;
        float originalSpawnY = 1;
        float originalSpawnZ = 1;

        float currentSpawnX = originalSpawnX;
        float currentSpawnY = originalSpawnY;
        float currentSpawnZ = originalSpawnZ;
        int currentLayer = 1;
        bool searchForMiddleCube = false;

        /* First we need to determine where exactly the center of the cube grid
         * will be; this is important since the camera will always be oriented 
         * towards the center */
        int gridCenterVal = (numRowsCols / 2) + 1;
        if (gridCenterVal == currentLayer) searchForMiddleCube = true;

        /* Start building the 3D cube grid by starting in the bottom-left,
         * front-most corner and build each horizontal layer, one on top of the
         * other */

        bool finishedGrid = false;

        while (!finishedGrid)
        {

            for (int i = 1; i <= numRowsCols; i++)
            {
                for (int j = 1; j <= numRowsCols; j++)
                {
                    GameObject currentCube = Instantiate(basicCubePrefab, new Vector3(currentSpawnX, currentSpawnY, currentSpawnZ), Quaternion.identity);
                    currentCube.transform.SetParent(cubeGridHolder.transform);
                    // Set the cube's index and type
                    currentCube.GetComponent<CubeIdentifier>().cubeXIndex = j-1;
                    currentCube.GetComponent<CubeIdentifier>().cubeYIndex = currentLayer-1;
                    currentCube.GetComponent<CubeIdentifier>().cubeZIndex = i-1;
                    currentCube.GetComponent<CubeIdentifier>().cubeType = CubeIdentifier.cubeTypes.safe;
                    // Place the cube in the array
                    correspondingCubeGrid3DArray[j-1, currentLayer-1, i-1] = currentCube;

                    // Check if the current cube is the MIDDLEMOST
                    if (searchForMiddleCube)
                    {
                        if (i == gridCenterVal && j == gridCenterVal)
                        {
                            centerGridCube = currentCube;
                            currentCube.name = "MIDDLEMOST";
                            inputCamManager.setCenterCubeReference(currentCube);
                            searchForMiddleCube = false;
                        }
                    }

                    currentSpawnX += cubeLengthWidthHeight;
                }
                currentSpawnZ += cubeLengthWidthHeight;
                /* Incrementing z means we have need to reset our x to 
                 * keep everything in line */
                currentSpawnX = originalSpawnX;
            }

            /* Exiting the nested for loop indicates a completed layer */
            currentLayer += 1;
            if (!searchForMiddleCube && currentLayer == gridCenterVal)
                searchForMiddleCube = true;

            // Exit condition
            if (currentLayer > numRowsCols)
                finishedGrid = true;
            else
            {
                /* Once we finish one horizontal layer, we need to reset and
                 * go upwards */
                currentSpawnX = originalSpawnX;
                currentSpawnZ = originalSpawnZ;
                currentSpawnY += cubeLengthWidthHeight;
            }
        } 
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mineCoords"></param>
    private void givenMineCoordsUpdateAllContactingCubes(int xCoord, int yCoord, int zCoord)
    {
        // Y
        updateContactingCubesInValidParamLayer(xCoord, yCoord, zCoord);

        // Y - 1
        if (yCoord - 1 > -1)
            updateContactingCubesInValidParamLayer(xCoord, yCoord - 1, zCoord);
        // Y + 1
        if (yCoord + 1 < numRowsCols)
            updateContactingCubesInValidParamLayer(xCoord, yCoord + 1, zCoord);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xCoord"></param>
    /// <param name="zCoord"></param>
    /// <param name="yLayer"></param>
    private void updateContactingCubesInValidParamLayer(int xCoord, int yLayer, int zCoord)
    {

        // X //////
        /* Check the base parameter location, as if we are on a layer above or
         * below the mine, the spot might not be a mine */
        incrementContactingCubesForSingleParamLocation(xCoord, yLayer, zCoord);
        // x, z + 1
        if (zCoord + 1 < numRowsCols)
            incrementContactingCubesForSingleParamLocation(xCoord, yLayer, zCoord + 1);
        // x, z - 1
        if (zCoord - 1 > -1)
            incrementContactingCubesForSingleParamLocation(xCoord, yLayer, zCoord - 1);

        // X PLUS ONE //////
        if (xCoord + 1 < numRowsCols)
        {
            // x + 1, z
            incrementContactingCubesForSingleParamLocation(xCoord + 1, yLayer, zCoord);

            // x + 1, z + 1
            if (zCoord + 1 < numRowsCols)
                incrementContactingCubesForSingleParamLocation(xCoord + 1, yLayer, zCoord + 1);
     
            // x + 1, z - 1
            if (zCoord - 1 > -1)
                incrementContactingCubesForSingleParamLocation(xCoord + 1, yLayer, zCoord - 1);
        }

        // X MINUS ONE //////
        if (xCoord - 1 > -1)
        {
            // x - 1, z
            incrementContactingCubesForSingleParamLocation(xCoord - 1, yLayer, zCoord);

            // x - 1, z + 1
            if (zCoord + 1 < numRowsCols)
                incrementContactingCubesForSingleParamLocation(xCoord - 1, yLayer, zCoord + 1);

            // x - 1, z - 1
            if (zCoord - 1 > -1)
                incrementContactingCubesForSingleParamLocation(xCoord - 1, yLayer, zCoord - 1);
        }
    }

    /// <summary>
    /// REQUIRES: Check to ensure x, y, z are within array bounds.
    /// </summary>
    /// <param name="xCoord"></param>
    /// <param name="yCoord"></param>
    /// <param name="zCoord"></param>
    private void incrementContactingCubesForSingleParamLocation(int xCoord, int yCoord, int zCoord)
    {
        if (correspondingCubeGrid3DArray[xCoord, yCoord, zCoord].GetComponent<CubeIdentifier>().cubeType != CubeIdentifier.cubeTypes.mine)
            correspondingCubeGrid3DArray[xCoord, yCoord, zCoord].GetComponent<CubeIdentifier>().sidesTouchingMines += 1;
    }

    /* ************************************************************************
     *                          PUBLIC FUNCTIONS
     * ***********************************************************************/

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int getNumRowsCols()
    {
        return numRowsCols;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool getMinesHaveBeenPlanted()
    {
        return minesHaveBeenPlanted;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xCoord"></param>
    /// <param name="yCoord"></param>
    /// <param name="zCoord"></param>
    public void givenCubeCoordsDeleteCloseByCubesIfSafe(int xCoord, int yCoord, int zCoord)
    {
        CubeIdentifier cubeID = correspondingCubeGrid3DArray[xCoord, yCoord, zCoord].GetComponent<CubeIdentifier>();

        // Hitting a mine means we return
        if (cubeID.cubeType == CubeIdentifier.cubeTypes.mine)
            return;

        if (cubeID.sidesTouchingMines == 0 && cubeID.gameObject.activeSelf)
        {
            cubeID.gameObject.SetActive(false);

            for (int yLayer = yCoord-1; yLayer < (yCoord + 2); yLayer++)
            {
                if (yLayer < numRowsCols && yLayer > -1)
                {

                    // X PLUS ONE
                    if (xCoord + 1 < numRowsCols)
                    {
                        // x + 1, z
                        givenCubeCoordsDeleteCloseByCubesIfSafe(xCoord + 1, yLayer, zCoord);

                        // x + 1, z + 1
                        if (zCoord + 1 < numRowsCols)
                            givenCubeCoordsDeleteCloseByCubesIfSafe(xCoord + 1, yLayer, zCoord + 1);

                        // x + 1, z - 1
                        if (zCoord - 1 > -1)
                            givenCubeCoordsDeleteCloseByCubesIfSafe(xCoord + 1, yLayer, zCoord - 1);
                    }

                    // X MINUS ONE
                    if (xCoord - 1 > -1)
                    {
                        // x - 1, z
                        givenCubeCoordsDeleteCloseByCubesIfSafe(xCoord - 1, yLayer, zCoord);

                        // x - 1, z + 1
                        if (zCoord + 1 < numRowsCols)
                            givenCubeCoordsDeleteCloseByCubesIfSafe(xCoord - 1, yLayer, zCoord + 1);

                        // x - 1, z - 1
                        if (zCoord - 1 > -1)
                            givenCubeCoordsDeleteCloseByCubesIfSafe(xCoord - 1, yLayer, zCoord - 1);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="firstCubeThatWasClicked"></param>
    public void plantMinesThroughoutGrid(GameObject firstCubeThatWasClicked)
    {
        inputCamManager.setPlayerInputAllowed(false);

        int firstClickedCubeX = firstCubeThatWasClicked.GetComponent<CubeIdentifier>().cubeXIndex;
        int firstClickedCubeY = firstCubeThatWasClicked.GetComponent<CubeIdentifier>().cubeYIndex;
        int firstClickedCubeZ = firstCubeThatWasClicked.GetComponent<CubeIdentifier>().cubeZIndex;

        int currentNumMines = 0;
        /* Randomly assign cubes as mines until we reach the maximum */
        while (currentNumMines < numMines)
        {
            int randomX = Random.Range(0, numRowsCols);
            int randomY = Random.Range(0, numRowsCols);
            int randomZ = Random.Range(0, numRowsCols);

            /* A mine cannot be placed on the first cube that was clicked,
             * nor can we place a mine on top of an already existing mine */
            if (
                (!(randomX == firstClickedCubeX && randomY == firstClickedCubeY && randomZ == firstClickedCubeZ)) &&
                (correspondingCubeGrid3DArray[randomX, randomY, randomZ].gameObject.GetComponent<CubeIdentifier>().cubeType != CubeIdentifier.cubeTypes.mine)
               )
            {
                GameObject newMine = correspondingCubeGrid3DArray[randomX, randomY, randomZ];
                newMine.GetComponent<CubeIdentifier>().cubeType = CubeIdentifier.cubeTypes.mine;
                newMine.transform.SetParent(mineHolder.transform);

                currentNumMines += 1;

                /* Now, we need to go through each cube that this newly-placed
                 * mine touches and update its value */
                givenMineCoordsUpdateAllContactingCubes(randomX, randomY, randomZ);
            }
        }
        
        // Set proper flags on exit
        minesHaveBeenPlanted = true;
        inputCamManager.setPlayerInputAllowed(true);
    }
}
