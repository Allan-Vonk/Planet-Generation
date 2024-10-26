﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MarchingChunk
{
    //Variables
    public Mesh mesh;
    public List<Vector3> m_vertices = new List<Vector3>();
    public List<int> m_triangles = new List<int>();


    ComputeBuffer inputBuffer; 
    ComputeBuffer resultBuffer;

    //Privates
    private MarchingCubeContext MarchingContext;
    private Point[] Points;
    private Vector3 CornerPosition = Vector3.zero;
    public MarchingChunk (MarchingCubeContext Context, Cube Boundary)
    {
        MarchingContext = Context;
        MarchingContext.Cube = Boundary;
        Points = new Point[MarchingContext.AmountOfPointsPerAxis + 1 * MarchingContext.AmountOfPointsPerAxis + 1* MarchingContext.AmountOfPointsPerAxis + 1];
    }
    //instantiate the mapvalues and assign world positions to them
    private struct positionBufferData
    {
        public Vector3 CornerPosition;
        public Vector3 SpaceBetweenPoints;
        public Vector3 CenterOfPlanet;
        public float NoiseScale;
        public float NoiseAmplitude;
        public int AmountOfPointsPerAxis;
    }
    public void GeneratePositionData ()
    {
        CornerPosition = (MarchingContext.Cube.position - new Vector3(MarchingContext.Cube.size.x / 2, MarchingContext.Cube.size.y / 2, MarchingContext.Cube.size.z / 2));
        Vector3 spaceBetweenPoints = MarchingContext.Cube.size / (MarchingContext.AmountOfPointsPerAxis-1);
        positionBufferData positionData = new positionBufferData 
        {
            CornerPosition = CornerPosition, 
            SpaceBetweenPoints = spaceBetweenPoints, 
            CenterOfPlanet = MarchingContext.CentreOfPlanet, 
            AmountOfPointsPerAxis = MarchingContext.AmountOfPointsPerAxis,
            NoiseAmplitude = MarchingContext.NoiseLayerSettings.Layers[0].Amplitude,
            NoiseScale = MarchingContext.NoiseLayerSettings.Layers[0].NoiseScale
        };
        positionBufferData[] positionDataArray = new positionBufferData[1];
        positionDataArray[0] = positionData;

        int totalPoints = (MarchingContext.AmountOfPointsPerAxis +1) *
                  (MarchingContext.AmountOfPointsPerAxis + 1) *
                  (MarchingContext.AmountOfPointsPerAxis + 1);


        inputBuffer = new ComputeBuffer(1, (sizeof(float) * 11) + sizeof(int) * 1);
        resultBuffer = new ComputeBuffer(totalPoints,sizeof(float) * 4 + sizeof(int) * 1 );

        inputBuffer.SetData(positionDataArray);

        int kernelIndex = MarchingContext.computeShader.FindKernel("CSMain");

        MarchingContext.computeShader.SetBuffer(kernelIndex, "PositionBuffer", inputBuffer);
        MarchingContext.computeShader.SetBuffer(kernelIndex, "ResultBuffer", resultBuffer);

        // Dispatch the compute shader

        MarchingContext.computeShader.Dispatch(kernelIndex, MarchingContext.AmountOfPointsPerAxis, MarchingContext.AmountOfPointsPerAxis, MarchingContext.AmountOfPointsPerAxis);

        // Retrieve the result data from the GPU
        Point[] resultArray = new Point[totalPoints];

        resultBuffer.GetData(resultArray);
        Points = resultArray;
        // Release buffers when done
        inputBuffer.Release();
        resultBuffer.Release();



        //for (int x = 0; x < MarchingContext.AmountOfPointsPerAxis + 1; x++)
        //{
        //    for (int y = 0; y < MarchingContext.AmountOfPointsPerAxis + 1; y++)
        //    {
        //        for (int z = 0; z < MarchingContext.AmountOfPointsPerAxis + 1; z++)
        //        {
        //            //POINTS PER AXIS
        //            //space between points
        //            //corner position
        //            //center pos
        //            //Assign position worldPositions

        //            CornerPosition = (MarchingContext.Cube.position - new Vector3(MarchingContext.Cube.size.x / 2, MarchingContext.Cube.size.y / 2, MarchingContext.Cube.size.z / 2));
        //            Points[x, y, z] = new Point();
        //            Vector3 spaceBetweenPoints = MarchingContext.Cube.size / MarchingContext.AmountOfPointsPerAxis;
        //            Points[x, y, z].position = CornerPosition + new Vector3(spaceBetweenPoints.x * x, spaceBetweenPoints.y * y, spaceBetweenPoints.z * z);
        //            //Assign position values
        //            float distancetocenter = Vector3.Distance(MarchingContext.CentreOfPlanet,Points[x,y,z].position);

        //            Points[x, y, z].value = distancetocenter / 10000 + (EvaluatePoint(Points[x,y,z].position,20, 1, 1.7f))/*(Get3DPerlinValue((Points[x,y,z].position + new Vector3(1000,1000,1000)) * MarchingContext.NoiseScale) * MarchingContext.Amplitude)*/;
        //        }
        //    }
        //}
    }
    public float EvaluatePoint(Vector3 point, int layers, float scale, float roughness)
    {
        float noiseValue = 0;
        //float frequency = MarchingContext.NoiseLayerSettings.NoiseScale;
        //float amplitude = MarchingContext.NoiseLayerSettings.Amplitude;

        for (int i = 0; i < MarchingContext.NoiseLayerSettings.Layers.Length; i++)
        {
            float v = Get3DPerlinValue(point * MarchingContext.NoiseLayerSettings.Layers[i].NoiseScale + new Vector3(1000, 1000, 1000));
            noiseValue += v * MarchingContext.NoiseLayerSettings.Layers[i].Amplitude;
        }

        //for (int i = 0; i < layers; i++)
        //{
        //    float v = Get3DPerlinValue(point * frequency + new Vector3(1000,1000,1000));
        //    noiseValue += v * amplitude;
        //    frequency *= roughness;
        //    amplitude *= .5f;
        //}
        return noiseValue * scale;
    }
    //clear mesh and generate new mesh data
    public void GenerateMeshData ()
    {
        ClearMesh();
        for (int x = 0; x < MarchingContext.AmountOfPointsPerAxis-1; x++)
        {
            for (int y = 0; y < MarchingContext.AmountOfPointsPerAxis-1; y++)
            {
                for (int z = 0; z < MarchingContext.AmountOfPointsPerAxis-1; z++)
                {
                    March(new Vector3Int(x, y, z));
                }
            }
        }
    }
    public int GetIndexOfItemFromFlattenedArray(int x, int y, int z, int amountOfPointsPerAxis)
    {
        //id.x + (id.y * data.AmountOfPointsPerAxis) + (id.z * data.AmountOfPointsPerAxis * data.AmountOfPointsPerAxis);
        // Calculate the index in the flattened array
        int index = x + (y * amountOfPointsPerAxis) + (z * amountOfPointsPerAxis * amountOfPointsPerAxis);

        // Return the value at the calculated index
        return index;
    }


    //Clear the mesh
    public void ClearMesh ()
    {
        m_vertices.Clear();
        m_triangles.Clear();
    }

    //Marching cube algorithm to generate mesh data
    public void March (Vector3Int pos)
    {
        //Get cube config
        float[] cube = new float[8];
        for (int i = 0; i < 8; i++)
        {
            cube[i] = SampleTerrain(pos + Tables.CornerIndex[i]);
        }
        int index = GetConfig(cube);
        //If the config is 0 or 256 this means that it is either entirely above the surface or below meaning it should be empty space
        if (index == 0 || index == 256)
        {
            return;
        }

        //For every triangle in cube
        int edgeIndex = 0;
        for (int t = 0; t < 5; t++)
        {
            //For every vertices in triangle
            for (int v = 0; v < 3; v++)
            {
                //Get indece from index in the lookuptable
                if (Tables.RegularCellData[Tables.RegularCellClass[index]].Indizes().Length == 0 || edgeIndex > Tables.RegularCellData[Tables.RegularCellClass[index]].Indizes().Length - 1)
                {
                    return;
                }
                //int Index = MarchingCubesLookupTable.instance.MarchingCubeEdgeTable[index][edgeIndex];
                int Index;
                Index = Tables.RegularCellData[Tables.RegularCellClass[index]].Indizes()[edgeIndex];
                //No need to run the calculations if index is -1
                if (Index == -1) 
                {
                    return;
                }
                //Smoothly place edgeposition
                //Debug.Log((Tables.RegularVertexData[index][edgeIndex] & 0x0F) / 10 + " " + (Tables.RegularVertexData[index][edgeIndex] & 0x0F) % 10);
                //Debug.Log($"{index}, {edgeIndex}, {Tables.RegularVertexData[index].Length}");

                int vd1 = Tables.RegularVertexData[index][Index] & 0x0F;
                int vd2 = (Tables.RegularVertexData[index][Index] >> 4) & 0x0F;

                //Debug.Log(vd);
                Vector3 vert1 = pos + Tables.CornerIndex[vd1];
                Vector3 vert2 = pos + Tables.CornerIndex[vd2];

                //Vector3 vert1 = pos + MarchingCubesLookupTable.instance.CornerTable[MarchingCubesLookupTable.instance.EdgeIndexes[Index, 0]];
                //Vector3 vert2 = pos + MarchingCubesLookupTable.instance.CornerTable[MarchingCubesLookupTable.instance.EdgeIndexes[Index, 1]];

                Vector3 vertPosition;

                vert1 = Points[GetIndexOfItemFromFlattenedArray((int)vert1.x, (int)vert1.y, (int)vert1.z,MarchingContext.AmountOfPointsPerAxis)].position;
                vert2 = Points[GetIndexOfItemFromFlattenedArray((int)vert2.x, (int)vert2.y, (int)vert2.z, MarchingContext.AmountOfPointsPerAxis)].position;

                //float vert1Sample = cube[MarchingCubesLookupTable.instance.EdgeIndexes[Index, 0]];
                //float vert2Sample = cube[MarchingCubesLookupTable.instance.EdgeIndexes[Index, 1]];

                float vert1Sample = cube[vd1];
                float vert2Sample = cube[vd2];
                float differ = vert2Sample - vert1Sample;

                if (differ == 0)
                {
                    differ = MarchingContext.SurfaceLevel;
                }
                else
                {
                    differ = (MarchingContext.SurfaceLevel - vert1Sample) / differ;
                }
                vertPosition = vert1 + ((vert2 - vert1) * differ);

                //vertPosition = (vert1 + vert2) / 2;
                //Add vertice to the triangle list
                m_triangles.Add(VertForIndice(vertPosition));
                edgeIndex++;
            }
        }
    }
    //Remove duplicate vertices
    int VertForIndice (Vector3 vert)
    {
        for (int i = 0; i < m_vertices.Count; i++)
        {
            if (m_vertices[i] == vert)
            {
                return i;
            }
        }
        m_vertices.Add(vert);
        return m_vertices.Count - 1;

    }
    //Sample a value from the valuemap (This is nescecery for readability) 
    float SampleTerrain (Vector3Int point)
    {
        return Points[GetIndexOfItemFromFlattenedArray(point.x, point.y, point.z,MarchingContext.AmountOfPointsPerAxis)].value;
    }

    //Get the int for triangles from the cube values
    private int GetConfig (float[] cube)
    {
        int index = 0;
        for (int i = 0; i < 8; i++)
        {
            if (cube[i] > MarchingContext.SurfaceLevel)
            {
                index |= 1 << i;
            }
        }
        return index;
    }
    float Get3DPerlinValue (Vector3 position)
    {
        float AB = Mathf.PerlinNoise(position.x, position.y);
        float BC = Mathf.PerlinNoise(position.y, position.z);
        float AC = Mathf.PerlinNoise(position.x, position.z);

        float BA = Mathf.PerlinNoise(position.y, position.x);
        float CB = Mathf.PerlinNoise(position.z, position.y);
        float CA = Mathf.PerlinNoise(position.z, position.x);
        float ABC = AB + BC + AC + BA + CB + CA;
        return ABC / 6f;
    }
}