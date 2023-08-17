using UnityEngine;

namespace HeroesFlight.System.Character
{
    public class CameraTargetController : MonoBehaviour
    {
        [SerializeField] Vector2 offsetBounds = new Vector2(2, 2);
        [SerializeField] float speed = 5f;
        CharacterControllerInterface controller;
        Transform parent;
        float sinTime;

        void Awake()
        {
            parent = transform.parent;
            controller = GetComponentInParent<CharacterControllerInterface>();
        }

        void Update()
        {
            var velocity = controller.GetVelocity();
            var resultVector = new Vector3(Mathf.Clamp(velocity.x, -offsetBounds.x, offsetBounds.x),
                Mathf.Clamp(velocity.y, -offsetBounds.y, offsetBounds.y));

            var desiredPosition = parent.position + resultVector;

            // if (transform.position != desiredPosition)
            // {
            //     sinTime += Time.deltaTime * speed;
            //     sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
            //     var t = Evaluate(sinTime);
            //     transform.position = Vector3.Lerp(transform.position, desiredPosition, t);
            // }
            transform.position = Vector3.Lerp(transform.position, desiredPosition, speed * Time.deltaTime);
        }


        float Evaluate(float x)
        {
            return 0.5f * Mathf.Sin(x - Mathf.PI / 2f) + 0.5f;
        }
    }
}