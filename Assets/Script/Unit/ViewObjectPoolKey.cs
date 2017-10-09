using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewObjectPoolKey {

    public EObjectID ID;
    public string Key;
    public string PrefabName;
    public int MakeCount;

    public ViewObjectPoolKey(EObjectID id, string key, string prefabName, int makeCount = 10)
    {
        ID = id;
        Key = key;
        PrefabName = prefabName;
        MakeCount = makeCount;
    }
	
}
