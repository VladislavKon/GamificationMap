using System;

[Serializable]
public class SaveMapModel
{
    public SaveMapModel(int color, int elevation)
    {
        this.color = color;
        this.elevation = elevation;
    }
    public int color;
    public int elevation;
}

