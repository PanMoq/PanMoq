# ![PanMoq logo](docs/images/logo-32.png) PanMoq

A no-fuss test fixture API that combines [AutoFixture](https://github.com/AutoFixture/AutoFixture) and [Moq](https://github.com/moq/moq4) together.

[![NuGet package version](https://img.shields.io/nuget/vpre/PanMoq.svg?color=royalblue)](https://www.nuget.org/packages/PanMoq) [![Branch Publish](https://github.com/PanMoq/PanMoq/actions/workflows/branch-publish.yml/badge.svg)](https://github.com/PanMoq/PanMoq/actions/workflows/branch-publish.yml) [![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com) [![Awesome Badges](https://img.shields.io/badge/badges-awesome-green.svg)](https://github.com/Naereen/badges)

## 📚 Overview

PanMoq streamlines the experience of using [AutoFixture](https://github.com/AutoFixture/AutoFixture) and [Moq](https://github.com/moq/moq4) together - letting AutoFixture create concrete types for you, and using Moq to automatically mock abstract dependencies at the same time.

### 🥧 PanMoq's API is dead simple

For basic use cases, PanMoq couldn't be any easier to use - just `new` up the `AutoMoqFixture` class, and then invoke `CreateSpecimen` to create an instance of the type you want:

```csharp
// Arrange
var fixture = new PanMoq.AutoMoqFixture();
MyClassToTest myInstanceToTest = fixture.CreateSpecimen<MyClassToTest>();

// Act
var someResult = myInstanceToTest.DoSomething();
```

Need extra data? PanMoq is built around AutoFixture, so it can populate arbitrary types for you all day long:

```csharp
// Arrange
var fixture = new PanMoq.AutoMoqFixture();
var (myInstanceToTest, myString, myInt) = fixture.CreateSpecimen<(MyClassToTest, string, int)>();

// Act
var someResult = myInstanceToTest.DoSomethingElse(myString, myInt);
```

### 🔧 PanMoq's API scales with you

PanMoq is designed to be safe to "build on" during test setup - without worrying about resetting between tests. This necessitates a few qualities:

1. PanMoq's configuration API is fluent.

    ```csharp
    // Arrange
    PanMoq.AutoMoqFixture fixtureBase = new();
    PanMoq.AutoMoqFixture configuredFixture =
        fixtureBase
            .Configure(autofixtureFixture => autofixtureFixture.Freeze<Mock<IMyFavoriteRepository>>())
            .Configure(autofixtureFixture => autofixtureFixture.Freeze<Mock<ISomeOtherService>>());
    var (myInstanceToTest, myString, myInt) = fixture.CreateSpecimen<(MyClassToTest, string, int)>();

    // Act
    var someResult = myInstanceToTest.DoSomethingElse(myString, myInt);
    ```

2. PanMoq's configuration API enforces immutability of the configuration.

    ```csharp
    // (continuing from above)

    var areSameFixture = object.ReferenceEquals(fixtureBase, configuredFixture); // false
    ```

3. PanMoq is stateless.

    PanMoq fixtures don't retain state (by default), so they are safe to setup once and then keep reusing them:

    ```csharp
    private static readonly PanMoq.AutoMoqFixture fixture = new();

    [Fact]
    public void MyTest1()
    {
        var subject = fixture.CreateSpecimen<MyClassToTest>();
    }

    [Fact]
    public void MyTest2()
    {
        var subject = fixture.CreateSpecimen<MyClassToTest>(); // an all-new instance, in an all-new object graph
    }
    ```

## ✨ Give it a try yourself

### 📦 Install PanMoq

Install the package:

```powershell
dotnet add package PanMoq --source https://api.nuget.org/v3/index.json
```

## 💡 Questions? Suggestions?

Whether it's a question or a suggestion, if you've got something on your mind (about PanMoq) that you'd like to speak up about, please feel free to file an issue so we can get in touch.

If your thoughts involve a potential code contribution - and it's figuratively burning a hole in your pocket - please consider opening a draft PR early so we can collaborate.

Either way, we look forward to hearing from you!
