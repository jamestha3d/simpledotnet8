using System;
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

// options provide the context store details on how to connect to db
public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
{
    // Representation of objects to be mapped to the db
    public DbSet<Game> Games => Set<Game>();// db set is used to query instances of games. 
    // LINQ queries will be converted to SQL queries
    public DbSet<Genre> Genres => Set<Genre>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // overwriting this, we need to run migration after doing this
        // static data for application
        modelBuilder.Entity<Genre>().HasData(
            new {Id = 1, Name = "Fighting"},
            new {Id = 2, Name = "Roleplaying"},
            new {Id = 3, Name = "Sports"},
            new {Id = 4, Name = "Racing"},
            new {Id = 5, Name = "Kids and Family"}
        );
    }

    
}
