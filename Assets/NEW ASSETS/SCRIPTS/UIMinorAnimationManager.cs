using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMinorAnimationManager : MonoBehaviour
{
    [System.Serializable]
    public class ObjectsAnimationProperties
    {
        [SerializeField] public string ObjectType;              // Type of the object
        [SerializeField] public RectTransform[] moveableObjects; // Array of UI objects to be animated
        [SerializeField] public Vector2 moveDirection;          // Direction of movement
        [SerializeField] public Vector2 movementXOffset;        // Offset range on the X-axis for movement limits
        [SerializeField] public Vector2 movementYOffset;        // Offset range on the Y-axis for movement limits
        [SerializeField] public float moveSpeed;                // Speed of movement
        [SerializeField] public float smoothing;                // Smoothing factor for movement

        public Vector2[] initialPos;                             // Initial positions of the UI objects
    }

    [SerializeField] private ObjectsAnimationProperties[] objectsAnimationProperties; // Array of animation properties

    private Vector2 randomDir = Vector2.zero; // Random direction for random movement

    private void Start()
    {
        // Initialize initialPos array for each ObjectsAnimationProperties
        for (int i = 0; i < objectsAnimationProperties.Length; i++)
        {
            objectsAnimationProperties[i].initialPos = new Vector2[objectsAnimationProperties[i].moveableObjects.Length];

            // Store the initial positions of moveableObjects
            for (int j = 0; j < objectsAnimationProperties[i].moveableObjects.Length; j++)
            {
                objectsAnimationProperties[i].initialPos[j] = objectsAnimationProperties[i].moveableObjects[j].anchoredPosition;
            }
        }

        // Generate a random direction vector for initial random movement
        randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }

    private void Update()
    {
        MoveObjects();
    }

    private void MoveObjects()
    {
        // Iterate through each ObjectsAnimationProperties
        for (int i = 0; i < objectsAnimationProperties.Length; i++)
        {
            // Check if both x and y components of moveDirection are non-zero
            bool doRandomizing = (objectsAnimationProperties[i].moveDirection.x != 0 && objectsAnimationProperties[i].moveDirection.y != 0);

            // Define the movement vector
            Vector2 movement;

            // If randomizing, generate a random movement vector within a range
            if (doRandomizing)
            {
                movement = new Vector2(randomDir.x, randomDir.y).normalized * objectsAnimationProperties[i].moveSpeed * Time.deltaTime;
            }
            // If not randomizing, use the specified moveDirection
            else
            {
                movement = objectsAnimationProperties[i].moveDirection * objectsAnimationProperties[i].moveSpeed * Time.deltaTime;
            }

            // Iterate through each moveable object in the current ObjectsAnimationProperties
            for (int j = 0; j < objectsAnimationProperties[i].moveableObjects.Length; j++)
            {
                // Calculate the minimum and maximum positions based on anchoredPosition
                float minPosX = objectsAnimationProperties[i].initialPos[j].x - objectsAnimationProperties[i].movementXOffset.x;
                float maxPosX = objectsAnimationProperties[i].initialPos[j].x + objectsAnimationProperties[i].movementXOffset.y;

                float minPosY = objectsAnimationProperties[i].initialPos[j].y - objectsAnimationProperties[i].movementYOffset.x;
                float maxPosY = objectsAnimationProperties[i].initialPos[j].y + objectsAnimationProperties[i].movementYOffset.y;

                Vector2 newPosition = objectsAnimationProperties[i].moveableObjects[j].anchoredPosition + movement;

                // Check if the new position is within the specified offsets
                if (newPosition.x < minPosX || newPosition.x > maxPosX)
                {
                    // Reverse the direction when reaching the offset
                    if (doRandomizing)
                    {
                        // If randomizing, generate a new random direction
                        randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    }
                    else
                    {
                        // If not randomizing, reverse the moveDirection on the X-axis
                        objectsAnimationProperties[i].moveDirection.x *= -1;
                    }
                }

                if (newPosition.y < minPosY || newPosition.y > maxPosY)
                {
                    // Reverse the direction when reaching the offset
                    if (doRandomizing)
                    {
                        // If randomizing, generate a new random direction
                        randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    }
                    else
                    {
                        // If not randomizing, reverse the moveDirection on the Y-axis
                        objectsAnimationProperties[i].moveDirection.y *= -1;
                    }
                }

                // Smoothly interpolate to the new position
                objectsAnimationProperties[i].moveableObjects[j].anchoredPosition = Vector2.Lerp(objectsAnimationProperties[i].moveableObjects[j].anchoredPosition, newPosition, objectsAnimationProperties[i].smoothing);
            }
        }
    }
}