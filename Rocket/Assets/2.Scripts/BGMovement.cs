using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BGMovement : MonoBehaviour
{

    [SerializeField] Transform[] Layer1 = null;
    [SerializeField] Transform[] Layer2 = null;
    [SerializeField] Transform[] Whell = null;

    float m_leftPosX = 0;
    float m_rightPosX = 0;

    float f_layer1_speed = -2f;
    float f_layer2_speed = -1.5f;
    public float f_total_speed = 1;


    private void Start()
    {
        float f_length = Layer1[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        m_leftPosX = -f_length;
        m_rightPosX = f_length * Layer1.Length;
    }

    private void Update()
    {
        for (int i = 0; i < Layer1.Length; i++)
        {
            Layer1[i].position += new Vector3(f_layer1_speed * f_total_speed, 0, 0) * Time.deltaTime;

            if (Layer1[i].position.x < m_leftPosX)
            {
                Vector3 t_selfPos = Layer1[i].position;
                t_selfPos.Set(t_selfPos.x + m_rightPosX, t_selfPos.y, t_selfPos.z);
                Layer1[i].position = t_selfPos;
            }
        }

        for (int i = 0; i < Layer2.Length; i++)
        {
            Layer2[i].position += new Vector3(f_layer2_speed * f_total_speed, 0, 0) * Time.deltaTime;

            if (Layer2[i].position.x < m_leftPosX)
            {
                Vector3 t_selfPos = Layer2[i].position;
                t_selfPos.Set(t_selfPos.x + m_rightPosX, t_selfPos.y, t_selfPos.z);
                Layer2[i].position = t_selfPos;
            }
        }

        for (int i = 0; i < Whell.Length; i++)
        {
            Whell[i].Rotate(new Vector3(0, 0, -180 * f_total_speed) * Time.deltaTime);
        }


    }

}
