using FCG.Application.Games.DTOs;
using FCG.Application.Games.Interfaces;
using FCG.Application.Games.Mappers;
using FCG.Domain.Games.Enums;
using FCG.Domain.Games.Interfaces;

namespace FCG.Application.Games.UseCases;

public class ListGamesUseCase(IGameRepository repository) : IListGamesUseCase
{
    public async Task<PagedGameResponse> ExecuteAsync(
        int page, int pageSize, GameGenre? genre = null, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 50) pageSize = 50;

        var (items, totalCount) = await repository.ListAsync(page, pageSize, genre, ct);

        var responses = items.Select(GameMapper.ToResponse).ToList();

        return new PagedGameResponse(responses, totalCount, page, pageSize);
    }
}
