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

    // Private
    private GameObject centerCubeReference;
    private readonly float cameraZDistanceMultiplier = 1.5f;
    private bool playerInputAllowed = false;

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
            if (Input.GetMouseButtonDown(0))
                StartCoroutine(revealCube());
            else if (Input.GetMouseButtonDown(1))
                placeOrRemoveMineFlag();

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
        }
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

            // Check what kind of cube we just clicked
            if (cube.GetComponent<CubeIdentifier>().cubeType == CubeIdentifier.cubeTypes.mine)
            {
                playerClickedMine();
            }
            else
            {
                CubeIdentifier cubeID = cube.GetComponent<CubeIdentifier>();
                GameManager.Instance.givenCubeCoordsDeleteCloseByCubesIfSafe(cubeID.cubeXIndex, cubeID.cubeYIndex, cubeID.cubeZIndex);
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

        cube.GetComponent<CubeIdentifier>().reverseMineFlagOnOrOff();
    }

    /// <summary>
    /// 
    /// </summary>
    private void playerClickedMine()
    {
        Debug.Log("FAIL");
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
    /// <param name="allowed"></param>
    public void setPlayerInputAllowed(bool allowed)
    {
        playerInputAllowed = allowed;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    public void setCenterCubeReference(GameObject obj)
    {
        centerCubeReference = obj;

        /* Once we have the center cube reference, we can place the camera in
         * the correct starting position and orientation */
        CameraLogic.Instance.setStartingCameraPos(centerCubeReference);
    }
}
