using UnityEngine;

public class TableAndRoomSetup : MonoBehaviour
{
    void Start()
    {
        // Create Floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.position = Vector3.zero;
        floor.transform.localScale = new Vector3(2, 1, 2);
        floor.name = "Floor";

        // Create Walls
        CreateWall(new Vector3(0, 2.5f, 10)); // Front
        CreateWall(new Vector3(0, 2.5f, -10)); // Back
        CreateWall(new Vector3(10, 2.5f, 0), true); // Right
        CreateWall(new Vector3(-10, 2.5f, 0), true); // Left

        // Create Table Top
        GameObject tableTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tableTop.transform.position = new Vector3(0, 1, 0);
        tableTop.transform.localScale = new Vector3(2, 0.1f, 2);
        tableTop.name = "TableTop";

        // Create Table Legs
        CreateTableLeg(new Vector3(0.9f, 0.5f, 0.9f));
        CreateTableLeg(new Vector3(-0.9f, 0.5f, 0.9f));
        CreateTableLeg(new Vector3(0.9f, 0.5f, -0.9f));
        CreateTableLeg(new Vector3(-0.9f, 0.5f, -0.9f));
    }

    void CreateWall(Vector3 position, bool rotate = false)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = position;
        wall.transform.localScale = rotate ? new Vector3(0.2f, 5, 20) : new Vector3(20, 5, 0.2f);
        wall.name = "Wall";
    }

    void CreateTableLeg(Vector3 position)
    {
        GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leg.transform.position = position;
        leg.transform.localScale = new Vector3(0.1f, 1, 0.1f);
        leg.name = "TableLeg";
    }
}
