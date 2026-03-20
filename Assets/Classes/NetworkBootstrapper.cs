using UnityEngine;

public class NetworkBootstrapper : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private bool HasArgument(string arg)
    {
        foreach (var a in System.Environment.GetCommandLineArgs())
        {
            if (a == arg) return true;
        }
        return false;
    }
}
