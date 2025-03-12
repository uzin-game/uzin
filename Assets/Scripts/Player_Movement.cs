using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public GameObject player;


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            while(Input.GetKeyDown(KeyCode.Z)) player.transform.position = new Vector3(0,1,0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            while(Input.GetKeyDown(KeyCode.S)) player.transform.position = new Vector3(0,-1,0);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            while(Input.GetKeyDown(KeyCode.Q)) player.transform.position = new Vector3(-1,0,0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            while(Input.GetKeyDown(KeyCode.D)) player.transform.position = new Vector3(+1,0,0);
        }
    }
}
