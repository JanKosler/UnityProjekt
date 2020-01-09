using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GroundBehavior : MonoBehaviour
{
    [SerializeField] private List<GroundType> groundTypes = new List<GroundType>();
    [SerializeField] private PlayerMovement player;
    [SerializeField] private string currentGround;

    void Start()
    {
        setGroundType(groundTypes[0]);
    }

    void Update()
    {
        
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Snow")
            setGroundType(groundTypes[1]);
        else if (hit.transform.tag == "Asphalt")
            setGroundType(groundTypes[2]);
        else
            setGroundType(groundTypes[0]);//defautni
    }
    private void setGroundType(GroundType ground)
    {
        if(currentGround != ground.name)
        {
           // player.walkStepSounds = ground.walkStepSounds;
            //player.runStepSounds = ground.runStepSounds;
            player.walkSpeed = ground.walkSpeed;
            player.runSpeed = ground.runSpeed;
            currentGround = ground.name;
        }
    }
    
}
[System.Serializable]
public class GroundType
{
    public string name;
    public AudioClip[] walkStepSounds;
    public AudioClip[] runStepSounds;
    public float walkSpeed = 5;
    public float runSpeed = 10;  
}
