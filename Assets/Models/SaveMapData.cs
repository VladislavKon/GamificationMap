using System;
using System.Collections.Generic;

[Serializable]
public class SaveMapData
{
    public SaveMapData(List<Cell> cells)
    {
        this.cells = cells;
    }
    public List<Cell> cells;
}
