using System.Collections.Generic;
using XT.MVC.Core.Domain.Tasks;

namespace XT.MVC.Core.Services.Tasks
{
    public partial interface IScheduleTaskService
    {
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="task"></param>
        void DeleteTask(ScheduleTask task);

        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="taskId">任务id</param>
        ScheduleTask GetTaskById(int taskId);

        /// <summary>
        /// Gets a task by its type
        /// </summary>
        /// <param name="type">Task type</param>
        /// <returns>Task</returns>
        ScheduleTask GetTaskByType(string type);

        /// <summary>
        /// Gets all tasks
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Tasks</returns>
        IList<ScheduleTask> GetAllTasks(bool showHidden = false);

        /// <summary>
        /// Inserts a task
        /// </summary>
        /// <param name="task">Task</param>
        void InsertTask(ScheduleTask task);

        /// <summary>
        /// Updates the task
        /// </summary>
        /// <param name="task">Task</param>
        void UpdateTask(ScheduleTask task);
    }
}
