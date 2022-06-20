using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    private Grid<Node> grid;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public Node cameFromNode;
    public List<Node> neighbors;

    public Node(Grid<Node> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void setNeighbours(List<Node> neighbors)
    {
        this.neighbors = neighbors;
    }
     
}
