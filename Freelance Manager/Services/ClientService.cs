using Freelance_Manager.Data;
using Freelance_Manager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freelance_Manager.Services
{

    public class ClientService
    {
        private readonly AppDbContext dbcontext;

        public ClientService(AppDbContext db)
        {
            dbcontext = db;
        }

        public Client AddClient(string name, string email, string phone = "", string company = "")
        {
            var client = new Client(name, email, phone, company);

            if (dbcontext.Clients.Any(c => c.Email == client.Email))
                throw new InvalidOperationException("A client with this email already exists.");

            dbcontext.Clients.Add(client);
            dbcontext.SaveChanges();
            return client; 
        }

        public List<Client> GetAllClients()
        {
            return dbcontext.Clients.AsNoTracking().OrderBy(c => c.Id).ToList();
        }

        
        public Client? GetClientById(int id)
        {
            return dbcontext.Clients.Find(id);
        }

        
        public List<Client> SearchClientsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Search text cannot be empty.");

            return dbcontext.Clients
                .AsNoTracking() //stop change tracker 
                .Where(c => c.Name.Contains(name.Trim()))
                .OrderBy(c => c.Name)
                .ToList();
        }

        public Client UpdateClient(int id, string name, string email, string phone, string company)
        {
            var client = GetExistingClient(id);

            if (dbcontext.Clients.Any(c => c.Id != id && c.Email == email.Trim()))
                throw new InvalidOperationException("Another client already uses this email.");

            try
            {
                client.Name = name;     
                client.Email = email;
                client.Phone = phone;
                client.Company = company;

                dbcontext.SaveChanges();
                return client;
            }
            catch
            {

                dbcontext.Entry(client).Reload();
                throw;
            }
        }
        public void DeleteClient(int id)
        {
            var client = GetExistingClient(id);
            //Note -> OnDelete(DeleteBehavior.Restrict)
            if (dbcontext.Projects.Any(p => p.ClientId == id))
                throw new InvalidOperationException(
                    "Cannot delete a client who has projects. Delete or reassign the projects first.");

            dbcontext.Clients.Remove(client);
            dbcontext.SaveChanges();
        }

        // Helper method 
        private Client GetExistingClient(int id)
        {
            return dbcontext.Clients.Find(id)
                ?? throw new KeyNotFoundException($"Client with ID {id} was not found.");
        }
    }
}
