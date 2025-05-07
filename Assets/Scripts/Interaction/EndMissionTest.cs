namespace Ineraction
{
    public class EndMissionTest : Interactable
    {
        public override void Interact(Interactor actor = null)
        {
            MissionManager.Instance.MissionEnd();
        }
    }
}