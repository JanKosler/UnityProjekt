using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentManager : MonoBehaviour
{

    /// <summary>
    /// 
    /// 
    /// 
    ///  pomucka aby kazdy objekt pridany do objektu enviroment byl typu Ground
    ///  v produkci odstranit
    ///  pouze pro prototypizaci prostredi
    ///  
    /// 
    /// 
    /// </summary>
    [SerializeField] private GameObject enviromentObject;
    
    void Start()
    {
        //vsechny deti jsou stejny layer 
        foreach (Transform child in enviromentObject.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = LayerMask.NameToLayer("Ground"); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
