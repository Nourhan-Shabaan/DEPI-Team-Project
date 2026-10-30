using System;
using Freelance_Manager.Data;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Manager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                using var db = new AppDbContext();
                // Apply pending EF Core migrations before opening the menus.
                db.Database.Migrate();

                var ui = new ConsoleUI(db);
                ui.Run();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The application could not start:");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.WriteLine("Make sure SQL Server is running and the connection string is correct. Press Enter to exit.");
                Console.ReadLine();
            }
        }
    }
}
