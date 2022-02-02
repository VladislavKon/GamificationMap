using Assets.Models;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
	int cellCountX, cellCountZ;
	/// <summary>
	/// Количество сегментов на карте
	/// </summary>
	public int chunkCountX = 4, chunkCountZ = 3;
	/// <summary>
	/// Цвет по умолчани(убрано для сохранения карты)
	/// </summary>
	//public Color defaultColor = Color.white;
	/// <summary>
	/// Цвет затронутой ячейки
	/// </summary>
	public Color touchedColor = Color.magenta;

	/// <summary>
	/// Префаб клетки
	/// </summary>
	public HexCell cellPrefab;
	/// <summary>
	/// Префаб лэйбла ячейки (текст координат)
	/// </summary>
	public Text cellLabelPrefab;
	// HexMesh hexMesh;

	/// <summary>
	/// Префаб сегмента
	/// </summary>
	public HexGridChunk chunkPrefab;

	/// <summary>
	/// Канвас грида
	/// </summary>
	// Canvas gridCanvas;

	/// <summary>
	/// Массив клеток
	/// </summary>
	HexCell[] cells;

	/// <summary>
	/// Текстура шума
	/// </summary>
	public Texture2D noiseSource;

	/// <summary>
	/// Массив сегментов
	/// </summary>
	HexGridChunk[] chunks;

	/// <summary>
	/// Для сохранения
	/// </summary>
	public Color[] colors;

	//private void Start()
	//   {
	//    hexMesh.Triangulate(cells);
	//   }

	/// <summary>
	/// Инициализация сетки
	/// </summary>
	void Awake () 
	{
		HexMetrics.noiseSource = noiseSource;
		HexMetrics.colors = colors;

		cellCountX = chunkCountX * HexMetrics.chunkSizeX;
		cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

		CreateChunks();
		CreateCells();
	}

	void CreateCells()
	{
		cells = new HexCell[cellCountZ * cellCountX];

		for (int z = 0, i = 0; z < cellCountZ; z++)
		{
			for (int x = 0; x < cellCountX; x++)
			{
				CreateCell(x, z, i++);
			}
		}
	}
	void CreateChunks () 
	{
		chunks = new HexGridChunk[chunkCountX * chunkCountZ];

		for (int z = 0, i = 0; z < chunkCountZ; z++) 
		{
			for (int x = 0; x < chunkCountX; x++) 
			{
				HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
				chunk.transform.SetParent(transform);
			}
		}
	}
	void OnEnable()
	{
		HexMetrics.noiseSource = noiseSource;
		HexMetrics.colors = colors;
	}
	//public void Refresh()
	//{
	//	 hexMesh.Triangulate(cells);
	//}

	/// <summary>
	/// Создание клетки
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
	/// <param name="i"></param>
	void CreateCell(int x, int z, int i)
	{
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		//cell.Color = defaultColor;

		if (x > 0)
		{
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
		}
		if (z > 0)
		{
			if ((z & 1) == 0)
			{
				cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
				if (x > 0)
				{
					cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
				}
			}
			else
			{
				cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
				if (x < cellCountX - 1)
				{
					cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
				}
			}
		}

		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		label.text = cell.coordinates.ToStringOnSeparateLines();
		cell.uiRect = label.rectTransform;

		cell.Elevation = 0;

		AddCellToChunk(x, z, cell);
	}

	/// <summary>
	/// Добавление ячеек в ссегменты
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
	/// <param name="cell"></param>
	void AddCellToChunk(int x, int z, HexCell cell)
	{
		int chunkX = x / HexMetrics.chunkSizeX;
		int chunkZ = z / HexMetrics.chunkSizeZ;
		HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

		int localX = x - chunkX * HexMetrics.chunkSizeX;
		int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
		chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
	}

	public HexCell GetCell(Vector3 position)
	{
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
		return cells[index];
	}

	/// <summary>
	/// Получение ячейки по координатам (для кисти)
	/// </summary>
	/// <param name="coordinates"></param>
	/// <returns></returns>
	public HexCell GetCell(HexCoordinates coordinates)
	{
		int z = coordinates.Z;
		if (z < 0 || z >= cellCountZ)
		{
			return null;
		}
		int x = coordinates.X + z / 2;
		if (x < 0 || x >= cellCountX)
		{
			return null;
		}
		return cells[x + z * cellCountX];
	}

	/// <summary>
	/// Видимость меток ячеек
	/// </summary>
	/// <param name="visible"></param>
	public void ShowUI(bool visible)
	{
		for (int i = 0; i < chunks.Length; i++)
		{
			chunks[i].ShowUI(visible);
		}
	}
	/// <summary>
	/// Сохранение грида
	/// </summary>
	/// <param name="writer"></param>
	public void Save(SaveMapData map)
	{
		for (int i = 0; i < cells.Length; i++)
		{
			cells[i].Save(map);
		}
	}

	public void Load(List<SaveMapModel> map)
	{
		for (int i = 0; i < cells.Length; i++)
		{
			cells[i].Load(map);
		}
		for (int i = 0; i < chunks.Length; i++)
		{
			chunks[i].Refresh();
		}
	}

}
