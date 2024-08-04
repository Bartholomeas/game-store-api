using GameStore.Api.Dtos;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";

    private static readonly List<GameDto> Games =
    [
        new(
            1,
            "Street Fighter",
            "Fighting",
            19.99M,
            new DateOnly(1992, 7, 15)
        ),
        new(
            2,
            "Street Fighter II",
            "Fighting",
            39.99M,
            new DateOnly(2010, 9, 30)
        ),
        new(
            3,
            "Fifa 24",
            "Sports",
            69.99M,
            new DateOnly(2023, 10, 10)
        )
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();
// GET /games
        group.MapGet("/", () => Games);
// GET /games/1
        group.MapGet("/{id}", (int id) =>
        {
            GameDto? game = Games.Find(game => game.Id == id);

            return game is null ? Results.NotFound() : Results.Ok(game);
        }).WithName(GetGameEndpointName);

// POST
        group.MapPost("/", (CreateGameDto newGame) =>
        {
            GameDto game = new GameDto(
                Games.Count + 1,
                newGame.Name,
                newGame.Genre,
                newGame.Price,
                newGame.ReleaseDate
            );

            Games.Add(game);

            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
        });

// PUT
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
        {
            var index = Games.FindIndex(game => game.Id == id);
            if (index == -1) return Results.NotFound();

            Games[index] = new GameDto(
                id,
                updatedGame.Name,
                updatedGame.Genre,
                updatedGame.Price,
                updatedGame.ReleaseDate
            );

            return Results.NoContent();
        });

// DELETE /games/1

        group.MapDelete("/{id}", (int id) =>
        {
            Games.RemoveAll(game => game.Id == id);
            return Results.NoContent();
        });

        return group;
    }
}