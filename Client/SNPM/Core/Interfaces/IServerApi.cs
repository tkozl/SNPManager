namespace SNPM.Core.Interfaces
{
    public interface IServerApi
    {
        public bool AttemptLogin(IAccount account, string serverAddr); 
    }
}
