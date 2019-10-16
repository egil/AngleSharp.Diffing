﻿using Egil.AngleSharp.Diffing.Core;
using Shouldly;
using Xunit;

namespace Egil.AngleSharp.Diffing.Strategies.IgnoreStrategies
{
    public class IgnoreElementComparerTest : DiffingTestBase
    {
        [Theory(DisplayName = "When a control element does not contain the 'diff:ignore' attribute or it is 'diff:ignore=false', the current decision is returned")]
        [InlineData(@"<p></p>")]
        [InlineData(@"<p diff:ignore=""false""></p>")]
        [InlineData(@"<p diff:ignore=""FALSE""></p>")]
        [InlineData(@"<p diff:ignore=""faLsE""></p>")]
        public void Test001(string controlHtml)
        {
            var comparison = ToComparison(controlHtml, "<p></p>");

            IgnoreElementComparer.Compare(comparison, CompareResult.Different).ShouldBe(CompareResult.Different);
            IgnoreElementComparer.Compare(comparison, CompareResult.DifferentAndBreak).ShouldBe(CompareResult.DifferentAndBreak);
            IgnoreElementComparer.Compare(comparison, CompareResult.Same).ShouldBe(CompareResult.Same);
            IgnoreElementComparer.Compare(comparison, CompareResult.SameAndBreak).ShouldBe(CompareResult.SameAndBreak);
        }

        [Theory(DisplayName = "When a control element has 'diff:ignore' attribute, SameAndBreak is returned")]
        [InlineData(@"<p diff:ignore></p>")]
        [InlineData(@"<p diff:ignore=""true""></p>")]
        [InlineData(@"<p diff:ignore=""TRUE""></p>")]
        [InlineData(@"<p diff:ignore=""TrUe""></p>")]
        public void Test002(string controlHtml)
        {
            var comparison = ToComparison(controlHtml, "<p></p>");

            IgnoreElementComparer.Compare(comparison, CompareResult.Different).ShouldBe(CompareResult.SameAndBreak);
        }
    }
}
