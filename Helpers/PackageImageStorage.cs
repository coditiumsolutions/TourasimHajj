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

    private static async Task<string?> SaveAsync(IWebHostEnvironment env, IFormFile? file, string folderName, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0) return null;

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            throw new InvalidOperationException("Only JPG, PNG, WEBP, or GIF images are allowed.");

        if (file.Length > 5 * 1024 * 1024)
            throw new InvalidOperationException("Image must be 5 MB or smaller.");

        var folder = Path.Combine(env.WebRootPath, "uploads", folderName);
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var physicalPath = Path.Combine(folder, fileName);

        await using var stream = new FileStream(physicalPath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        return $"/uploads/{folderName}/{fileName}";
    }
}
