using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy_Movement : MonoBehaviour
{
    [System.Serializable]
    public class NestedList
    {
        public List<Transform> inner_list = new List<Transform>();
    }

    [SerializeField]
    public List<NestedList> enemys_list = new List<NestedList>();   // 몬스터 리스트
    public Transform tr_tower;                                      // 타워 위치
    public Transform tr_spawn_pos;                                  // 스폰 위치
    public Transform tr_down_enemy;                                 // 현재 내려가는 몬스터
    public Transform tr_up_enemy;                                   // 현재 올라가는 몬스터

    public float f_enemySpeed = 3f;                                 // 이동 속도
    public float f_riseHeight = 1.2f;                               // 위로 올라가는 높이
    public float f_spacing = -0.7f;                                 // 몬스터 간격

    public List<bool> is_up = new List<bool>();                     // 몬스터 올라가는 중
    public bool is_down = false;                                    // 몬스터 내려가는 중
    float f_down_delay = 0.5f;                                      // 내려가는 딜레이 시간


    private void Awake()
    {
        enemys_list.Clear();

        for (int i = 0; i < 10; i++)
        {
            enemys_list.Add(new NestedList());
            is_up.Add(false);
        }
    }

    void Update()
    {
        if (enemys_list.Count >= 1)
        {
            MoveEnemys();
        }
    }

    // 몬스터 이동 로직
    void MoveEnemys()
    {

        f_down_delay -= Time.deltaTime;

        for (int i = 0; i < 10; i++)
        {

            int m_floor = i;

            List<Transform> floor_monster = enemys_list[m_floor].inner_list;

            List<Transform> base_enemy = floor_monster.FindAll(x => !x.GetComponent<Enemy_Control>().is_logic);

            if (base_enemy.Count < 1)
            {
                continue;
            }

            // 1.맨 앞 몬스터는 타워 앞에 정렬
            Transform firstMonster = base_enemy[0];
            Vector2 firstPosition = new Vector2(tr_tower.position.x, tr_spawn_pos.transform.position.y + (f_riseHeight * m_floor));
            firstMonster.position = Vector2.MoveTowards(firstMonster.position, firstPosition, Time.deltaTime * f_enemySpeed);

            if (Vector2.Distance(firstMonster.position, firstPosition) < 0.1f)
            {
                firstMonster.GetComponent<Enemy_Control>().is_check = true;
                if (tr_down_enemy == firstMonster) is_down = false;
            }
            else
            {
                firstMonster.GetComponent<Enemy_Control>().is_check = false;
            }

            // 2. 나머지 몬스터들은 앞 몬스터를 따라가며 일정한 간격 유지
            if (base_enemy.Count > 1)
            {
                for (int j = 1; j < base_enemy.Count; j++)
                {
                    Vector2 targetPosition = new Vector2(tr_tower.position.x - (j * f_spacing), tr_spawn_pos.transform.position.y + (f_riseHeight * m_floor));
                    base_enemy[j].position = Vector2.MoveTowards(base_enemy[j].position, targetPosition, Time.deltaTime * f_enemySpeed);
                    if (Vector2.Distance(base_enemy[j].position, targetPosition) < 0.1f)
                    {
                        base_enemy[j].GetComponent<Enemy_Control>().is_check = true;
                    }
                    else
                    {
                        base_enemy[j].GetComponent<Enemy_Control>().is_check = false;
                    }

                }
            }


            List<Transform> move_enemys = floor_monster.FindAll(x => x.GetComponent<Enemy_Control>().is_check && !x.GetComponent<Enemy_Control>().is_logic);

            //3. 맨뒤 몬스터 위로 올리기
            List<Transform> Up_floor_monster = enemys_list[m_floor + 1].inner_list;
            int limit_floor_enemy = move_enemys.Count > 10 ? 10 : move_enemys.Count < 3 ? 1 : move_enemys.Count - 2;

            if (move_enemys.Count >= 2 && Up_floor_monster.Count < limit_floor_enemy && !is_up[m_floor])
            {
                is_up[m_floor] = true;

                Transform lastMonster = move_enemys[move_enemys.Count - 1];
                lastMonster.GetComponent<Enemy_Control>().is_logic = true;

                enemys_list[m_floor].inner_list.Remove(lastMonster);
                enemys_list[m_floor + 1].inner_list.Add(lastMonster);

                Vector2 risePosition = lastMonster.position + (Vector3.up * f_riseHeight);
                risePosition.x += f_spacing;

                StartCoroutine(MoveUpAndForward(lastMonster, risePosition, i));

            }


        }


        // 4. 2층이상 몬스터 맨앞줄 내리기
        if (f_down_delay <= 0)
        {
            List<Tuple<int, Transform>> DownList = new List<Tuple<int, Transform>>();

            f_down_delay = Random.Range(1, 10f) / 10f;

            for (int i = 1; i < 10; i++)
            {
                int m_floor = i;

                List<Transform> floor_monster = enemys_list[m_floor].inner_list;

                List<Transform> move_enemys = floor_monster.FindAll(x => x.GetComponent<Enemy_Control>().is_check && !x.GetComponent<Enemy_Control>().is_logic);

                //3. 2층이상 내려오기
                if (move_enemys.Count > 0)
                {
                    DownList.Add(new Tuple<int, Transform>(m_floor, floor_monster[0]));
                }
            }

            foreach (var item in DownList)
            {
                int m_floor = item.Item1;
                Transform FristMonster = item.Item2;
                tr_down_enemy = FristMonster;
                enemys_list[m_floor].inner_list.Remove(FristMonster);
                enemys_list[m_floor - 1].inner_list.Insert(0, FristMonster);
                FristMonster.GetComponent<Enemy_Control>().is_check = false;
            }
        }


    }


    // 맨뒤 몬스터 위로 올리기
    IEnumerator MoveUpAndForward(Transform monster, Vector2 risePos, int floor)
    {

        tr_up_enemy = monster;

        while (Vector2.Distance(monster.position, risePos) > 0.1f)
        {
            monster.position = Vector2.MoveTowards(monster.position, risePos, Time.deltaTime * f_enemySpeed);
            yield return null;
        }


        monster.position = risePos;
        monster.GetComponent<Enemy_Control>().is_logic = false;
        monster.GetComponent<Enemy_Control>().is_check = false;

        is_up[floor] = false;
        tr_up_enemy = null;

    }

}
