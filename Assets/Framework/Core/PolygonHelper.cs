namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    
    public class PolygonHelper
    {
        public static Rect CalculateBoundingBox(IEnumerable<Vector2> points)
        {
            var minX = points.Min(p => p.x);
            var minY = points.Min(p => p.y);
            var maxX = points.Max(p => p.x);
            var maxY = points.Max(p => p.y);
            
            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }
        
        public static float CalculateArea(Vector2[] polygon)
        {
            int i,j;
            float area = 0; 
            
            for (i=0; i < polygon.Length; i++) {
                j = (i + 1) % polygon.Length;
                
                area += polygon[i].x * polygon[j].y;
                area -= polygon[i].y * polygon[j].x;
            }
            
            area /= 2;
            return (area < 0 ? -area : area);
        }
        
        public static bool ContainsPoint(Vector2[] polyPoints, Vector2 p) { 
            var j = polyPoints.Length-1; 
            var inside = false; 
            for (var i = 0; i < polyPoints.Length; j = i++) { 
                if ( ((polyPoints[i].y <= p.y && p.y < polyPoints[j].y) || (polyPoints[j].y <= p.y && p.y < polyPoints[i].y)) && 
                    (p.x < (polyPoints[j].x - polyPoints[i].x) * (p.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x)) 
                    inside = !inside; 
            } 
            return inside; 
        }
        
        public static bool ContainsRect(Vector2[] polyPoints, Rect rect)
        {
            return
                PolygonHelper.ContainsPoint(polyPoints, new Vector2(rect.xMin, rect.yMin)) &&
                    PolygonHelper.ContainsPoint(polyPoints, new Vector2(rect.xMax, rect.yMin)) &&
                    PolygonHelper.ContainsPoint(polyPoints, new Vector2(rect.xMax, rect.yMax)) &&
                    PolygonHelper.ContainsPoint(polyPoints, new Vector2(rect.xMin, rect.yMax));
        }
        
        public static Mesh CreateMesh(Vector2[] poly)
        {
            Triangulator triangulator = new Triangulator(poly);
            
            int[] tris = triangulator.Triangulate();
            
            Mesh m = new Mesh();
            
            Vector3[] vertices = new Vector3[poly.Length];
            
            for(int i=0;i<poly.Length;i++)            
            {
                vertices[i].x = poly[i].x;
                vertices[i].y = 0f;
                vertices[i].z = poly[i].y;
            }
            
            m.vertices = vertices;
            
            m.triangles = tris;
            
            Vector2[] uvs = new Vector2[vertices.Length];
            int j = 0;
            while (j < uvs.Length) {
                uvs[j] = new Vector2(vertices[j].x, vertices[j].z);
                j++;
            }
            
            m.uv = uvs;
            
            m.RecalculateNormals();
            
            m.RecalculateBounds();
            
            m.Optimize();
            
            return m;
        }
        
        public static Vector2[] TrianglePolygon(float size)
        {
            var poly = new [] {
                new Vector2(0f, 0f),
                new Vector2(size, 0f),
                new Vector2(0f, size)
            };
            
            return poly;
        }
        
        public static Mesh CreateTriangle(float size)
        {
            var mesh = CreateMesh(TrianglePolygon(size));
            
            return mesh;
        }
        
        public static Vector2[] SquarePolygon(float size)
        {
            var poly = new [] {
                new Vector2(size, size),
                new Vector2(size, -size),
                new Vector2(-size, -size),
                new Vector2(-size, size)
            };
            
            return poly;
        }
        
        public static Mesh CreateSquare(float size)
        {
            var mesh = CreateMesh(SquarePolygon(size));
            
            return mesh;
        }
        
        public static Vector2[] RandomPolygon(float radius)
        {
            var numPoints = Random.Range(3, 10);
            var stepSize = Mathf.PI * 2f / numPoints;
            var points = new Vector2[numPoints];
            
            for (int i=0; i < numPoints; i++)
            {
                var a = i * stepSize;
                var x = radius * Mathf.Cos(a);
                var y = radius * Mathf.Sin(a);
                points[i] = new Vector2(x, y);
            }
            
            return points;
        }
    }
}