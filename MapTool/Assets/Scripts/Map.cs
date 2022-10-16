using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 설치 했을 때의 정보
[Serializable]
public class CreatedInfo
{
    // 만들어진 게임오브젝트
    public GameObject go;
    // 선택된 오브젝트의 Idx
    public int idx;
}


[Serializable]
public class SaveJsonInfo
{
    public int idx;
    public Vector3 position;
    public Vector3 eulerAngle;
    public Vector3 localScale;
}
[Serializable]
public class ArrayJson
{
    public List<SaveJsonInfo> datas;
}


public class Map : MonoBehaviour
{
    // 맵의 가로 크기
    public int tileX;
    [HideInInspector]
    // 맵의 세로 크기
    public int tileZ;

    // 바닥 Prefab
    public GameObject floor;

    // 파란색 큐브 Prefab
    public GameObject blueCube;

    // 생성하고 싶은 게임오브젝트들 담을 변수
    public GameObject[] objs;

    // 선택한 오브젝트 Index
    public int selectObjIdx;

    // 만들어진 오브젝트들 담아놓을 리스트
    public List<CreatedInfo> createdObjects = new List<CreatedInfo>();
}
