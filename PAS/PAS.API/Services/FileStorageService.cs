using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PAS.API.Configurations;

namespace PAS.API.Services;

public class FileStorageService : IFileStorageService
{
    private readonly FileStorageSettings _settings;
    private readonly ILogger<FileStorageService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _environment;

    public FileStorageService(
        IOptions<FileStorageSettings> settings,
        ILogger<FileStorageService> logger,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment environment)
    {
        _settings = settings.Value;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _environment = environment;
    }

    public async Task<string> SaveFileAsync(byte[] content, string fileName, string folder, CancellationToken cancellationToken = default)
    {
        var root = ResolveRootPath(folder);
        var directoryPath = Path.Combine(root, folder);
        Directory.CreateDirectory(directoryPath);

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(directoryPath, uniqueFileName);

        await File.WriteAllBytesAsync(filePath, content, cancellationToken);
        _logger.LogInformation("File saved: {FilePath}", filePath);

        return filePath;
    }

    public async Task<byte[]> GetFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        return await File.ReadAllBytesAsync(filePath, cancellationToken);
    }

    public Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            _logger.LogInformation("File deleted: {FilePath}", filePath);
        }

        return Task.CompletedTask;
    }

    public Task<string> GetFileUrlAsync(string filePath)
    {
        var root = ResolveRootPath(string.Empty);
        var profileRoot = ResolveRootPath("profile");
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            var fullFilePath = Path.GetFullPath(filePath);
            var fullProfileRoot = Path.GetFullPath(profileRoot);
            var fullRoot = Path.GetFullPath(root);

            if (fullFilePath.StartsWith(fullProfileRoot, StringComparison.OrdinalIgnoreCase))
            {
                root = fullProfileRoot;
            }
            else if (fullFilePath.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase))
            {
                root = fullRoot;
            }
        }

        var relativePath = Path.GetRelativePath(Path.GetFullPath(root), Path.GetFullPath(filePath))
            .Replace("\\", "/");
        relativePath = relativePath.TrimStart('/');

        var request = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = !string.IsNullOrWhiteSpace(_settings.BaseUrl)
            ? _settings.BaseUrl
            : request != null
                ? $"{request.Scheme}://{request.Host}"
                : string.Empty;

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return Task.FromResult($"/uploads/{relativePath}");
        }

        var trimmedBase = baseUrl.TrimEnd('/');
        if (trimmedBase.EndsWith("/uploads", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult($"{trimmedBase}/{relativePath}");
        }

        return Task.FromResult($"{trimmedBase}/uploads/{relativePath}");
    }

    private string ResolveRootPath(string folder)
    {
        if (string.Equals(folder, "profile", StringComparison.OrdinalIgnoreCase))
        {
            var webRoot = string.IsNullOrWhiteSpace(_environment.WebRootPath)
                ? Path.Combine(AppContext.BaseDirectory, "wwwroot")
                : _environment.WebRootPath;
            return Path.Combine(webRoot, "uploads");
        }

        if (string.IsNullOrWhiteSpace(_settings.LocalPath))
        {
            return "uploads";
        }

        return _settings.LocalPath;
    }
}