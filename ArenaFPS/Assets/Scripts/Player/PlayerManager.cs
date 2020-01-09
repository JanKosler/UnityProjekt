using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    /// <summary>
    /// 
    /// 
    /// Ukaladame hrace ve scene do promene aby jsme nemuseli iterovat vsechny objekty (singleton)
    /// 
    /// 
    /// </summary>
    public static PlayerManager instance;

    

    void Awake()
    {
        instance = this;
    }
    //nas hrac
    public GameObject player;
}
