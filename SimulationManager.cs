using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{

    public Transform treePrefab;
    public int gridSize;
    public float distance;
    Transform[,] grid;
    public float timestep = 0.2f;
    public float m = 1f;

    // Start is called before the first frame update
    void Start()
    {
        grid = new Transform[gridSize,gridSize];
        initialize();
        initNeighbours();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void initialize()
    {
        Transform tree;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                tree = Instantiate(treePrefab, transform);
                grid[i,j] = tree;
                tree.position = new Vector3(i * distance, 0, j * distance);
            }
        }
    }

    void initNeighbours()
    {
        Cell cell;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                cell = grid[i,j].GetComponent<Cell>();
                cell.timestep = timestep;
                cell.m = m;

                if (i > 0) {
                    cell.close_neighbours.Add(grid[i - 1, j].GetComponent<Cell>());
                    if(j > 0)
                    {
                        cell.close_neighbours.Add(grid[i - 1, j-1].GetComponent<Cell>());
                    }
                    if (j < gridSize - 1)
                    {
                        cell.close_neighbours.Add(grid[i - 1, j + 1].GetComponent<Cell>());
                    }
                }
                if (j > 0)
                {
                    cell.close_neighbours.Add(grid[i, j - 1].GetComponent<Cell>());
                    if (i < gridSize - 1)
                    {
                        cell.close_neighbours.Add(grid[i + 1, j - 1].GetComponent<Cell>());
                    }
                }
                if (j < gridSize - 1)
                {
                    cell.close_neighbours.Add(grid[i, j + 1].GetComponent<Cell>());
                    if (i < gridSize - 1)
                    {
                        cell.close_neighbours.Add(grid[i + 1, j + 1].GetComponent<Cell>());
                    }
                }
                if (i < gridSize - 1)
                    cell.close_neighbours.Add(grid[i + 1, j].GetComponent<Cell>());
            }
        }
    }
}
