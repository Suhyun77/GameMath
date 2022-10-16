using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter01 : MonoBehaviour
{

    void Update()
    {
        // 마우스 좌표
        Vector3 mPos = Input.mousePosition;
        // 오브젝트 좌표
        Vector3 objPos = transform.position;
        // 게임 오브젝트와 카메라 z축 차이
        mPos.z = objPos.z - Camera.main.transform.position.z;
        // 마우스 좌표 : 스크린 -> 월드
        Vector3 target = Camera.main.ScreenToWorldPoint(mPos);

        // 각도 구하기
        float dx = target.x - objPos.x;
        float dy = target.y - objPos.y;
        float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg; // Atan2 : 아크 탄젠트(라디안 반환), Rad2Deg : 라디안 -> 일반각

        // z축 오일러 회전
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
