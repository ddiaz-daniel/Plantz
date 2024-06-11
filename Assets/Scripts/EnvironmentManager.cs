using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    // image for skybox

    public Material skyboxImage;
    //speed of rotation
    public float speed = 0.05f;
    public float sunSpeed = 0.05f;
    public float solarAngle = 0.0f;

    //spotlight
    public Light spotLight;
    //point light
    public Light pointLight;

    void FixedUpdate()
    {
        // Rotate the skybox image
        skyboxImage.SetFloat("_Rotation", Time.time * speed);
        //rotate the pointlight around the y axis with a speed of 0.1 and a radius of 5
        if (pointLight.transform.position.y < 0)
        {
            sunSpeed = 0.05f;
        }
        //rotate the pointlight around the y axis with a speed of 0.1 and a radius of 5
        else
        {
            sunSpeed = 0.01f;
        }
        solarAngle += sunSpeed * 0.05f;

        pointLight.transform.position = new Vector3(70 * Mathf.Sin(solarAngle), 40 * Mathf.Cos(solarAngle), 0);


        //when the spotlight is at the negative x axis, make the spotlight angle increase by 0.2 to 51
        if (pointLight.transform.position.y < 0)
        {
            if (spotLight.spotAngle < 50)
                spotLight.spotAngle += 1f;
        }
        //when the spotlight is at the positive x axis, make the spotlight angle decrease by 1 to 1
        else
        {
            if (spotLight.spotAngle > 1)
                spotLight.spotAngle -= 1f;
        }
    }


}