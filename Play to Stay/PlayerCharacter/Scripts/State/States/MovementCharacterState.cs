namespace SpiritGarden.Character
{
    public class MovementCharacterState : CharacterState
    {
        private CMF.CharacterInput characterInput;

        public override void OnEnter()
        {
            characterInput.activated = true;
        }

        public override void OnExit()
        {
            characterInput.activated = false;
        }

        public override void OnUpdate()
        {

        }

        public MovementCharacterState(CharacterReference characterReference)
        {
            this.characterInput = characterReference.CharacterControls;
        }
    }
}