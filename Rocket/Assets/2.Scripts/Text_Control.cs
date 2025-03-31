using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Text_Control : MonoBehaviour
{
    public string sortingLayerName;
    public int sortingOrder;

    void Start()
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        mesh.sortingLayerName = sortingLayerName;
        mesh.sortingOrder = sortingOrder;

        transform.GetComponent<TextMeshPro>().DOFade(1,0.1f);

        float ranx = Random.Range(-5, 10) / 10f;
        float rany = Random.Range(5, 10) / 10f;

        Vector3 pos = transform.position + new Vector3(ranx, rany, 0);
        transform.DOJump(pos, 1, 1, 0.4f).SetEase(Ease.InOutQuad);
        transform.GetComponent<TextMeshPro>().DOFade(0, 0.3f).SetDelay(0.3f);

        StartCoroutine(Co_Recycle());
    }

    IEnumerator Co_Recycle()
    {
        yield return new WaitForSeconds(1.0f);

        ObjectPool.Recycle(gameObject);
    }
}
