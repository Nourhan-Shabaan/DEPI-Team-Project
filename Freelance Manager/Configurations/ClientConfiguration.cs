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
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {

            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Email).IsRequired().HasMaxLength(150);
            builder.Property(c => c.Phone).HasMaxLength(20);
            builder.Property(c => c.Company).HasMaxLength(100);
            builder.HasIndex(c => c.Email).IsUnique();

            // not allow delete a client who has projects.
            builder.HasMany(c => c.Projects)
                   .WithOne(p => p.Client)
                   .HasForeignKey(p => p.ClientId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
