using System;

namespace GameStore.Api.Entities;

public class Game
{
    public int Id {get; set;} // shortcut type prop
    public required string Name {get; set;}

    public int GenreId {get; set;}

    public Genre? Genre { get; set; }

    public decimal Price { get; set; } // typing prop is a shortcut to get this
    public DateOnly ReleaseDate { get; set; }
}
