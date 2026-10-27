using Freelance_Manager.Data;
using Freelance_Manager.Enums;
using Freelance_Manager.Models;
using Microsoft.EntityFrameworkCore;
using TaskStatusEnum = Freelance_Manager.Enums.MyTaskStatus;

namespace Freelance_Manager.Services
{
    public class TaskService
    {
        private readonly AppDbContext dbcontext;

        public TaskService(AppDbContext db)
        {
            dbcontext = db;
        }

        public TaskItem AddTask(string title, string description, DateTime dueDate,
                                TaskPriority priority, int projectId)
        {
            EnsureProjectExists(projectId);
            var task = new TaskItem(title, description, dueDate, priority, projectId);

            dbcontext.Tasks.Add(task);
            dbcontext.SaveChanges();
            return task;
        }

        public List<TaskItem> GetAllTasks()
        {
            return TasksWithProject().AsNoTracking().OrderBy(t => t.Id).ToList();
        }
        public TaskItem? GetTaskById(int id)
        {
            return TasksWithProject().FirstOrDefault(t => t.Id == id);
        }

        public List<TaskItem> GetTasksByProjectId(int projectId)
        {
            EnsureProjectExists(projectId);

            return TasksWithProject().AsNoTracking()
                .Where(t => t.ProjectId == projectId)
                .OrderBy(t => t.DueDate)
                .ToList();
        }

        public List<TaskItem> SearchTasksByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Search text cannot be empty.");

            return TasksWithProject().AsNoTracking()
                .Where(t => t.Title.Contains(title.Trim()))
                .OrderBy(t => t.Title)
                .ToList();
        }

        public List<TaskItem> SearchTasksByProjectTitle(string projectTitle)
        {
            if (string.IsNullOrWhiteSpace(projectTitle))
                throw new ArgumentException("Search text cannot be empty.");

            return TasksWithProject().AsNoTracking()
                .Where(t => t.Project.Title.Contains(projectTitle.Trim()))
                .OrderBy(t => t.Project.Title)
                .ToList();
        }

        public List<TaskItem> FilterTasksByStatus(TaskStatusEnum status)
        {
            return TasksWithProject().AsNoTracking()
                .Where(t => t.Status == status)
                .OrderBy(t => t.DueDate)
                .ToList();
        }

        public List<TaskItem> FilterTasksByPriority(TaskPriority priority)
        {
            return TasksWithProject().AsNoTracking()
                .Where(t => t.Priority == priority)
                .OrderBy(t => t.DueDate)
                .ToList();
        }

        public TaskItem UpdateTask(int id, string title, string description, DateTime dueDate,
                                   TaskPriority priority, int projectId)
        {
            var task = GetExistingTask(id);
            EnsureProjectExists(projectId);

            try
            {
                task.Title = title;          
                task.Description = description;
                task.DueDate = dueDate;
                task.Priority = priority;
                task.ProjectId = projectId;

                dbcontext.SaveChanges();
                return task;
            }
            catch
            {
                dbcontext.Entry(task).Reload();
                throw;
            }
        }

        public TaskItem ChangeTaskStatus(int id, TaskStatusEnum status)
        {
            var task = GetExistingTask(id);
            task.Status = status;
            dbcontext.SaveChanges();
            return task;
        }

        public TaskItem ChangeTaskPriority(int id, TaskPriority priority)
        {
            var task = GetExistingTask(id);
            task.Priority = priority;
            dbcontext.SaveChanges();
            return task;
        }


        public void DeleteTask(int id)
        {
            var task = GetExistingTask(id);
            dbcontext.Tasks.Remove(task);
            dbcontext.SaveChanges();
        }

        // ---------- Helpers ----------
        private IQueryable<TaskItem> TasksWithProject()
        {
            return dbcontext.Tasks.Include(t => t.Project);
        }

        private TaskItem GetExistingTask(int id)
        {
            return dbcontext.Tasks.Find(id)
                ?? throw new KeyNotFoundException($"Task with ID {id} was not found.");
        }

        private void EnsureProjectExists(int projectId)
        {
            if (!dbcontext.Projects.Any(p => p.Id == projectId))
                throw new KeyNotFoundException($"Project with ID {projectId} was not found.");
        }
    }
}