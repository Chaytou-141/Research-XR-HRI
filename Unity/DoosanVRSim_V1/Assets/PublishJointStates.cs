using System.Collections.Generic; // <-- Important to add this!
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.MyCustom; // Update if needed

public class PublishJointStates : MonoBehaviour
{
    [Header("Joints to Publish")] // (Optional, for nice label)
    public List<ArticulationBody> jointArticulations = new List<ArticulationBody>();

    public float publishRate = 0.5f;

    private ROSConnection ros;
    private float timeElapsed;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<SimpleJointAnglesMsg>("/joint_states_unity");
    }

    void Update()
{
    timeElapsed += Time.deltaTime;

    if (timeElapsed > publishRate)
    {
        SimpleJointAnglesMsg jointMsg = new SimpleJointAnglesMsg();

        if (jointArticulations.Count >= 6)
        {
            jointMsg.joint_1 = jointArticulations[0].jointPosition[0];
            jointMsg.joint_2 = jointArticulations[1].jointPosition[0];
            jointMsg.joint_3 = jointArticulations[2].jointPosition[0];
            jointMsg.joint_4 = jointArticulations[3].jointPosition[0];
            jointMsg.joint_5 = jointArticulations[4].jointPosition[0];
            jointMsg.joint_6 = jointArticulations[5].jointPosition[0];

            Debug.Log($"Publishing joints: {jointMsg.joint_1}, {jointMsg.joint_2}, {jointMsg.joint_3}, {jointMsg.joint_4}, {jointMsg.joint_5}, {jointMsg.joint_6}");
        }

        ros.Publish("/joint_states_unity", jointMsg);
        timeElapsed = 0;
    }
}
}
