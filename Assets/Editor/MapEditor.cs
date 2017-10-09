using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Editor Script는 Editor폴더 안에 있어야한다.
// 에디터 스크립트는 어떤 클래스 혹은 스크립트를 다루는지 명시해야한다.
[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor {

    public override void OnInspectorGUI()
    {
        // CustomEditor 키워드로 이 에디터 스크립트가 다룰것이라 선언한 오브젝트는 target으로 접근할 수 있게 자동으로 설정된다.
        MapGenerator map = target as MapGenerator;

        //base.OnInspectorGUI(); // 성능 잡아먹음 계속 돌아서

        if (DrawDefaultInspector())
            map.GenerateMap();

        //수동 변경
        if(GUILayout.Button("Generate Map"))
            map.GenerateMap();

    }

}
