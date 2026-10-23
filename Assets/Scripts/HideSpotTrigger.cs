using UnityEngine;

public class HideSpotTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PlayerHideState hide = other.GetComponentInParent<PlayerHideState>();
        if (hide != null)
        {
            hide.SetHiding(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        PlayerHideState hide = other.GetComponentInParent<PlayerHideState>();
        if (hide != null)
        {
            hide.SetHiding(false);
        }
    }
}