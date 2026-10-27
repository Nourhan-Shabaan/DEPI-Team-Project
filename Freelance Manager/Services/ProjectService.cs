using Freelance_Manager.Data;
using Freelance_Manager.Enums;
using Freelance_Manager.Models;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Manager.Services
{
    public class ProjectService
    {
        private readonly AppDbContext dbcontext;

        public ProjectService(AppDbContext db)
        {
            dbcontext = db;
        }
        public Project AddProject(string title, string description, int clientId,
                                  decimal budget, DateTime startDate, DateTime deadline)
        {
            EnsureClientExists(clientId);
            var project = new Project(title, description, clientId, budget, startDate, deadline);

            dbcontext.Projects.Add(project);
            dbcontext.SaveChanges();
            return project;
        }

        public List<Project> GetAllProjects()
        {
            return ProjectsWithClient().AsNoTracking().OrderBy(p => p.Id).ToList();
        }

        //using fun that load relation daata
        public Project? GetProjectById(int id)
        {
            return ProjectsWithClient().FirstOrDefault(p => p.Id == id);
        }

        public List<Project> SearchProjectsByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Search text cannot be empty.");

            return ProjectsWithClient().AsNoTracking()
                .Where(p => p.Title.Contains(title.Trim()))
                .OrderBy(p => p.Title)
                .ToList();
        }

        public List<Project> GetProjectsByClientId(int clientId)
        {
            return ProjectsWithClient().AsNoTracking()
                .Where(p => p.ClientId == clientId)
                .OrderBy(p => p.Id)
                .ToList();
        }

        public List<Project> SearchProjectsByClientName(string clientName)
        {
            if (string.IsNullOrWhiteSpace(clientName))
                throw new ArgumentException("Search text cannot be empty.");

            return ProjectsWithClient().AsNoTracking()
                .Where(p => p.Client.Name.Contains(clientName.Trim()))
                .OrderBy(p => p.Client.Name)
                .ToList();
        }

        public List<Project> FilterProjectsByStatus(ProjectStatus status)
        {
            return ProjectsWithClient().AsNoTracking()
                .Where(p => p.Status == status)
                .OrderBy(p => p.Id)
                .ToList();
        }

       
        public Project UpdateProject(int id, string title, string description, int clientId,
                                     decimal budget, DateTime startDate, DateTime deadline)
        {
            var project = GetExistingProject(id);
            EnsureClientExists(clientId);

            decimal totalPayments = dbcontext.Payments
                .Where(p => p.ProjectId == id)
                .Sum(p =>p.Amount);

            if (budget < totalPayments)
                throw new InvalidOperationException(
                    $"Budget cannot be less than the total payments already added ({totalPayments}).");

            try
            {
                project.Title = title;           
                project.Description = description;
                project.Budget = budget;
                project.SetDates(startDate, deadline);
                project.ClientId = clientId;

                dbcontext.SaveChanges();
                return project;
            }
            catch
            {
                dbcontext.Entry(project).Reload();
                throw;
            }
        }

    
        public Project ChangeProjectStatus(int id, ProjectStatus status)
        {
            var project = GetExistingProject(id);
            project.Status = status;
            dbcontext.SaveChanges();
            return project;
        }

        
        public void DeleteProject(int id)
        {
            var project = GetExistingProject(id);

            if (dbcontext.Payments.Any(p => p.ProjectId == id && p.Status == PaymentStatus.Paid))
                throw new InvalidOperationException(
                    "Cannot delete a project that has paid payments.");

            //  المهام والدفعات المعلّقة بتتحذف معاه -> (Cascade)
            dbcontext.Projects.Remove(project);
            dbcontext.SaveChanges();
        }

        // ---------- Helpers ----------
        //Egar loading 
        private IQueryable<Project> ProjectsWithClient()
        {
            return dbcontext.Projects.Include(p => p.Client);
        }

        private Project GetExistingProject(int id)
        {
            return dbcontext.Projects.Find(id)
                ?? throw new KeyNotFoundException($"Project with ID {id} was not found.");
        }

        private void EnsureClientExists(int clientId)
        {
            if (!dbcontext.Clients.Any(c => c.Id == clientId))
                throw new KeyNotFoundException($"Client with ID {clientId} was not found.");
        }
    }
}