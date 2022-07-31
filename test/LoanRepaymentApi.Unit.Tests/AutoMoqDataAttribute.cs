namespace LoanRepaymentApi.Tests;

using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

// Source: https://blog.ploeh.dk/2010/10/08/AutoDataTheorieswithAutoFixture/ & https://venugopalekambaram.wordpress.com/2019/08/23/steps-to-implement-autofixture-for-unit-tests-net-core/

public class AutoMoqDataAttribute : AutoDataAttribute
{
    private static readonly Func<IFixture> FixtureFactory = () => new Fixture().Customize(new AutoMoqCustomization());

    public AutoMoqDataAttribute()
        : base(FixtureFactory)
    {
    }
}