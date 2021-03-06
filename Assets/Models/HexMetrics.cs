using Assets.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
	/// <summary>
	/// Размер сегмента
	/// </summary>
	public const int chunkSizeX = 5, chunkSizeZ = 5;
	/// <summary>
	/// Внешний радиус гексогона
	/// </summary>
	public const float outerRadius = 10f;
	/// <summary>
	/// Внутренний радиус гексогона
	/// </summary>
    public const float innerRadius = outerRadius * 0.866025404f;
	/// <summary>
	/// Размер внутреннего шестиугольника 80%
	/// </summary>
	public const float solidFactor = 0.8f;
	public const float blendFactor = 1f - solidFactor;
	/// <summary>
	/// Шаг изменения высоты
	/// </summary>
	public const float elevationStep = 3f;
	/// <summary>
	/// Колличество ступеней
	/// </summary>
	public const int terracesPerSlope = 2;	
	public const int terraceSteps = terracesPerSlope * 2 + 1;
	public const float horizontalTerraceStepSize = 1f / terraceSteps;
	public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);

	/// <summary>
	/// Текстура шума для неровностей
	/// </summary>
	public static Texture2D noiseSource;

	/// <summary>
	/// Сила шума
	/// </summary>
	public const float cellPerturbStrength = 4f;

	/// <summary>
	/// Масштабирование шума (уменьшаем искажения)
	/// </summary>
	public const float noiseScale = 0.003f;

	/// <summary>
	/// Сила шума для одной отдельной ячейки( ячейки будут разной высоты)
	/// </summary>
	public const float elevationPerturbStrength = 1.5f;

	/// <summary>
	/// Цвета для сохранения карты
	/// </summary>
	public static Color[] colors;

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

	/// <summary>
	/// Интерполяция ступеней
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="step"></param>
	/// <returns></returns>
	public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
	{
		float h = step * HexMetrics.horizontalTerraceStepSize;
		a.x += (b.x - a.x) * h;
		a.z += (b.z - a.z) * h;
		float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
		a.y += (b.y - a.y) * v;
		return a;
	}

	public static Color TerraceLerp(Color a, Color b, int step)
	{
		float h = step * HexMetrics.horizontalTerraceStepSize;
		return Color.Lerp(a, b, h);
	}
	/// <summary>
	/// Получение типа склона
	/// </summary>
	/// <param name="elevation1"></param>
	/// <param name="elevation2"></param>
	/// <returns></returns>
	public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
	{
		if (elevation1 == elevation2)
		{
			return HexEdgeType.Flat;
		}
		int delta = elevation2 - elevation1;
		if (delta == 1 || delta == -1)
		{
			return HexEdgeType.Slope;
		}
		return HexEdgeType.Cliff;
	}

	/// <summary>
	/// Метод выборки шума
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	public static Vector4 SampleNoise(Vector3 position)
	{
		return noiseSource.GetPixelBilinear(
			position.x * noiseScale,
			position.z * noiseScale);
	}
}

