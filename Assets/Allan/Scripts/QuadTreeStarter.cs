using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
    public MarchingCubeContext Context;

    private bool initiated = false;

    private QuadTree qt;
    private void Start ()
    {
        initiated = true;
        Debug.Log("Starting");
        Initiate();
    }
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (!initiated)
            {
                Initiate();
                initiated = true;
            }
        }
    }
    private void Initiate ()
    {
        Debug.Log("Initiating"); 
        Context.CentreOfPlanet = transform;

        qt = new QuadTree(new Cube(CentreOfPlanet, new Vector3Int(10000, 10000, 10000)), this, startLod);
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
    private void OnDrawGizmos ()
    {
        int highestLod = Leaves.Max(leaf => leaf.lod);
        foreach (QuadTree leaf in Leaves)
        {
            if (leaf.divided == false && leaf.lod == highestLod)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(leaf.RelativeBoundary.position + Context.CentreOfPlanet.position, leaf.RelativeBoundary.size);
            }
        }
    }
}

