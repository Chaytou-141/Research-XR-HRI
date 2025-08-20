using UnityEngine;

public class JointKeyboardController : MonoBehaviour
{
    public ArticulationBody jointToMove;
    public float speed = 30f;
    private float targetAngle = 0f;

    void Start()
    {
        if (jointToMove != null)
        {
            var drive = jointToMove.xDrive;
            drive.stiffness = 10000f;
            drive.damping = 100f;
            drive.forceLimit = 1000f; // Can be float.MaxValue
            jointToMove.xDrive = drive;
            Debug.Log("✅ Drive parameters set up.");
        }
    }

    void Update()
    {
        if (jointToMove == null)
        {
            Debug.LogWarning("❌ No joint assigned to JointKeyboardController!");
            return;
        }

        if (Input.GetKey(KeyCode.W))
        {
            targetAngle += speed * Time.deltaTime;
            Debug.Log("⬆ Pressed W: Increasing target angle to " + targetAngle);
        }

        if (Input.GetKey(KeyCode.S))
        {
            targetAngle -= speed * Time.deltaTime;
            Debug.Log("⬇ Pressed S: Decreasing target angle to " + targetAngle);
        }

        // Clamp angle to avoid unrealistic values
        targetAngle = Mathf.Clamp(targetAngle, -150f, 150f);

        var drive = jointToMove.xDrive;
        drive.target = targetAngle;
        jointToMove.xDrive = drive;
    }
}
