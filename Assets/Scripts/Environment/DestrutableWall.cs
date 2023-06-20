public class DestrutableWall : Shatter
{
    private void OnDestroy()
    {
        GameManager.Instance.StartSlowMotion(.25f);
    }
}
