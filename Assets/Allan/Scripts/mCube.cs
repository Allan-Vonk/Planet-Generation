using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mCube
{
    public Transform transform;
    public Point[] points = new Point[8];
    public int[] binaryArray = new int[8];
    public Point[] edgePoints = new Point[12];
    public Mesh mesh;
}
