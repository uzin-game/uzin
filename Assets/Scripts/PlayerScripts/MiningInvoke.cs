using UnityEngine;
using UnityEngine.Events;

public class MiningInvoke : MonoBehaviour
{
    public KeyCode InteractKey;
    public UnityEvent InteractAction;
    [SerializeField] public Mining MiningScript;

    void Update()
    {
        if (Input.GetKeyDown(InteractKey))
        {
            InteractAction.Invoke();
        }
    }
}
