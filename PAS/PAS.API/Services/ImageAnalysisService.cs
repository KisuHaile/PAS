using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Options;
using PAS.API.Configurations;

namespace PAS.API.Services;

public class ImageAnalysisService : IImageAnalysisService
{
    private readonly ImageAnalysisSettings _settings;

    public ImageAnalysisService(IOptions<ImageAnalysisSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<bool> ContainsPersonAsync(byte[] imageBytes, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.Endpoint) || string.IsNullOrWhiteSpace(_settings.Key))
        {
            throw new InvalidOperationException("Image analysis is not configured.");
        }

        var client = new ImageAnalysisClient(new Uri(_settings.Endpoint), new AzureKeyCredential(_settings.Key));
        using var stream = new MemoryStream(imageBytes);
        var result = await client.AnalyzeAsync(BinaryData.FromStream(stream), VisualFeatures.People, cancellationToken: cancellationToken);
        var people = result.Value.People?.Values;
        return people is { Count: > 0 };
    }
}
