using UnityEngine;

namespace HeroesFlight.System.Input.Model
{
    public class InputVectorValue
    {
        public InputVectorValue(Vector2 inputVector)
        {
            InputValue = inputVector;
        }
        public Vector2 InputValue { get; }
    }
}