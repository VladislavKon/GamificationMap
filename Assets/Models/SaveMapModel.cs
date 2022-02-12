using System;

[Serializable]
public class SaveMapModel
{
    public SaveMapModel(int color, int elevation, HexCoordinates coordinates)
    {
        this.color = color;
        this.elevation = elevation;
        this.coordinates = coordinates;
    }
    public int color;
    public int elevation;
    public HexCoordinates coordinates;
}

