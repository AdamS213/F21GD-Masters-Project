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
        Grid<GameObject> grid = new Grid<GameObject>(10, 10, testCube.transform.localScale.x,Vector3.zero, (Grid<GameObject> g, int x, int y) => createObjectsInWorld(g,x,y));
    }

    GameObject createObjectsInWorld(Grid<GameObject> g, int x, int y)
    {
        return GameObject.Instantiate(testCube, g.GetWorldPosition(x, y), Quaternion.identity);
    }
    
}
