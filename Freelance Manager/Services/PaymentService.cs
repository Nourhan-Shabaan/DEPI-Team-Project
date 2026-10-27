using Freelance_Manager.Data;
using Freelance_Manager.Enums;
using Freelance_Manager.Models;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Manager.Services
{
    public class PaymentService
    {
        private readonly AppDbContext dbcontxt;

        public PaymentService(AppDbContext db)
        {
            dbcontxt = db;
        }

       
        public Payment AddPayment(int projectId, decimal amount, DateTime date)
        {
            EnsureProjectExists(projectId);

            var payment = new Payment(projectId, amount, date); 
            dbcontxt.Payments.Add(payment);
            dbcontxt.SaveChanges();
            return payment;
        }

        public List<Payment> GetAllPayments()
        {
            return dbcontxt.Payments.Include(p => p.Project).AsNoTracking().ToList();
        }

        public Payment? GetPaymentById(int id)
        {
            return dbcontxt.Payments.Include(p => p.Project).FirstOrDefault(p => p.Id == id);
        }

        public List<Payment> GetPaidPayments()
        {
            return GetPaymentsByStatus(PaymentStatus.Paid);
        }

        public List<Payment> GetPendingPayments()
        {
            return GetPaymentsByStatus(PaymentStatus.Pending);
        }

        public Payment UpdatePayment(int id, int projectId, decimal amount, DateTime date)
        {
            var payment = GetExistingPayment(id);
            EnsureProjectExists(projectId);

            payment.Amount = amount; // لو غلط بيرمي exception قبل أي تعديل تاني
            payment.Date = date;
            payment.ProjectId = projectId;

            dbcontxt.SaveChanges();
            return payment;
        }

        public Payment ChangePaymentStatus(int id, PaymentStatus status)
        {
            var payment = GetExistingPayment(id);
            payment.Status = status;
            dbcontxt.SaveChanges();
            return payment;
        }

        public void DeletePayment(int id)
        {
            dbcontxt.Payments.Remove(GetExistingPayment(id));
            dbcontxt.SaveChanges();
        }



        // TotalEarnings = sum of Paid Payments
        public decimal GetTotalEarnings()
        {
            return dbcontxt.Payments
                .Where(p => p.Status == PaymentStatus.Paid)
                .Sum(p =>p.Amount) ;
        }

        //RemainingPayments = Budget - (sum of Paid Payments)
        public decimal CalculateRemainingPayments(int projectId)
        {
            var project = dbcontxt.Projects.Find(projectId)
                ?? throw new KeyNotFoundException($"Project with ID {projectId} was not found.");

            decimal paid = dbcontxt.Payments
                .Where(p => p.ProjectId == projectId && p.Status == PaymentStatus.Paid)
                .Sum(p =>p.Amount);

            return project.Budget - paid;
        }

        // ---------- Helpers ----------
        private List<Payment> GetPaymentsByStatus(PaymentStatus status)
        {
            return dbcontxt.Payments.Include(p => p.Project).AsNoTracking()
                .Where(p => p.Status == status)
                .OrderBy(p => p.Date)
                .ToList();
        }

        private Payment GetExistingPayment(int id)
        {
            return dbcontxt.Payments.Find(id)
                ?? throw new KeyNotFoundException($"Payment with ID {id} was not found.");
        }

        private void EnsureProjectExists(int projectId)
        {
            if (!dbcontxt.Projects.Any(p => p.Id == projectId))
                throw new KeyNotFoundException($"Project with ID {projectId} was not found.");
        }
    }
}