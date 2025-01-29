using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class QuadTreeStarter : MonoBehaviour
{
    public Dictionary<Vector3Int, float> GlobalModifications = new Dictionary<Vector3Int, float>();

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
        Context.Root = this;
        qt = new QuadTree(new Cube(CentreOfPlanet, new Vector3(Context.PlanetSize, Context.PlanetSize, Context.PlanetSize)), this, startLod);
    }
    public void AddGlobalModification(Vector3 markPoint, int pointRadius, float strength){
        //It should alter all points inside the radius of the markpoint and add value to the point according the strength and the distance from the markpoint
        //The radius should be in chunks
        Vector3Int markPointChunk = worldSpaceToChunkSpace(markPoint);
        for (int x = -pointRadius; x <= pointRadius; x++)
        {
            for (int y = -pointRadius; y <= pointRadius; y++)
            {
                for (int z = -pointRadius; z <= pointRadius; z++)
                {
                    Vector3Int point = new Vector3Int(markPointChunk.x + x, markPointChunk.y + y, markPointChunk.z + z);
                    float modificationValue = strength * (1 - Mathf.Sqrt(x * x + y * y + z * z) / pointRadius);

                    if (GlobalModifications.ContainsKey(point))
                    {
                        GlobalModifications[point] += modificationValue;
                    }
                    else
                    {
                        GlobalModifications.Add(point, modificationValue);
                    }
                }
            }
        }
        Debug.Log("Global Modifications: " + GlobalModifications.Count);
    }
    private Vector3Int worldSpaceToChunkSpace(Vector3 point)
    {
        // Calculate chunk size at maximum LOD (smallest possible chunks)
        float chunkSize = (Context.PlanetSize / (1 << (Context.MaxLod)))/Context.AmountOfPointsPerAxis;
        
        // Convert world position to chunk grid coordinates
        Vector3 relativePos = point;
        return new Vector3Int(
            Mathf.FloorToInt(relativePos.x),
            Mathf.FloorToInt(relativePos.y),
            Mathf.FloorToInt(relativePos.z)
        );
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


        //If the player presses space it should add a global modification to the point the player is looking at
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddGlobalModification(Player.transform.position, 10, 1);
            
            int highestLod = Leaves.Max(leaf => leaf.lod);
            //Also re generate the leaves
            foreach (QuadTree leaf in Leaves)
            {
                if (leaf.lod == highestLod)
                {
                    Debug.Log("Re generating leaf");
                    leaf.regenerating = true;
                    leaf.CreateChunk();
                }
            }
            foreach (var i in GlobalModifications)
            {
                Debug.Log("Global Modification: " + i.Key + " " + i.Value);
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
            Handles.Label(leaf.marchingChunk.cornerPosition + Context.CentreOfPlanetPosition, "Corner Position: " + leaf.marchingChunk.cornerPosition);
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

        // Debug chunk space conversion (add at end of method)
        if (Application.isPlaying && Player != null)
        {
            Vector3Int chunkPos = worldSpaceToChunkSpace(Player.transform.position);
            Handles.color = Color.cyan;
            Handles.Label(Player.transform.position + Vector3.up, 
                $"Chunk: {chunkPos}\nLOD: {maxLod}");
        }
        //Show radius applied in the update global modification as a wire sphere
        Handles.color = Color.red;
        Gizmos.DrawWireSphere(Player.transform.position, 10);
    }
}

