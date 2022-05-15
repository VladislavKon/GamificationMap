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
		ShowUI(true);
	}

	//void Start()
	//{
	//	hexMesh.Triangulate(cells);
	//}

	public void Refresh()
	{
		enabled = true;
	}
	
	void LateUpdate()
	{
		hexMesh.Triangulate(cells);
		enabled = false;
	}

	/// <summary>
	/// Добавление ячеек в массив
	/// </summary>
	/// <param name="index"></param>
	/// <param name="cell"></param>
	public void AddCell(int index, HexCell cell)
	{
		cells[index] = cell;
		cell.chunk = this;
		cell.transform.SetParent(transform, false);
		cell.uiRect.SetParent(gridCanvas.transform, false);
	}

	/// <summary>
	/// Метод показа координат(меток) ячеек
	/// </summary>
	/// <param name="visible"></param>
	public void ShowUI(bool visible)
	{
		gridCanvas.gameObject.SetActive(visible);
	}
}
