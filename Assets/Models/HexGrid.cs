using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    int cellCountX;
    int cellCountZ;
	/// <summary>
	/// Количество сегментов на карте
	/// </summary>
	public int chunkCountX = 4, chunkCountZ = 3;
	/// <summary>
	/// Цвет по умолчанию
	/// </summary>
	public Color defaultColor = Color.white;
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

	//private void Start()
 //   {
	//    hexMesh.Triangulate(cells);
 //   }

    /// <summary>
    /// Инициализация сетки
    /// </summary>
    void Awake()
	{
		HexMetrics.noiseSource = noiseSource;
		cells = new HexCell[cellCountZ * chunkCountX];

		// gridCanvas = GetComponentInChildren<Canvas>();
		// hexMesh = GetComponentInChildren<HexMesh>();

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
	void CreateChunks()
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
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f); ;
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);			

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		// cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.Color = defaultColor;		

		//Паттерн присваивания соседей
		if (x > 0)
		{
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
		}
		if (z > 0)
		{
			if ((z & 1) == 0)
			{
				cell.SetNeighbor(HexDirection.SE, cells[i - chunkCountX]);
				if (x > 0)
				{
					cell.SetNeighbor(HexDirection.SW, cells[i - chunkCountX - 1]);
				}
			}
			else
			{
				cell.SetNeighbor(HexDirection.SW, cells[i - chunkCountX]);
				if (x < chunkCountX - 1)
				{
					cell.SetNeighbor(HexDirection.SE, cells[i - chunkCountX + 1]);
				}
			}
		}

		Text label = Instantiate<Text>(cellLabelPrefab);
		// label.rectTransform.SetParent(gridCanvas.transform, false);
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
		int index = coordinates.X + coordinates.Z * chunkCountX + coordinates.Z / 2;
		return cells[index];		
	}

}
