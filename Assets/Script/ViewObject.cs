using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewObject : MonoBehaviour {

    public string ObjectPoolKey;
    public void SetObjectPoolKey(string strKey)
    {
        ObjectPoolKey = strKey;
    }

    public virtual void DoAwake() { }
    public virtual void DoDestroy() { }
}
