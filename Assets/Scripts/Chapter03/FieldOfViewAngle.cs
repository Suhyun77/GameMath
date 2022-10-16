using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewAngle : MonoBehaviour
{
    // �÷��̾�
    public Transform trPlayer;
    // �þ߰�
    public float viewAngle = 45;
    // ��Ƽ����
    Material [] mat;
    // ���� ����Ʈ
    public List<Color> colorList = new List<Color>();

    private void Start()
    {
        mat = GetComponent<MeshRenderer>().materials;
    }

    void Update()
    {
        // 1. �÷��̾� - �߽� ������(���̰���) ���ϱ�
        Vector3 player = trPlayer.position - transform.position;
        float cosAngle = Vector3.Dot(player, -transform.forward);
        
        print("cosAngle : " + cosAngle);

        // 2. �������� �������̰� ������ �þ߰�/2���� ���� ���
        if (cosAngle > 0 && Mathf.Abs(cosAngle) < viewAngle/2)
        {
            Debug.DrawLine(transform.position, player);
            // 3. �÷��̾ �տ� �ִٰ� �Ǵ��Ͽ� ��Ƽ���� ����
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
