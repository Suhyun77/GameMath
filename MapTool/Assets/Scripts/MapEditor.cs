using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// 타겟 지정 : 누구를 커스텀 할것이냐?
[CustomEditor(typeof(Map))]


public class MapEditor : Editor // Editor 상속
{
    Map map;
    // map.objs 들의 이름을 담을 변수
    string[] objsName;

    // 저장 파일 이름
    string saveFileName;

    //  Hierarchy에서 Map이 클릭 되었을 때 호출 되는 함수
    private void OnEnable()
    {
        map = (Map)target;

        // 오브젝트들 이름 셋팅
        objsName = new string[map.objs.Length];
        for (int i=0; i < map.objs.Length; i++)
        {
            objsName[i] = map.objs[i].name;
        }
    }

    // Inspector를 그리는 함수
    public override void OnInspectorGUI()
    {
        // 부모의 내용말고 내가 직접 만들기 ( 주석 처리하면 Map 필드의 public 변수 안보임)
        //base.OnInspectorGUI();

        map.tileX = EditorGUILayout.IntField("타일 가로", map.tileX);
        map.tileZ = EditorGUILayout.IntField("타일 세로", map.tileZ);

        // 최소최대값 정하기
        map.tileX = Mathf.Clamp(map.tileX, 1, 500);
        map.tileZ = Mathf.Clamp(map.tileZ, 1, 500);

        // 바닥 Prefab Field
        // allowSceneObjects = false : Project창에 있는 오브젝트만 허용 (Scene에 이미 있는 오브젝트 불허)
        map.floor = (GameObject)EditorGUILayout.ObjectField("바닥", map.floor, typeof(GameObject), false);

        // 파란색 큐브 Prefab Field
        map.blueCube = (GameObject)EditorGUILayout.ObjectField("파란색 큐브", map.blueCube, typeof(GameObject), false);

        // map.objs Field
        EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("objs"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("createdObjects"));
        if (check.changed)
        {
            // 인스펙터 상에서 변경을 하겠다.
            serializedObject.ApplyModifiedProperties();
        }

        // 선택한 오브젝트 Idx Field
        map.selectObjIdx = EditorGUILayout.Popup("선택 오브젝트", map.selectObjIdx, objsName);

        // Inspector 공간 추가
        EditorGUILayout.Space();

        // 바닥 생성 버튼
        if (GUILayout.Button("바닥 생성"))
        {
            CreateFloor();
        }

        saveFileName = EditorGUILayout.TextField("저장파일이름", saveFileName);
        // Json 저장 버튼
        if (GUILayout.Button("Json 저장"))
        {
            SaveJson();
        }

        // Json 불러오기 버튼
        if (GUILayout.Button("Json 불러오기"))
        {
            LoadJson();
        }

        // 만약 Inspector의 값이 변경되었다면
        if (GUI.changed)
        {
            //별표 모양 표시(씬 이동시, 유니티를 끌 때 저장 팝업이 뜨게)
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

    }

    void LoadJson()
    {
        // 만약에 saveFileName의 길이가 0이라면 함수 나가기
        if (saveFileName.Length <= 0)
        {
            // 파일이름을 입력하세요.
            Debug.LogError("파일 이름을 입력하세요.");
            return;
        }

        // 이전 맵 데이터를 삭제
        CreateFloor();

        // mapData.txt 불러오기
        string jsonData = File.ReadAllText(Application.dataPath + "/" + saveFileName + ".txt");  // 모바일의 경우 : Application.persistentDataPath 사용 i,9k 
        // ArrayJson 형태로 Json 변환
        ArrayJson arrayJson = JsonUtility.FromJson<ArrayJson>(jsonData);
        // ArrayJson의 데이터를 가지고 오브젝트 생성
        for (int i=0; i<arrayJson.datas.Count; i++)
        {
            SaveJsonInfo info = arrayJson.datas[i];
            LoadObject(info.idx, info.position, info.eulerAngle, info.localScale);
        }
    }



    // Scene 그리는 함수
    private void OnSceneGUI()
    {
        // Map이 선택되었을 때, Scene에서 다른 오브젝트를 클릭해도 선택이 되지 않게 하기
        int id = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(id);

        DrawGrid();

        CreateObject();

        DeleteObject();
    }

    void SaveJson()
    {
        // 만약에 saveFileName의 길이가 0이라면 함수 나가기
        if (saveFileName.Length <= 0)
        {
            // 파일이름을 입력하세요.
            Debug.LogError("파일 이름을 입력하세요.");
            return;
        }

        // map.createdObjects에 있는 정보를 json으로 변환
        // idx, position, eulerAngle, localScale 
        // map.createdObjects 갯수만큼 SaveJsonInfo 만들어 세팅
        // ArrayJson 만든다.
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
            // ArrayJson 하나 만들어서 datas에 하나씩 추가
            arrayJson.datas.Add(info);
        }
        // arrayJson을 Json으로 변환
        string jsonData = JsonUtility.ToJson(arrayJson, true);
        Debug.Log("jsonData : " + jsonData);
        // jsonData를 파일로 저장
        File.WriteAllText(Application.dataPath + "/" + saveFileName + ".txt", jsonData);

    }

    void DeleteObject()
    {
        Event e = Event.current;
        // 마우스 왼쪽 버튼 & 컨트롤 키를 누르고 있으면
        if (e.type == EventType.MouseDown && e.button == 0 && e.control)
        {
            // 마우스 포인터에서 Ray를 만들고
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            // 만든 Ray를 발사해서 부딪힌 놈이 있다면
            if (Physics.Raycast(ray, out hit))
            {
                // 부딪힌 놈의  Layer가 Object라면
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Object"))
                { 
                    // map.createdObjects에서 hit.transform.gameObject를 찾아서 빼주자
                    for (int i=0; i < map.createdObjects.Count; i++)
                    {
                        if (map.createdObjects[i].go == hit.transform.gameObject)
                        {
                            map.createdObjects.RemoveAt(i);
                            break;
                        }
                    }
                    // 해당 오브젝트를 파괴하겠다.
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
        // 세로줄
        Handles.color = Color.magenta;
        for (int i = 0; i <= map.tileX; i++)
        {
            start = new Vector3(i, 0, 0);
            end = new Vector3(i, 0, map.tileZ);
            Handles.DrawLine(start, end);
        }
        // 가로줄
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
        // 만약에 기존 바닥이 있었다면 지우자
        if (floor != null)
        {
            DestroyImmediate(floor);
        }

        GameObject empty = GameObject.Find("obj_parent");
        if (empty != null)
        {
            DestroyImmediate(empty);
        }

        // 빈 오브젝트 (배치한 오브젝트 담는 오브젝트들의 부모)
        empty = new GameObject();
        empty.name = "obj_parent";

        // 기본 바닥 생성
        floor = (GameObject) PrefabUtility.InstantiatePrefab(map.floor);
        // tileX, tileY만큼 크기를 키운다.
        floor.transform.localScale = new Vector3(map.tileX, 1, map.tileZ);
        // 만들어진 오브젝트 리스트 지우기
        map.createdObjects.Clear();
    }

    void LoadObject(int idx, Vector3 position, Vector3 eulerAngle, Vector3 localScale)
    {
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(map.objs[idx]);
        obj.transform.position = position;
        obj.transform.eulerAngles = eulerAngle;
        obj.transform.localScale = localScale;

        // 부모를 obj_parent로 하기
        // 부모를 obj_parent로 하기
        obj.transform.parent = GameObject.Find("obj_parent").transform;

        // 만들어진 오브젝트를 리스트에 추가
        CreatedInfo info = new CreatedInfo();
        info.go = obj;
        info.idx = idx;
        map.createdObjects.Add(info);
    }

    // 클릭한 지점 오브젝트 생성
    void CreateObject()
    {
        // 현재 Input 이벤트 관리
        Event e = Event.current;

        // 마우스 눌렀다면 
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
