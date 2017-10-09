using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

    #region Singleton
    private const string POOL_NAME = "ObjectPool";
    private static ObjectPool g_Instance;
    public static ObjectPool Instance()
    {
        GameObject obj;
        if(g_Instance == null)
        {
            obj = GameObject.Find(POOL_NAME);
            if(obj == null)
            {
                obj = new GameObject(POOL_NAME);
            }
            g_Instance = obj.GetComponent<ObjectPool>();
            if(g_Instance == null)
            {
                g_Instance = obj.AddComponent<ObjectPool>();
            }
        }

        return g_Instance;
    }
    #endregion

    private Dictionary<string, ObjectPoolItem> m_dictObjectPool;

    void Awake()
    {
        if(g_Instance == null)
        {
            ObjectPool pInstance = ObjectPool.Instance();
            if(pInstance != this)
            {
                Debug.LogError("THIS INSTANCE IS ABNORMAL ObjectPool INSTANCE");
                GameObject.DestroyImmediate(gameObject);
                return;
            }
        }

        m_dictObjectPool = new Dictionary<string, ObjectPoolItem>();
    }

    public void MakeObject(string strKey, GameObject pPrefab, int nAddItemCount = ObjectPoolItem.DEFAULT_CREATE_ITEM_COUNT)
    {
        if (m_dictObjectPool.ContainsKey(strKey)) // 동일한 키가 있으면 예외처리
        {
            Debug.Log("Duplicate ObjectPool Key [ " + strKey + " ] ");
            return;
        }

        ObjectPoolItem pPoolItem = new ObjectPoolItem(strKey, pPrefab);
        pPoolItem.AddItem(nAddItemCount);

        m_dictObjectPool.Add(strKey, pPoolItem);
    }

    public ViewObject GetObject(string strKey)
    {
        if(!m_dictObjectPool.ContainsKey(strKey))
        {
            Debug.Log("Not Contains ObjectPool Key [ " + strKey + " ] ");
            return null;
        }

        ObjectPoolItem pPoolItem = m_dictObjectPool[strKey];

        ViewObject pViewObject = pPoolItem.GetItem();

        pViewObject.DoAwake();

        return pViewObject;
    }

    public void ReturnObject(ViewObject pViewObject)
    {
        if(!m_dictObjectPool.ContainsKey(pViewObject.ObjectPoolKey))
        {
            Debug.Log("NOT Pool Object key [ " + pViewObject.ObjectPoolKey + " ] ");
            return;
        }

        ObjectPoolItem pPoolItem = m_dictObjectPool[pViewObject.ObjectPoolKey];

        pViewObject.DoDestroy();

        pPoolItem.ReturnItem(pViewObject);
    }

    public void ReturnAllObject()
    {
        foreach(KeyValuePair<string, ObjectPoolItem> pair in m_dictObjectPool)
        {
            pair.Value.ReturnAllItem();
        }
    }
}

public class ObjectPoolItem
{
    public const int DEFAULT_CREATE_ITEM_COUNT = 10; // object 생성시 객체의 기본 생성 갯수

    private string m_strKey; // key name

    private GameObject m_pPrefab; // obejct prefab
    private int m_nItemCount; // item count

    private Stack<ViewObject> m_stackUnusedItem; // 사용안된 아이템
    private List<ViewObject> m_listUsedItem; // 사용된 아이템

    public ObjectPoolItem(string strKey, GameObject pPrefab) // 생성자
    {
        m_stackUnusedItem = new Stack<ViewObject>();
        m_listUsedItem = new List<ViewObject>();

        m_strKey = strKey;
        m_pPrefab = pPrefab;
    }

    public ViewObject CreateItem() // Object Create
    {
        GameObject pObj = GameObject.Instantiate(m_pPrefab);
        ++m_nItemCount;
        ViewObject pViewObject = pObj.GetComponent<ViewObject>();
        pViewObject.SetObjectPoolKey(m_strKey);

        pObj.transform.parent = ObjectPool.Instance().transform;
        pObj.SetActive(false);

        return pViewObject;
    }

    public void AddItem(int nAddItemCount = ObjectPoolItem.DEFAULT_CREATE_ITEM_COUNT) // nAddItemCount 만큼 미리 만들어 놓기
    {
        for (int i = 0; i < nAddItemCount; i++)
        {
            m_stackUnusedItem.Push(CreateItem());
        }
    }

    public ViewObject GetItem() // Object 사용
    {
        if(m_stackUnusedItem.Count.Equals(0))
        {
            AddItem(m_nItemCount.Equals(0) ? ObjectPoolItem.DEFAULT_CREATE_ITEM_COUNT : m_nItemCount);
        }

        ViewObject pViewObject = m_stackUnusedItem.Pop();
        m_listUsedItem.Add(pViewObject);

        pViewObject.gameObject.SetActive(true);

        return pViewObject;
    }

    public void ReturnItem(ViewObject pViewObject) // 사용 끝난 obejct
    {
        m_listUsedItem.Remove(pViewObject);
        m_stackUnusedItem.Push(pViewObject);

        pViewObject.transform.parent = ObjectPool.Instance().transform;
        pViewObject.gameObject.SetActive(false);
    }

    public void ReturnAllItem()
    {
        ViewObject[] pObjectArr = m_listUsedItem.ToArray();

        for(int i = 0; i < pObjectArr.Length; i++)
        {
            ObjectPool.Instance().ReturnObject(pObjectArr[i]);
        }
    }
}
