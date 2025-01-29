using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.Rendering;

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
    private GameObject Player;
    private void Start ()
    {
        initiated = true;
        Debug.Log("Starting");
        Initiate();
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnValidate()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if (!Application.isPlaying)
        {
            Initiate();
        }
    }
    private void Initiate ()
    {
        Debug.Log("Initiating"); 
        Context.CentreOfPlanet = transform;

        qt = new QuadTree(new Cube(CentreOfPlanet, new Vector3(Context.PlanetSize, Context.PlanetSize, Context.PlanetSize)), this, startLod);
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
        Handles.zTest = CompareFunction.LessEqual; // Set depth testing to respect the depth buffer

        foreach (QuadTree leaf in Leaves)
        {
            Vector3 position = leaf.RelativeBoundary.position + Context.CentreOfPlanet.position;
            Vector3 size = leaf.RelativeBoundary.size;

            // Calculate the color based on the LOD level
            float lodRatio = (float)leaf.lod / highestLod;
            Handles.color = Color.Lerp(Color.green, Color.red, lodRatio);

            Handles.DrawWireCube(position, size);
        }


    // Draw a circle around the player to visualize the LOD radius
    if (Player != null)
    {
        float lodRadius = (Context.BaseLodRadius* (Context.PlanetSize/10000)) * 
                         Mathf.Pow(Context.LodFalloff, Context.MaxLod - 0);
        Vector3 playerPosition = Player.transform.position;
        Gizmos.color = Color.yellow; // Set the color of the circle
        Gizmos.DrawWireSphere(playerPosition, lodRadius); // Draw a wire sphere around the player
    }
    }
}

