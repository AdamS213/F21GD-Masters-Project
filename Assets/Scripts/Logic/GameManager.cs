using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    [SerializeField] private int WIDTH;
    [SerializeField] private int HEIGHT;
    [SerializeField] private int SCALE;
    [SerializeField] public Vector3 levelOffset;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI StartEndText;
    [SerializeField] private GameObject StartEndScreen;
    public LayerMask obstaclesLayerMask;
    
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
        
        
        levelGrid = new Grid<GridObject>(WIDTH, HEIGHT, SCALE, Vector3.zero, (Grid<GridObject> g, int x, int z) => new GridObject(g, new GridPosition(x, z)));
        pathfinding = new Pathfinding(WIDTH, HEIGHT, SCALE, Vector3.zero,levelGrid);
        SetSelectedAction(player.GetAction<MoveAction>());

    }
    void Start()
    {
        camCont.SetPlayerControls(playerControls);
        button.onClick.AddListener(() =>
        {
            StartEndScreen.SetActive(false);
        });
        playerControls.Mouse.LeftMouseClick.performed += _ => LeftMouseClick();
        playerControls.Mouse.RightMouseClick.performed += _ => RightMouseClick();
        StealAction.onSteal += StealAction_OnSteal;



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
    

    //Converts mouse position to world position, returns true if we clicked on something, false if not
    public bool MouseToWorld(Vector2 mousePos, out Vector3Int WorldPos)
    {
        Ray ray = cam.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            WorldPos = Vector3Int.RoundToInt(raycastHit.point);
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
    private void StealAction_OnSteal(object sender, EventArgs e)
    {
        StartEndScreen.SetActive(true);
        button.gameObject.SetActive(false);
        StartEndText.text = "You stole the Liches treasure right out from under them! Well done!";
    }


}
