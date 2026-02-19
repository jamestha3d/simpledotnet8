using System;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoint
{
    const string GetGameEndpointName  = "GetGame";

    // add static  cos a static class needs static methods, 
    // and readonly cos we wont be modifying the list from scratch

    // public static WebApplication MapGamesEndpoints(this WebApplication app) // extension method
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app) // extension method
    {
        // this group mapping made us change return type from WebApplication to RouteGroupBuilder
        // and change all app to group
        var group = app.MapGroup("games").WithParameterValidation();

        // GET /games
        group.MapGet("/", async (GameStoreContext dbContext) =>
            await dbContext.Games // added async await (even though not strictly necessary here)
                .Include(game => game.Genre)
                .Select(game => game.ToGameSummaryDto())
                .AsNoTracking() // by default EF tracks all items
                .ToListAsync()); // Using Async to return Task instead
                //we can say no need to track this because we are just returning it. optimization

        // GET /games/1
        // group.MapGet("games/{id}", (int id) => games.Find(game => game.Id == id))
        //     .WithName(GetGameEndpointName); // WithName is giving the route a name
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) => 
            {
                Game? game = await dbContext.Games.FindAsync(id); 
                // .NET is very efficient,it will first find the game in memory
                // if it doesnt find it, it will then check the db

                return game is null ? 
                    Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
            })
            .WithName(GetGameEndpointName); // WithName is giving the route a name

        // POST /games
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            // if (string.IsNullOrEmpty(newGame.Name))
            // {
            //     return Results.BadRequest("Name is Required");
            // }
            Game game = newGame.ToEntity();
            // game.Genre = dbContext.Genres.Find(newGame.GenreId);

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();
            
            return Results.CreatedAtRoute("GetGame", new {id = game.Id}, game.ToGameDetailsDto());
        });

        // PUT /games
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            // var index = games.FindIndex(game => game.Id == id);
            var existingGame = await dbContext.Games.FindAsync(id);
            // if (index == -1)
            // {
            //     return Results.NotFound();
            // }
            if (existingGame is null)
            {
                return Results.NotFound();
            }
            // games[index] = new GameSummaryDto(
            //     id,
            //     updatedGame.Name,
            //     updatedGame.Genre,
            //     updatedGame.Price,
            //     updatedGame.ReleaseDate
            // );
            dbContext.Entry(existingGame)
                .CurrentValues
                .SetValues(updatedGame.ToEntity(id));
            
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // DELETE /games
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games
                .Where(game => game.Id == id)
                .ExecuteDeleteAsync(); // very efficient, batch delete

            // games.RemoveAll(game => game.Id == id);
            return Results.NoContent();
        });

        // group.MapGet("/", () => "Hello World!");

        return group;

    }
}
