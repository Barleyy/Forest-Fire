using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public enum Combustibility { Unburned, StartBurning, FullBurning, Burned };

    public float state = 0;
    public float new_state = 0;
	
	public float lat;
	public float lon;
	
	public int  i;
	public int j;
	
    public List<Cell> close_neighbours;
    public List<Cell> diag_neighbours;

    public Dictionary<SimulationManager.Direction, Cell> Neighbours = new Dictionary<SimulationManager.Direction, Cell>();
    public Dictionary<SimulationManager.Direction, Cell> diagonalNeighbours;

    public float timestep = 0.2f;
    const float DIAG_FACTOR = 0.83f;
    const float FINISH_FACTOR = 0.90f;
    public float v;
    public float windAngle;
    public float m = 1f;
    public float Rmax = 1f;
    public float R = 1f;
    public float h;
    public float width;
   
    public float T;
    public float RH;
    float Kw;
    float Kh;

    //time index 
    float Kr = 1f;

    //combustible index
    float Ks = 1.2f;
    float R0;
    float W = 1f;

    Material material;
    bool _heightInit = false;
    Transform fire = null;

    public Combustibility burnState = Combustibility.Unburned;

    public float getState()
    {
        return state;
    }

   
    void Start()
    {
        material = GetComponent<Renderer>().material;
        //material.SetColor("_Color", Color.green);
        StartCoroutine(compute());
        initR0();
    }

    void initR0()
    {
        float a = 0.03f;
        float b = 0.05f;
        float c = 0.01f;
        float d = 0.3f;
        R0 = a * T + b * W + c * RH - d;
        W = Mathf.Floor(Mathf.Pow(v / 0.836f, 2f / 3f));
    }

    void Update()
    {
        if (!_heightInit)
            initHeigth();
    }

    void initHeigth()
    {
       // transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
    }

    private void FixedUpdate()
    {
        state = new_state;
        if (burnState == Combustibility.Unburned && new_state >= 1)
        {
            burnState = Combustibility.StartBurning;
            //ColorMesh(Color.magenta);
            fire = Instantiate(transform.parent.GetComponent<SimulationManager>().firePrefab,transform);
            fire.localPosition = new Vector3(1, 4, 1);
            Instantiate(transform.parent.GetComponent<SimulationManager>().fireBolt, transform);
            //fire.localScale = new Vector3(0.5f, 1, 0.5f);
        }
        else if(burnState == Combustibility.StartBurning && new_state >=2)
        {
            burnState = Combustibility.FullBurning;
            //ColorMesh(Color.red);
        }
        else if(burnState == Combustibility.FullBurning && new_state >= 3)
        {
            Destroy(fire.gameObject);
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().isKinematic = false;
            burnState = Combustibility.Burned;
            ColorMesh(Color.black);
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
        foreach (var item  in Neighbours)
        {
            Cell c = item.Value;
            if(c.burnState == Combustibility.StartBurning || c.burnState == Combustibility.FullBurning)
            {
                float fireSpreadAngle = Forest_Fire.Wind.windAngle(item.Key);
                // Difference between wind direction and fire spread direction 
                float windAngleDiff = Mathf.Deg2Rad*(fireSpreadAngle - 180f - windAngle);				
                Kw = Mathf.Exp(0.1783f * v * Mathf.Cos(windAngleDiff));

                // Height Factor
                float slope = calculateSlope(c.h);
                float g = h > c.h ? 1f : -1f;
                Kh = Mathf.Exp(3.553f * g * Mathf.Atan(1.2f * slope));


                R = R0 * Kw * Kh * Ks * Kr;
                new_state += R * m / Rmax;
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
        foreach (var item in Neighbours)
        {
            Cell c = item.Value;

            if (c.burnState < Combustibility.FullBurning)
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Terrain")
        {
            _heightInit = true;
        }
    }

    float calculateSlope(float neighbourHeight)
    {
        return Mathf.Atan((neighbourHeight - h) / width); 
    }
	
}
