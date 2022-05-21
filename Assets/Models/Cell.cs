using System;

[Serializable]
public class Cell
{
    public Cell(int color, int elevation, int x, int y, int z, string ownerId = null)
    {
        this.color = color;
        this.elevation = elevation;
        this.x = x;
        this.y = y;
        this.z = z;
        this.ownerId = ownerId;
    }
    public int color;
    public int elevation;
    public int x;
    public int y;
    public int z;
    public string ownerId;
}

