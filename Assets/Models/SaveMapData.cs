using System;
using System.Collections.Generic;

[Serializable]
public class SaveMapData
{
    public SaveMapData(List<SaveMapModel> saveMapModels)
    {
        this.saveMapModels = saveMapModels;
    }
    public List<SaveMapModel> saveMapModels;
}
