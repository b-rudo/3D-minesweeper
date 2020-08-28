/* 8/27/2020 */
/* B. Rudolph */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAndCameraManager : Singleton<InputAndCameraManager>
{
    // Set in inspector
    [Header("--- Controls --- ")]
    /*
    public KeyCode cameraLeftKey;
    public KeyCode cameraRightKey;
    public KeyCode cameraUpKey;
    public KeyCode cameraDownKey;
    */
    public KeyCode cubeDragKey1;
    public KeyCode cubeDragKey2;

    [Header("--- Other Dynamic Vars --- ")]
    public float cameraRotateSpeed = 0.05f;

    // Private
    private GameObject centerCubeReference;
    private enum cameraMovementDirection { left, right, up, down }
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

        /* We also need to adjust the camera rotate speed, since having bigger
         * grids and being zoomed out to a larger degree will slow how long it
         * takes to rotate to another cube grid face */
        cameraRotateSpeed *= gridNumRowsCols;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        if (playerInputAllowed)
        {
            // Keyboard input 
            if (centerCubeReference != null)
            {
                /*
                if (Input.GetKey(cameraLeftKey))
                {
                    moveCameraAroundGrid(cameraMovementDirection.left);
                }
                if (Input.GetKey(cameraRightKey))
                {
                    moveCameraAroundGrid(cameraMovementDirection.right);
                }
                if (Input.GetKey(cameraUpKey))
                {
                    moveCameraAroundGrid(cameraMovementDirection.up);
                }
                if (Input.GetKey(cameraDownKey))
                {
                    moveCameraAroundGrid(cameraMovementDirection.down);
                }
                */
            }

            // Mouse input
            if (Input.GetMouseButtonDown(0) && !Input.GetKey(cubeDragKey1) && !Input.GetKey(cubeDragKey2))
                StartCoroutine(revealCube());
            else if (Input.GetMouseButtonDown(2))
                placeMineMarker();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator revealCube()
    {
        GameObject cube = returnClickedCubeIfExists();

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
    private void placeMineMarker()
    {

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
    private GameObject returnClickedCubeIfExists()
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dir"></param>
    private void moveCameraAroundGrid(cameraMovementDirection dir)
    {
        Vector3 axis = new Vector3(0, 0, 0);

        float newCamX = Camera.main.transform.position.x;
        float newCamY = Camera.main.transform.position.y;
        float newCamZ = Camera.main.transform.position.z;

        switch (dir)
        {
            case (cameraMovementDirection.left):
                axis.y = -1;
                break;
            case (cameraMovementDirection.right):
                axis.y = 1;
                break;
            case (cameraMovementDirection.up):
                axis.x = 1;
                break;
            case (cameraMovementDirection.down):
                axis.x = -1;
                break;
            default:
                break;
        }

        Camera.main.transform.RotateAround(centerCubeReference.transform.position, axis, cameraRotateSpeed);
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
        Camera.main.transform.position = new Vector3(centerCubeReference.transform.position.x,
                                                     centerCubeReference.transform.position.y,
                                                     Camera.main.transform.position.z);
        Camera.main.transform.LookAt(centerCubeReference.transform);

        //
        DragToRotate.Instance.setCenterCubeReference(obj);
    }
}
