using UnityEngine;

public class FurnaceInteraction : MonoBehaviour
{
    public bool IsInteracting;
    [SerializeField] private GameObject FurnaceUI;

    public void Interact()
    {
        if (!IsInteracting)
        {
            IsInteracting = true;
            FurnaceUI.SetActive(true);
        }

        else if (IsInteracting)
        {
            IsInteracting = false;
            FurnaceUI.SetActive(false);
        }
    }
}
