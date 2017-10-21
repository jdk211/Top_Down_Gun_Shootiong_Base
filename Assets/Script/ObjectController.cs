using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour {

    #region SingleTon
    private const string CONTROLLER_NAME = "ObjectController";
    private static ObjectController g_Instance;
    public static ObjectController Instance()
    {
        GameObject obj;
        if(g_Instance == null)
        {
            obj = GameObject.Find(CONTROLLER_NAME);
            if (obj == null)
            {
                obj = new GameObject(CONTROLLER_NAME);
            }
            g_Instance = obj.GetComponent<ObjectController>();
            if(g_Instance == null)
            {
                g_Instance = obj.AddComponent<ObjectController>();
            }
        }

        return g_Instance;
    }

    private void CheckSingleton()
    {
        if(g_Instance == null)
        {
            ObjectController pInstance = ObjectController.Instance();
            if(pInstance != this)
            {
                Debug.LogError("THIS INSTANCE IS ABNORMAL ObjectController INSTANCE");
                GameObject.DestroyImmediate(gameObject);
                return;
            }
        }
    }
    #endregion

    private Dictionary<EObjectID, ViewObjectPoolKey> m_dictObjectPoolKey;

    private void Awake()
    {
        CheckSingleton();

        InitObjectPoolKey();
        MakeViewObjectPool();
    }

    private void InitObjectPoolKey()
    {
        ViewObjectPoolKey pObjectPoolKey;
        pObjectPoolKey = new ViewObjectPoolKey(EObjectID.Enemy, "Enemy", "Enemy", 10);

        m_dictObjectPoolKey = new Dictionary<EObjectID, ViewObjectPoolKey>();
        m_dictObjectPoolKey.Add(pObjectPoolKey.ID, pObjectPoolKey);
    }

    public void MakeViewObjectPool()
    {
        EObjectID[] objectIdArray = { EObjectID.Enemy };
        ViewObjectPoolKey pObjectPoolKey;

        const string PREFIX = "Prefabs/";

        for(int i = 0; i < objectIdArray.Length; i++)
        {
            EObjectID id = objectIdArray[i];
            pObjectPoolKey = m_dictObjectPoolKey[id];
            GameObject pPrefab = Resources.Load(PREFIX + pObjectPoolKey.PrefabName) as GameObject;
            ObjectPool.Instance().MakeObject(pObjectPoolKey.Key, pPrefab, pObjectPoolKey.MakeCount);
        }
    }
}
