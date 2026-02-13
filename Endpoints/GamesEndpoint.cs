using System;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoint
{
    const string GetGameEndpointName  = "GetGame";

    // add static  cos a static class needs static methods, 
    // and readonly cos we wont be modifying the list from scratch
    private static readonly List<GameDto> games = [
        new(
            1,
            "Street Fighter II",
            "Fighting",
            19.99M,
            new DateOnly(1992, 7, 15)
        ),
        new(
            2,
            "Final Fantasy XIV",
            "Roleplaying",
            59.99M,
            new DateOnly(2010, 9, 30)
        ),
        new(
            3,
            "FIFA 23",
            "Sports",
            69.99M,
            new DateOnly(2022, 9, 27)
        ),

    ];

    // public static WebApplication MapGamesEndpoints(this WebApplication app) // extension method
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app) // extension method
    {
        // this group mapping made us change return type from WebApplication to RouteGroupBuilder
        // and change all app to group
        var group = app.MapGroup("games").WithParameterValidation();
        // GET /games
        group.MapGet("/", () => games);

        // GET /games/1
        // group.MapGet("games/{id}", (int id) => games.Find(game => game.Id == id))
        //     .WithName(GetGameEndpointName); // WithName is giving the route a name
        group.MapGet("/{id}", (int id) => 
            {
                GameDto? game = games.Find(game => game.Id == id);

                return game is null ? Results.NotFound() : Results.Ok(game);
            })
            .WithName(GetGameEndpointName); // WithName is giving the route a name

        // POST /games
        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            // if (string.IsNullOrEmpty(newGame.Name))
            // {
            //     return Results.BadRequest("Name is Required");
            // }
            Game game = new()
            {
                Name = newGame.Name,
                Genre = dbContext.Genres.Find(newGame.GenreId),
                GenreId = newGame.GenreId,
                Price = newGame.Price,
                ReleaseDate = newGame.ReleaseDate

            };

            dbContext.Games.Add(game);
            dbContext.SaveChanges();
            GameDto gameDto = new(
                game.Id,
                game.Name,
                game.Genre!.Name, // we know that genre will never be null so !
                game.Price,
                game.ReleaseDate
            );
            return Results.CreatedAtRoute("GetGame", new {id = game.Id}, gameDto);
        });

        // PUT /games
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
        {
            var index = games.FindIndex(game => game.Id == id);
            if (index == -1)
            {
                return Results.NotFound();
            }
            games[index] = new GameDto(
                id,
                updatedGame.Name,
                updatedGame.Genre,
                updatedGame.Price,
                updatedGame.ReleaseDate
            );

            return Results.NoContent();
        });

        // DELETE /games
        group.MapDelete("/{id}", (int id) =>
        {
            games.RemoveAll(game => game.Id == id);
            return Results.NoContent();
        });

        // group.MapGet("/", () => "Hello World!");

        return group;

    }
}
