using System;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

// Add this file to help run pending migrations everytime the app runs
public static class DataExtensions
{
    public static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        await dbContext.Database.MigrateAsync();
    }

}
