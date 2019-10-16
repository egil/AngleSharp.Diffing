﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Egil.AngleSharp.Diffing.Core;

namespace Egil.AngleSharp.Diffing
{

    public delegate FilterDecision FilterStrategy<TSource>(in TSource source, FilterDecision currentDecision);
    public delegate IEnumerable<TComparison> MatchStrategy<in TSources, out TComparison>(DiffContext context, TSources controlSources, TSources testSources);
    public delegate CompareResult CompareStrategy<TComparison>(in TComparison comparison, CompareResult currentDecision);

    public class DiffingStrategyPipeline : IFilterStrategy, IMatcherStrategy, ICompareStrategy
    {
        private readonly List<FilterStrategy<ComparisonSource>> _nodeFilters = new List<FilterStrategy<ComparisonSource>>();
        private readonly List<FilterStrategy<AttributeComparisonSource>> _attrsFilters = new List<FilterStrategy<AttributeComparisonSource>>();
        private readonly List<MatchStrategy<SourceCollection, Comparison>> _nodeMatchers = new List<MatchStrategy<SourceCollection, Comparison>>();
        private readonly List<MatchStrategy<SourceMap, AttributeComparison>> _attrsMatchers = new List<MatchStrategy<SourceMap, AttributeComparison>>();
        private readonly List<CompareStrategy<Comparison>> _nodeComparers = new List<CompareStrategy<Comparison>>();
        private readonly List<CompareStrategy<AttributeComparison>> _attrComparers = new List<CompareStrategy<AttributeComparison>>();

        public FilterDecision Filter(in ComparisonSource comparisonSource) => Filter(comparisonSource, _nodeFilters);
        public FilterDecision Filter(in AttributeComparisonSource attributeComparisonSource) => Filter(attributeComparisonSource, _attrsFilters);

        public IEnumerable<Comparison> Match(DiffContext context, SourceCollection controlSources, SourceCollection testSources)
        {
            foreach (var matcher in _nodeMatchers)
            {
                foreach (var comparison in matcher(context, controlSources, testSources))
                {
                    controlSources.MarkAsMatched(comparison.Control);
                    testSources.MarkAsMatched(comparison.Test);
                    yield return comparison;
                }
            }
        }
            
        public IEnumerable<AttributeComparison> Match(DiffContext context, SourceMap controlAttrSources, SourceMap testAttrSources)
        {
            foreach (var matcher in _attrsMatchers)
            {
                foreach (var comparison in matcher(context, controlAttrSources, testAttrSources))
                {
                    controlAttrSources.MarkAsMatched(comparison.Control);
                    testAttrSources.MarkAsMatched(comparison.Test);
                    yield return comparison;
                }
            }
        }

        public CompareResult Compare(in Comparison comparison) 
            => Compare(comparison, _nodeComparers, CompareResult.DifferentAndBreak);
        public CompareResult Compare(in AttributeComparison comparison)
            => Compare(comparison, _attrComparers, CompareResult.Different);

        public void AddFilter(FilterStrategy<ComparisonSource> filterStrategy) => _nodeFilters.Add(filterStrategy);
        public void AddFilter(FilterStrategy<AttributeComparisonSource> filterStrategy) => _attrsFilters.Add(filterStrategy);

        public void AddMatcher(MatchStrategy<SourceCollection, Comparison> matchStrategy) => _nodeMatchers.Add(matchStrategy);
        public void AddMatcher(MatchStrategy<SourceMap, AttributeComparison> matchStrategy) => _attrsMatchers.Add(matchStrategy);

        public void AddComparer(CompareStrategy<Comparison> compareStrategy) => _nodeComparers.Add(compareStrategy);
        public void AddComparer(CompareStrategy<AttributeComparison> compareStrategy) => _attrComparers.Add(compareStrategy);

        private FilterDecision Filter<T>(in T source, List<FilterStrategy<T>> filterStrategies)
        {
            var result = FilterDecision.Keep;
            for (int i = 0; i < filterStrategies.Count; i++)
            {
                result = filterStrategies[i](source, result);
            }
            return result;
        }

        private CompareResult Compare<TComparison>(in TComparison comparison, List<CompareStrategy<TComparison>> compareStrategies, CompareResult initialResult)
        {
            var result = initialResult;
            foreach (var comparer in compareStrategies)
            {
                result = comparer(comparison, result);
            }
            return result;
        }
    }
}
