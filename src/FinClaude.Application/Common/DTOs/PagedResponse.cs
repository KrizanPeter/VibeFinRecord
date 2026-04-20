namespace FinClaude.Application.Common.DTOs;

public record PagedResponse<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize);
