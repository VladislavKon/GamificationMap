using System;
using System.Collections.Generic;

[Serializable]
public class SaveMapData
{
    public SaveMapData(List<Cell> cells)
    {
        this.Cells = cells;
    }
    public List<Cell> Cells;
}
