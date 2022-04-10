using Assets.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HexCell : MonoBehaviour
{
    /// <summary>
    /// Координаты ячейки
    /// </summary>
    public HexCoordinates coordinates;
    /// <summary>
    /// Цвет ячейки
    /// </summary>
    public Color Color
    {
        get
        {
            return HexMetrics.colors[ColorIndex];
        }
        set
        {
            if (color == value)
            {
                return;
            }
            color = value;
            Refresh();
        }
    }
    public Color color;
    
    // Индекс цвета для сохраенения для сохранения
    public int ColorIndex
    {
        get
        {
            return colorIndex;
        }
        set
        {
            if (colorIndex != value)
            {
                colorIndex = value;
                Refresh();
            }
        }
    }
    // Для сохранения(сохраняем не цвет, а его индекс)    
    int colorIndex;

    /// <summary>
    /// Уровень высоты
    /// </summary>
    int elevation = int.MinValue;
    /// <summary>
    /// Ссылка на сегмент
    /// </summary>
    public HexGridChunk chunk;
    /// <summary>
    /// Массив соседей
    /// </summary>
    [SerializeField]
    HexCell[] neighbors;

    public RectTransform uiRect;

    void Refresh()
    {
        if (chunk) {
			chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }
    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            if (elevation == value)
            {
                return;
            }
            elevation = value;
            RefreshPosition();
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            position.y +=
                (HexMetrics.SampleNoise(position).y * 2f - 1f) *
                HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = -position.y;
            uiRect.localPosition = uiPosition;
            Refresh();
        }
    }
    /// <summary>
    /// Свойство для получения позиции (убирает разрывы при действии шума)
    /// </summary>
    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }
    /// <summary>
    /// Получение соседа ячейки в одном направлении
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }
    /// <summary>
    /// Метод установки соседа
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="cell"></param>
    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }
    /// <summary>
    /// Получение типа склона по направлению
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(
            elevation, neighbors[(int)direction].elevation
        );
    }
    /// <summary>
    /// Определение наклона между двумя любымми ячейками
    /// </summary>
    /// <param name="otherCell"></param>
    /// <returns></returns>
    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(
            elevation, otherCell.elevation
        );
    }
    public void Save(SaveMapData map)
    {
        map.cells.Add(new Cell(colorIndex, elevation, coordinates.X, coordinates.Y, coordinates.Z));
    }

    public void Load(Cell cell)
    {
        colorIndex = cell.color;
        elevation = cell.elevation;
        RefreshPosition();
    }
    /// <summary>
    /// Обновление высоты после загрузыки
    /// </summary>
    void RefreshPosition()
    {
        Vector3 position = transform.localPosition;
        position.y = elevation * HexMetrics.elevationStep;
        position.y +=
            (HexMetrics.SampleNoise(position).y * 2f - 1f) *
            HexMetrics.elevationPerturbStrength;
        transform.localPosition = position;

        Vector3 uiPosition = uiRect.localPosition;
        uiPosition.z = -position.y;
        uiRect.localPosition = uiPosition;
    }
}
