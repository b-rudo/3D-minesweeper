/* 8/27/2020 */
/* B. Rudolph */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAndCameraManager : Singleton<InputAndCameraManager>
{
    // Set in inspector
    [Header("--- Controls --- ")]
    public KeyCode expandShrinkCubeDistanceKey;
    public KeyCode openPauseMenuKey;
    public KeyCode hideRevealedCubeOutlineKey;

    // Private
    private GameObject centerCubeReference;
    private readonly float cameraZDistanceMultiplier = 1.5f;
    private bool playerInputAllowed = false;
    private bool canRevealOrFlagCubes = false;

    private float downClickTime;
    private const float clickDeltaTime = 0.2f;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        int gridNumRowsCols = GameManager.Instance.getNumRowsCols();

        /* First set the camera's correct z value based on how big the cube
         * grid is */
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
                                                     Camera.main.transform.position.y,
                                                     -1 * cameraZDistanceMultiplier * gridNumRowsCols);
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        if (playerInputAllowed)
        {
            // All mouse clicks
            if (canRevealOrFlagCubes)
            {
                /* The below, more complicated implementation is necessary to
                 * avoid the case wherein players click to rotate the cube and
                 * the game interprets it as a regular mouse click to reveal a
                 * cube */
                if (Input.GetMouseButtonDown(0))
                    downClickTime = Time.time;
                if (Input.GetMouseButtonUp(0))
                {
                    if (Time.time - downClickTime <= clickDeltaTime)
                    {
                        StartCoroutine(revealCube());
                    }
                }

                if (Input.GetMouseButtonDown(1))
                    placeOrRemoveMineFlag();
            }

            // Mouse scrolls
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (Input.GetKey(expandShrinkCubeDistanceKey))
                    GameManager.Instance.modifyInterCubeSpacing('-');
                else
                    CameraLogic.Instance.attemptZoomInOrOut('+');
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (Input.GetKey(expandShrinkCubeDistanceKey))
                    GameManager.Instance.modifyInterCubeSpacing('+');
                else
                    CameraLogic.Instance.attemptZoomInOrOut('-');
            }

            // Key presses
            if (Input.GetKeyDown(hideRevealedCubeOutlineKey))
            {
                GameManager.Instance.toggleRevealedCubeTransparency();
            }
        }

        /* Allow using this key even when player input isn't allowed since
         * otherwise you can't close the pause menu with it */
        if (Input.GetKeyDown(openPauseMenuKey))
            GameManager.Instance.onQuestionMarkBtnClick();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator revealCube()
    {
        GameObject cube = returnMousePosCubeIfExists();

        if (cube != null)
        {
            if (!GameManager.Instance.getMinesHaveBeenPlanted())
            {
                GameManager.Instance.plantMinesThroughoutGrid(cube);
                // We must wait until all mines are planted to continue
                yield return new WaitUntil(() => GameManager.Instance.getMinesHaveBeenPlanted() == true);
            }

            // If the cube is flagged, don't do anything
            if (!cube.GetComponent<CubeIdentifier>().getIsMineFlagged())
            {
                // Check what kind of cube we just clicked
                if (cube.GetComponent<CubeIdentifier>().cubeType == CubeIdentifier.cubeTypes.mine)
                {
                    GameManager.Instance.onGameLost();
                }
                else
                {
                    CubeIdentifier cubeID = cube.GetComponent<CubeIdentifier>();
                    GameManager.Instance.givenCubeCoordsDeleteCloseByCubesIfSafe(cubeID.cubeXIndex, cubeID.cubeYIndex, cubeID.cubeZIndex);
                }
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    private void placeOrRemoveMineFlag()
    {
        GameObject cube = returnMousePosCubeIfExists();

        if (!cube) return;

        // Check for the case of adding flag when you're out of flags
        if (!cube.GetComponent<CubeIdentifier>().getIsMineFlagged() &&
            GameManager.Instance.getNumberOfMineFlagsLeft() <= 0)
        {
            return;
        }

        bool flagOn = cube.GetComponent<CubeIdentifier>().reverseMineFlagOnOrOff();

        // Update the HUD mines place text
        int valToAdd = 1;
        if (flagOn) valToAdd = -1;
        GameManager.Instance.updateMinesPlacedTxt(valToAdd);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private GameObject returnMousePosCubeIfExists()
    {
        // Send out a raycast to see if we clicked on a game cube
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "GameCube")
                return hit.transform.gameObject;
        }

        return null;
    }


    /* ************************************************************************
     *                          PUBLIC FUNCTIONS
     * ***********************************************************************/

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool getPlayerInputAllowed()
    {
        return playerInputAllowed;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="allowed"></param>
    public void setPlayerInputAllowed(bool allowed)
    {
        playerInputAllowed = allowed;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="canReveal"></param>
    public void setCanRevealOrFlagCubes(bool canReveal)
    {
        canRevealOrFlagCubes = canReveal;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool getCanRevealOrFlagCubes()
    {
        return canRevealOrFlagCubes;
    }

    /// <summary>
    /// 
    /// </summary>
    public void onGameStart()
    {
        setPlayerInputAllowed(true);
        canRevealOrFlagCubes = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void onGameEnd()
    {
        canRevealOrFlagCubes = false;
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
