using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class ChatterSubscriber : MonoBehaviour
{
    ROSConnection ros;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<StringMsg>("/chatter", ReceiveMessage);
    }

    void ReceiveMessage(StringMsg msg)
    {
        Debug.Log("Received: " + msg.data);
    }
}
