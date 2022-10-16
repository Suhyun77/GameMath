using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��ġ ���� ���� ����
[Serializable]
public class CreatedInfo
{
    // ������� ���ӿ�����Ʈ
    public GameObject go;
    // ���õ� ������Ʈ�� Idx
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
    // ���� ���� ũ��
    public int tileX;
    [HideInInspector]
    // ���� ���� ũ��
    public int tileZ;

    // �ٴ� Prefab
    public GameObject floor;

    // �Ķ��� ť�� Prefab
    public GameObject blueCube;

    // �����ϰ� ���� ���ӿ�����Ʈ�� ���� ����
    public GameObject[] objs;

    // ������ ������Ʈ Index
    public int selectObjIdx;

    // ������� ������Ʈ�� ��Ƴ��� ����Ʈ
    public List<CreatedInfo> createdObjects = new List<CreatedInfo>();
}
