using UnityEngine;
using Cinemachine;

/// <summary>
/// Manages the camera. Camerawork should go through here.
/// </summary>
public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    public static CameraManager Instance { get { return _instance; } }

    private CinemachineVirtualCamera vCam;
    [SerializeField] private Transform freeCameraTarget;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("More than one Camera Manager, deleting one.");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        vCam = GetComponent<CinemachineVirtualCamera>();

        //UseFreeCamera();
        //MissionManager.OnAttackEnd.AddListener(UseFreeCamera);
        //PlayerInput.OnCommandModeEnter.AddListener(UseFreeCamera);
        PlayerInput.OnCommandModeExit.AddListener(FollowPlayer);
        
    }

    private void OnDestroy()
    {
        //MissionManager.OnAttackEnd.RemoveListener(UseFreeCamera);
        //PlayerInput.OnCommandModeEnter.RemoveListener(UseFreeCamera);
        PlayerInput.OnCommandModeExit.RemoveListener(FollowPlayer);
    }

    public void FollowPlayer()
    { 
        freeCameraTarget.GetComponent<FreeCameraMovingTarget>().isBeingFollowed = false;
        FollowTarget(MissionManager.Instance.Player.ControlledActor.CameraFollowTransform);
        //vCam.
    }

    public void UseFreeCamera()
    {
        freeCameraTarget.GetComponent<FreeCameraMovingTarget>().isBeingFollowed = true;
        FollowTarget(freeCameraTarget);
    }

    public void FollowTarget(Transform toFollow)
    {
        vCam.Follow = toFollow;
    }
}
