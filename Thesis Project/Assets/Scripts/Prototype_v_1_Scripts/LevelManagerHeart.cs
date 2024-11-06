using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagerHeart : MonoBehaviour
{
    // PLAYER
    
    // TARGET -- BIRD
    public GameObject target;
    
    // Start is called before the first frame update
    void Start()
    {
        // NOTE: temporarily  
        target = GameObject.Find("ShellRoot");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
