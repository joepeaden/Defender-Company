/// <summary>
/// Requires a script to implement Activate and DeActivate. The idea is for stuff in rooms like Actors and bombs, gas, etc. to implement this so they be activated or deactivated upon room entry.
/// </summary>
public interface ISetActive
{
    public void Activate();
    public void DeActivate();
}
