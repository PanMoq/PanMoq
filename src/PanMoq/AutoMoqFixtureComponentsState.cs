using AutoFixture;
using Moq;

namespace PanMoq;

public readonly record struct AutoMoqFixtureComponentsState(MockRepository MockRepository, Fixture Fixture);
