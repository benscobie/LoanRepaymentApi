#nullable disable warnings

namespace LoanRepaymentApi.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using Moq;

// Source: https://stackoverflow.com/a/36880624

public class StrictAutoMoqCustomization : ICustomization
{
    public StrictAutoMoqCustomization() : this(new MockRelay())
    {
    }

    public StrictAutoMoqCustomization(ISpecimenBuilder relay)
    {
        Relay = relay;
    }

    public ISpecimenBuilder Relay { get; }

    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new MockPostprocessor(new AutoFixture.Kernel.MethodInvoker(new StrictMockConstructorQuery())));
        fixture.ResidueCollectors.Add(Relay);
    }
}

public class StrictMockConstructorMethod : IMethod
{
    private readonly ConstructorInfo ctor;
    private readonly ParameterInfo[] paramInfos;

    public StrictMockConstructorMethod(ConstructorInfo ctor, ParameterInfo[] paramInfos)
    {
        this.ctor = ctor;
        this.paramInfos = paramInfos;
    }

    public IEnumerable<ParameterInfo> Parameters => paramInfos;

    public object Invoke(IEnumerable<object> parameters) => ctor.Invoke(parameters?.ToArray() ?? new object[] { });
}

public class StrictMockConstructorQuery : IMethodQuery
{
    public IEnumerable<IMethod> SelectMethods(Type type)
    {
        if (!IsMock(type))
        {
            return Enumerable.Empty<IMethod>();
        }

        if (!GetMockedType(type).IsInterface && !IsDelegate(type))
        {
            return Enumerable.Empty<IMethod>();
        }

        var ctor = type.GetConstructor(new[] { typeof(MockBehavior) });

        return new IMethod[]
        {
            new StrictMockConstructorMethod(ctor, ctor.GetParameters())
        };
    }

    private static bool IsMock(Type type)
    {
        return type != null && type.IsGenericType && typeof(Mock<>).IsAssignableFrom(type.GetGenericTypeDefinition()) &&
               !GetMockedType(type).IsGenericParameter;
    }

    private static Type GetMockedType(Type type)
    {
        return type.GetGenericArguments().Single();
    }

    private static bool IsDelegate(Type type)
    {
        return typeof(MulticastDelegate).IsAssignableFrom(type.BaseType);
    }
}