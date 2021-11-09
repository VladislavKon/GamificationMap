using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Направление соседей в направлениях компоса
/// </summary>
public enum HexDirection
{
	NE, E, SE, SW, W, NW
}
/// <summary>
/// 
/// </summary>
public static class HexDirectionExtensions
{
	/// <summary>
	/// Получить ротивоположное направление
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public static HexDirection Opposite(this HexDirection direction)
	{
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}
	/// <summary>
	/// Перейти к предыдущемму направлению
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public static HexDirection Previous(this HexDirection direction)
	{
		return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
	}
	/// <summary>
	/// Перейти к следующему направлению
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public static HexDirection Next(this HexDirection direction)
	{
		return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
	}

}
