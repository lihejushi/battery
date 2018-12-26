namespace XT.MVC.Core.Infrastructure
{
    /// <summary>
    /// 任务接口
    /// </summary>
    public interface IStartupTask 
    {
        void Execute();

        int Order { get; }
    }
}
