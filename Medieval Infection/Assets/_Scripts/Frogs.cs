using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Frogs : MonoBehaviour
{

    private IEnumerator _manageFrogs;

    private void Awake()
    {
        _manageFrogs = ManageFrogs();


        //for (int i = 0; temp.MoveNext() && i < 20; i++)
        //{
        //    Debug.Log(temp.Current);
        //}

    }

    private void OnEnable()
    {
        StartCoroutine(_manageFrogs);
    }



    IEnumerator ManageFrogs()
    {
        AudioSource audio = GetComponent<AudioSource>();
        yield return new WaitForSeconds(Random.Range(-3f, 7f));
        while (true)
        {
            audio.enabled = true;
            yield return new WaitForSeconds(Random.Range(2f, 15f));
            audio.enabled = false;
            yield return new WaitForSeconds(Random.Range(10f, 20f));
        }
    }


}
