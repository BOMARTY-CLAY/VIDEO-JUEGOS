using UnityEngine;
using Unity.Cinemachine;
public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public CinemachineBrain cmBrain;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
