namespace LendasDoQuintal.Core
{
    public interface IInteractable
    {
        string Prompt { get; }
        void Interact();
    }
}
