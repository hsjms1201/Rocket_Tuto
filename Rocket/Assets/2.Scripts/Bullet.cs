using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    float f_recycle_time = 1.0f;
    int m_atk = 70;

    private void OnEnable()
    {
        f_recycle_time = 1.0f;

    }

    private void Update()
    {
        f_recycle_time -= Time.deltaTime;
        if (f_recycle_time <= 0.0f)
        {
            ObjectPool.Recycle(this);
        }


        if (transform.position.y < -3f)
        {
            GameManager.instance.Spawn_Bullet_Marks(transform.position);
            ObjectPool.Recycle(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy_Control>())
        {
            if (!GameManager.instance.list_bullet_enemy.Contains(collision))
            {
                collision.GetComponent<Enemy_Control>().Hp -= m_atk;
                GameManager.instance.list_bullet_enemy.Add(collision);
                GameManager.instance.Set_Damege_Txt(collision.transform.position + (Vector3.up), m_atk);

            }

            ObjectPool.Recycle(gameObject);
        }
    }
}
