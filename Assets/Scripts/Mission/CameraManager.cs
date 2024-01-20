using UnityEngine;
using Cinemachine;

/// <summary>
/// Manages the camera. Camerawork should go through here.
/// </summary>
public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    public static CameraManager Instance { get { return _instance; } }

    [SerializeField] private CinemachineVirtualCamera vCam;
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

        UseFreeCamera();
        MissionManager.OnAttackEnd.AddListener(UseFreeCamera);
        PlayerInput.OnCommandModeEnter.AddListener(UseFreeCamera);
        PlayerInput.OnCommandModeExit.AddListener(FollowPlayer);
        
    }

    private void OnDestroy()
    {
        MissionManager.OnAttackEnd.RemoveListener(UseFreeCamera);
        PlayerInput.OnCommandModeEnter.RemoveListener(UseFreeCamera);
        PlayerInput.OnCommandModeExit.RemoveListener(FollowPlayer);
    }

    public void FollowPlayer()
    { 
        FollowPlayer(MissionManager.Instance.Player.ControlledActor.transform);
    }

    public void FollowPlayer(Transform playerPawn)
    {
        freeCameraTarget.GetComponent<FreeCameraMovingTarget>().isBeingFollowed = false;
        FollowTarget(playerPawn);
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
