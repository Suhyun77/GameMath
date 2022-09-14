using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{

    void Update()
    {
        // ���콺 ��ǥ
        Vector3 mPos = Input.mousePosition;
        // ������Ʈ ��ǥ
        Vector3 objPos = transform.position;
        // ���� ������Ʈ�� ī�޶� z�� ����
        mPos.z = objPos.z - Camera.main.transform.position.z;

        // ���콺 ��ǥ : ��ũ�� -> ����
        Vector3 target = Camera.main.ScreenToWorldPoint(mPos);

        // ���� ���ϱ�
        float dx = target.x - objPos.x;
        float dy = target.y - objPos.y;
        float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg; // Rad2Deg : ���� -> �Ϲݰ�

        // z�� ���Ϸ� ȸ��
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
