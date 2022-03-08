using System;

[Serializable]
public class Cell
{
    public Cell(int color, int elevation, int x, int y, int z)
    {
        Color = color;
        Elevation = elevation;
        X = x;
        Y = y;
        Z = z;
    }
    public int Color;
    public int Elevation;
    public int X;
    public int Y;
    public int Z;
}

