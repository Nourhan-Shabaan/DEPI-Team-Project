using System;
using System.Collections.Generic;
using System.Text;


using Freelance_Manager.Enums;

namespace Freelance_Manager.Models
{
    public class Project
    {
        private string _title = string.Empty;
        private decimal _budget;
        private Project() { }

        public Project(string title, string description, int clientId,
                       decimal budget, DateTime startDate, DateTime deadline)
        {
            Title = title;
            Description = description;
            ClientId = clientId;
            Budget = budget;
            SetDates(startDate, deadline);
            Status = ProjectStatus.Pending;
        }

        public int Id { get; private set; }

        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Project title cannot be empty.");
                _title = value.Trim();
            }
        }

        public string Description { get; set; } = string.Empty;

        public decimal Budget
        {
            get => _budget;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Budget must be greater than zero.");
                _budget = value;
            }
        }

        public DateTime StartDate { get; private set; }
        public DateTime Deadline { get; private set; }

        public ProjectStatus Status { get; set; }


        #region project , client
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
        #endregion

        #region  Project -> Tasks
        public List<TaskItem> Tasks { get; private set; } = new List<TaskItem>();
        #endregion

        #region  Project -> Payments
        public List<Payment> Payments { get; private set; } = new List<Payment>();
        #endregion

        #region validate date
        public void SetDates(DateTime startDate, DateTime deadline)
        {
            if (deadline < startDate)
                throw new ArgumentException("Deadline cannot be before the start date.");
            StartDate = startDate;
            Deadline = deadline;
        } 
        #endregion
    }
}
