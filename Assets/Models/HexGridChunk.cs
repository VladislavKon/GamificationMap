using UnityEngine;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour
{
	HexCell[] cells;

	HexMesh hexMesh;
	Canvas gridCanvas;

	void Awake()
	{
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

		cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
	}

	void Start()
	{
		hexMesh.Triangulate(cells);
	}

	/// <summary>
	/// Добавление ячеек в массив
	/// </summary>
	/// <param name="index"></param>
	/// <param name="cell"></param>
	public void AddCell(int index, HexCell cell)
	{
		cells[index] = cell;
		cell.transform.SetParent(transform, false);
		cell.uiRect.SetParent(gridCanvas.transform, false);
	}
}
