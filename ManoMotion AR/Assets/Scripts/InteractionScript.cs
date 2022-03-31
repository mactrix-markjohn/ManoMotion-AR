using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.XR.ARFoundation;

public class InteractionScript : MonoBehaviour
{

    [SerializeField] private GameObject Bullet;
    [SerializeField] private float movePower;
    [SerializeField] private Camera Camera;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        // --------- Request for permission from  the user to use the Camera --------------
        
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        
        
        // ---------- End of Request for Camera permission ----------------
    }

    // Update is called once per frame
    void Update()
    {
        if (ARSession.state == ARSessionState.SessionTracking)
        {
            FollowPalmCenter();
            
        }
    }

    private void FollowPalmCenter()
    {

        HandInfo currentlyDetectedHand = ManomotionManager.Instance.Hand_infos[0].hand_info;

        ManoGestureTrigger currentlyDetectedManoClass = currentlyDetectedHand.gesture_info.mano_gesture_trigger;
        Vector3 palmCenterPosition = currentlyDetectedHand.tracking_info.palm_center;

        
        // ---------- this will spawn on hand release gesture ------------------------
        if (currentlyDetectedManoClass == ManoGestureTrigger.RELEASE_GESTURE)
        {
            
            _ShowAndroidToastMessage("Release Gesture activated");
            
            
            GameObject prefabObject = Instantiate(Bullet, transform.position, Quaternion.identity);
           
            prefabObject.transform.position = ManoUtils.Instance.CalculateNewPositionDepth(palmCenterPosition,
                currentlyDetectedHand.tracking_info.depth_estimation);

            if (!prefabObject.GetComponent<Rigidbody>())
            {
                prefabObject.AddComponent<Rigidbody>();
            }
            
            prefabObject.GetComponent<Rigidbody>().AddForce(Camera.transform.forward * movePower, ForceMode.Impulse);
            
            
        }
        


    }
    
    
    public static void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>(
                    "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
