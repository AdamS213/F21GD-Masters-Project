using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridVisual : MonoBehaviour
{
    public static GridVisual Instance { get; private set; }

    [Serializable] public struct GridVisualTypeMaterial {
        public GridVisualType gridVisualType;
        public Material material;
    }

    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft
    }
    [SerializeField] private Transform gridVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;
    private GridVisualSingle[,] gridVisualSingles;
    private int width;
    private int height;

    private void Awake()
    {
        if (Instance != null)
        {
            // prevents duplicates
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        
        width = GameManager.Instance.GetWIDTH();
        height = GameManager.Instance.GetHEIGHT();
        gridVisualSingles = new GridVisualSingle[width,height];
        for (int x = 0; x < width;x++)
        {
            for(int z = 0; z < height;z++)
            {
                Transform gridVisualSingleTransform = Instantiate(gridVisualSinglePrefab, GameManager.Instance.levelGrid.GetWorldPosition(x, z) + GameManager.Instance.levelOffset,Quaternion.identity);
                gridVisualSingles[x, z] = gridVisualSingleTransform.GetComponent<GridVisualSingle>();
            }
        }

        GameManager.Instance.OnSelectedActionChange += GameManager_OnSelectedActionChange;
        UpdateGridVisual();
    }

   

    public void HideAllGridPositions()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                gridVisualSingles[x, z].Hide();
            }
        }
    }

    public void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositions = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition positionToCheck = offsetGridPosition + gridPosition;
                if (positionToCheck == gridPosition)
                {
                    continue;
                }
                //checks position is on grid 
                if (!GameManager.Instance.levelGrid.isOnGrid(positionToCheck))
                {
                    continue;
                }
                gridPositions.Add(positionToCheck);
            }
        }
        ShowGridPositionList(gridPositions, gridVisualType);
    }
    public void ShowGridPositionList(List<GridPosition> gridPositions, GridVisualType gridVisualType)
    {
        foreach(GridPosition gridPosition in gridPositions)
        {
            gridVisualSingles[gridPosition.x,gridPosition.y].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    public void UpdateGridVisual()
    {
        HideAllGridPositions();

        Unit player = GameManager.Instance.GetPlayer();

        BaseAction selectedAction = GameManager.Instance.GetSelectedAction();

        GridVisualType gridVisualType;

        switch(selectedAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case AttackAction attackAction:
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRange(player.GetGridPosition(), attackAction.GetAttackRange(),GridVisualType.RedSoft);
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case StealAction stealAction:
                gridVisualType = GridVisualType.Blue;
                break;
        }
        ShowGridPositionList(selectedAction.GetValidActionGridPositions(),gridVisualType);
    }

    private void GameManager_OnSelectedActionChange(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach(GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList )
        {
            if(gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }
        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
        return null;
    }
}
