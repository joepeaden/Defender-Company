public class DestrutableWall : Shatter
{
    private void OnDestroy()
    {
        MissionManager.Instance.StartSlowMotion(.25f);
    }
}
