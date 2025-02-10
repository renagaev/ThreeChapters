using Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UseCases.Services;

namespace UseCases.UnitTests;

public class ReportParserTests
{
    [Fact]
    public void Should_Parse_Report()
    {
        // Arrange
        const string text = """
                            8 февраля 2025

                            ✅ user1: 1-Царств 23-26 
                            ✅ user2: Деяния 1-4
                            ❔ user3
                            ❔ user4
                            """;
        var firstUserEntries = new[] { new ParsedReadEntry("1-Царств", 23, "1-Царств", 26) };
        var secondUserEntries = new[] { new ParsedReadEntry("Деяния", 1, "Деяния", 4) };
        var intervalParserMock = new Mock<IIntervalParser>();
        intervalParserMock.Setup(x => x.Parse("1-Царств 23-26", It.IsAny<ICollection<Book>>()))
            .Returns(firstUserEntries);
        intervalParserMock.Setup(x => x.Parse("Деяния 1-4", It.IsAny<ICollection<Book>>()))
            .Returns(secondUserEntries);
        var reportParser = new ReportParser(intervalParserMock.Object, NullLogger<ReportParser>.Instance);

        // Act
        var result = reportParser.Parse(text, []);

        // Assert 
        Assert.NotNull(result);
        Assert.Equal(new DateOnly(2025, 2, 8), result.Date);
        var firstUserItem = result.Items.FirstOrDefault(x => x.User == "user1");
        Assert.NotNull(firstUserItem);
        Assert.Equal(firstUserEntries, firstUserItem.Intervals);
        var secondUserItem = result.Items.FirstOrDefault(x => x.User == "user2");
        Assert.NotNull(secondUserItem);
        Assert.Equal(secondUserEntries, secondUserItem.Intervals);
        var user3Item = result.Items.FirstOrDefault(x => x.User == "user3");
        Assert.NotNull(user3Item);
        Assert.Empty(user3Item.Intervals);
        var user4Item = result.Items.FirstOrDefault(x => x.User == "user4");
        Assert.NotNull(user4Item);
        Assert.Empty(user4Item.Intervals);
    }
}