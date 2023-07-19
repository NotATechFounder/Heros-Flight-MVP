using HeroesFlight.System.Input.Enum;

namespace HeroesFlight.System.Input.Model
{
    public class InputModel
    {
        public InputModel(InputType type, InputVectorValue value)
        {
            InputType = type;
            InputValue = value;
        }

        public InputType InputType { get; }
        public InputVectorValue InputValue { get; }
    }
}