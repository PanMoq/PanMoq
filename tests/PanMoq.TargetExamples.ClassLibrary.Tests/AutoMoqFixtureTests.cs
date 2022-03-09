using System;
using AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;

namespace PanMoq.Specimens.ClassLibrary.Tests;

public class AutoMoqFixtureTests : IClassFixture<AutoMoqFixture>
{
    private readonly AutoMoqFixture _fixture;

    public AutoMoqFixtureTests(AutoMoqFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void CreateSpecimen_Resolves_SubjectWithAbstractDependency()
    {
        var subject = _fixture.CreateSpecimen<Dog>();

        var actual = subject.Vocalize();

        actual.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void CreateSpecimen_GivenNoFreezes_Returns_Different_Mock()
    {
        var (subject, mockedService) = _fixture.CreateSpecimen<(Dog, Mock<ICirculatorySystem>)>();

        subject.CirculatorySystem.Should().NotBeSameAs(mockedService.Object);
    }

    [Fact(Skip = "Fails because ValueTuple has fields populated after constructor call")]
    public void CreateSpecimen_GivenFrozenMock_GivenValueTupleSubject_OnlyCreatesMockOnce()
    {
        var (subject, mockedService) = _fixture
            .Configure(autofixtureFixture => autofixtureFixture.Freeze<Mock<ICirculatorySystem>>())
            .CreateSpecimen<(Dog, Mock<ICirculatorySystem>)>();

        subject.CirculatorySystem.Should().BeSameAs(mockedService.Object);

        mockedService.VerifyAdd(cs => cs.Pulse += It.IsAny<EventHandler>(), Times.Once);
    }

    [Fact]
    public void CreateSpecimen_GivenFrozenMock_GivenNonWritableRecordSubject_OnlyCreatesMockOnce()
    {
        var (subject, mockedService) = _fixture
            .Configure(autofixtureFixture => autofixtureFixture.Freeze<Mock<ICirculatorySystem>>())
            .CreateSpecimen<NonWritableRecordSubject>();

        subject.CirculatorySystem.Should().BeSameAs(mockedService.Object);

        mockedService.VerifyAdd(cs => cs.Pulse += It.IsAny<EventHandler>(), Times.Once);
    }

    [Fact(Skip = "Fails because Record has properties populated after constructor call")]
    public void CreateSpecimen_GivenFrozenMock_GivenSimpleRecordSubject_OnlyCreatesMockOnce()
    {
        var (subject, mockedService) = _fixture
            .Configure(autofixtureFixture => autofixtureFixture.Freeze<Mock<ICirculatorySystem>>())
            .CreateSpecimen<SimpleRecordSubject>();

        subject.CirculatorySystem.Should().BeSameAs(mockedService.Object);

        mockedService.VerifyAdd(cs => cs.Pulse += It.IsAny<EventHandler>(), Times.Once);
    }

    public record SimpleRecordSubject(Dog Dog, Mock<ICirculatorySystem> MockCirculatorySystem);

    public record NonWritableRecordSubject(Dog Dog, Mock<ICirculatorySystem> MockCirculatorySystem)
    {
        public Dog Dog { get; } = Dog;
        public Mock<ICirculatorySystem> MockCirculatorySystem { get; } = MockCirculatorySystem;
    }
}
