using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    /// <summary>
    /// Координаты ячейки
    /// </summary>
    public HexCoordinates coordinates;
    /// <summary>
    /// Цвет ячейки
    /// </summary>
    public Color color;
    /// <summary>
    /// Уровень высоты
    /// </summary>
    private int elevation;
    /// <summary>
    /// Массив соседей
    /// </summary>
    [SerializeField]
    HexCell[] neighbors;

    public RectTransform uiRect;
    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -HexMetrics.elevationStep;
            uiRect.localPosition = uiPosition;
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
}
