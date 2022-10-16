using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A반 연락처 정보
[System.Serializable]
public class UserInfoA
{
    public string name;
    public string phone;
    public int age;
}

[System.Serializable]
public class UserInfoB
{
    public string name;
    public string phone;
    public string email;
    public int age;
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    // A반 학생들 정보 담을 변수
    public List<UserInfoA> userInfoA;

    // B반 학생들 정보 담을 변수
    public List<UserInfoB> userInfoB;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        userInfoA =CSV.instance.Parsing<UserInfoA>("Table_Contact_A");
        for (int i=0; i<userInfoA.Count; i++)
        {
            print(userInfoA[i].name);
        }
        //userInfoB =CSV.instance.Parsing<UserInfoB>("Table_Contact_B");
    }

    
    void Update()
    {
        
    }
}
