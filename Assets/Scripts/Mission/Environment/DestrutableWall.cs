using UnityEngine;

public class DestrutableWall : MonoBehaviour//:Shatter
{

    public int hp;

    //public GameObject 

    private void Update()
    {
        if (hp <= 0)
        {
            Destroy(transform.parent.gameObject);
        }
    }


    private void OnDestroy()
    {
        //MissionManager.Instance.StartSlowMotion(.25f);
    }
}
