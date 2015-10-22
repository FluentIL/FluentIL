# FluentIL
Do you like emitting? Do you need it? FluentIL is an emitting helper library that can help you.

This is a community project, free and open source. Everyone is invited to contribute, fork, share and use the code. No money shall be charged by this software, nor it will be. Ever.

[![Nuget count](http://img.shields.io/nuget/v/fluentil.svg)](https://www.nuget.org/packages/fluentil/)
[![Nuget downloads](http://img.shields.io/nuget/dt/fluentil.svg)](https://www.nuget.org/packages/fluentil/)

If you are looking for some Mono.Cecil integration, please check [FluentIL.Cecil](http://github.com/fluentil/fluentil.cecil) project.

## Installing

FluentIL is avaiable on Nuget

````shell
Install-Package FluentIL
````

## Getting and building the code

To build the source code:

````shell
git clone https://github.com/FluentIL/FluentIL.git
cd FluentIL
nuget restore
.\build.cmd
````

Be happy!

## Contributing

Questions, comments, bug reports, and pull requests are all welcome. Bug reports that include steps-to-reproduce (including code) are preferred. Even better, make them in the form of pull requests. Before you start to work on an existing issue, check if it is not assigned to anyone yet, and if it is, talk to that person.

## Pull request

If you wrote something that you would like to share, please create a pull request.

You have to do that in the command line:

````shell
# add the main repo with the `fluentil` name
git remote add fluentil https://github.com/FluentIL/FluentIL.git
# checkout the master branch
git checkout master
# download the latest changes from the master repo
git pull fluentil master
# go back to your working branch
git checkout <youbranchname>
# integrate your changes
git merge master
# solve integration conflicts
````

You can solve the conflicts in your favorite text editor, or, if you are using Visual Studio, you can use it as well. Visual Studio actually presents the conflict in a very nice way to solve them. Also, on the go back to your working branch step you can go back to using Visual Studio to control git, if you prefer that.

If you know git well, you can rebase your changes instead of merging them. If not, it is ok to merge them. When your changes are up to date with the master branch then you should push them to your Github repo and then you will be able to issue a pull request and mention the issue you were working on. Make your PR message clear. If when you are creating the pull request on Github it mentions that the PR cannot be merged because there are conflicts it means you forgot to integrate the master branch. Correct that push the changes to your personal repo. This will automatically update the PR. The project maintainers should not have to resolve merge conflicts, you should.

After your pull request is accepted you may delete your local branch if you want. Update your master branch so you can continue to contribute in the future. And thank you! :)

If your pull request is denied try to understand why. It is not uncommon that PRs are denied but after some discussing and fixing they are accepted. Work with the community to get it to be the best 

## Samples
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

There are a lot of samples in the test and demo directory.

Why boring write a clone method again?!

````csharp
private static Func<T, T> GenerateConventionalCloner<T>()
{
    var type = typeof (T);

    var method = IL.NewMethod()
        .WithParameter(type, "source")
        .WithVariable(type, "result")
        .WithOwner(type)
        .Returns(type)

        .Newobj<T>()
        .Stloc("result");

    foreach (var field in type.GetAllFields())
    {
        method
            .Ldloc("result")
            .Ldarg("source")
            .Ldfld(field)
            .Stfld(field);
    }

    method
        .Ldloc("result")
        .Ret();

    return (Func<T, T>) method.AsDynamicMethod.CreateDelegate(typeof(Func<T, T>));
}
````

