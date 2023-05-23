namespace FSM
{
    public interface ITransition
    {
        string Name { get; }
        string ToStateName { get; }
        bool OnCheck(IState fromState);
    }
}