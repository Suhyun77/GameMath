using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

public class CSV : MonoBehaviour
{
    public static CSV instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Generic
    public List<T> Parsing<T>(string fileName) where T : new()
    {
        List<T> list = new List<T>();

        string path = Application.streamingAssetsPath + "/" + fileName + ".csv";

        // 파일 열자
        //string stringData = File.ReadAllText(path);
        byte[] byteData = File.ReadAllBytes(path);
        string stringData = Encoding.GetEncoding("euc-kr").GetString(byteData);

        // 엔터 기준으로 한줄씩 자르기
        string[] lines = stringData.Split("\n");
        // \r 을 제거
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].Replace("\r", "");
        }

        // 변수 나누기
        string[] variable = lines[0].Split(",");

        // 값 나누기 (엑셀 첫번째 줄부터 시작)
        for (int i = 1; i < lines.Length; i++)
        {
            // 만약에 값이 없다면(길이가 0이라면)
            if (lines[i].Length == 0) continue;

            string[] value = lines[i].Split(",");

            // 정보 만들기
            T data = new T();

            for (int j=0; j < variable.Length; j++)
            {
                // variable[0] = "name", variable[1] = "phone", variabl[2] = "age"
                // 해당 이름으로 되어있는 변수의 정보를 얻어오자
                System.Type type = data.GetType();
                System.Reflection.FieldInfo fieldInfo = data.GetType().GetField(variable[j]);
                // int.parse, float.parse ... 무엇을 할지 결정해놓는다.
                TypeConverter typeConverter = TypeDescriptor.GetConverter(fieldInfo.FieldType);
                // 값을 셋팅
                fieldInfo.SetValue(data, typeConverter.ConvertFrom(value[j]));
            }

            // 정보 추가
            list.Add(data);
        }

        return list;
    }

    
    public List<UserInfoA> Parsing(string fileName)
    {
        List<UserInfoA> list = new List<UserInfoA>();

        string path = Application.dataPath + "/" + fileName + ".csv";
        // 파일 열자
        //string stringData = File.ReadAllText(path);
        byte [] byteData = File.ReadAllBytes(path);
        string  stringData = Encoding.GetEncoding("euc-kr").GetString(byteData);

        // 엔터 기준으로 한줄씩 자르기
        string [] lines = stringData.Split("\n");
        // \r 을 제거
        for (int i=0; i<lines.Length; i++)
        {
            lines[i].Replace("\r", "");
        }

        // 변수 나누기
        string[] variable = lines[0].Split(",");

        // 값 나누기 (엑셀 첫번째 줄부터 시작)
        for (int i=1; i<lines.Length; i++)
        {
            // 만약에 값이 없다면(길이가 0이라면)
            if (lines[i].Length == 0) continue;

            string[] value = lines[i].Split(",");
            // 정보 만들기
            UserInfoA data = new UserInfoA();
            data.name = value[0];
            data.phone = value[1];
            data.age = int.Parse(value[2]);

            list.Add(data);
        }

        return list;
    }
}
