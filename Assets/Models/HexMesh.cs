using Assets.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Полигональная сетка
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh hexMesh;
    /// <summary>
    /// Коллекция векторов
    /// </summary>
    List<Vector3> vertices;
    /// <summary>
    /// Коллекция треугольников
    /// </summary>
    List<int> triangles;
    /// <summary>
    /// Коллекция цветов
    /// </summary>
    List<Color> colors;
    /// <summary>
    /// Коллайдер для взаимодействия с объектами
    /// </summary>
    MeshCollider meshCollider;
   
    /// <summary>
    /// Инициализация полигональной сетки
    /// </summary>
    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();

        colors = new List<Color>();

        hexMesh.name = "Hex Mesh";

        vertices = new List<Vector3>();
        triangles = new List<int>();
    }
    /// <summary>
    /// Триангуляция массива ячеек
    /// </summary>
    /// <param name="cells"></param>
    public void Triangulate(HexCell[] cells)
    {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.colors = colors.ToArray();
        hexMesh.RecalculateNormals();
        meshCollider.sharedMesh = hexMesh;
    }
    
    /// <summary>
    /// Триангуляция ячейки (разбиение на треугольники)
    /// </summary>
    /// <param name="cell"></param>
    private void Triangulate(HexCell cell)    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(d, cell);
        }        
    }

    void Triangulate(HexDirection direction, HexCell cell)
    {
        Vector3 center = cell.transform.localPosition;
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

        AddTriangle(center, v1, v2);
        AddTriangleColor(cell.color);
        if (direction <= HexDirection.SE)
        {
            TriangulateConnection(direction, cell, v1, v2);
        }
    }
    /// <summary>
    /// Триангуляция соединений между ячейками
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="cell"></param>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    void TriangulateConnection(
        HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2
    )
    {
        HexCell neighbor = cell.GetNeighbor(direction);
        if (neighbor == null)
        {
            return;
        }

        Vector3 bridge = HexMetrics.GetBridge(direction);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;
        v3.y = v4.y = neighbor.Elevation * HexMetrics.elevationStep;

        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        if (direction <= HexDirection.E && nextNeighbor != null)
        {
            Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
            v5.y = nextNeighbor.Elevation * HexMetrics.elevationStep;

            /// Проверка уровеня соседних ячеек 
            if (cell.Elevation <= neighbor.Elevation)
            {
                if (cell.Elevation <= nextNeighbor.Elevation)
                {
                    TriangulateCorner(v2, cell, v4, neighbor, v5, nextNeighbor);
                }
                else
                {
                    TriangulateCorner(v5, nextNeighbor, v2, cell, v4, neighbor);
                }
            }
            else if (neighbor.Elevation <= nextNeighbor.Elevation)
            {
                TriangulateCorner(v4, neighbor, v5, nextNeighbor, v2, cell);
            }
            else
            {
                TriangulateCorner(v5, nextNeighbor, v2, cell, v4, neighbor);
            }
            AddTriangle(v2, v4, v5);
            AddTriangleColor(cell.color, neighbor.color, nextNeighbor.color);
        }
        if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
        {
            TriangulateEdgeTerraces(v1, v2, cell, v3, v4, neighbor);
        }
        else
        {
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(cell.color, neighbor.color);
        }
        //AddQuad(v1, v2, v3, v4);
        //AddQuadColor(cell.color, neighbor.color);
    }
    /// <summary>
    /// Триангуляция ступеней
    /// </summary>
    /// <param name="beginLeft"></param>
    /// <param name="beginRight"></param>
    /// <param name="beginCell"></param>
    /// <param name="endLeft"></param>
    /// <param name="endRight"></param>
    /// <param name="endCell"></param>
    void TriangulateEdgeTerraces(
        Vector3 beginLeft, Vector3 beginRight, HexCell beginCell,
        Vector3 endLeft, Vector3 endRight, HexCell endCell
    )
    {
        Vector3 v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(beginRight, endRight, 1);
        Color c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, 1);

        AddQuad(beginLeft, beginRight, v3, v4);
        AddQuadColor(beginCell.color, c2);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c2;
            v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, i);
            v4 = HexMetrics.TerraceLerp(beginRight, endRight, i);
            c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, i);
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(c1, c2);
        }

        AddQuad(v3, v4, endLeft, endRight);
        AddQuadColor(c2, endCell.color);
    }
    /// <summary>
    /// Триангуляция площадки (треугольника), связывающей 3 ячейки
    /// </summary>
    /// <param name="bottom"></param>
    /// <param name="bottomCell"></param>
    /// <param name="left"></param>
    /// <param name="leftCell"></param>
    /// <param name="right"></param>
    /// <param name="rightCell"></param>
    void TriangulateCorner(
        Vector3 bottom, HexCell bottomCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell
    )
    {
        HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
        HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

        if (leftEdgeType == HexEdgeType.Slope)
        {
            if (rightEdgeType == HexEdgeType.Slope)
            {
                TriangulateCornerTerraces(
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
            else if (rightEdgeType == HexEdgeType.Flat)
            {
                TriangulateCornerTerraces(
                    left, leftCell, right, rightCell, bottom, bottomCell
                );
            }
            else
            {
                TriangulateCornerTerracesCliff(
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
        }
        else if (rightEdgeType == HexEdgeType.Slope)
        {
            if (leftEdgeType == HexEdgeType.Flat)
            {
                TriangulateCornerTerraces(
                    right, rightCell, bottom, bottomCell, left, leftCell
                );
            }
            else
            {
                TriangulateCornerCliffTerraces(
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
        }
        else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            if (leftCell.Elevation < rightCell.Elevation)
            {
                TriangulateCornerCliffTerraces(
                    right, rightCell, bottom, bottomCell, left, leftCell
                );
            }
            else
            {
                TriangulateCornerTerracesCliff(
                    left, leftCell, right, rightCell, bottom, bottomCell
                );
            }
        }
        else
        {
            AddTriangle(bottom, left, right);
            AddTriangleColor(bottomCell.color, leftCell.color, rightCell.color);
        }
        AddTriangle(bottom, left, right);
        AddTriangleColor(bottomCell.color, leftCell.color, rightCell.color);
    }    
    void TriangulateCornerTerraces(
        Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell
    )
    {
        Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
        Color c3 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);
        Color c4 = HexMetrics.TerraceLerp(beginCell.color, rightCell.color, 1);

        AddTriangle(begin, v3, v4);
        AddTriangleColor(beginCell.color, c3, c4);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c3;
            Color c2 = c4;
            v3 = HexMetrics.TerraceLerp(begin, left, i);
            v4 = HexMetrics.TerraceLerp(begin, right, i);
            c3 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
            c4 = HexMetrics.TerraceLerp(beginCell.color, rightCell.color, i);
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(c1, c2, c3, c4);
        }

        AddQuad(v3, v4, left, right);
        AddQuadColor(c3, c4, leftCell.color, rightCell.color);
    }
    void TriangulateCornerTerracesCliff(
    Vector3 begin, HexCell beginCell,
    Vector3 left, HexCell leftCell,
    Vector3 right, HexCell rightCell)
    {
        float b = 1f / (rightCell.Elevation - beginCell.Elevation);
        if (b < 0)
        {
            b = -b;
        }
        Vector3 boundary = Vector3.Lerp(begin, right, b);
        Color boundaryColor = Color.Lerp(beginCell.color, rightCell.color, b);

        TriangulateBoundaryTriangle(
            begin, beginCell, left, leftCell, boundary, boundaryColor
        );

        if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            TriangulateBoundaryTriangle(
                left, leftCell, right, rightCell, boundary, boundaryColor
            );
        }
        else
        {
            AddTriangle(left, right, boundary);
            AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
        }
    }
    void TriangulateCornerCliffTerraces(
        Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 right, HexCell rightCell
    )
    {
        float b = 1f / (leftCell.Elevation - beginCell.Elevation);
        if (b < 0)
        {
            b = -b;
        }
        Vector3 boundary = Vector3.Lerp(begin, left, b);
        Color boundaryColor = Color.Lerp(beginCell.color, leftCell.color, b);

        TriangulateBoundaryTriangle(
            right, rightCell, begin, beginCell, boundary, boundaryColor
        );

        if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            TriangulateBoundaryTriangle(
                left, leftCell, right, rightCell, boundary, boundaryColor
            );
        }
        else
        {
            AddTriangle(left, right, boundary);
            AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
        }
    }

    void TriangulateBoundaryTriangle(
        Vector3 begin, HexCell beginCell,
        Vector3 left, HexCell leftCell,
        Vector3 boundary, Color boundaryColor
    )
    {
        Vector3 v2 = HexMetrics.TerraceLerp(begin, left, 1);
        Color c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);

        AddTriangle(begin, v2, boundary);
        AddTriangleColor(beginCell.color, c2, boundaryColor);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v2;
            Color c1 = c2;
            v2 = HexMetrics.TerraceLerp(begin, left, i);
            c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
            AddTriangle(v1, v2, boundary);
            AddTriangleColor(c1, c2, boundaryColor);
        }

        AddTriangle(v2, left, boundary);
        AddTriangleColor(c2, leftCell.color, boundaryColor);
    }
    /// <summary>
    /// Цвет для каждого угла
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <param name="c3"></param>
    void AddTriangleColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }
    void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }

    /// <summary>
    ///  Разбиение на векторы и треугольники
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="v3"></param>
    void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    /// <summary>
    /// Добавление трапеции(упрощение триангуляции)
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="v3"></param>
    /// <param name="v4"></param>
    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }
    /// <summary>
    /// Добавление цвета трапеции
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <param name="c3"></param>
    /// <param name="c4"></param>
    void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        colors.Add(c4);
    }
    void AddQuadColor(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }

}
