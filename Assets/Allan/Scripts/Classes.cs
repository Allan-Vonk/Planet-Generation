using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Rectangle
{
    public Vector2 size;
    public Vector2 position;
    public Rectangle (Vector2 position, Vector2 size)
    {
        this.position = position;
        this.size = size;
    }
    public bool contains (Point p)
    {
        return (p.position.x > position.x - size.x && p.position.x < position.x + size.x && p.position.y > position.y - size.y && p.position.y < position.y + size.y);
    }
}
public class Cube
{
    public Vector3 size;
    public Vector3 position;
    public Cube(Vector3 position, Vector3 size)
    {
        this.size = size;
        this.position = position;
    }
    public bool contains(Point3 p3)
    {
        return (p3.position.x > position.x - size.x && p3.position.x < position.x + size.x && p3.position.y > position.y - size.y && p3.position.y < position.y + size.y && p3.position.z < position.z + size.z && p3.position.z >position.z - size.z);
    }
}
public class Point3
{
    public Vector3 position;
    public Point3(Vector3 position)
    {
        this.position = position;
    }
}

