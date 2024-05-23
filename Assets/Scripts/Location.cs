using UnityEngine;

public class Location : MonoBehaviour
{
    public float x;
    public float y;

    public Location(float x, float y)
    { 
        this.x = x;
        this.y = y;
    }

    public float CalcDistance(Location location)
    {
        // Calculate the distance using Pythagoras
        float deltaX = this.x - location.x;
        float deltaY = this.y - location.y;
        float distance = Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);

        return distance;
    }
}
