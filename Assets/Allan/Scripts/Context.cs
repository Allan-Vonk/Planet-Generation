using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public struct MarchingCubeContext
{
    public Cube Cube;
    public int AmountOfPointsPerAxis;
    public float SurfaceLevel;
    public Vector3 CentreOfPlanet;
    public int MaxLod;
    public NoiseLayerSettings NoiseLayerSettings;
    public ComputeShader computeShader;

}
[Serializable]
public struct NoiseLayerSettings
{
    public NoiseLayer[] Layers;
}
[Serializable]
public struct NoiseLayer 
{
    public float NoiseScale;
    public float Amplitude;
}

