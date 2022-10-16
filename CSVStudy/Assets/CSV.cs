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

        // ���� ����
        //string stringData = File.ReadAllText(path);
        byte[] byteData = File.ReadAllBytes(path);
        string stringData = Encoding.GetEncoding("euc-kr").GetString(byteData);

        // ���� �������� ���پ� �ڸ���
        string[] lines = stringData.Split("\n");
        // \r �� ����
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].Replace("\r", "");
        }

        // ���� ������
        string[] variable = lines[0].Split(",");

        // �� ������ (���� ù��° �ٺ��� ����)
        for (int i = 1; i < lines.Length; i++)
        {
            // ���࿡ ���� ���ٸ�(���̰� 0�̶��)
            if (lines[i].Length == 0) continue;

            string[] value = lines[i].Split(",");

            // ���� �����
            T data = new T();

            for (int j=0; j < variable.Length; j++)
            {
                // variable[0] = "name", variable[1] = "phone", variabl[2] = "age"
                // �ش� �̸����� �Ǿ��ִ� ������ ������ ������
                System.Type type = data.GetType();
                System.Reflection.FieldInfo fieldInfo = data.GetType().GetField(variable[j]);
                // int.parse, float.parse ... ������ ���� �����س��´�.
                TypeConverter typeConverter = TypeDescriptor.GetConverter(fieldInfo.FieldType);
                // ���� ����
                fieldInfo.SetValue(data, typeConverter.ConvertFrom(value[j]));
            }

            // ���� �߰�
            list.Add(data);
        }

        return list;
    }

    
    public List<UserInfoA> Parsing(string fileName)
    {
        List<UserInfoA> list = new List<UserInfoA>();

        string path = Application.dataPath + "/" + fileName + ".csv";
        // ���� ����
        //string stringData = File.ReadAllText(path);
        byte [] byteData = File.ReadAllBytes(path);
        string  stringData = Encoding.GetEncoding("euc-kr").GetString(byteData);

        // ���� �������� ���پ� �ڸ���
        string [] lines = stringData.Split("\n");
        // \r �� ����
        for (int i=0; i<lines.Length; i++)
        {
            lines[i].Replace("\r", "");
        }

        // ���� ������
        string[] variable = lines[0].Split(",");

        // �� ������ (���� ù��° �ٺ��� ����)
        for (int i=1; i<lines.Length; i++)
        {
            // ���࿡ ���� ���ٸ�(���̰� 0�̶��)
            if (lines[i].Length == 0) continue;

            string[] value = lines[i].Split(",");
            // ���� �����
            UserInfoA data = new UserInfoA();
            data.name = value[0];
            data.phone = value[1];
            data.age = int.Parse(value[2]);

            list.Add(data);
        }

        return list;
    }
}