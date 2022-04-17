using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour, IPointerClickHandler
{
	public Color[] colors;
	public static int colorIndex;

	private static HexCell SelectedCell;

	// включен ли цветовой редактор
	bool applyColor;

	public HexGrid hexGrid;

	/// <summary>
	/// Активный цвет (выбранный на интерфейсе)
	/// </summary>
	private Color activeColor;

	/// <summary>
	/// Активный тип местности (для сохранения)
	/// </summary>
	//int activeTerrainTypeIndex;

	int activeElevation;

	public bool applyElevation;

	// Размер кисти
	int brushSize;
	ILogger logger;
	public HexMapEditor(ILogger logger)
	{
		this.logger = logger;
	}

	//void Awake()
	//{
	//	SelectColor(-1);
	//}
	void Update()
	{		
		HandleInput();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(inputRay, out hit))
			{
				var cell = hexGrid.GetCell(hit.point);
				EditCells(cell);
			}
		}
	}
	
	/// <summary>
	/// Обработчик клика мыши по гриду
	/// </summary>
	void HandleInput()
	{
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit))
		{
			var cell = hexGrid.GetCell(hit.point);
            if (SelectedCell != cell)
            {
				SelectedCell?.DisableHighlight();
				SelectedCell = cell;
			}
			cell.EnableHighlight(Color.green);
			if (Input.GetMouseButtonDown((int)PointerEventData.InputButton.Left) && !EventSystem.current.IsPointerOverGameObject())
			{
				EditCells(cell);
			}
		}
	}
	/// <summary>
	///  Метод редактирования нескольких ячеек (для размера кисти кисти)
	/// </summary>
	/// <param name="center"></param>
	void EditCells(HexCell center)
	{
		int centerX = center.coordinates.X;
		int centerZ = center.coordinates.Z;

		for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
		{
			for (int x = centerX - r; x <= centerX + brushSize; x++)
			{
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
		for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
		{
			for (int x = centerX - brushSize; x <= centerX + r; x++)
			{
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
	}

	public void EditCell(HexCell cell)
	{
		if (cell)
		{
			if (applyColor)
			{
				cell.ColorIndex = Array.IndexOf(hexGrid.colors, activeColor);
				cell.Color = activeColor;
			}
			//         if (activeTerrainTypeIndex >= 0)
			//{
			//	cell.ColorIndex = activeTerrainTypeIndex;
			//}
			if (applyElevation)
			{
				cell.Elevation = activeElevation;
			}
			Cell updatedCell = new Cell(cell.ColorIndex, cell.Elevation, cell.coordinates.X, cell.coordinates.Y, cell.coordinates.Z);
			UpdateTargetCell(updatedCell);
		}
	}
	/// <summary>
	/// Метод для захвата ячейки
	/// </summary>
	/// <param name="cell"></param>
	public void CaptureCell(Cell cell)
    {
		var cellCoordinates = new HexCoordinates(cell.x, cell.z);
		var updatedCell = hexGrid.GetCell(cellCoordinates);
		updatedCell.ColorIndex = cell.color;
    }

	public void SelectColor(int index)
	{
		applyColor = index >= 0;
		if (applyColor)
		{
			activeColor = hexGrid.colors[index];
		}
	}

	public void SetElevation(float elevation)
	{
		activeElevation = (int)elevation;
	}

	/// <summary>
	/// Метод отключения/включения ползунка высоты
	/// </summary>
	/// <param name="toggle"></param>
	public void SetApplyElevation(bool toggle)
	{
		applyElevation = toggle;
	}

	/// <summary>
	/// Метод отвчающий за размер кисти
	/// </summary>
	/// <param name="size"></param>
	public void SetBrushSize(float size)
	{
		brushSize = (int)size;
	}

	/// <summary>
	/// Метод управления индексом активного типа местности
	/// </summary>
	/// <param name="index"></param>
	//public void SetTerrainTypeIndex(int index)
	//{
	//	activeTerrainTypeIndex = index;
	//}

	/// <summary>
	/// Метод показа координат(меток) ячеек
	/// </summary>
	/// <param name="visible"></param>
	public void ShowUI(bool visible)
	{
		hexGrid.ShowUI(visible);
	}

	[DllImport("__Internal")]
	private static extern void SaveMap(string mapData);
	[DllImport("__Internal")]
	private static extern void LoadMap();
	[DllImport("__Internal")]
	private static extern void UpdateCell(string cell);

	public void Save()
	{
		var mapData = new SaveMapData(new List<Cell>());
		hexGrid.Save(mapData);
		string jsonMap = JsonUtility.ToJson(mapData);
#if UNITY_WEBGL == true && UNITY_EDITOR == false
				SaveMap(jsonMap);
#endif
	}

	public void UpdateTargetCell(Cell cell)
	{
		string jsonCell = JsonUtility.ToJson(cell);
#if UNITY_WEBGL == true && UNITY_EDITOR == false
				UpdateCell(jsonCell);
#endif
	}
	/// <summary>
	/// Метод загрузки карты (Unity->React)
	/// </summary>
	public void Load()
	{
#if UNITY_WEBGL == true && UNITY_EDITOR == false
		LoadMap();
#endif
	}
	/// <summary>
	/// Метод загрузки карты (React->Unity)
	/// </summary>
	/// <param name="mapJson"></param>
	public void SetMapData(string mapJson)
	{
		SaveMapData map = JsonUtility.FromJson<SaveMapData>(mapJson);
		hexGrid.Load(map);
	}
	/// <summary>
	/// Метод захвата ячейки (React(SignalR) -> Unity)
	/// </summary>
	/// <param name="cellJson"></param>
	public void GrabCell(string cellJson)
    {
		Cell cell = JsonUtility.FromJson<Cell>(cellJson);
		CaptureCell(cell);

	}    
}