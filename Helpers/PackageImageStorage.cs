namespace TourismHajj.Helpers;

public static class PackageImageStorage
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    public static Task<string?> SaveCoverAsync(IWebHostEnvironment env, IFormFile? file, CancellationToken cancellationToken = default)
        => SaveAsync(env, file, "packages", cancellationToken);

    public static Task<string?> SaveGalleryAsync(IWebHostEnvironment env, IFormFile? file, CancellationToken cancellationToken = default)
        => SaveAsync(env, file, "gallery", cancellationToken);

    public static Task<string?> SaveHotelAsync(IWebHostEnvironment env, IFormFile? file, CancellationToken cancellationToken = default)
        => SaveAsync(env, file, "hotels", cancellationToken);

    public static Task<string?> SaveTravelerDocumentAsync(IWebHostEnvironment env, IFormFile? file, CancellationToken cancellationToken = default)
        => SaveAsync(env, file, "traveler-docs", cancellationToken, DocumentExtensions, 10 * 1024 * 1024,
            "Only JPG, PNG, WEBP, GIF, or PDF files are allowed.");

    private static readonly HashSet<string> DocumentExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif", ".pdf"
    };

    private static async Task<string?> SaveAsync(
        IWebHostEnvironment env,
        IFormFile? file,
        string folderName,
        CancellationToken cancellationToken,
        HashSet<string>? allowedExtensions = null,
        long maxBytes = 5 * 1024 * 1024,
        string? invalidTypeMessage = null)
    {
        if (file == null || file.Length == 0) return null;

        var allowed = allowedExtensions ?? AllowedExtensions;
        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !allowed.Contains(extension))
            throw new InvalidOperationException(invalidTypeMessage ?? "Only JPG, PNG, WEBP, or GIF images are allowed.");

        if (file.Length > maxBytes)
            throw new InvalidOperationException($"File must be {maxBytes / (1024 * 1024)} MB or smaller.");

        var folder = Path.Combine(env.WebRootPath, "uploads", folderName);
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var physicalPath = Path.Combine(folder, fileName);

        await using var stream = new FileStream(physicalPath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        return $"/uploads/{folderName}/{fileName}";
    }
}
