using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using UsersManagement.Entities;
using UsersManagement.Enums;

namespace UsersManagement.Persistence
{
    public class MainDbContext : DbContext, IMainDbContext
    {
        public new IModel Model => base.Model;
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var item in ChangeTracker.Entries<IEntity>().AsEnumerable())
            {
               
                item.Entity.CreatedAt = DateTime.Now;
                item.Entity.UpdatedAt = DateTime.Now;
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Database.BeginTransactionAsync(cancellationToken);
        }
        public async Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await SaveChangesAsync();
            await Database.CurrentTransaction?.CommitAsync(cancellationToken);
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await Database.CurrentTransaction?.RollbackAsync(cancellationToken);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PhoneNumber>(entity =>
            {
                entity.Property(e => e.NumberType)
                      .HasConversion<int>();
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.Property(e => e.Gender)
                      .HasConversion<int>();
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("Persons");

                entity.HasIndex(p => p.PersonalNumber).IsUnique();

                entity.Property(p => p.Gender).HasConversion(
                    v => v.ToString(),
                    v => (GenderType)Enum.Parse(typeof(GenderType), v));


                entity.HasOne(p => p.City)
                      .WithMany()
                      .HasForeignKey(p => p.CityId);


            });


            modelBuilder.Entity<PhoneNumber>(entity =>
            {
                entity.ToTable("PhoneNumbers");


                entity.HasOne(pn => pn.Person)
                      .WithMany(p => p.PhoneNumbers)
                      .HasForeignKey(pn => pn.PersonId);


                entity.Property(pn => pn.NumberType).HasConversion(
                    v => v.ToString(),
                    v => (PhoneNumberType)Enum.Parse(typeof(PhoneNumberType), v));


            });


            modelBuilder.Entity<Relationship>(entity =>
            {
                entity.ToTable("Relationships");


                entity.HasOne(r => r.Person)
                      .WithMany(p => p.Relationships)
                      .HasForeignKey(r => r.PersonId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.RelatedPerson)
                      .WithMany()
                      .HasForeignKey(r => r.RelatedPersonId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("Cities");

            });


            //  Cities
            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "Tbilisi" },
                new City { Id = 2, Name = "Kutaisi" },
                new City { Id = 3, Name = "Batumi" },
                new City { Id = 4, Name = "Sokhumi" }
            );

            //  Persons
            modelBuilder.Entity<Person>().HasData(
                new Person { Id = 1, Name = "Alex", Surname = "Johnson", Gender = GenderType.Male, PersonalNumber = "12345678901", DateOfBirth = new DateTime(1990, 01, 01), CityId = 1, Image = "path/to/image1.jpg" },
                new Person { Id = 2, Name = "Maria", Surname = "Garcia", Gender = GenderType.Female, PersonalNumber = "12345678902", DateOfBirth = new DateTime(1985, 05, 23), CityId = 2, Image = "path/to/image2.jpg" },
                new Person { Id = 3, Name = "Ivan", Surname = "Ivanov", Gender = GenderType.Male, PersonalNumber = "12345678903", DateOfBirth = new DateTime(1982, 07, 15), CityId = 3, Image = "path/to/image3.jpg" },
                new Person { Id = 4, Name = "Sophia", Surname = "Petrova", Gender = GenderType.Female, PersonalNumber = "12345678904", DateOfBirth = new DateTime(1995, 11, 30), CityId = 4, Image = "path/to/image4.jpg" }
            );

            // Extending PhoneNumbers
            modelBuilder.Entity<PhoneNumber>().HasData(
                new PhoneNumber { Id = 1, NumberType = PhoneNumberType.Mobile, Number = "5551234", PersonId = 1 },
                new PhoneNumber { Id = 2, NumberType = PhoneNumberType.Home, Number = "5555678", PersonId = 2 },
                new PhoneNumber { Id = 3, NumberType = PhoneNumberType.Office, Number = "5559012", PersonId = 3 },
                new PhoneNumber { Id = 4, NumberType = PhoneNumberType.Mobile, Number = "5553456", PersonId = 4 }
            );

            //  Relationships
            modelBuilder.Entity<Relationship>().HasData(
                new Relationship { Id = 1, RelationshipType = RelationshipType.Colleague, PersonId = 1, RelatedPersonId = 2 },
                new Relationship { Id = 2, RelationshipType = RelationshipType.Acquaintance, PersonId = 2, RelatedPersonId = 3 },
                new Relationship { Id = 3, RelationshipType = RelationshipType.Relative, PersonId = 3, RelatedPersonId = 4 },
                new Relationship { Id = 4, RelationshipType = RelationshipType.Other, PersonId = 4, RelatedPersonId = 1 }
            );

        } 
    }
}
