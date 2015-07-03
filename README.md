# FluentIL

Do you like emitting? Do you need it? What do you think about do that in this way?

````csharp
[Test]
public void TwoPlusTwoWithNamedParameters()
{
    // arrange
    var dm = IL.NewMethod()
        .WithParameter<int>("a")
        .WithParameter<int>("b")
        .Returns<int>()
  
        .Ldarg("a", "b")
        .Add()
        .Ret();
  
    // act
    var result = dm.Invoke(2, 2);
  
    // assert
    result.Should().Be(4);
}
````

It's really simple to emit a method with two arguments? Isn't it?

Conditionals?

````csharp
[Test]
public void MultipleConditions_4()
{
    var dm = IL.NewMethod()
        .WithParameter(typeof(int), "a")
        .Returns(typeof(int))
        .If("a>=10&&a<=20")
            .Ldc(2)
        .Else()
            .Ldc(4)
        .EndIf()
        .Ret();
 
    dm.Invoke(10).Should().Be(2);
    dm.Invoke(9).Should().Be(4);
    dm.Invoke(21).Should().Be(4);
}
````

For?

````csharp
public IPrimeChecker CreatePrimeCheckerV4()
{
    var t = IL.NewType().Implements<IPrimeChecker>()
        .WithMethod("IsPrime")
        .WithVariable(typeof(int), "i")
        .WithParameter(typeof(int), "number")
        .Returns(typeof(bool))
            .If("number<=1")
                .Ret(false)
            .EndIf()
            .For("i", 2, "number/2")
                .If("(number%i)==0")
                    .Ret(false)
                .EndIf()
            .Next()
            .Ret(true)
        .AsType;
 
    return (IPrimeChecker)Activator.CreateInstance(t);
}
````

While?

````csharp
public IPrimeChecker CreatePrimeCheckerV6()
{
    var t = IL.NewType().Implements<IPrimeChecker>()
        .WithMethod("IsPrime")
        .WithVariable<int>("i")
        .WithParameter<int>("number")
        .Returns<bool>()
            .If("number<=1", @then: m => m
                .Ret(false)
            )
            .Stloc(2, "i")
            .While("i <= number/2", @do: m => m
                .If("(number%i)==0", @then: b => b
                    .Ret(false)
                )
                .Inc("i")
            )
            .Ret(true)
        .AsType;
 
    return (IPrimeChecker)Activator.CreateInstance(t);
}
````

Until?

````csharp
public IPrimeChecker CreatePrimeCheckerV7()
{
    var t = IL.NewType().Implements<IPrimeChecker>()
        .WithMethod("IsPrime")
        .WithVariable<int>("i")
        .WithParameter<int>("number")
        .Returns<bool>()
            .If("number<=1", @then: m => m
                .Ret(false)
            )
            .Stloc(2, "i")
            .Until("i > number/2", @do: m => m
                .If("(number%i)==0", @then: b => b
                    .Ret(false)
                )
                .Inc("i")
            )
            .Ret(true)
        .AsType;
 
    return (IPrimeChecker)Activator.CreateInstance(t);
}
````

That's what you can do while using this FluentIL.
