using UnityEngine.Events;
using UnityEngine;

/// <summary>
/// Goes on the aim glow; purpose is to detect when player aim point is close to enemy so that it's easier for controller aiming.
/// </summary>
public class AimPoint : MonoBehaviour
{
    public UnityEvent<bool> OnTargetInSights = new UnityEvent<bool>();

    private Actor targetActor;

    private void Start()
    {
        OnTargetInSights.AddListener(GameManager.Instance.GetPlayerScript().HandleTargetInSights);
    }

    private void Update()
    {
        if (targetActor != null && !targetActor.IsAlive)
        {
            OnTargetInSights.Invoke(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SnapToAimBox") && !other.CompareTag("Player"))
        {
            targetActor = other.transform.parent.GetComponent<Actor>();
            OnTargetInSights.Invoke(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SnapToAimBox") && !other.CompareTag("Player"))
        {
            targetActor = null;
            OnTargetInSights.Invoke(false);
        }
    }

    private void OnDestroy()
    {
        OnTargetInSights.RemoveAllListeners();
    }
}
