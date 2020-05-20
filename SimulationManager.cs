using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SimulationManager : MonoBehaviour
{
    public enum Direction
    {
        North, West, South, East, NorthWest, NorthEast, SouthWest, SouthEast
    }
    public Direction direction;
	
	public float startLat;
	public float endLat;
	public float startLon;
	public float endLon;
	
	float latStep;
	float lonStep;
	
	
    public Transform[] treePrefabs;
    public int gridSize;
    public float distance;
    Transform[,] grid;
    public float timestep = 0.2f;
    public float m = 1f;
    public Terrain terrain;
    public Transform firePrefab;
    public Transform fireBolt;
    public Transform Windcube;
	public Transform windParent;
	
    public float Temperature = 22f;
    public float AirHum = 59f;
    const int heightmapResolution = 513;
    public float WindForce = 1.4f;

    WindSpeedReader windSpeedReader;
    // Start is called before the first frame update
    void Start()
    {
		latStep = (endLat - startLat) / gridSize; 
		lonStep = (endLon - startLon) / gridSize;
        windSpeedReader = GetComponent<WindSpeedReader>();
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

                tree = Instantiate(treePrefabs[Random.Range(0,4)], transform);
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
				cell.i = i;
				cell.j = j;
                if (i > 0) {
                    cell.close_neighbours.Add(grid[i - 1, j].GetComponent<Cell>());
                    cell.Neighbours.Add(Direction.West, grid[i - 1, j].GetComponent<Cell>());
                    if(j > 0)
                    {
                        cell.Neighbours.Add(Direction.SouthWest, grid[i - 1, j - 1].GetComponent<Cell>());
                        cell.close_neighbours.Add(grid[i - 1, j-1].GetComponent<Cell>());
                    }
                    if (j < gridSize - 1)
                    {
                        cell.Neighbours.Add(Direction.NorthWest, grid[i - 1, j + 1].GetComponent<Cell>());
                        cell.close_neighbours.Add(grid[i - 1, j + 1].GetComponent<Cell>());
                    }
                }
                if (j > 0)
                {
                    cell.close_neighbours.Add(grid[i, j - 1].GetComponent<Cell>());
                    cell.Neighbours.Add(Direction.South, grid[i, j - 1].GetComponent<Cell>());
                    if (i < gridSize - 1)
                    {
                        cell.Neighbours.Add(Direction.SouthEast, grid[i + 1, j - 1].GetComponent<Cell>());
                        cell.close_neighbours.Add(grid[i + 1, j - 1].GetComponent<Cell>());
                    }
                }
                if (j < gridSize - 1)
                {
                    cell.close_neighbours.Add(grid[i, j + 1].GetComponent<Cell>());
                    cell.Neighbours.Add(Direction.North, grid[i, j + 1].GetComponent<Cell>());
                    if (i < gridSize - 1)
                    {
                        cell.Neighbours.Add(Direction.NorthEast, grid[i + 1, j + 1].GetComponent<Cell>());
                        cell.close_neighbours.Add(grid[i + 1, j + 1].GetComponent<Cell>());
                    }
                }
                if (i < gridSize - 1)
                {
                    cell.Neighbours.Add(Direction.East, grid[i + 1, j].GetComponent<Cell>());
                    cell.close_neighbours.Add(grid[i + 1, j].GetComponent<Cell>());
                }

                initHeight(cell,i,j);
                initWind(cell);
                initCoefficients(cell);
                initWindSpeed(cell, i, j);
				initCoordinates(cell,i,j);
            }
        }
    }

    void initHeight(Cell cell, int i, int j)
    {
        float terrainHeight = terrain.terrainData.GetHeight((int) (i*heightmapResolution/gridSize),
            (int)(j * heightmapResolution / gridSize));
        //Debug.Log(terrainHeight);
        Vector3 pos = cell.transform.position;
        cell.transform.position = new Vector3(pos.x, pos.y + terrainHeight, pos.z);
        cell.h = pos.y + terrainHeight*90f;
    }

    void initWind(Cell cell)
    {
        cell.windAngle = Forest_Fire.Wind.windAngle(direction);
        cell.v = WindForce;
    }

    void initCoefficients(Cell cell)
    {
        cell.T = Temperature;
        cell.RH = AirHum;
        cell.width = (8f / gridSize)*1000f;
    }

    void initWindSpeed(Cell cell, int i, int j)
    {
        cell.v = windSpeedReader.getWindAtCoord(gridSize - j-1, i, gridSize);
        Instantiate(Windcube, cell.transform);
    }
	
	void initCoordinates(Cell cell, int i, int j){
		
		
        Vector3 pos = cell.transform.position;
        cell.lat = startLat + latStep*(gridSize - j-1);
		cell.lon = startLon + lonStep*i;
	}
}
