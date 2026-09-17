using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TrainingCatalog.Application;

namespace TrainingCatalog.Api.Tests;

public sealed class TrainingCreationTests
{
    [Fact]
    public async Task ReturnsCreatedWhenLessonScheduleFitsTrainingDuration()
    {
        using var factory = new TrainingCatalogApiFactory();
        using var client = factory.CreateClient();
        var request = new CreateTrainingRequest(
            "Fundamentos de C#",
            "Introdução ao C#",
            "2026-09-14",
            8,
            2,
            4);

        var response = await client.PostAsJsonAsync("/api/trainings", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsBadRequestWhenLessonScheduleExceedsTrainingDuration()
    {
        using var factory = new TrainingCatalogApiFactory();
        using var client = factory.CreateClient();
        var request = new CreateTrainingRequest(
            "Fundamentos de C#",
            "Introdução ao C#",
            "2026-09-14",
            8,
            3,
            4);

        var response = await client.PostAsJsonAsync("/api/trainings", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var error = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(error.RootElement.GetProperty("errors").TryGetProperty("lessonDurationHours", out _));
    }

    [Fact]
    public async Task ReturnsBadRequestWhenLessonDurationIsGreaterThanFourHours()
    {
        using var factory = new TrainingCatalogApiFactory();
        using var client = factory.CreateClient();
        var request = new CreateTrainingRequest(
            "Fundamentos de C#",
            "Introdução ao C#",
            "2026-09-14",
            8,
            1,
            5);

        var response = await client.PostAsJsonAsync("/api/trainings", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var error = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(error.RootElement.GetProperty("errors").TryGetProperty("lessonDurationHours", out _));
    }

    [Theory]
    [InlineData(0, 1, "lessonCount")]
    [InlineData(-1, 1, "lessonCount")]
    [InlineData(1, 0, "lessonDurationHours")]
    [InlineData(1, -1, "lessonDurationHours")]
    public async Task ReturnsBadRequestWhenLessonCountOrDurationIsNotPositive(int lessonCount, int lessonDurationHours, string fieldName)
    {
        using var factory = new TrainingCatalogApiFactory();
        using var client = factory.CreateClient();
        var request = new CreateTrainingRequest(
            "Fundamentos de C#",
            "Introdução ao C#",
            "2026-09-14",
            8,
            lessonCount,
            lessonDurationHours);

        var response = await client.PostAsJsonAsync("/api/trainings", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var error = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(error.RootElement.GetProperty("errors").TryGetProperty(fieldName, out _));
    }

    [Fact]
    public async Task ReturnsConflictWhenStartDateAlreadyExists()
    {
        using var factory = new TrainingCatalogApiFactory();
        using var client = factory.CreateClient();
        var request = new CreateTrainingRequest(
            "Fundamentos de C#",
            "Introdução ao C#",
            "2026-09-15",
            8,
            2,
            4);

        var firstResponse = await client.PostAsJsonAsync("/api/trainings", request);
        var secondResponse = await client.PostAsJsonAsync(
            "/api/trainings",
            request with { Title = "C# Avançado" });

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);

        using var error = JsonDocument.Parse(await secondResponse.Content.ReadAsStringAsync());
        Assert.Equal(
            "Já existe um treinamento com esta data de início.",
            error.RootElement.GetProperty("errors").GetProperty("startDate")[0].GetString());
    }
}