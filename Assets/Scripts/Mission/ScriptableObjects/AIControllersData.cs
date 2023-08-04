using UnityEngine;

/// <summary>
/// Handles data that is not unique amongst AI controllers.
/// </summary>
[CreateAssetMenu(fileName = "AIControllersData", menuName = "MyScriptables/AIControllersData")]
public class AIControllersData : ScriptableObject
{
	/// <summary>
	/// Max time the actor hides behind cover before popping out to shoot
	/// </summary>
	public float maxBehindCoverTime;
	/// <summary>
	/// Min time the actor hides behind cover before popping out to shoot
	/// </summary>
	public float minBehindCoverTime;
	/// <summary>
	/// Interval of seconds in which actor makes decisions (like looking for cover)
	/// </summary>
    public float decisionRate;
	/// <summary>
    /// Radius actor will search for cover.
    /// </summary>
	public int coverSearchRadius;
}
