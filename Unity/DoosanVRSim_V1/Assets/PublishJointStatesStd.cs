using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;              // JointStateMsg
using RosMessageTypes.Std;                 // HeaderMsg
using RosMessageTypes.BuiltinInterfaces;   // TimeMsg

public class PublishJointStatesStd : MonoBehaviour
{
    public List<ArticulationBody> jointArticulations = new List<ArticulationBody>();
    public List<string> jointNames = new List<string>{
        "a0509_joint_1","a0509_joint_2","a0509_joint_3",
        "a0509_joint_4","a0509_joint_5","a0509_joint_6"
    };

    public float publishPeriodSec = 0.1f;   // 10 Hz

    private ROSConnection ros;
    private float t;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JointStateMsg>("/joint_states");
    }

    void Update()
    {
        t += Time.deltaTime;
        if (t >= Mathf.Max(0.01f, publishPeriodSec))
        {
            PublishOnce();
            t = 0f;
        }
    }

    public void PublishOnce()
{
    int n = Mathf.Min(jointArticulations.Count, jointNames.Count);
    var positions = new double[n];
    for (int i = 0; i < n; i++)
        positions[i] = jointArticulations[i].jointPosition[0];  // radians

    // Header with default stamp (zero) and empty frame_id
    var header = new RosMessageTypes.Std.HeaderMsg();

    var msg = new RosMessageTypes.Sensor.JointStateMsg(
        header,
        jointNames.ToArray(),
        positions,
        System.Array.Empty<double>(),
        System.Array.Empty<double>()
    );

    ros.Publish("/joint_states", msg);
}

}
