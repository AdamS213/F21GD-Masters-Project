using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControls : MonoBehaviour
{
    private PlayerControls playerControls;
    private bool isLerping = false;
    private float timer = 0.0f;
    [SerializeField] Camera cam;
    private CameraController camCont;

    private List<Vector3> currentPath;
    private int currentPathIndex;
    private void Awake()
    {
        playerControls = new PlayerControls();

    }

    private void OnEnable()
    {
        playerControls.Mouse.Enable();
        playerControls.Camera.focusOnPlayer.Enable();
    }
    private void OnDisable()
    {
        playerControls.Mouse.Disable();
        playerControls.Camera.focusOnPlayer.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        playerControls.Mouse.MouseClick.performed += _ => MouseClick();
        camCont = cam.GetComponentInParent<CameraController>();
    }

    private void Update()
    {
        HandleMovement();
        
            if (playerControls.Camera.focusOnPlayer.triggered)
            {
                camCont.focusCameraOnPoint(transform.position + Vector3.down);
            }
        
    }

    private void MouseClick()
    {
        if(currentPath == null)
        {
            Vector2 mousePos = playerControls.Mouse.MousePosition.ReadValue<Vector2>();
            Ray ray = cam.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
               setTargetPosition( Vector3Int.RoundToInt(raycastHit.point));
            }
        }
    }

    private void HandleMovement()
    {
        if (currentPath != null)
        {
            if (!isLerping)
            {
           
                if (currentPathIndex != currentPath.Count-1)
                {
                    isLerping = true;
                }
                else
                {
                    currentPath = null;
                    currentPathIndex = 0;
                }
            }
            else
            {
                timer += Time.deltaTime*5;
                transform.position = Vector3.Lerp(currentPath[currentPathIndex], currentPath[currentPathIndex + 1], timer);
                // should make camera follow player during movement
                camCont.focusCameraOnPoint(transform.position + Vector3.down);
                if (timer >= 1.0f)
                {
                    isLerping = false;
                    currentPathIndex++;
                    timer = 0.0f;
                }
            }
        }
    }

    private void setTargetPosition(Vector3 targetPosition)
    {
        currentPathIndex = 0;
        isLerping = true;
        //avoids error caused by clicking the square the player occupies
        if(targetPosition != transform.position)
        {
            currentPath = Pathfinding.Instance.FindPath(transform.position, targetPosition);
        }
        
        
    }
}
    
