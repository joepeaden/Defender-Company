using UnityEngine;
using UnityEngine.Events;

public abstract class ActorController : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent OnActorDeath = new UnityEvent();

    //private ControllerData Data => data;
    [SerializeField]
    protected ControllerData data;
    protected Actor actor;

    protected void Awake()
    {
        actor = GetComponent<Actor>();
        actor.OnDeath.AddListener(HandleDeath);
        actor.OnGetHit.AddListener(HandleGetHit);
    }

    protected void Start()
    {
        // keep in mind. Player doesn't really need this (do they?). If not, then when implementing friendly AI can put it in
        // future ActorAIController class. This would allow seperation of the ControllerData scriptable object between player and AI.
        actor.SetAgentSpeed(data.navAgentSpeed);
    }

    protected void OnDestroy()
    {
        actor.OnDeath.RemoveListener(HandleDeath);
    }

    /// <summary>
    /// Some times ya get hit. But you gotta just get up. Keep pushing forward. Don't stop. You can do this.
    /// </summary>
    protected abstract void HandleGetHit();

    /// <summary>
    /// Think about life. How it went. Did you do all you could? Did you say what you needed to say?
    /// </summary>
    protected virtual void HandleDeath()
    {
        StopAllCoroutines();
        enabled = false;
    }

    /// <summary>
    /// Add callback for actor's death.
    /// </summary>
    /// <param name="listener"></param>
    public void AddDeathListener(UnityAction listener)
    {
        OnActorDeath.AddListener(listener);
    }

    /// <summary>
    /// Remove callback for actor's death.
    /// </summary>
    /// <param name="listener"></param>
    public void RemoveDeathListener(UnityAction listener)
    {
        OnActorDeath.AddListener(listener);
    }

    /// <summary>
    /// Get the actor script.
    /// </summary>
    /// <returns></returns>
    public Actor GetActor()
    {
        return actor;
    }

    public void SetControllerData(ControllerData newControllerData)
    {
        data = newControllerData;
    }
}
