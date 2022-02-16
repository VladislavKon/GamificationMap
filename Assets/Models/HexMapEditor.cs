using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using System;
using System.Runtime.InteropServices;

public class HexMapEditor : MonoBehaviour
{
	public Color[] colors;
	public static int colorIndex;

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

	bool applyElevation = true;

	// Размер кисти
	int brushSize;

	//void Awake()
	//{
	//	SelectColor(-1);
	//}
	void Update()
	{
		if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
		{
			HandleInput();
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
			EditCells(hexGrid.GetCell(hit.point));
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

	void EditCell(HexCell cell)
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
		}
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
	public void SetApplyElevation (bool toggle)
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
	private static extern void SaveGame(string mapData);

	public void Save()
	{		
		Debug.Log(Application.persistentDataPath);		
		var mapData = new SaveMapData(new List<Cell>());		
		hexGrid.Save(mapData);
		string jsonMap = JsonUtility.ToJson(mapData);
#if UNITY_WEBGL == true && UNITY_EDITOR == false
				SaveGame (jsonMap);
#endif
	}
	public void Load()
	{
		//string path = Path.Combine(Application.persistentDataPath, "test.map");
		//SaveMapData data = JsonUtility.FromJson<SaveMapData>(savestring);
		//using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
		//{
		//	int header = reader.ReadInt32();
		//	if (header == 0)
		//	{
		//		hexGrid.Load(reader);
		//	}
		//	else
		//	{
		//		Debug.LogWarning("Unknown map format " + header);
		//	}
		//}
	}
}