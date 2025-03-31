using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck_Check : MonoBehaviour
{

    //Ʈ�� �ӵ� üũ ����
    public Vector3 vec_target_check = new Vector3(-0.7f, 0.5f, 0);

    //üũ ���� ũ��
    public Vector3 vec_box_size = new Vector3(-0.19F, 1.19F, 1);

    private void FixedUpdate()
    {
        int total = 0;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + vec_target_check, vec_box_size, 0);

        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<Enemy_Control>())
                total += 1;
        }

        //�� ������ ���� �ӵ� ����
        switch (total)
        {
            case 0:
            case 1:
                GameManager.instance.Change_Bg_Speed(1f);
                break;

            case 2:
                GameManager.instance.Change_Bg_Speed(0.5f);
                break;

            default:
                GameManager.instance.Change_Bg_Speed(0);

                break;
        }
    }

}
