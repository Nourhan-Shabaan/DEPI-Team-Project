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
    public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {

            builder.Property(t => t.Title).IsRequired().HasMaxLength(150);
            builder.Property(t => t.Description).HasMaxLength(1000);

            // store value of Enum as string in DB
            builder.Property(t => t.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(t => t.Priority)
                   .HasConversion<string>()
                   .HasMaxLength(20);
        }
    }
}
