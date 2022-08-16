using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding 
{

    
    private Grid<GridObject> grid;
    private List<GridObject> openList;
    private List<GridObject> closeList;
    public Pathfinding(int width, int height, float scale, Vector3 originPosition,Grid<GridObject> grid)
    {
        
       this.grid = grid; 
    }

    public List<Vector3> FindPath(Vector3 startWPos, Vector3 endWPos)
    {
        if(!grid.isOnGrid(startWPos)|| !grid.isOnGrid(endWPos))
        {
            return null;
        }
        GridPosition startGPos = grid.GetGridPosition(startWPos);
        GridPosition endGPos = grid.GetGridPosition(endWPos);

        List <GridObject> path = FindPath(startGPos, endGPos);
        if(path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach(GridObject node in path)
            {
                vectorPath.Add(grid.GetWorldPosition(node.GetGridPosition()) + GameManager.Instance.levelOffset);
            }
            return vectorPath;
        }
    }

    public List<GridObject> FindPath(GridPosition startPos, GridPosition endPos)
    {
        GridObject startNode = grid.GetGridObject(startPos);
        GridObject endNode = grid.GetGridObject(endPos);

        openList = new List<GridObject> { startNode};
        closeList = new List<GridObject>();

        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                GridObject node = grid.GetGridObject(x, y);
                node.gCost = int.MaxValue;
                node.CalculateFCost();
                node.cameFromNode = null;
                node.setNeighbours(GetNeighborList(node));
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode.GetGridPosition(), endNode.GetGridPosition());
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            GridObject currentNode = GetLowestFCostNode(openList);
            if(currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            foreach (GridObject n in currentNode.neighbors)
            {
                if(closeList.Contains(n))
                {
                    continue; 
                }

                if(n.HasUnit())
                {
                    continue;
                }
                int tempGCost = currentNode.gCost + CalculateDistanceCost(currentNode.GetGridPosition(), n.GetGridPosition());
                if(tempGCost < n.gCost)
                {
                    n.cameFromNode = currentNode;
                    n.gCost = tempGCost;
                    n.hCost = CalculateDistanceCost(currentNode.GetGridPosition(), n.GetGridPosition());
                    n.CalculateFCost();

                    if(!openList.Contains(n))
                    {
                        openList.Add(n);
                    }
                }

            }
        }
        //couldn't find path
        return null;
    }

    private int CalculateDistanceCost(GridPosition a, GridPosition b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return Mathf.Min(xDistance, yDistance) + 10 * remaining;
    } 

    private GridObject GetLowestFCostNode(List<GridObject> nodeList)
    {
        GridObject lowestFCostNode = nodeList[0];
        for(int i = 1; i < nodeList.Count;i++)
        {
            if(nodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = nodeList[i];
            }
        }
        return lowestFCostNode;
    }

    private List<GridObject> CalculatePath(GridObject endNode)
    {
        List<GridObject> path = new List<GridObject>();
        path.Add(endNode);
        GridObject currentNode = endNode;
        while(currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path; 
    }

    private List<GridObject> GetNeighborList(GridObject node)
    {
        List<GridObject> neighborList = new List<GridObject>();
        GridPosition gridPosition = node.GetGridPosition();
        if(gridPosition.x - 1 >= 0)
        {
            //left
            neighborList.Add(grid.GetGridObject(gridPosition.x - 1, gridPosition.y));
        }
        if(gridPosition.x + 1 < grid.width)
        {
            neighborList.Add(grid.GetGridObject(gridPosition.x + 1, gridPosition.y));
        }
        if (gridPosition.y - 1 >= 0)
        {
            //left
            neighborList.Add(grid.GetGridObject(gridPosition.x, gridPosition.y - 1));
        }
        if (gridPosition.y + 1 < grid.height)
        {
            neighborList.Add(grid.GetGridObject(gridPosition.x, gridPosition.y + 1));
        }

        return neighborList;
    }
}
