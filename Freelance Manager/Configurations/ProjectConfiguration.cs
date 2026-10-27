using Freelance_Manager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Freelance_Manager.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.Property(p => p.Title).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Description).HasMaxLength(1000);
            builder.Property(p => p.Budget).HasPrecision(18, 2);

            // store value of Enum as string in DB
            builder.Property(p => p.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            // Deleting a project -> deletes its tasks and payments.
            builder.HasMany(p => p.Tasks)
                   .WithOne(t => t.Project)
                   .HasForeignKey(t => t.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Payments)
                   .WithOne(pay => pay.Project)
                   .HasForeignKey(pay => pay.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
