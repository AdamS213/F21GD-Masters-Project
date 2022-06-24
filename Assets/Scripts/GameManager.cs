using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject testCube;
    // Start is called before the first frame update
    void Start()
    {
        Pathfinding instance = new Pathfinding(30, 30);
        Grid<GameObject> grid = new Grid<GameObject>(30, 30, testCube.transform.localScale.x,Vector3.zero, (Grid<GameObject> g, int x, int y) => createObjectsInWorld(g,x,y));
    }

    GameObject createObjectsInWorld(Grid<GameObject> g, int x, int y)
    {
        return GameObject.Instantiate(testCube, g.GetWorldPosition(x, y), Quaternion.identity);
    }
    
}
