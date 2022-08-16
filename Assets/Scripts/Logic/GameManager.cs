using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{

    [SerializeField] public GameObject testCube;
    [SerializeField] private int WIDTH;
    [SerializeField] private int HEIGHT;
    [SerializeField] private int SCALE;
    [SerializeField] public Vector3 levelOffset;
    
    public static GameManager Instance { get; private set; }

    public event EventHandler OnSelectedActionChange;
    public event EventHandler OnActionStarted;

    public PlayerControls playerControls;
    public Camera cam;
    public CameraController camCont;
    [SerializeField] private Unit player;
    private BaseAction selectedAction;
    public Grid<GameObject> preFabGrid { get; private set; }
    public Grid<GridObject> levelGrid { get; private set; }
    
    public Pathfinding pathfinding;

    private bool isBusy;
    private GridPosition leftMouseGridPosition;
    private GridPosition rightMouseGridPosition;
    private Vector3 leftMouseWorldPosition;
    private Vector3 rightMouseWorldPosition;
    #region Unity Bulletins

    private void Awake()
    {
        if (Instance != null)
        {
            // prevents duplicates
            Destroy(gameObject);
            return;
        }
        Instance = this;
        playerControls = new PlayerControls();
        
        //creates 30 by 30 sized grid that contains GameObjects, we also pass in a function to Instantiate the GameObjects at the Correct postions
        //preFabGrid = new Grid<GameObject>(HEIGHT, HEIGHT, SCALE, Vector3.zero, (Grid<GameObject> g, int x, int z) => createObjectsInWorld(g, x, z));
        levelGrid = new Grid<GridObject>(WIDTH, HEIGHT, SCALE, Vector3.zero, (Grid<GridObject> g, int x, int z) => new GridObject(g, new GridPosition(x, z)));
        pathfinding = new Pathfinding(WIDTH, HEIGHT, SCALE, Vector3.zero,levelGrid);
        SetSelectedAction(player.GetAction<MoveAction>());

    }
    void Start()
    {
        camCont.SetPlayerControls(playerControls);

        playerControls.Mouse.LeftMouseClick.performed += _ => LeftMouseClick();
        playerControls.Mouse.RightMouseClick.performed += _ => RightMouseClick();
        

    }

    

    private void OnEnable()
    {
       playerControls.Mouse.LeftMouseClick.Enable();
       playerControls.Mouse.MousePosition.Enable();
       playerControls.Mouse.RightMouseClick.Enable();
    }
    private void OnDisable()
    {
       playerControls.Mouse.LeftMouseClick.Disable();
       playerControls.Mouse.MousePosition.Disable();
       playerControls.Mouse.RightMouseClick.Enable();
    }

    #endregion

    #region Utility Functions
    // function that we pass into the Grid Constructor
    GameObject createObjectsInWorld(Grid<GameObject> g, int x, int z)
    {
        return GameObject.Instantiate(testCube, g.GetWorldPosition(x, z), Quaternion.identity);
    }

    //Converts mouse position to world position, returns true if we clicked on something, false if not
    public bool MouseToWorld(Vector2 mousePos, out Vector3Int WorldPos)
    {
        Ray ray = cam.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            WorldPos = Vector3Int.RoundToInt(raycastHit.point);
            Debug.Log("Yay");
            return true;
        }
        WorldPos = Vector3Int.zero;
        return false;
    }
   
    private void LeftMouseClick()
    {
        Vector2 mousePos = playerControls.Mouse.MousePosition.ReadValue<Vector2>();
        Vector3Int worldPos;
        bool hit = MouseToWorld(mousePos, out worldPos);
        leftMouseWorldPosition = worldPos;
        leftMouseGridPosition = levelGrid.GetGridPosition(worldPos);
        if (hit)
        {
            if (isBusy)
            {
                return;
            }

            if(EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if(!TurnSystem.Instance.IsPlayerTurn())
            {
                return;
            }

            if (!selectedAction.isValidActionGridPosition(leftMouseGridPosition))
            { 
                return;    
            }

            if (!player.TrySpendPointsToTakeAction(selectedAction))
            {
                return;
            }
            SetBusy();
            selectedAction.TakeAction(leftMouseGridPosition, ClearBusy);
            OnActionStarted?.Invoke(this, EventArgs.Empty);

        }
        
    }

    private void RightMouseClick()
    {
        Vector2 mousePos = playerControls.Mouse.MousePosition.ReadValue<Vector2>();
        Vector3Int worldPos;
        bool hit = MouseToWorld(mousePos, out worldPos);
        rightMouseWorldPosition = worldPos;
        rightMouseGridPosition = levelGrid.GetGridPosition(worldPos);
        if (hit)
        {
            if (!isBusy && !EventSystem.current.IsPointerOverGameObject())
            {
                //add functionality
            }
        }
    }

    private void SetBusy()
    {
        isBusy = true;
    }

    private void ClearBusy()
    {
        isBusy = false;
    }

    public Unit GetPlayer() => player;

    public int GetWIDTH() => WIDTH;
    public int GetHEIGHT() => HEIGHT;

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;

        OnSelectedActionChange?.Invoke(this, EventArgs.Empty);
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }
    #endregion



}
