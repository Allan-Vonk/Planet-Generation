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
    public int PlanetSize;
    public Transform CentreOfPlanet;
    public Vector3 CentreOfPlanetPosition;
    public int MaxLod;
    public float LodDistanceMultiplier;
    public NoiseLayerSettings NoiseLayerSettings;
    public ComputeShader computeShader;
    public QuadTreeStarter Root;
    public float BaseLodRadius;
    public float LodFalloff;

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

