namespace FSM
{
    public interface ITransition
    {
        string Name { get; }
        IState FromState { get; }
        IState ToState { get; }
        bool OnCheck();
        bool OnCompleteCallBack();
    }
}