using System;
using UnityEngine;
using Unity.Robotics;

namespace Unity.Robotics.UrdfImporter.Control
{
    public enum RotationDirection { None = 0, Positive = 1, Negative = -1 };
    public enum ControlType { PositionControl };

    public class Controller : MonoBehaviour
    {
        private ArticulationBody[] articulationChain;
        private Color[] prevColor;
        private int previousIndex;

        [InspectorReadOnly(hideInEditMode: true)]
        public string selectedJoint;
        [HideInInspector]
        public int selectedIndex;

        public ControlType control = ControlType.PositionControl;
        public float stiffness = 10000f;
        public float damping = 10f;
        public float forceLimit = 1000f;
        public float speed = 5f;
        public float torque = 100f;
        public float acceleration = 5f;

        [Tooltip("Color to highlight the currently selected joint")]
        public Color highLightColor = new Color(1.0f, 0, 0, 1.0f);

        void Start()
        {   
             Debug.Log("Controller Start() running...");

            previousIndex = selectedIndex = 1;

            this.gameObject.AddComponent<FKRobot>();
            articulationChain = this.GetComponentsInChildren<ArticulationBody>();

            foreach (ArticulationBody joint in articulationChain)
            {
                // Skip if fixed joint (won’t move anyway)
                if (joint.jointType == ArticulationJointType.FixedJoint)
                    continue;

                if (!joint.TryGetComponent<JointControl>(out _))
                    joint.gameObject.AddComponent<JointControl>();

                joint.jointFriction = 10f;
                joint.angularDamping = damping;

                var drive = joint.xDrive;
                drive.forceLimit = forceLimit;
                drive.stiffness = stiffness;
                drive.damping = damping;
                joint.xDrive = drive;
            }

            DisplaySelectedJoint(selectedIndex);
            StoreJointColors(selectedIndex);
        }

        void SetSelectedJointIndex(int index)
        {
            if (articulationChain.Length > 0)
            {
                selectedIndex = (index + articulationChain.Length) % articulationChain.Length;
            }
        }

        void Update()
        {// TEMP DEBUG: Ensure all joints have JointControl script
    foreach (var joint in articulationChain)
    {
        if (!joint.TryGetComponent(out JointControl _))
        {
            joint.gameObject.AddComponent<JointControl>();
            Debug.Log("✅ Added JointControl to: " + joint.name);
        }
    }
            bool moveRight = Input.GetKeyDown(KeyCode.RightArrow);
            bool moveLeft = Input.GetKeyDown(KeyCode.LeftArrow);

            SetSelectedJointIndex(selectedIndex);
            UpdateDirection(selectedIndex);

            if (moveLeft)
            {
                SetSelectedJointIndex(selectedIndex - 1);
                Highlight(selectedIndex);
            }
            else if (moveRight)
            {
                SetSelectedJointIndex(selectedIndex + 1);
                Highlight(selectedIndex);
            }

            UpdateDirection(selectedIndex);
        }

        private void Highlight(int selectedIndex)
        {
            if (selectedIndex == previousIndex || selectedIndex < 0 || selectedIndex >= articulationChain.Length)
                return;

            ResetJointColors(previousIndex);
            StoreJointColors(selectedIndex);
            DisplaySelectedJoint(selectedIndex);

            Renderer[] rendererList = articulationChain[selectedIndex].transform.GetChild(0).GetComponentsInChildren<Renderer>();
            foreach (var mesh in rendererList)
            {
                MaterialExtensions.SetMaterialColor(mesh.material, highLightColor);
            }
        }

        void DisplaySelectedJoint(int selectedIndex)
        {
            if (selectedIndex < 0 || selectedIndex >= articulationChain.Length)
                return;

            selectedJoint = articulationChain[selectedIndex].name + " (" + selectedIndex + ")";
        }

       private void UpdateDirection(int jointIndex)
{
    if (jointIndex < 0 || jointIndex >= articulationChain.Length)
        return;

    float move = Input.GetAxis("Vertical");
    var currentJoint = articulationChain[jointIndex];

    JointControl current = currentJoint.GetComponent<JointControl>();
    if (current == null)
    {
        Debug.LogWarning($"No JointControl script found on {currentJoint.name} (index {jointIndex}). Skipping.");
        return;
    }

    // Reset previous direction if changed
    if (previousIndex != jointIndex)
    {
        var previous = articulationChain[previousIndex].GetComponent<JointControl>();
        if (previous != null)
            previous.direction = RotationDirection.None;

        previousIndex = jointIndex;
    }

    if (current.controltype != control)
        UpdateControlType(current);

    if (move > 0)
        current.direction = RotationDirection.Positive;
    else if (move < 0)
        current.direction = RotationDirection.Negative;
    else
        current.direction = RotationDirection.None;

    Debug.Log($"[UpdateDirection] {currentJoint.name} | Move: {move} | Direction: {current.direction}");
}

        private void StoreJointColors(int index)
        {
            Renderer[] renderers = articulationChain[index].transform.GetChild(0).GetComponentsInChildren<Renderer>();
            prevColor = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                prevColor[i] = MaterialExtensions.GetMaterialColor(renderers[i]);
            }
        }

        private void ResetJointColors(int index)
        {
            Renderer[] renderers = articulationChain[index].transform.GetChild(0).GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                MaterialExtensions.SetMaterialColor(renderers[i].material, prevColor[i]);
            }
        }

        public void UpdateControlType(JointControl joint)
        {
            joint.controltype = control;
            joint.speed = speed;
            joint.torque = torque;
            joint.acceleration = acceleration;

            var drive = joint.joint.xDrive;
            drive.stiffness = stiffness;
            drive.damping = damping;
            drive.forceLimit = forceLimit;
            joint.joint.xDrive = drive;
        }

        public void OnGUI()
        {
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            GUI.Label(new Rect(Screen.width / 2 - 200, 10, 400, 20), "Press left/right arrow keys to select a robot joint.", centeredStyle);
            GUI.Label(new Rect(Screen.width / 2 - 200, 30, 400, 20), "Press up/down arrow keys to move " + selectedJoint + ".", centeredStyle);
        }
    }
}
