/* 8/27/2020 */
/* B. Rudolph */

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    private const float interCubeSpacingIncrementVal = 0.1f;
    private const float interCubeSpacingMin = 0.1f;
    private const float interCubeSpacingMax = 2f;

    // Set in inspector
    [Header("--- Prefab References --- ")]
    public GameObject basicCubePrefab;

    [Header("--- Dynamic Variables --- ")]
    public int numRowsCols = 5;
    public int numMines = 1;

    [Header("--- Obj References --- ")]
    public GameObject allCubeHolder;

    [Header("--- Main Canvas References --- ")]
    public GameObject mainCanvas;

    public Slider numRowsColsSlider;
    public TextMeshProUGUI numRowsColsTxt;

    public Slider numMinesSlider;
    public TextMeshProUGUI numMinesTxt;

    [Header("--- Hud Canvas References --- ")]
    public GameObject hudCanvas;
    public TextMeshProUGUI timerTxt;
    public TextMeshProUGUI mineFlagsPlacedTxt;
    public GameObject youWonTxt;
    public GameObject youLostTxt;
    public GameObject menuRestartBtnParent;


    // Private vars
    private float cubeLengthWidthHeight = 0;
    private float currentInterCubeAdditionalSpacing = 0.2f;

    private bool minesHaveBeenPlanted = false;

    private int numberOfMineFlagsLeft = 0;
    private int totalNumberCubes = 0;
    private int numCubesRevealed = 0;

    private bool timerIsActive = false;
    private float currentTimerTime = 0;

    private GameObject mineHolder;
    private GameObject centerGridCube;
    private GameObject[,,] correspondingCubeGrid3DArray;

    // Start is called before the first frame update
    private void Awake()
    {
        // Init variables
        cubeLengthWidthHeight = basicCubePrefab.transform.localScale.x;

        // Old implementation
        /* numMines is the total number of cubes times the special mine
         * quantity val */
        // numMines = (int)((numRowsCols * numRowsCols * numRowsCols) * mineQuantityVal);

        mineHolder = new GameObject();
        mineHolder.name = "Mine Holder";
        mineHolder.transform.SetParent(allCubeHolder.transform);
    }

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        youWonTxt.SetActive(false);
        youLostTxt.SetActive(false);
        menuRestartBtnParent.SetActive(false);

        hudCanvas.SetActive(false);
        mainCanvas.SetActive(true);
        onNumRowsColsSliderChange();
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        if (timerIsActive)
        {
            currentTimerTime += Time.deltaTime;
            timerTxt.text = ((int)currentTimerTime).ToString();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void buildCubeGrid()
    {
        int currentLayer = 1;
        bool searchForMiddleCube = false;

        /* First we need to determine where exactly the center of the cube grid
         * will be; this is important since the camera will always be oriented 
         * towards the center */
        int gridCenterVal = (numRowsCols / 2) + 1;
        if (gridCenterVal == currentLayer) searchForMiddleCube = true;

        float originalSpawnX = 1;
        float originalSpawnY = 1;
        float originalSpawnZ = 1;

        float currentSpawnX = originalSpawnX;
        float currentSpawnY = originalSpawnY;
        float currentSpawnZ = originalSpawnZ;

        GameObject parentOfAllCubesButMiddleCube = new GameObject();
        parentOfAllCubesButMiddleCube.name = "Parent of all Cubes But Middle";
        parentOfAllCubesButMiddleCube.transform.SetParent(allCubeHolder.transform);

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
                            currentCube.transform.SetParent(allCubeHolder.transform);
                            currentCube.name = "MIDDLEMOST CUBE";

                            DragToRotate.Instance.setCenterCubeReference(centerGridCube);

                            searchForMiddleCube = false;
                        }
                    }
                    if (centerGridCube != currentCube)
                        currentCube.transform.SetParent(parentOfAllCubesButMiddleCube.transform);

                    currentSpawnX += cubeLengthWidthHeight + currentInterCubeAdditionalSpacing;
                }
                currentSpawnZ += cubeLengthWidthHeight + currentInterCubeAdditionalSpacing;
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
                currentSpawnY += cubeLengthWidthHeight + currentInterCubeAdditionalSpacing;
            }
        }

        /* Now that we are done, we need to move the entire cube grid such that
         * the middlemost cube is at the origin for rotation to work correctly.
         So... */

        // ... make the middle cube the parent of the rest of the cubes...
        parentOfAllCubesButMiddleCube.transform.SetParent(centerGridCube.transform);
        // ... and move it to the original, taking everything with it
        centerGridCube.transform.position = new Vector3(0, 0, 0);

        InputAndCameraManager.Instance.setCenterCubeReference(centerGridCube);

        // Now, we can unparent the middle cube from the rest of the cubes...
        parentOfAllCubesButMiddleCube.transform.SetParent(allCubeHolder.transform);
        /* ... and unparent parentOfAllCubesButMiddleCube from the rest of the
         * cubes (necessary for cube spacing expansion) */
        while (parentOfAllCubesButMiddleCube.transform.childCount > 0)
        {
            for (int i = 0; i < parentOfAllCubesButMiddleCube.transform.childCount; i++)
            {
                parentOfAllCubesButMiddleCube.transform.GetChild(i).SetParent(allCubeHolder.transform);
            }
        }

        // Finally, since it is unneeded now, delete
        Destroy(parentOfAllCubesButMiddleCube);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cubePos"></param>
    /// <param name="centerCubePos"></param>
    /// <returns></returns>
    private float calculateCubeSpacingAwayFromCenterCube(float cubePos, float centerCubePos, char increaseOrDecrease)
    {
        if (cubePos > centerCubePos)
        {
            if (increaseOrDecrease == '+')
                return interCubeSpacingIncrementVal;
            else
                return interCubeSpacingIncrementVal * -1;
        }
        else if (cubePos < centerCubePos)
        {
            if (increaseOrDecrease == '+')
                return interCubeSpacingIncrementVal * -1;
            else
                return interCubeSpacingIncrementVal;
        }
        else
            return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    private void onGameWonOrLost()
    {
        timerIsActive = false;
        timerTxt.color = Color.red;

        // Reset Vars
        minesHaveBeenPlanted = false;

        menuRestartBtnParent.SetActive(true);

        InputAndCameraManager.Instance.onGameEnd();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator deleteOldCubeHolderAndMineHolderChildren()
    {
        // Remove allCubeHolder children
        for (int i = 0; i < allCubeHolder.transform.childCount; i++)
        {
            if (allCubeHolder.transform.GetChild(i).gameObject != mineHolder)
                Destroy(allCubeHolder.transform.GetChild(i).gameObject);
        }

        // Remove mineHolder children
        for (int i = 0; i < mineHolder.transform.childCount; i++)
        {
            Destroy(mineHolder.transform.GetChild(i).gameObject);
        }

        yield return new WaitUntil(() => allCubeHolder.transform.childCount <= 1 && mineHolder.transform.childCount <= 0);

    }

    /// <summary>
    /// 
    /// </summary>
    private void turnOffAllPostWinOrFailHudElements()
    {
        youWonTxt.SetActive(false);
        youLostTxt.SetActive(false);
        menuRestartBtnParent.SetActive(false);
    }

    /* ************************************************************************
     *                          PUBLIC FUNCTIONS
     * ***********************************************************************/

    /// <summary>
    /// 
    /// </summary>
    public void startGame()
    {
        // Turn off our menu canvas
        mainCanvas.SetActive(false);
        /* Delete all the allCubeHolder's children if they exist from a
         * previous game */
        if (allCubeHolder.transform.childCount > 1 && mineHolder.transform.childCount >= 0)
        {
            StartCoroutine(deleteOldCubeHolderAndMineHolderChildren());
        }
        // Update our important vals needed for building grid
        numRowsCols = (int)numRowsColsSlider.value;
        numMines = (int)numMinesSlider.value;
        totalNumberCubes = (int)Math.Pow(numRowsCols, 3);
        correspondingCubeGrid3DArray = new GameObject[numRowsCols, numRowsCols, numRowsCols];

        buildCubeGrid();

        // Once initial setup is finished, start the game
        timerTxt.color = Color.white;

        numCubesRevealed = 0;
        currentTimerTime = 0;
        timerIsActive = true;

        numberOfMineFlagsLeft = numMines;
        updateMinesPlacedTxt(0);

        hudCanvas.SetActive(true);
        InputAndCameraManager.Instance.onGameStart();
    }

    /// <summary>
    /// 
    /// </summary>
    public void onReturnToMenuBtnClick()
    {
        turnOffAllPostWinOrFailHudElements();

        CameraLogic.Instance.onReturnToMenuAfterGame();

        hudCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }

    public void onRedoBtnClick()
    {
        turnOffAllPostWinOrFailHudElements();
        startGame();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valToAdd"></param>
    public void updateMinesPlacedTxt(int valToAdd)
    {
        numberOfMineFlagsLeft += valToAdd;
        mineFlagsPlacedTxt.text = numberOfMineFlagsLeft.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int getNumberOfMineFlagsLeft()
    {
        return numberOfMineFlagsLeft;
    }

    /// <summary>
    /// 
    /// </summary>
    public void onGameWon()
    {
        onGameWonOrLost();
        youWonTxt.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void onGameLost()
    {
        onGameWonOrLost();
        youLostTxt.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void onNumRowsColsSliderChange()
    {
        numRowsColsTxt.text = numRowsColsSlider.value.ToString();

        int totalNumCubes = (int)Math.Pow(numRowsColsSlider.value, 3);

        numMinesSlider.minValue = (int)totalNumCubes * 0.25f;
        numMinesSlider.value = numMinesSlider.minValue;
        numMinesSlider.maxValue = totalNumCubes - 1;

        onNumMinesSliderChange();
    }

    /// <summary>
    /// 
    /// </summary>
    public void onNumMinesSliderChange()
    {
        numMinesTxt.text = numMinesSlider.value.ToString();
    }


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
            numCubesRevealed++;
            // cubeID.totallyRemoveCube();

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
        else if (cubeID.sidesTouchingMines != 0)
        {
            cubeID.showSidesTouchingMinesText();
            numCubesRevealed++;
        }

        // Winning Game condition
        if (numCubesRevealed >= (totalNumberCubes - numMines))
        {
            onGameWon();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="firstCubeThatWasClicked"></param>
    public void plantMinesThroughoutGrid(GameObject firstCubeThatWasClicked)
    {
        InputAndCameraManager.Instance.setPlayerInputAllowed(false);

        int firstClickedCubeX = firstCubeThatWasClicked.GetComponent<CubeIdentifier>().cubeXIndex;
        int firstClickedCubeY = firstCubeThatWasClicked.GetComponent<CubeIdentifier>().cubeYIndex;
        int firstClickedCubeZ = firstCubeThatWasClicked.GetComponent<CubeIdentifier>().cubeZIndex;

        int currentNumMines = 0;
        /* Randomly assign cubes as mines until we reach the maximum */
        while (currentNumMines < numMines)
        {
            int randomX = UnityEngine.Random.Range(0, numRowsCols);
            int randomY = UnityEngine.Random.Range(0, numRowsCols);
            int randomZ = UnityEngine.Random.Range(0, numRowsCols);

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
        InputAndCameraManager.Instance.setPlayerInputAllowed(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="increaseOrDecrease"></param>
    public void modifyInterCubeSpacing(char increaseOrDecrease)
    {
        // Ensure we do not go past our min or max spacing bounds
        if ((currentInterCubeAdditionalSpacing > interCubeSpacingMax && increaseOrDecrease == '+') ||
            (currentInterCubeAdditionalSpacing < interCubeSpacingMin && increaseOrDecrease == '-'))
            return;

        // Go through each active cube to space it out from the center cube
        foreach(GameObject cube in correspondingCubeGrid3DArray)
        {
            if (cube.activeSelf)
            {
                float additionalDistanceX = calculateCubeSpacingAwayFromCenterCube(cube.transform.localPosition.x,
                                                                                   centerGridCube.transform.localPosition.x,
                                                                                   increaseOrDecrease);
                float additionalDistanceY = calculateCubeSpacingAwayFromCenterCube(cube.transform.localPosition.y,
                                                                                   centerGridCube.transform.localPosition.y,
                                                                                   increaseOrDecrease);
                float additionalDistanceZ = calculateCubeSpacingAwayFromCenterCube(cube.transform.localPosition.z,
                                                                                   centerGridCube.transform.localPosition.z,
                                                                                   increaseOrDecrease);

                cube.transform.localPosition += new Vector3(additionalDistanceX, additionalDistanceY, additionalDistanceZ);
            }
        }

        // Make sure to update current cube spacing var
        if (increaseOrDecrease == '+')
            currentInterCubeAdditionalSpacing += interCubeSpacingIncrementVal;
        else
            currentInterCubeAdditionalSpacing -= interCubeSpacingIncrementVal;
    }
}
