using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;

public class Enemy_Control : MonoBehaviour
{

    public bool is_check = false;
    public bool is_logic = false;

    List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

    int m_hp = 210;
    readonly float f_total_hp = 210;
    Slider slider_hp;
    Slider slider_hpshow;
    List<Material> materials = new List<Material>();
    Enemy_Movement movement;

    public int Hp
    {
        get { return m_hp; }
        set
        {
            m_hp = value;

            slider_hp.gameObject.SetActive(true);
            float f_percnet = m_hp / f_total_hp;
            slider_hp.value = f_percnet;
            DOTween.To(() => slider_hpshow.value, x => slider_hpshow.value = x, f_percnet, 0.8f);
            StartCoroutine("Co_Hit_Effect");
            if (m_hp <= 0)
            {
                foreach (var item in movement.enemys_list)
                {
                    if (item.inner_list.Contains(transform))
                    {
                        item.inner_list.Remove(transform);
                        if (GameManager.instance.tr_target_enemy == transform) GameManager.instance.tr_target_enemy = null;
                        if (movement.tr_down_enemy != null && movement.tr_down_enemy.Equals(transform)) movement.is_down = false;

                        ObjectPool.Recycle(gameObject);

                    }
                }
            }
        }
    }

    // 히트시 효과
    private IEnumerator Co_Hit_Effect()
    {
        foreach (var item in materials)
        {
            item.SetFloat("_FlashAmount", 0.5f); // 흰색으로 변경

        }
        yield return new WaitForSeconds(0.2f);

        foreach (var item in materials)
        {
            item.SetFloat("_FlashAmount", 0); // 원래 색으로 복귀

        }
    }

    public void Set_Enemy(int sorting, Enemy_Movement _movement)
    {
        if (spriteRenderers.Count < 1)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true).ToList();

            //반짝이는 효과 마테리얼 추가
            foreach (var item in spriteRenderers)
            {
                Material ma = new Material(item.material);
                materials.Add(ma);
                item.material = ma;
            }

            slider_hp = transform.Find("HPPanel").GetComponentInChildren<Slider>(true);
            slider_hpshow = slider_hp.transform.Find("HpShowSlider").GetComponent<Slider>();
        }

        // 레이어 순서 조정
        string name = string.Format("Enemy_{0}", sorting);
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.sortingLayerID = SortingLayer.NameToID(name);
        }

        movement = _movement;

        Reset_Enemy_Info();
    }

    // 정보 초기화
    public void Reset_Enemy_Info()
    {
        m_hp = (int)f_total_hp;
        float f_percnet = m_hp / f_total_hp;
        slider_hp.value = f_percnet;
        slider_hpshow.value = f_percnet;
        slider_hp.gameObject.SetActive(false);
        is_check = false;
        is_logic = false;

        foreach (var item in materials)
        {
            item.SetFloat("_FlashAmount", 0); // 원래 색으로 복귀

        }
    }

}

