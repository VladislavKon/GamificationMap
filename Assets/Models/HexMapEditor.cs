using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
	public Color[] colors;

	// включен ли цветовой редактор
	bool applyColor;

	public HexGrid hexGrid;

	/// <summary>
	/// Активный цвет (выбранный на интерфейсе)
	/// </summary>
	private Color activeColor;

	int activeElevation;

	bool applyElevation = true;

	// Размер кисти
	int brushSize;

	void Awake()
	{
		SelectColor(-1);
	}
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
				cell.Color = activeColor;
			}
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
			activeColor = colors[index];
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
	/// Метод показа координат(меток) ячеек
	/// </summary>
	/// <param name="visible"></param>
	public void ShowUI(bool visible)
	{
		hexGrid.ShowUI(visible);
	}
}