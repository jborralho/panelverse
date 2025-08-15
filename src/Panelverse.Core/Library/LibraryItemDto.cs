namespace Panelverse.Core.Library;

public sealed record LibraryItemDto(
    long Id,
    string Title,
    string? Series,
    int? Volume,
    int PagesTotal,
    int PagesRead,
    string LocationPath,
    bool IsFolder,
    long? ParentId,
    string? ThumbnailPath,
    DateTimeOffset AddedAt,
    DateTimeOffset? LastOpenedAt
);


