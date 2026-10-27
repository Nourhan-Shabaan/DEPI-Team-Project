using Freelance_Manager.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Freelance_Manager.Models
{
    public class TaskItem
    {
        private string _title = string.Empty;
        private TaskItem() { }
        public TaskItem(string title, string description, DateTime dueDate,
                        TaskPriority priority, int projectId)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            ProjectId = projectId;
            Status = MyTaskStatus.ToDo;
        }

        public int Id { get; private set; }

        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Task title cannot be empty.");
                _title = value.Trim();
            }
        }

        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public MyTaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }

        #region Task -> Project
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!; 
        #endregion

    }
}