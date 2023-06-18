namespace Cmd
{
    public interface ICMDAction
    {
        public void Send(string siid, params string[] paras);

        public void Recv(string siid, string cmd);

        string FORMAT { get; }
    }
}