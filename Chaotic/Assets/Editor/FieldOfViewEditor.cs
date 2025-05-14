using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Enemy))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        Enemy enemy = (Enemy)target;

        // Set the color to white for the field of view arc
        Handles.color = Color.white;
        
        // Draw the wireframe circle representing the field of view radius
        Handles.DrawWireArc(enemy.transform.position, Vector3.up, Vector3.forward, 360, enemy.radius);

        // Calculate the two points for the field of view's angle
        Vector3 viewAngle01 = DirectionFromAngle(enemy.transform.eulerAngles.y, -enemy.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(enemy.transform.eulerAngles.y, enemy.angle / 2);

        // Set the color to yellow for the lines representing the view angles
        Handles.color = Color.yellow;
        Handles.DrawLine(enemy.transform.position, enemy.transform.position + viewAngle01 * enemy.radius);
        Handles.DrawLine(enemy.transform.position, enemy.transform.position + viewAngle02 * enemy.radius);

        // If the enemy can see the player, draw a green line to the player's position
        if (enemy.canSeePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(enemy.transform.position, enemy.playerRef.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        // Convert the angle into radians and calculate the direction vector
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
