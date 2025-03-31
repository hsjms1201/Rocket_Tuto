using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectRecycle : MonoBehaviour
{
    public float f_recycle_time = 1.0f;

    private void OnEnable()
    {
        f_recycle_time = 1.0f;

        if (GetComponent<SpriteRenderer>())
        {
            GetComponent<SpriteRenderer>().DOFade(0, 0.3f).SetDelay(0.3f);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        f_recycle_time -= Time.deltaTime;
        if (f_recycle_time <= 0.0f)
        {
            ObjectPool.Recycle(this);
        }
    }
}
