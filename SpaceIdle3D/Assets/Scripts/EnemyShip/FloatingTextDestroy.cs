using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextDestroy : MonoBehaviour
{
    public GameObject prefab;
    public float disableTime = 2.0f;

    private List<GameObject> pool = new List<GameObject>();

    public static FloatingTextDestroy floatTextDes;

    private void Awake()
    {
        floatTextDes = this;

        GameObject obj;
        for (int i = 0; i < 10; i++)
        {
            obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public void EnableObject(string text, Vector3 pos)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].GetComponentInChildren<TextMesh>().text = text;
                pool[i].transform.position = pos;
                //pool[i].transform.rotation = Quaternion.identity;
                pool[i].SetActive(true);

                StartCoroutine(DisableText(pool[i]));

                return;
            }
        }
    }

    private IEnumerator DisableText(GameObject obj)
    {
        yield return new WaitForSeconds(disableTime);

        obj.SetActive(false);

        yield return null;
    }
}
