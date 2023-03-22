﻿namespace AngleSharp.Diffing.Core;

/// <summary>
/// Represents a result of a comparison.
/// </summary>
/// <param name="Decision">Gets the latest <see cref="CompareResultDecision"/> of the comparison.</param>
/// <param name="Diff">Gets the optional <see cref="IDiff"/> related to the current <paramref name="Decision"/>.</param>
public readonly record struct CompareResult(CompareResultDecision Decision, IDiff? Diff = null)
{
    /// <summary>
    /// Use when the compare result is unknown.
    /// </summary>
    public static readonly CompareResult Unknown = default;

    /// <summary>
    /// Use when the two compared nodes or attributes are the same.
    /// </summary>
    public static readonly CompareResult Same = new CompareResult(CompareResultDecision.Same);

    /// <summary>
    /// Use when the comparison should be skipped and any child-nodes or attributes skipped as well.
    /// </summary>
    public static readonly CompareResult Skip = new CompareResult(CompareResultDecision.Skip);

    /// <summary>
    /// Use when the comparison should skip any child-nodes.
    /// </summary>
    public static readonly CompareResult SkipChildren = new CompareResult(CompareResultDecision.SkipChildren);

    /// <summary>
    /// Use when the comparison should skip any attributes.
    /// </summary>
    public static readonly CompareResult SkipAttributes = new CompareResult(CompareResultDecision.SkipAttributes);

    /// <summary>
    /// Use when the two compared nodes or attributes are the different.
    /// </summary>
    public static CompareResult Different(IDiff? diff) => new CompareResult(CompareResultDecision.Different, diff);
}

/// <summary>
/// Represents the decision of a comparison.
/// </summary>
[Flags]
public enum CompareResultDecision
{
    /// <summary>
    /// Use when the compare result is unknown.
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// Use when the two compared nodes or attributes are the same.
    /// </summary>
    Same = 1,
    /// <summary>
    /// Use when the two compared nodes or attributes are the different.
    /// </summary>
    Different = 2,
    /// <summary>
    /// Use when the comparison should be skipped and any child-nodes or attributes skipped as well.
    /// </summary>
    Skip = 4,
    /// <summary>
    /// Use when the comparison should skip any child-nodes.
    /// </summary>
    SkipChildren = 8,
    /// <summary>
    /// Use when the comparison should skip any attributes.
    /// </summary>
    SkipAttributes = 16,
}

/// <summary>
/// Helper methods for <see cref="CompareResult"/>
/// </summary>
public static class CompareResultExtensions
{
    /// <summary>
    /// Checks if a <see cref="CompareResult"/> is either a <see cref="CompareResult.Same"/> or <see cref="CompareResult.Skip"/>.
    /// </summary>
    /// <param name="compareResult">The compare result</param>
    public static bool IsSameOrSkip(this CompareResult compareResult) => compareResult == CompareResult.Same || compareResult == CompareResult.Skip;
}

