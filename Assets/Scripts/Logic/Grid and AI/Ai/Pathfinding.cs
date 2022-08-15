using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding 
{

    
    private Grid<Node> grid;
    private List<Node> openList;
    private List<Node> closeList;
    public Pathfinding(int width, int height, float scale, Vector3 originPosition)
    {
        
        grid = new Grid<Node>(width, height, scale, originPosition, (Grid<Node> g, int x, int y) => new Node(g, x, y));
    }

    public List<Vector3> FindPath(Vector3 startWPos, Vector3 endWPos)
    {
        if(!grid.isOnGrid(startWPos)|| !grid.isOnGrid(endWPos))
        {
            return null;
        }
        GridPosition startGPos = grid.GetGridPosition(startWPos);
        GridPosition endGPos = grid.GetGridPosition(endWPos);

        List <Node> path = FindPath(startGPos, endGPos);
        if(path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach(Node node in path)
            {
                vectorPath.Add(grid.GetWorldPosition(node.pos) + GameManager.Instance.levelOffset);
            }
            return vectorPath;
        }
    }

    public List<Node> FindPath(GridPosition startPos, GridPosition endPos)
    {
        Node startNode = grid.GetGridObject(startPos);
        Node endNode = grid.GetGridObject(endPos);

        openList = new List<Node> { startNode};
        closeList = new List<Node>();

        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                Node node = grid.GetGridObject(x, y);
                node.gCost = int.MaxValue;
                node.CalculateFCost();
                node.cameFromNode = null;
                node.setNeighbours(GetNeighborList(node));
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            Node currentNode = GetLowestFCostNode(openList);
            if(currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            foreach (Node n in currentNode.neighbors)
            {
                if(closeList.Contains(n))
                {
                    continue; 
                }
                int tempGCost = currentNode.gCost + CalculateDistanceCost(currentNode, n);
                if(tempGCost < n.gCost)
                {
                    n.cameFromNode = currentNode;
                    n.gCost = tempGCost;
                    n.hCost = CalculateDistanceCost(n, endNode);
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

    private int CalculateDistanceCost(Node a, Node b)
    {
        int xDistance = Mathf.Abs(a.pos.x - b.pos.x);
        int yDistance = Mathf.Abs(a.pos.y - b.pos.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return Mathf.Min(xDistance, yDistance) + 10 * remaining;
    } 

    private Node GetLowestFCostNode(List<Node> nodeList)
    {
        Node lowestFCostNode = nodeList[0];
        for(int i = 1; i < nodeList.Count;i++)
        {
            if(nodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = nodeList[i];
            }
        }
        return lowestFCostNode;
    }

    private List<Node> CalculatePath(Node endNode)
    {
        List<Node> path = new List<Node>();
        path.Add(endNode);
        Node currentNode = endNode;
        while(currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path; 
    }

    private List<Node> GetNeighborList(Node node)
    {
        List<Node> neighborList = new List<Node>();
        if(node.pos.x - 1 >= 0)
        {
            //left
            neighborList.Add(grid.GetGridObject(node.pos.x - 1, node.pos.y));
        }
        if(node.pos.x + 1 < grid.width)
        {
            neighborList.Add(grid.GetGridObject(node.pos.x + 1, node.pos.y));
        }
        if (node.pos.y - 1 >= 0)
        {
            //left
            neighborList.Add(grid.GetGridObject(node.pos.x, node.pos.y - 1));
        }
        if (node.pos.y + 1 < grid.height)
        {
            neighborList.Add(grid.GetGridObject(node.pos.x, node.pos.y + 1));
        }

        return neighborList;
    }
}
