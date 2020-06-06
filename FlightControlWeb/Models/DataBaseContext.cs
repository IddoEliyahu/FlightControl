using FlightControl;
using FlightControl.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightControlWeb.Models
{
    public class DataBaseContext : DbContext

    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }

        public DbSet<FlightPlan> FlightPlans { get; set; }

        public DbSet<Server> Servers { get; set; }

        public DbSet<ServerById> ServersById { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LocationWithTime>().HasNoKey();
            modelBuilder.Entity<Segment>().HasNoKey();
            modelBuilder.Entity<FlightPlan>()
                .Property(flightPlan => flightPlan._initial_location).HasColumnName("initial_location");
            modelBuilder.Entity<FlightPlan>()
                .Property(flightPlan => flightPlan._segments).HasColumnName("segments");
            // modelBuilder.Entity<FlightPlan>().HasOne<LocationWithTime>(flightPlan => flightPlan.initial_location).WithOne(time => null);
            // modelBuilder.Entity<FlightPlan>().HasMany<Segment>(plan => plan.segments);
        }
    }
}