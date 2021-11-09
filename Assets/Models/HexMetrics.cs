using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
	/// <summary>
	/// Внешний радиус гексогона
	/// </summary>
    public const float outerRadius = 10f;
	/// <summary>
	/// Внутренний радиус гексогона
	/// </summary>
    public const float innerRadius = outerRadius * 0.866025404f;
	/// <summary>
	/// Область смешивания цветов 75%
	/// </summary>
	public const float solidFactor = 0.75f;

	public const float blendFactor = 1f - solidFactor;
	/// <summary>
	/// Шаг изменения высоты
	/// </summary>
	public const float elevationStep = 1f;

	/// <summary>
	/// Векторы гексогона
	/// </summary>
	static Vector3[] corners = {
		new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(0f, 0f, outerRadius)
	};
	public static Vector3 GetFirstCorner(HexDirection direction)
	{
		return corners[(int)direction];
	}

	public static Vector3 GetSecondCorner(HexDirection direction)
	{
		return corners[(int)direction + 1];
	}

	public static Vector3 GetFirstSolidCorner(HexDirection direction)
	{
		return corners[(int)direction] * solidFactor;
	}

	public static Vector3 GetSecondSolidCorner(HexDirection direction)
	{
		return corners[(int)direction + 1] * solidFactor;
	}

	/// <summary>
	/// Отсечение цвета по краям
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public static Vector3 GetBridge(HexDirection direction)
	{
		return (corners[(int)direction] + corners[(int)direction + 1]) *
			blendFactor;
	}
}

