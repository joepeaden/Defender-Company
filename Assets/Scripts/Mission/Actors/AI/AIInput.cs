/// <summary>
/// Input stuff for AIStates.
/// </summary>
public class AIInput
{
    /// <summary>
    /// Is the actor engaging a target?
    /// </summary>
    public bool targetInDetectionRadius;
    /// <summary>
    /// Is it time for the actor to make a decision?
    /// </summary>
    public bool timeForDecision;
    /// <summary>
    /// Is the actor's target in range of its weapon?
    /// </summary>
    public bool targetInRange;
    /// <summary>
    /// Is the actor's target in optimal range of its weapon?
    /// </summary>
    public bool targetInOptimalRange;
    /// <summary>
    /// Is the actor's target in line of sight?
    /// </summary>
    public bool targetInLOS;
    /// <summary>
    /// How far the actor is from pod leader
    /// </summary>
    public float distFromPodLeader;
    /// <summary>
    /// Pod is alerted to a target
    /// </summary>
    public bool podAlerted;
    /// <summary>
    /// Is the actor in cover (entire width of actor)
    /// </summary>
    public bool fullyInCover;
    /// <summary>
    /// Is the actor in cover (entire width of actor)
    /// </summary>
    public bool fullyOutOfCover;
}
