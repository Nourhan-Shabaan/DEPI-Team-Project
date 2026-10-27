using Freelance_Manager.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Freelance_Manager.Models
{

    public class Payment
    {
        private decimal _amount;
        private Payment() { }

        public Payment(int projectId, decimal amount, DateTime date)
        {
            ProjectId = projectId;
            Amount = amount;
            Date = date;
            Status = PaymentStatus.Pending;
        }

        public int Id { get; private set; }
        public decimal Amount
        {
            get => _amount;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Payment amount must be greater than zero.");
                _amount = value;
            }
        }

        public DateTime Date { get; set; }
        public PaymentStatus Status { get; set; }

        #region Payment -> Project

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!; 
        #endregion
    }
}
