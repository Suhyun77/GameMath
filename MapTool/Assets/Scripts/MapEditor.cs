using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Ÿ�� ���� : ������ Ŀ���� �Ұ��̳�?
[CustomEditor(typeof(Map))]


public class MapEditor : Editor // Editor ���
{
    Map map;
    // map.objs ���� �̸��� ���� ����
    string[] objsName;

    // ���� ���� �̸�
    string saveFileName;

    //  Hierarchy���� Map�� Ŭ�� �Ǿ��� �� ȣ�� �Ǵ� �Լ�
    private void OnEnable()
    {
        map = (Map)target;

        // ������Ʈ�� �̸� ����
        objsName = new string[map.objs.Length];
        for (int i=0; i < map.objs.Length; i++)
        {
            objsName[i] = map.objs[i].name;
        }
    }

    // Inspector�� �׸��� �Լ�
    public override void OnInspectorGUI()
    {
        // �θ��� ���븻�� ���� ���� ����� ( �ּ� ó���ϸ� Map �ʵ��� public ���� �Ⱥ���)
        //base.OnInspectorGUI();

        map.tileX = EditorGUILayout.IntField("Ÿ�� ����", map.tileX);
        map.tileZ = EditorGUILayout.IntField("Ÿ�� ����", map.tileZ);

        // �ּ��ִ밪 ���ϱ�
        map.tileX = Mathf.Clamp(map.tileX, 1, 500);
        map.tileZ = Mathf.Clamp(map.tileZ, 1, 500);

        // �ٴ� Prefab Field
        // allowSceneObjects = false : Projectâ�� �ִ� ������Ʈ�� ��� (Scene�� �̹� �ִ� ������Ʈ ����)
        map.floor = (GameObject)EditorGUILayout.ObjectField("�ٴ�", map.floor, typeof(GameObject), false);

        // �Ķ��� ť�� Prefab Field
        map.blueCube = (GameObject)EditorGUILayout.ObjectField("�Ķ��� ť��", map.blueCube, typeof(GameObject), false);

        // map.objs Field
        EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("objs"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("createdObjects"));
        if (check.changed)
        {
            // �ν����� �󿡼� ������ �ϰڴ�.
            serializedObject.ApplyModifiedProperties();
        }

        // ������ ������Ʈ Idx Field
        map.selectObjIdx = EditorGUILayout.Popup("���� ������Ʈ", map.selectObjIdx, objsName);

        // Inspector ���� �߰�
        EditorGUILayout.Space();

        // �ٴ� ���� ��ư
        if (GUILayout.Button("�ٴ� ����"))
        {
            CreateFloor();
        }

        saveFileName = EditorGUILayout.TextField("���������̸�", saveFileName);
        // Json ���� ��ư
        if (GUILayout.Button("Json ����"))
        {
            SaveJson();
        }

        // Json �ҷ����� ��ư
        if (GUILayout.Button("Json �ҷ�����"))
        {
            LoadJson();
        }

        // ���� Inspector�� ���� ����Ǿ��ٸ�
        if (GUI.changed)
        {
            //��ǥ ��� ǥ��(�� �̵���, ����Ƽ�� �� �� ���� �˾��� �߰�)
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

    }

    void LoadJson()
    {
        // ���࿡ saveFileName�� ���̰� 0�̶�� �Լ� ������
        if (saveFileName.Length <= 0)
        {
            // �����̸��� �Է��ϼ���.
            Debug.LogError("���� �̸��� �Է��ϼ���.");
            return;
        }

        // ���� �� �����͸� ����
        CreateFloor();

        // mapData.txt �ҷ�����
        string jsonData = File.ReadAllText(Application.dataPath + "/" + saveFileName + ".txt");  // ������� ��� : Application.persistentDataPath ��� i,9k 
        // ArrayJson ���·� Json ��ȯ
        ArrayJson arrayJson = JsonUtility.FromJson<ArrayJson>(jsonData);
        // ArrayJson�� �����͸� ������ ������Ʈ ����
        for (int i=0; i<arrayJson.datas.Count; i++)
        {
            SaveJsonInfo info = arrayJson.datas[i];
            LoadObject(info.idx, info.position, info.eulerAngle, info.localScale);
        }
    }



    // Scene �׸��� �Լ�
    private void OnSceneGUI()
    {
        // Map�� ���õǾ��� ��, Scene���� �ٸ� ������Ʈ�� Ŭ���ص� ������ ���� �ʰ� �ϱ�
        int id = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(id);

        DrawGrid();

        CreateObject();

        DeleteObject();
    }

    void SaveJson()
    {
        // ���࿡ saveFileName�� ���̰� 0�̶�� �Լ� ������
        if (saveFileName.Length <= 0)
        {
            // �����̸��� �Է��ϼ���.
            Debug.LogError("���� �̸��� �Է��ϼ���.");
            return;
        }

        // map.createdObjects�� �ִ� ������ json���� ��ȯ
        // idx, position, eulerAngle, localScale 
        // map.createdObjects ������ŭ SaveJsonInfo ����� ����
        // ArrayJson �����.
        ArrayJson arrayJson = new ArrayJson();
        arrayJson.datas = new List<SaveJsonInfo>();

        SaveJsonInfo info;
        for (int i=0; i < map.createdObjects.Count; i++)
        {
            CreatedInfo createdInfo = map.createdObjects[i];

            info = new SaveJsonInfo();
            info.idx = createdInfo.idx;
            info.position = createdInfo.go.transform.position;
            info.eulerAngle = createdInfo.go.transform.eulerAngles;
            info.localScale = createdInfo.go.transform.localScale;
            // ArrayJson �ϳ� ���� datas�� �ϳ��� �߰�
            arrayJson.datas.Add(info);
        }
        // arrayJson�� Json���� ��ȯ
        string jsonData = JsonUtility.ToJson(arrayJson, true);
        Debug.Log("jsonData : " + jsonData);
        // jsonData�� ���Ϸ� ����
        File.WriteAllText(Application.dataPath + "/" + saveFileName + ".txt", jsonData);

    }

    void DeleteObject()
    {
        Event e = Event.current;
        // ���콺 ���� ��ư & ��Ʈ�� Ű�� ������ ������
        if (e.type == EventType.MouseDown && e.button == 0 && e.control)
        {
            // ���콺 �����Ϳ��� Ray�� �����
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            // ���� Ray�� �߻��ؼ� �ε��� ���� �ִٸ�
            if (Physics.Raycast(ray, out hit))
            {
                // �ε��� ����  Layer�� Object���
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Object"))
                { 
                    // map.createdObjects���� hit.transform.gameObject�� ã�Ƽ� ������
                    for (int i=0; i < map.createdObjects.Count; i++)
                    {
                        if (map.createdObjects[i].go == hit.transform.gameObject)
                        {
                            map.createdObjects.RemoveAt(i);
                            break;
                        }
                    }
                    // �ش� ������Ʈ�� �ı��ϰڴ�.
                    DestroyImmediate(hit.transform.gameObject);
                }
            }
        }
    }

    void DrawGrid()
    {
        //Handles.color = Color.red; 
        //Handles.DrawLine(Vector3.zero, Vector3.one * 5);
        //Handles.color = Color.blue;
        //Handles.DrawLine(Vector3.zero, -Vector3.one * 5);

        Vector3 start;
        Vector3 end;
        // ������
        Handles.color = Color.magenta;
        for (int i = 0; i <= map.tileX; i++)
        {
            start = new Vector3(i, 0, 0);
            end = new Vector3(i, 0, map.tileZ);
            Handles.DrawLine(start, end);
        }
        // ������
        Handles.color = Color.cyan;
        for (int i = 0; i <= map.tileZ; i++)
        {
            start = new Vector3(0, 0, i);
            end = new Vector3(map.tileX, 0, i);
            Handles.DrawLine(start, end);
        }
    }
    
    void CreateFloor()
    {
        GameObject floor = GameObject.Find("Floor");
        // ���࿡ ���� �ٴ��� �־��ٸ� ������
        if (floor != null)
        {
            DestroyImmediate(floor);
        }

        GameObject empty = GameObject.Find("obj_parent");
        if (empty != null)
        {
            DestroyImmediate(empty);
        }

        // �� ������Ʈ (��ġ�� ������Ʈ ��� ������Ʈ���� �θ�)
        empty = new GameObject();
        empty.name = "obj_parent";

        // �⺻ �ٴ� ����
        floor = (GameObject) PrefabUtility.InstantiatePrefab(map.floor);
        // tileX, tileY��ŭ ũ�⸦ Ű���.
        floor.transform.localScale = new Vector3(map.tileX, 1, map.tileZ);
        // ������� ������Ʈ ����Ʈ �����
        map.createdObjects.Clear();
    }

    void LoadObject(int idx, Vector3 position, Vector3 eulerAngle, Vector3 localScale)
    {
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(map.objs[idx]);
        obj.transform.position = position;
        obj.transform.eulerAngles = eulerAngle;
        obj.transform.localScale = localScale;

        // �θ� obj_parent�� �ϱ�
        // �θ� obj_parent�� �ϱ�
        obj.transform.parent = GameObject.Find("obj_parent").transform;

        // ������� ������Ʈ�� ����Ʈ�� �߰�
        CreatedInfo info = new CreatedInfo();
        info.go = obj;
        info.idx = idx;
        map.createdObjects.Add(info);
    }

    // Ŭ���� ���� ������Ʈ ����
    void CreateObject()
    {
        // ���� Input �̺�Ʈ ����
        Event e = Event.current;

        // ���콺 �����ٸ� 
        if (e.type == EventType.MouseDown && e.button == 0 && !e.control)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    Vector3 p = new Vector3((int)hit.point.x, hit.point.y, (int)hit.point.z);
                    LoadObject(map.selectObjIdx, p, Vector3.zero, Vector3.one);
                }
                
            }
        }
    }
}
