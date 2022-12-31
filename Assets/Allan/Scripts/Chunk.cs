using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector3Int width;
    Point[,,] points;
    List<mCube> Cubes;
    List<CombineInstance> combineInstances = new List<CombineInstance>();
    private MeshFilter mf;
    private MeshCollider mc;
    [SerializeField] private float noiseScale;
    [SerializeField] private float multiplier;
    [SerializeField] private float surfacelevel;
    [SerializeField] private float terrainSurface;
    [SerializeField] private float smoothing;
    private MeshRenderer mr;
    private void Start()
    {
        mr = gameObject.GetComponent<MeshRenderer>();
        mf = gameObject.GetComponent<MeshFilter>();
        mc = gameObject.GetComponent<MeshCollider>();
        Cubes = new List<mCube>();
        #region point generation
        points = new Point[width.x,width.y,width.z];
        for (int x = 0; x < width.x; x++)
        {
            for (int y = 0; y < width.y; y++)
            {
                for (int z = 0; z < width.z; z++)
                {
                    points[x, y, z] = new Point { position = new Vector3(x, y, z)};
                }
            }
        }
        #endregion
        #region test Terrain Region
        for (int x = 0; x < width.x; x++)
        {
            for (int y = 0; y < width.y; y++)
            {
                for (int z = 0; z < width.z; z++)
                {
                    float perlinValue = Mathf.PerlinNoise((points[x,y,z].position.x + transform.position.x) * noiseScale, (points[x, y, z].position.z + transform.position.z) * noiseScale) * multiplier + surfacelevel;
                    if (points[x,y,z].position.y + transform.position.y > perlinValue)
                    {
                        points[x, y, z].state = false;
                    }
                    else
                    {
                        points[x, y, z].state = true;
                        points[x, y, z].position.y = perlinValue - transform.position.y;
                    }
                    points[x, y, z].value = points[x, y, z].position.y + transform.position.y;
                }
            }
        }
        #endregion
        #region cube point assignment
        for (int x = 0; x < width.x -1; x++)
        {
            for (int y = 0; y < width.y -1; y++)
            {
                for (int z = 0; z < width.z -1; z++)
                {
                    mCube tempcube = new mCube();
                    #region Setting points;
                    //Vertices
                    tempcube.points[0] = points[1 + x, 0 + y, 1 + z];
                    tempcube.points[1] = points[1 + x, 0 + y, 0 + z];
                    tempcube.points[2] = points[0 + x, 0 + y, 0 + z];
                    tempcube.points[3] = points[0 + x, 0 + y, 1 + z];
                    tempcube.points[4] = points[1 + x, 1 + y, 1 + z];
                    tempcube.points[5] = points[1 + x, 1 + y, 0 + z];
                    tempcube.points[6] = points[0 + x, 1 + y, 0 + z];
                    tempcube.points[7] = points[0 + x, 1 + y, 1 + z];
                    //Edges
                    tempcube.edgePoints[0] = new Point { position = (tempcube.points[0].position + tempcube.points[1].position) / 2 - transform.position };
                    tempcube.edgePoints[1] = new Point { position = (tempcube.points[1].position + tempcube.points[2].position) / 2 - transform.position };
                    tempcube.edgePoints[2] = new Point { position = (tempcube.points[2].position + tempcube.points[3].position) / 2 - transform.position };
                    tempcube.edgePoints[3] = new Point { position = (tempcube.points[3].position + tempcube.points[0].position) / 2 - transform.position };
                    tempcube.edgePoints[4] = new Point { position = (tempcube.points[4].position + tempcube.points[5].position) / 2 - transform.position };
                    tempcube.edgePoints[5] = new Point { position = (tempcube.points[5].position + tempcube.points[6].position) / 2 - transform.position };
                    tempcube.edgePoints[6] = new Point { position = (tempcube.points[6].position + tempcube.points[7].position) / 2 - transform.position };
                    tempcube.edgePoints[7] = new Point { position = (tempcube.points[7].position + tempcube.points[4].position) / 2 - transform.position };
                    tempcube.edgePoints[8] = new Point { position = (tempcube.points[0].position + tempcube.points[4].position) / 2 - transform.position };
                    tempcube.edgePoints[9] = new Point { position = (tempcube.points[1].position + tempcube.points[5].position) / 2 - transform.position };
                    tempcube.edgePoints[10] = new Point { position = (tempcube.points[2].position + tempcube.points[6].position) / 2 - transform.position };
                    tempcube.edgePoints[11] = new Point { position = (tempcube.points[3].position + tempcube.points[7].position) / 2 - transform.position };


                        ////0 => 4
                        //if (tempcube.points[0].value - tempcube.points[4].value != 0.0f)
                        //{
                        //    tempcube.edgePoints[8] = new Point { position = tempcube.edgePoints[8].position - new Vector3(0, tempcube.points[0].value / (tempcube.points[0].value - tempcube.points[4].value) * smoothing,0) };
                        //}
                        ////1 => 4
                        //if (tempcube.points[1].value - tempcube.points[5].value != 0.0f)
                        //{
                        //    tempcube.edgePoints[9] = new Point { position = tempcube.edgePoints[9].position - new Vector3(0, tempcube.points[1].value / (tempcube.points[1].value - tempcube.points[5].value) * smoothing, 0) };
                        //}
                        ////2 =>6
                        //if (tempcube.points[2].value - tempcube.points[6].value != 0.0f)
                        //{
                        //    tempcube.edgePoints[10] = new Point { position = tempcube.edgePoints[10].position - new Vector3(0, tempcube.points[2].value / (tempcube.points[2].value - tempcube.points[6].value) * smoothing, 0) };
                        //}

                        ////3 => 7
                        //if (tempcube.points[3].value - tempcube.points[7].value != 0.0f)
                        //{
                        //    tempcube.edgePoints[11] = new Point { position = tempcube.edgePoints[11].position - new Vector3(0, tempcube.points[3].value / (tempcube.points[3].value - tempcube.points[7].value) * smoothing, 0) };
                        //}
                        #endregion
                        tempcube.transform = gameObject.transform;
                    tempcube.transform.position = transform.position;
                    Cubes.Add(tempcube);
                }
            }
        }
        #endregion

        #region convert to binary array and generate mesh
        foreach (var cube in Cubes)
        {
            int[] temparray = new int[8];
            for (int i = 0; i < 8; i++)
            {
                if (cube.points[i].state == true)
                {
                    temparray[i] = 1;
                }
                else
                {
                    temparray[i] = 0;
                }
            }
            cube.binaryArray = temparray;
            Vector3[] vertices = new Vector3[12];
            for (int i = 0; i < 12; i++)
            {
                vertices[i] = cube.edgePoints[i].position;
            }
            cube.mesh = new Mesh();
            cube.mesh.vertices = vertices;
            int cubeindex = 0;
            for (int i = 0; i < 8; i++)
            {
                if (cube.binaryArray[i] > 0)
                {
                    cubeindex |= 1 << i;
                }
            }
            int[] triangles = new int[12];

            for (int i = 0; i < 12; i++)
            {
                if (MarchingCubesLookupTable.instance.MarchingCubeEdgeTable[cubeindex][i] != -1)
                {
                    triangles[11-i] = MarchingCubesLookupTable.instance.MarchingCubeEdgeTable[cubeindex][i];
                }
            }
            cube.mesh.triangles = triangles;
            CombineInstance ci = new CombineInstance();
            ci.mesh = cube.mesh;
            ci.transform = cube.transform.localToWorldMatrix;
            combineInstances.Add(ci);
        }

        #endregion
        #region combine all combine Instances and add to mesh render
        Mesh tempMesh = new Mesh();
        tempMesh.CombineMeshes(combineInstances.ToArray());
        tempMesh.RecalculateNormals();
        tempMesh.RecalculateBounds();
        tempMesh.RecalculateTangents();
        mf.mesh = tempMesh;
        mc.sharedMesh = mf.mesh;
        #endregion
    }
    private void OnDrawGizmosSelected()
    {
        //draw gizmos for debug purposes
        foreach (var item in Cubes)
        {
            foreach (var point in item.points)
            {
                if (point.state == true)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(point.position + transform.position, 0.1f);
                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(point.position + transform.position, 0.1f);
                }
            }
        }
    }
}
