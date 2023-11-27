using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlight.System.Environment.Parallax
{
    public class ParallaxController : MonoBehaviour
    {
        [SerializeField] Transform[] layers;     
        [SerializeField] float smoothing = 1f;        

        [SerializeField] float[] parallaxScales;     
        [SerializeField] Transform cam;              
        private Vector3 previousCamPosition; 

        void Start()
        {
            cam = Camera.main.transform;
            previousCamPosition = cam.position;
        }

        void Update()
        {
            for(int i = 0; i < layers.Length; i++)
            {
                float parallaxX = (previousCamPosition.x - cam.position.x) * parallaxScales[i];
                float parallaxY = (previousCamPosition.y - cam.position.y) * parallaxScales[i];

                float backgroundTargetPosX = layers[i].position.x + parallaxX;
                float backgroundTargetPosY = layers[i].position.y + parallaxY;

                Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgroundTargetPosY, layers[i].position.z);

                layers[i].position = Vector3.Lerp(layers[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
            }

            previousCamPosition = cam.position;
        }
    }
}