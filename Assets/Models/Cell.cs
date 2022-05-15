using System;

[Serializable]
public class Cell
{
    public Cell(int color, int elevation, int x, int y, int z)
    {
        this.color = color;
        this.elevation = elevation;
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public int color;
    public int elevation;
    public int x;
    public int y;
    public int z;
    public Guid? owner;
}

