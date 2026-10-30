using System;
using System.Collections.Generic;
using System.Linq;
using Freelance_Manager.Data;
using Freelance_Manager.Enums;
using Freelance_Manager.Models;
using Freelance_Manager.Services;
using TaskStatusEnum = Freelance_Manager.Enums.MyTaskStatus;

namespace Freelance_Manager
{
    internal sealed class ConsoleUI
    {
        private readonly ClientService _clients;
        private readonly ProjectService _projects;
        private readonly TaskService _tasks;
        private readonly PaymentService _payments;
        private readonly DashboardService _dashboard;

        public ConsoleUI(AppDbContext db)
        {
            _clients = new ClientService(db);
            _projects = new ProjectService(db);
            _tasks = new TaskService(db);
            _payments = new PaymentService(db);
            _dashboard = new DashboardService(db);
        }

        public void Run()
        {
            Console.Title = "Freelance Manager";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Header("FREELANCE MANAGER", "Manage clients, projects, tasks, and payments", ConsoleColor.DarkRed);
                Menu("Main Menu", ConsoleColor.DarkRed,
                    "1) Dashboard", "2) Clients", "3) Projects",
                    "4) Tasks", "5) Payments", "0) Exit");

                switch (ReadChoice("Choose an option"))
                {
                    case "1": ShowDashboard(); Pause(); break;
                    case "2": ClientsMenu(); break;
                    case "3": ProjectsMenu(); break;
                    case "4": TasksMenu(); break;
                    case "5": PaymentsMenu(); break;
                    case "0": return;
                    default: Error("Invalid option."); Pause(); break;
                }
            }
        }

        private void ShowDashboard()
        {
            Header("Dashboard", theme: ConsoleColor.DarkCyan);
            WriteRow("Clients", _dashboard.GetTotalClients().ToString(), ConsoleColor.Cyan);
            WriteRow("Total projects", _dashboard.GetTotalProjects().ToString(), ConsoleColor.Yellow);
            WriteRow("Active projects", _dashboard.GetActiveProjects().ToString(), ConsoleColor.DarkYellow);
            WriteRow("Completed projects", _dashboard.GetCompletedProjects().ToString(), ConsoleColor.Green);
            WriteRow("Pending projects", _dashboard.GetPendingProjects().ToString(), ConsoleColor.Yellow);
            WriteRow("Total tasks", _dashboard.GetTotalTasks().ToString(), ConsoleColor.Magenta);
            WriteRow("Completed tasks", _dashboard.GetCompletedTasks().ToString(), ConsoleColor.Green);
            WriteRow("Earnings received", Money(_dashboard.GetTotalEarnings()), ConsoleColor.Green);
            WriteRow("Pending payments", Money(_dashboard.GetPendingPayments()), ConsoleColor.Yellow);
        }

        private void ClientsMenu()
        {
            while (true)
            {
                Header("Clients", theme: ConsoleColor.DarkCyan);
                Menu("Client Management", ConsoleColor.Cyan,
                    "1) List all", "2) Search by name", "3) Add client",
                    "4) Update client", "5) Delete client", "0) Back");

                switch (ReadChoice("Choose an option"))
                {
                    case "1": ShowClients(_clients.GetAllClients()); Pause(); break;
                    case "2": ShowClients(_clients.SearchClientsByName(ReadText("Name or part of name"))); Pause(); break;
                    case "3": AddClient(); Pause(); break;
                    case "4": UpdateClient(); Pause(); break;
                    case "5": DeleteClient(); Pause(); break;
                    case "0": return;
                    default: Error("Invalid option."); Pause(); break;
                }
            }
        }

        private void ProjectsMenu()
        {
            while (true)
            {
                Header("Projects", theme: ConsoleColor.DarkYellow);
                Menu("Project Management", ConsoleColor.Yellow,
                    "1) List all", "2) Search by title", "3) Add project",
                    "4) Change status", "5) Delete project", "0) Back");

                switch (ReadChoice("Choose an option"))
                {
                    case "1": ShowProjects(_projects.GetAllProjects()); Pause(); break;
                    case "2": ShowProjects(_projects.SearchProjectsByTitle(ReadText("Project title or part of title"))); Pause(); break;
                    case "3": AddProject(); Pause(); break;
                    case "4": ChangeProjectStatus(); Pause(); break;
                    case "5": DeleteProject(); Pause(); break;
                    case "0": return;
                    default: Error("Invalid option."); Pause(); break;
                }
            }
        }

        private void TasksMenu()
        {
            while (true)
            {
                Header("Tasks", theme: ConsoleColor.DarkGreen);
                Menu("Task Management", ConsoleColor.Green,
                    "1) List all", "2) List tasks by project", "3) Add task",
                    "4) Change status", "5) Change priority", "6) Delete task", "0) Back");

                switch (ReadChoice("Choose an option"))
                {
                    case "1": ShowTasks(_tasks.GetAllTasks()); Pause(); break;
                    case "2": ShowTasks(_tasks.GetTasksByProjectId(ReadInt("Project ID"))); Pause(); break;
                    case "3": AddTask(); Pause(); break;
                    case "4": ChangeTaskStatus(); Pause(); break;
                    case "5": ChangeTaskPriority(); Pause(); break;
                    case "6": DeleteTask(); Pause(); break;
                    case "0": return;
                    default: Error("Invalid option."); Pause(); break;
                }
            }
        }

        private void PaymentsMenu()
        {
            while (true)
            {
                Header("Payments", theme: ConsoleColor.DarkMagenta);
                Menu("Payment Management", ConsoleColor.Magenta,
                    "1) List all", "2) List paid", "3) List pending",
                    "4) Add payment", "5) Change status", "6) Delete payment", "0) Back");

                switch (ReadChoice("Choose an option"))
                {
                    case "1": ShowPayments(_payments.GetAllPayments()); Pause(); break;
                    case "2": ShowPayments(_payments.GetPaidPayments()); Pause(); break;
                    case "3": ShowPayments(_payments.GetPendingPayments()); Pause(); break;
                    case "4": AddPayment(); Pause(); break;
                    case "5": ChangePaymentStatus(); Pause(); break;
                    case "6": DeletePayment(); Pause(); break;
                    case "0": return;
                    default: Error("Invalid option."); Pause(); break;
                }
            }
        }

        private void AddClient()
        {
            _clients.AddClient(
                ReadText("Client name"),
                ReadText("Email"),
                ReadOptional("Phone"),
                ReadOptional("Company"));
            Success("Client added.");
        }

        private void UpdateClient()
        {
            int id = ReadInt("Client ID");
            _clients.UpdateClient(
                id,
                ReadText("Name"),
                ReadText("Email"),
                ReadOptional("Phone"),
                ReadOptional("Company"));
            Success("Client updated.");
        }

        private void DeleteClient()
        {
            int id = ReadInt("Client ID");
            if (Confirm("Delete this client?"))
            {
                _clients.DeleteClient(id);
                Success("Deleted.");
            }
        }

        private void AddProject()
        {
            ShowClients(_clients.GetAllClients());
            _projects.AddProject(
                ReadText("Project title"),
                ReadOptional("Description"),
                ReadInt("Client ID"),
                ReadDecimal("Budget"),
                ReadDate("Start date yyyy-MM-dd"),
                ReadDate("Deadline yyyy-MM-dd"));
            Success("Project added.");
        }

        private void ChangeProjectStatus()
        {
            int id = ReadInt("Project ID");
            var status = ReadEnum<ProjectStatus>("New status");
            _projects.ChangeProjectStatus(id, status);
            Success("Project status updated.");
        }

        private void DeleteProject()
        {
            int id = ReadInt("Project ID");
            if (Confirm("The project, its tasks, and pending payments will be deleted. Continue?"))
            {
                _projects.DeleteProject(id);
                Success("Deleted.");
            }
        }

        private void AddTask()
        {
            ShowProjects(_projects.GetAllProjects());
            _tasks.AddTask(
                ReadText("Task title"),
                ReadOptional("Description"),
                ReadDate("Due date yyyy-MM-dd"),
                ReadEnum<TaskPriority>("Priority"),
                ReadInt("Project ID"));
            Success("Task added.");
        }

        private void ChangeTaskStatus()
        {
            int id = ReadInt("Task ID");
            _tasks.ChangeTaskStatus(id, ReadEnum<TaskStatusEnum>("New status"));
            Success("Status updated.");
        }

        private void ChangeTaskPriority()
        {
            int id = ReadInt("Task ID");
            _tasks.ChangeTaskPriority(id, ReadEnum<TaskPriority>("New priority"));
            Success("Priority updated.");
        }

        private void DeleteTask()
        {
            int id = ReadInt("Task ID");
            if (Confirm("Delete this task?"))
            {
                _tasks.DeleteTask(id);
                Success("Deleted.");
            }
        }

        private void AddPayment()
        {
            ShowProjects(_projects.GetAllProjects());
            _payments.AddPayment(
                ReadInt("Project ID"),
                ReadDecimal("Amount"),
                ReadDate("Date yyyy-MM-dd"));
            Success("Payment added with pending status.");
        }

        private void ChangePaymentStatus()
        {
            int id = ReadInt("Payment ID");
            _payments.ChangePaymentStatus(id, ReadEnum<PaymentStatus>("New status"));
            Success("Status updated.");
        }

        private void DeletePayment()
        {
            int id = ReadInt("Payment ID");
            if (Confirm("Delete this payment?"))
            {
                _payments.DeletePayment(id);
                Success("Deleted.");
            }
        }

        private static void ShowClients(IEnumerable<Client> rows)
        {
            Header("Client List", theme: ConsoleColor.DarkCyan);
            foreach (var client in rows)
            {
                WriteRow(
                    $"#{client.Id} {client.Name}",
                    $"{client.Email} | {client.Phone} | {client.Company}",
                    ConsoleColor.Cyan);
            }
        }

        private static void ShowProjects(IEnumerable<Project> rows)
        {
            Header("Project List", theme: ConsoleColor.DarkYellow);
            foreach (var project in rows)
            {
                WriteRow(
                    $"#{project.Id} {project.Title}",
                    $"{project.Client?.Name} | {project.Status} | {Money(project.Budget)} | {project.Deadline:yyyy-MM-dd}",
                    ConsoleColor.Yellow);
            }
        }

        private static void ShowTasks(IEnumerable<TaskItem> rows)
        {
            Header("Task List", theme: ConsoleColor.DarkGreen);
            foreach (var task in rows)
            {
                WriteRow(
                    $"#{task.Id} {task.Title}",
                    $"{task.Project?.Title} | {task.Status} | {task.Priority} | {task.DueDate:yyyy-MM-dd}",
                    ConsoleColor.Green);
            }
        }

        private static void ShowPayments(IEnumerable<Payment> rows)
        {
            Header("Payment List", theme: ConsoleColor.DarkMagenta);
            foreach (var payment in rows)
            {
                WriteRow(
                    $"#{payment.Id} {payment.Project?.Title}",
                    $"{Money(payment.Amount)} | {payment.Status} | {payment.Date:yyyy-MM-dd}",
                    ConsoleColor.Magenta);
            }
        }

        private static T ReadEnum<T>(string prompt) where T : struct, Enum
        {
            var values = (T[])Enum.GetValues(typeof(T));

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{prompt}: {string.Join(" / ", values)}");
                Console.ResetColor();

                if (Enum.TryParse<T>(Console.ReadLine(), true, out var value) &&
                    Enum.IsDefined(typeof(T), value))
                {
                    return value;
                }

                Error("Enter one of the displayed names.");
            }
        }

        private static string ReadChoice(string prompt)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"\n{prompt}: ");
            Console.ResetColor();
            return Console.ReadLine()?.Trim() ?? "";
        }

        private static string ReadText(string prompt)
        {
            while (true)
            {
                var value = ReadChoice(prompt);
                if (!string.IsNullOrWhiteSpace(value))
                    return value;

                Error("A value is required.");
            }
        }

        private static string ReadOptional(string prompt) => ReadChoice(prompt);

        private static int ReadInt(string prompt)
        {
            while (true)
            {
                if (int.TryParse(ReadText(prompt), out int number))
                    return number;

                Error("Enter a valid whole number.");
            }
        }

        private static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                if (decimal.TryParse(ReadText(prompt), out decimal amount))
                    return amount;

                Error("Enter a valid amount.");
            }
        }

        private static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                if (DateTime.TryParse(ReadText(prompt), out var date))
                    return date;

                Error("Invalid date format.");
            }
        }

        private static bool Confirm(string prompt) =>
            ReadChoice(prompt + " (y/n)").Equals("y", StringComparison.OrdinalIgnoreCase);

        private static string Money(decimal amount) => $"{amount:N2}";

        private static void Pause()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("\nPress Enter to continue...");
            Console.ResetColor();
            Console.ReadLine();
        }

        private static void Header(
            string title,
            string? subtitle = null,
            ConsoleColor theme = ConsoleColor.DarkRed)
        {
            Console.Clear();
            Console.BackgroundColor = theme;
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║ {title,-64} ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");

            Console.ResetColor();

            if (!string.IsNullOrWhiteSpace(subtitle))
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(subtitle);
                Console.ResetColor();
            }

            Console.WriteLine();
        }

        private static void Menu(string title, ConsoleColor itemColor, params string[] items)
        {
            Console.ForegroundColor = itemColor;
            Console.WriteLine($"── {title} ──");
            Console.ResetColor();

            foreach (var item in items)
            {
                Console.ForegroundColor = item.StartsWith("0)")
                    ? ConsoleColor.DarkGray
                    : itemColor;
                Console.WriteLine("  " + item);
            }

            Console.ResetColor();
        }

        private static void WriteRow(
            string label,
            string value,
            ConsoleColor accent = ConsoleColor.Cyan)
        {
            Console.ForegroundColor = accent;
            Console.Write($"  {label,-28}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(value);
            Console.ResetColor();
        }

        private static void Success(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ " + text);
            Console.ResetColor();
        }

        private static void Error(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("! " + text);
            Console.ResetColor();
        }
    }
}