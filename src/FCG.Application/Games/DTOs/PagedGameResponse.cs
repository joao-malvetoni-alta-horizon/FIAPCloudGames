namespace FCG.Application.Games.DTOs;

public record PagedGameResponse(
    IReadOnlyList<GameResponse> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
