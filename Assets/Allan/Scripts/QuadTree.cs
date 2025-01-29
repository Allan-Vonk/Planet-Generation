using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class QuadTree
{
    //Quadtree variables
    public bool divided = false;
    public List<QuadTree>Children = new List<QuadTree>();
    private QuadTreeStarter Root;
    public Cube RelativeBoundary;

    //Marching cube variables
    public MarchingChunk marchingChunk;
    private GameObject ColliderObject;
    private GameObject Player;
    public int lod;
    //Unity variables
    private Mesh mesh = new Mesh();

    // Add initialization state tracking
    private bool m_IsGenerating;
    private bool m_IsDisposed;

    public QuadTree (Cube Boundary, QuadTreeStarter Root, int lod)
    {
        //Setting other variables
        this.RelativeBoundary = Boundary;
        this.lod = lod + 1;
        this.Root = Root;

        //Get the player
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        if (m_IsGenerating == false)
        {
            m_IsGenerating = true;
            CreateChunk();
        }
    }
    //Update being called from a monobehavior
    public void CalledUpdate ()
    {
        UpdateColliderState();

        UpdateLOD();

        //Draw mesh
        if (divided == false && marchingChunk != null && mesh != null)
        {
            Graphics.DrawMesh(mesh, Root.Context.CentreOfPlanet.position, Quaternion.identity,Root.material,0);
        }
    }
    private void UpdateColliderState ()
    {
        if (ColliderObject && !divided != ColliderObject.activeSelf)
        {
            ColliderObject.SetActive(!divided);
        }
    }
    private void UpdateLOD ()
    {
        Vector3 playerPos = Player.transform.position;
        Vector3 chunkWorldPos = RelativeBoundary.position + Root.transform.position;
        float sqrDistance = (playerPos - chunkWorldPos).sqrMagnitude;

        int desiredLod = 0;
        // Find the highest appropriate LOD for this distance
        for (int lodLevel = Root.Context.MaxLod; lodLevel >= 0; lodLevel--)
        {
            float lodRadius = Root.Context.BaseLodRadius * 
                             Mathf.Pow(Root.Context.LodFalloff, Root.Context.MaxLod - lodLevel);
            float sqrLodRadius = lodRadius * lodRadius;
            
            if (sqrDistance <= sqrLodRadius)
            {
                desiredLod = lodLevel;
                break;
            }
        }

        // Handle LOD transitions with hysteresis buffer
        if (desiredLod > lod && !divided && lod < Root.Context.MaxLod)
        {
            SubDivide();
            divided = true;
        }
        else if (desiredLod < lod && divided)
        {
            UnDivide();
            divided = false;
        }
    }
    //Generate mesh & meshCollider
    private async void CreateChunk ()
    {


        //Starting a asynchronous task to generate mesh data
        //var result = await Task.Run(()=>
        //{
        //    return new MarchingChunk(MarchingContext,Boundary);
        //});
        //Making mesh here instead of in the MarchingChunk for easier acces and because its not possible to create a new mesh on another threat
        marchingChunk = new MarchingChunk(Root.Context, RelativeBoundary);
        await marchingChunk.Initialize(Root.Context, RelativeBoundary);

        MarchingChunk.MeshData meshData = marchingChunk.GetMeshData();

        mesh.vertices = meshData.Vertices.ToArray();
        meshData.Triangles.Reverse();
        mesh.triangles = meshData.Triangles.ToArray();
        mesh.RecalculateNormals();

        // Only generate collider if not already generated
        if (Application.isPlaying && !ColliderObject) 
        {
            GenerateCollider();
        }

    }
    
    //Generate a collider + Gamobject for the collider to attach to
    public void GenerateCollider ()
    {
        Debug.Log("Generating collider " + m_IsGenerating);
        if (!ColliderObject )
        {
            ColliderObject = new GameObject();
            ColliderObject.name = "Collider: " + RelativeBoundary.position;
            ColliderObject.transform.parent = Root.gameObject.transform;
            ColliderObject.transform.position = Root.Context.CentreOfPlanet.position;
            MeshCollider collider = ColliderObject.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }

    }
    //Get the leaves in the quad tree 
    public void UnDivide ()
    {
        foreach(QuadTree child in Children)
        {
            child.Dispose();
        }
        Children.Clear();
        divided = false;
    }
    //Devides the Quadtree into 8 more Quads
    public void SubDivide ()
    {
        if (divided == false)
        {
            // Extracting boundary properties for easier access
            float quarterWidth = RelativeBoundary.size.x / 4;
            float quarterHeight = RelativeBoundary.size.y / 4;
            float quarterLength = RelativeBoundary.size.z / 4;
            float x = RelativeBoundary.position.x;
            float y = RelativeBoundary.position.y;
            float z = RelativeBoundary.position.z;
            // Creating boundaries for the 8 sub-cubes
            Cube[] subCubes = new Cube[8]
            {
                new Cube(new Vector3(x + quarterWidth, y + quarterHeight, z + quarterLength), new Vector3(quarterWidth * 2, quarterHeight * 2, quarterLength * 2)),
                new Cube(new Vector3(x + quarterWidth, y + quarterHeight, z - quarterLength), new Vector3(quarterWidth * 2, quarterHeight * 2, quarterLength * 2)),
                new Cube(new Vector3(x - quarterWidth, y + quarterHeight, z - quarterLength), new Vector3(quarterWidth * 2, quarterHeight * 2, quarterLength * 2)),
                new Cube(new Vector3(x - quarterWidth, y + quarterHeight, z + quarterLength), new Vector3(quarterWidth * 2, quarterHeight * 2, quarterLength * 2)),
                new Cube(new Vector3(x + quarterWidth, y - quarterHeight, z + quarterLength), new Vector3(quarterWidth * 2, quarterHeight * 2, quarterLength * 2)),
                new Cube(new Vector3(x + quarterWidth, y - quarterHeight, z - quarterLength), new Vector3(quarterWidth * 2, quarterHeight * 2, quarterLength * 2)),
                new Cube(new Vector3(x - quarterWidth, y - quarterHeight, z - quarterLength), new Vector3(quarterWidth * 2, quarterHeight * 2, quarterLength * 2)),
                new Cube(new Vector3(x - quarterWidth, y - quarterHeight, z + quarterLength), new Vector3(quarterWidth * 2, quarterHeight * 2, quarterLength * 2))
            };
            // Instantiating QuadTrees for each sub-cube
            foreach (Cube subCube in subCubes)
            {
                Children.Add(new QuadTree(subCube, Root, lod));
            }
            divided = true;
        }
        else
        {
            foreach (QuadTree quadTree in Children)
            {
                quadTree.SubDivide();
            }
        }
    }
    public List<QuadTree> GetLeaves ()
    {
        List<QuadTree>leaves = new List<QuadTree>();
        if (divided == false)
        {
            leaves.Add(this);
        }
        else
        {
            foreach (QuadTree quadTree in Children)
            {
                leaves.AddRange(quadTree.GetLeaves());
            }
        }
        return leaves;
    }
    public List<QuadTree> GetBranchesAndLeaves ()
    {
        List<QuadTree>BranchAndLeaves = new List<QuadTree>();
        if (divided == true)
        {
            foreach (QuadTree quadTree in Children)
            {
                BranchAndLeaves.AddRange(quadTree.GetBranchesAndLeaves());
            }
            BranchAndLeaves.Add(this);
        }
        else
        {
            BranchAndLeaves.Add(this);
        }
        return BranchAndLeaves;
    }

    public void Dispose()
    {
        if(m_IsDisposed) return;
        m_IsDisposed = true;

        // Clean up collider and children
        if(ColliderObject && divided == false)
        {
            if(Application.isPlaying)
                GameObject.Destroy(ColliderObject);
        }

        // Clean up children	
        foreach(QuadTree child in Children)
        {
            child.Dispose();
        }
        Children.Clear();

        // Release mesh memory
        if(mesh != null)
        {
            if(Application.isPlaying)
                GameObject.Destroy(mesh);

        }
    }
}
