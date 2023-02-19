using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeStarter : MonoBehaviour
{
    public List<Cube>Boundaries = new List<Cube>();
    public List<Point3>Points = new List<Point3>();
    public List<Point>marchingCubePoints = new List<Point>();
    public List<QuadTree>Leaves = new List<QuadTree>();
    public List<QuadTree>BranchesAndLeaves = new List<QuadTree>();
    public float NoiseScale;
    public float Amplitude;
    public float surfaceLevel;
    public int startLod;
    public int maxLod;
    public Vector3 CentreOfPlanet;
    public int AmountOfPointsPerAxis = 20;
    public Material material;

    [SerializeField] MarchingCubeContext Context;

    private QuadTree qt;
    private void Start ()
    {
        Application.targetFrameRate = 60;
        Initiate();
    }
    private void Initiate ()
    {
        Context.CentreOfPlanet = transform.position;

        qt = new QuadTree(new Cube(CentreOfPlanet, new Vector3Int(10000, 10000, 10000)), this, material, startLod, Context);

        //for (int i = 0; i < 0; i++)
        //{
        //    qt.SubDevide();
        //}
        //Gather leaves
        //Generate cubes for the leaves
        Leaves = qt.GetLeaves();
        foreach (QuadTree quadTree in Leaves)
        {
            quadTree.CreateChunk();
            //foreach (Point point in quadTree.marchingChunk.Points)
            //{
            //    marchingCubePoints.Add(point);
            //}
        }
    }
    public void KillChunk (GameObject gameObject)
    {
        StartCoroutine(KillGameObject(gameObject));
    }
    IEnumerator KillGameObject (GameObject gameObject)
    {
        yield return new WaitForSeconds(0);
        Object.Destroy(gameObject);
        yield return null;
    }
    private void Update ()
    {
        Leaves = qt.GetLeaves();
        BranchesAndLeaves = qt.GetBranchesAndLeaves();
        foreach (QuadTree quadTree in BranchesAndLeaves)
        {
            if (quadTree != null)
            {
                quadTree.CalledUpdate();
            }
        }
        //MarchingCubeContext PContext = new MarchingCubeContext
        //{
        //    AmountOfPointsPerAxis = AmountOfPointsPerAxis,
        //    SurfaceLevel = surfaceLevel,
        //    CentreOfPlanet = CentreOfPlanet,
        //    MaxLod = maxLod,
            
        //    Amplitude = Amplitude,
        //    NoiseScale = NoiseScale
        //};
        //if (Context.Amplitude != PContext.Amplitude || Context.NoiseScale != PContext.NoiseScale || Context.SurfaceLevel != PContext.SurfaceLevel)
        //{
        //    Context = PContext;
        //    Initiate();
        //}
    }
    public Vector3 GetWorldPosition ()
    {
        return transform.position;
    }
    private void OnDrawGizmos ()
    {
        foreach (Cube cube in Boundaries)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(cube.position, cube.size);
        }
        foreach (Point3 point in Points)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(point.position, .1f);
        }
        foreach (Point point in marchingCubePoints)
        {
            if (point != null)
            {
                Gizmos.color = (point.value > surfaceLevel) ? Color.green : Color.red;
                Gizmos.DrawWireSphere(point.position, 10f);
            }
        }
    }
}

