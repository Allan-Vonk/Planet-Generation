using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class QuadTreeStarter : MonoBehaviour
{
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

    public bool reset = false;
    public MarchingCubeContext Context;

    private QuadTree qt;
    private void Start ()
    {
        Application.targetFrameRate = 60;
        Initiate();
    }
    private void OnValidate()
    {
        Start();
    }
    private void Initiate ()
    {
        Context.CentreOfPlanet = transform;

        qt = new QuadTree(new Cube(CentreOfPlanet, new Vector3Int(10000, 10000, 10000)), this, startLod);
    }
    private void Update ()
    {
        if (reset)
        {
            Start();
            reset = false;
        }
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
    private void OnDrawGizmos ()
    {
        foreach (QuadTree leaf in Leaves)
        {
            if (leaf.divided == false)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(leaf.RelativeBoundary.position + Context.CentreOfPlanet.position, leaf.RelativeBoundary.size);
            }
        }
    }
}

