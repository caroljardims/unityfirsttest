using UnityEngine;

public class ParallaxController : MonoBehaviour
{

Transform cam; // Main camera
Vector3 camStartPos; // Start Position
float distance; // distance between the camera start position and current position

GameObject[] backgrounds;
Material[] mat;
float[] backSpeed;
float farBack;

[Range(0.01f,0.05f)]
public float parallaxSpeed;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;

        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        backgrounds = new GameObject[backCount];

        for (int i=0; i < backCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mat[i] = backgrounds[i].GetComponent<Renderer>().material;
        }
        BackSpeedCalculate(backCount);
    }

    void BackSpeedCalculate(int backCount)
    {
        for (int i=0; i<backCount; i++) // find the far background
        {
            if((backgrounds[i].transform.position.z - cam.position.z) > farBack)
            {
                farBack = backgrounds[i].transform.position.z - cam.position.z;
            }
        }

        for (int i=0; i<backCount; i++) // set the speed of backgrounds
        {
            backSpeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z)/farBack;
        }
    }

    private void LateUpdate()
    {
        distance = cam.position.x - camStartPos.x;
        transform.position = new Vector3(cam.position.x, transform.position.y, 0);

        for (int i=0; i<backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;
            mat[i].SetTextureOffset("_MainTex", new Vector2(distance, 0)*speed);
        }
    }
}
