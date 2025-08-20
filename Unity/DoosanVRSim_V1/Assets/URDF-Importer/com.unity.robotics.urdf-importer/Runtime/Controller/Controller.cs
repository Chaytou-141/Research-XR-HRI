using System;
using Unity.Robotics;
using UnityEngine;

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
        public float stiffness;
        public float damping;
        public float forceLimit;
        public float speed = 5f;
        public float torque = 100f;
        public float acceleration = 5f;

        [Tooltip("Color to highlight the currently selected joint")]
        public Color highLightColor = new Color(1.0f, 0, 0, 1.0f);

        void Start()
        {
            previousIndex = selectedIndex = 1;
            this.gameObject.AddComponent<FKRobot>();
            articulationChain = this.GetComponentsInChildren<ArticulationBody>();
            int defDyanmicVal = 10;

            foreach (ArticulationBody joint in articulationChain)
            {
                joint.gameObject.AddComponent<JointControl>();
                joint.jointFriction = defDyanmicVal;
                joint.angularDamping = defDyanmicVal;
                ArticulationDrive currentDrive = joint.xDrive;
                currentDrive.forceLimit = forceLimit;
                joint.xDrive = currentDrive;
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
        {
            bool SelectionInput1 = Input.GetKeyDown("right");
            bool SelectionInput2 = Input.GetKeyDown("left");

            SetSelectedJointIndex(selectedIndex);
            UpdateDirection(selectedIndex);

            if (SelectionInput2)
            {
                SetSelectedJointIndex(selectedIndex - 1);
                Highlight(selectedIndex);
            }
            else if (SelectionInput1)
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

            Renderer[] rendererList = articulationChain[selectedIndex].GetComponentsInChildren<Renderer>();
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

            float moveDirection = Input.GetAxis("Vertical");
            JointControl current = articulationChain[jointIndex].GetComponent<JointControl>();

            if (previousIndex != jointIndex)
            {
                JointControl previous = articulationChain[previousIndex].GetComponent<JointControl>();
                previous.direction = RotationDirection.None;
                previousIndex = jointIndex;
            }

            if (current.controltype != control)
            {
                UpdateControlType(current);
            }

            if (moveDirection > 0)
            {
                current.direction = RotationDirection.Positive;
            }
            else if (moveDirection < 0)
            {
                current.direction = RotationDirection.Negative;
            }
            else
            {
                current.direction = RotationDirection.None;
            }
        }

        private void StoreJointColors(int index)
        {
            Renderer[] materialLists = articulationChain[index].GetComponentsInChildren<Renderer>();
            prevColor = new Color[materialLists.Length];

            for (int counter = 0; counter < materialLists.Length; counter++)
            {
                prevColor[counter] = MaterialExtensions.GetMaterialColor(materialLists[counter]);
            }
        }

        private void ResetJointColors(int index)
        {
            Renderer[] previousRendererList = articulationChain[index].GetComponentsInChildren<Renderer>();

            for (int counter = 0; counter < previousRendererList.Length; counter++)
            {
                MaterialExtensions.SetMaterialColor(previousRendererList[counter].material, prevColor[counter]);
            }
        }

        public void UpdateControlType(JointControl joint)
        {
            joint.controltype = control;

            if (control == ControlType.PositionControl)
            {
                ArticulationDrive drive = joint.joint.xDrive;
                drive.stiffness = stiffness;
                drive.damping = damping;
                joint.joint.xDrive = drive;
            }
        }

       public void OnGUI()
{
    GUIStyle backgroundStyle = new GUIStyle(GUI.skin.box);
    backgroundStyle.normal.background = Texture2D.whiteTexture; // simple white background
    backgroundStyle.normal.textColor = Color.black;

    GUIStyle textStyle = new GUIStyle(GUI.skin.label);
    textStyle.alignment = TextAnchor.MiddleLeft;
    textStyle.fontSize = 16;
    textStyle.normal.textColor = Color.black;

    int panelWidth = 400;
    int panelHeight = 80;
    int padding = 10;

    // Position in the Top Right
    Rect panelRect = new Rect(Screen.width - panelWidth - padding, padding, panelWidth, panelHeight);

    GUI.Box(panelRect, GUIContent.none, backgroundStyle);

    float lineHeight = 25f;
    GUI.Label(new Rect(panelRect.x + 10, panelRect.y + 10, panelWidth - 20, lineHeight), "Press ←/→ to select a joint.", textStyle);
    GUI.Label(new Rect(panelRect.x + 10, panelRect.y + 35, panelWidth - 20, lineHeight), "Press ↑/↓ to move " + selectedJoint + ".", textStyle);
}


    }
}
