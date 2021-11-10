using System;
using UnityEngine;

public class RethrowError : MonoBehaviour
{
    void Update()
    {
        try
        {
            DoSomethingInteresting();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw e;
        }
    }

    private void DoSomethingInteresting()
    {
        throw new System.NotImplementedException();
    }
}
