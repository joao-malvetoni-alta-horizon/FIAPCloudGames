using FCG.Application.Games.DTOs;
using FCG.Domain.Games.Enums;

namespace FCG.Application.Games.Interfaces;

public interface IListGamesUseCase
{
    Task<PagedGameResponse> ExecuteAsync(int page, int pageSize, GameGenre? genre = null, CancellationToken ct = default);
}
