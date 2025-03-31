using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    BGMovement bGMove;

    public GameObject obj_Enemy;                        // 몬스터 오브젝트
    public GameObject obj_Bullet;                       // 총알 오브젝트
    public GameObject obj_Damage;                       // 데미지 텍스트 오브젝트
    public GameObject obj_Bullet_Marks;                 // 총알 자국 오브젝트
                                                    
    public Transform[] arr_spawn_pos;                   // 스폰 위치
                                                    
    public Enemy_Movement[] monster_Movements;          // 몬스터 이동 로직 스크립트
                                                    
    public Transform tr_shotgun;                        // 샷건
    public Transform tr_firePoint;                      // 총알 발사 위치
    public Transform tr_target_enemy;                   // 총알 발사 타켓 몬스터
    public List<Collider2D> list_bullet_enemy;          // 총에 맞은 몬스터

    private void Awake()
    {
        instance = this;

        bGMove = GetComponent<BGMovement>();

        // 오브젝트 풀 생성
        ObjectPool.CreatePool(obj_Enemy, 100);
        ObjectPool.CreatePool(obj_Bullet, 12);
        ObjectPool.CreatePool(obj_Bullet_Marks, 12);
        ObjectPool.CreatePool(obj_Damage, 100);
       
    }

    private void Start()
    {
        StartCoroutine("Co_Spawn_Enemy");
        StartCoroutine("Co_Attack_Shotgun");
    }
    

    // 총알 자국 생성
    public void Spawn_Bullet_Marks(Vector3 _pos)
    {
        GameObject obj_damage = ObjectPool.Spawn(obj_Bullet_Marks, _pos);
    }

    // 배경 속도 조절
    public void Change_Bg_Speed(float _speed)
    {
        DOTween.To(() => bGMove.f_total_speed, x => bGMove.f_total_speed = x, _speed, 0.5f);
    }

    // 1초마다 샷건 공격
    IEnumerator Co_Attack_Shotgun()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);

            if (tr_target_enemy == null)
            {
                tr_target_enemy = FindTargetEnemy();
            }
            
            if (tr_target_enemy != null && InsideScreen(tr_target_enemy))
            {
                Shot();
            }

        }
    }
    
    // 데미지 텍스트 생성
    public void Set_Damege_Txt(Vector3 _pos,int _damage)
    {
        GameObject obj_damage = ObjectPool.Spawn(obj_Damage, _pos);
        obj_damage.GetComponent<TextMeshPro>().text = _damage.ToString();
    }

    private void Update()
    {
        if (tr_target_enemy != null)
        {
            // 샷건 움직임
            Vector2 newPos = tr_target_enemy.transform.position - tr_shotgun.transform.position;
            float rotZ = Mathf.Atan2(newPos.y, newPos.x) * Mathf.Rad2Deg;
            tr_shotgun.transform.rotation = Quaternion.Euler(0, 0, rotZ - 30f);
        }

    }

    // 총알 발사
    public void Shot()
    {
        list_bullet_enemy.Clear();

        for (int i = 0; i < 6; i++)
        {

            Vector3 originalRotVec3 = tr_firePoint.eulerAngles;
            Vector3 newRotation = originalRotVec3 + new Vector3(0,0,Random.Range(-5, 5));

            // 총알 생성
            GameObject _bullet = ObjectPool.Spawn(obj_Bullet, tr_firePoint.position, Quaternion.Euler(newRotation));

            // Rigidbody가 있는지 확인 후 힘 추가
            Rigidbody2D rb = _bullet.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                int ran = Random.Range(15, 20);
                rb.AddForce(_bullet.transform.right * ran, ForceMode2D.Impulse); // 속도 설정
            }
        }

    }

    // 제일 가까운 타겟 찾기
    Transform FindTargetEnemy()
    {
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (Enemy_Movement enemy in monster_Movements)
        {
            if (enemy.enemys_list[0].inner_list.Count > 0)
            {
                float dist = Vector3.Distance(tr_shotgun.position, enemy.enemys_list[0].inner_list[0].position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = enemy.enemys_list[0].inner_list[0];
                }
            }
        }

        return closest;
    }

    // 화면 안에 있는지 확인
    public bool InsideScreen(Transform target)
    {
        var screenPoint = Camera.main.WorldToScreenPoint(target.position);
        var isInScreen = screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height;
        return isInScreen;
    }

    // 몬스터 스폰
    IEnumerator Co_Spawn_Enemy()
    {
       while (true) {
            int spwan_index = Random.Range(0, arr_spawn_pos.Length);

            Transform _enemy = ObjectPool.Spawn(obj_Enemy).transform;

            monster_Movements[spwan_index].enemys_list[0].inner_list.Add(_enemy);
            _enemy.GetComponent<Enemy_Control>().Set_Enemy(spwan_index + 1, monster_Movements[spwan_index]);
            _enemy.transform.position = arr_spawn_pos[spwan_index].position;
            _enemy.transform.SetParent(arr_spawn_pos[spwan_index]);
            _enemy.gameObject.layer = 6 + spwan_index;
            _enemy.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.7f);
        }
    }

}
