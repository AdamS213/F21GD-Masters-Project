using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding 
{

    public static Pathfinding Instance { get; private set; }
    private Grid<Node> grid;
    private List<Node> openList;
    private List<Node> closeList;
    public Pathfinding(int width, int height)
    {
        Instance = this;
        grid = new Grid<Node>(width, height, 1f, Vector3.zero, (Grid<Node> g, int x, int y) => new Node(g, x, y));
    }

    public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
    {
        if(!grid.isOnGrid(startPos)|| !grid.isOnGrid(endPos))
        {
            return null;
        }
        grid.GetXY(startPos, out int startX, out int startY);
        grid.GetXY(endPos, out int endX, out int endY);

        List <Node> path = FindPath(startX, startY, endX, endY);
        if(path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach(Node node in path)
            {
                vectorPath.Add(grid.GetWorldPosition(node.x, node.y) + new Vector3(0, 1, 0));
            }
            return vectorPath;
        }
    }

    public List<Node> FindPath(int startX, int startY, int endX, int endY)
    {
        Node startNode = grid.GetGridObject(startX, startY);
        Node endNode = grid.GetGridObject(endX, endY);

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
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
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
        if(node.x - 1 >= 0)
        {
            //left
            neighborList.Add(grid.GetGridObject(node.x - 1, node.y));
        }
        if(node.x + 1 < grid.width)
        {
            neighborList.Add(grid.GetGridObject(node.x + 1, node.y));
        }
        if (node.y - 1 >= 0)
        {
            //left
            neighborList.Add(grid.GetGridObject(node.x, node.y - 1));
        }
        if (node.y + 1 < grid.height)
        {
            neighborList.Add(grid.GetGridObject(node.x, node.y + 1));
        }

        return neighborList;
    }
}
