using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public enum Combustibility { Unburned, StartBurning, FullBurning, Burned };

    public float state = 0;
    public float new_state = 0;
    public List<Cell> close_neighbours;
    public List<Cell> diag_neighbours;
    public float timestep = 0.2f;
    const float DIAG_FACTOR = 0.83f;
    const float FINISH_FACTOR = 0.90f;
    public float m = 1f;
    public float Rmax = 1f;
    public float R = 1f;
    Material material;


    public Combustibility burnState = Combustibility.Unburned;

    public float getState()
    {
        return state;
    }


    void Start()
    {
        material = GetComponent<Renderer>().material;
        material.SetColor("_Color", Color.green);
        StartCoroutine(compute());
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        state = new_state;
        if (burnState == Combustibility.Unburned && new_state >= 1)
        {
            burnState = Combustibility.StartBurning;
            ColorMesh(Color.magenta);
        }
        else if(burnState == Combustibility.StartBurning && new_state >=2)
        {
            burnState = Combustibility.FullBurning;
            ColorMesh(Color.red);
        }
        else if(burnState == Combustibility.FullBurning && new_state >= 3)
        {
            burnState = Combustibility.Burned;
            ColorMesh(Color.white);
        }
    }

    IEnumerator compute()
    {
        if(new_state >= 3.0)
        {
            yield break;
        }
        yield return new WaitForSeconds(timestep);
        switch (burnState)
        {
            case Combustibility.Unburned:
                computeState();
                break;
            case Combustibility.StartBurning:
                fullBurn();
                break;
            case Combustibility.FullBurning:
                extinguish();
                break;
            case Combustibility.Burned:
                break;
            default:
                break;
        }
        StartCoroutine(compute());
    }

    void computeState()
    {
        new_state = state;
        foreach (Cell c in close_neighbours)
        {
            if(c.burnState == Combustibility.StartBurning || c.burnState == Combustibility.FullBurning)
            {
                new_state += c.R * m / c.Rmax;
            }
        }
        if(new_state >= 1)
        {
            new_state = 1;
        }
    }

    void fullBurn()
    {
        new_state = 2;
    }

    void extinguish()
    {
        bool canExtinguish = true;
        foreach (Cell c in close_neighbours)
        {
            if(c.burnState < Combustibility.FullBurning)
            {
                canExtinguish = false;
            }
        }
        if(canExtinguish == true)
        {
            new_state = 3;
        }
    }

    void burn()
    {
        foreach (Cell c in close_neighbours)
        {
            new_state += c.getState();
        }
        foreach (Cell c in diag_neighbours)
        {
            new_state += DIAG_FACTOR * c.getState();
        }
    }

    void ColorMesh(Color color)
    {
        material.SetColor("_Color", color);
    }
}
