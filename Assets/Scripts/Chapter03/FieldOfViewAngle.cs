using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewAngle : MonoBehaviour
{
    // 플레이어
    public Transform trPlayer;
    // 시야각
    public float viewAngle = 45;
    // 머티리얼
    Material [] mat;
    // 색상 리스트
    public List<Color> colorList = new List<Color>();

    private void Start()
    {
        mat = GetComponent<MeshRenderer>().materials;
    }

    void Update()
    {
        // 1. 플레이어 - 중심 내적값(사이각도) 구하기
        Vector3 player = trPlayer.position - transform.position;
        float cosAngle = Vector3.Dot(player, -transform.forward);
        
        print("cosAngle : " + cosAngle);

        // 2. 내적값이 음수값이고 절댓값이 시야각/2보다 작을 경우
        if (cosAngle > 0 && Mathf.Abs(cosAngle) < viewAngle/2)
        {
            Debug.DrawLine(transform.position, player);
            // 3. 플레이어가 앞에 있다고 판단하여 머티리얼 변경
            mat[0].color = Color.blue;
            print("Yes");
        }
        else
        {
            mat[0].color = Color.white;
            print("No");
        }


    }
    
}
