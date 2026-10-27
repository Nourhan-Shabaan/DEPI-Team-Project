using Freelance_Manager.Data;
using Freelance_Manager.Enums;
using TaskStatusEnum = Freelance_Manager.Enums.MyTaskStatus;

namespace Freelance_Manager.Services
{
    public class DashboardService
    {
        private readonly AppDbContext dbcontext;

        public DashboardService(AppDbContext db)
        {
            dbcontext = db;
        }
       
        //client 
        public int GetTotalClients() => dbcontext.Clients.Count();

        // Projects
        public int GetTotalProjects() => dbcontext.Projects.Count();

        public int GetActiveProjects() => CountProjects(ProjectStatus.InProgress);
        public int GetCompletedProjects() => CountProjects(ProjectStatus.Completed);
        public int GetPendingProjects() => CountProjects(ProjectStatus.Pending);

        //Tasks 
        public int GetTotalTasks() => dbcontext.Tasks.Count();
        public int GetCompletedTasks() =>
            dbcontext.Tasks.Count(t => t.Status == TaskStatusEnum.Done);

        // Money 
        public decimal GetTotalEarnings() => SumPayments(PaymentStatus.Paid);
        public decimal GetPendingPayments() => SumPayments(PaymentStatus.Pending);

        // ---------- Helpers ----------
        private int CountProjects(ProjectStatus status) =>
            dbcontext.Projects.Count(p => p.Status == status);

        private decimal SumPayments(PaymentStatus status) =>
            dbcontext.Payments.Where(p => p.Status == status)
                        .Sum(p => p.Amount);
    }
}