using System;
using System.Collections.Generic;
using System.Text;

namespace Freelance_Manager.Models
{
    public class Client
    {
        private string _name = string.Empty;
        private string _email = string.Empty;
        private Client() { }
        public Client(string name, string email, string phone = "", string company = "")
        {
            Name = name;
            Email = email;
            Phone = phone;
            Company = company;
        }
        public int Id { get; private set; }
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Client name cannot be empty.");
                //remove space
                _name = value.Trim();
            }
        }
        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Client email cannot be empty.");
                if (!value.Contains('@') || value.StartsWith('@') || value.EndsWith('@') )
                    throw new ArgumentException("Client email is not valid.");
                _email = value.Trim();
            }
        }
        public string Phone { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;


        #region Client -> Projects
        public List<Project> Projects { get; private set; } = new List<Project>(); 
        #endregion
    }
}
