using AngleSharp.Dom;
using AngleSharp.Diffing.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AngleSharp.Diffing.Core
{
    public readonly struct AttributeComparisonSource : IEquatable<AttributeComparisonSource>, IComparisonSource
    {
        /// <summary>
        /// Gets the attribute attached to this source.
        /// </summary>
        public IAttr Attribute { get; }

        /// <summary>
        /// Gets the element source this attribute source is related to.
        /// </summary>
        public ComparisonSource ElementSource { get; }

        /// <summary>
        /// Gets the path to the attribute in the source node tree.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the source type, e.g. if it is a test or control source.
        /// </summary>
        public ComparisonSourceType SourceType { get; }

        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "Path should be in lower case")]
        public AttributeComparisonSource(string attributeName, in ComparisonSource elementSource)
        {
            if (string.IsNullOrEmpty(attributeName)) throw new ArgumentNullException(nameof(attributeName));
            if (!elementSource.Node.TryGetAttr(attributeName, out var attribute))
                throw new ArgumentException("The comparison source does not contain an element or the specified attribute is missing on the element.", nameof(elementSource));

            Attribute = attribute;
            ElementSource = elementSource;
            SourceType = elementSource.SourceType;
            Path = $"{elementSource.Path}[{attribute.Name.ToLowerInvariant()}]";
        }

        #region Equals and HashCode
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool Equals(AttributeComparisonSource other) => Object.ReferenceEquals(Attribute, other.Attribute) && Path.Equals(other.Path, StringComparison.Ordinal) && ElementSource.Equals(other.ElementSource);
        public override int GetHashCode() => (Attribute, ElementSource).GetHashCode();
        public override bool Equals(object? obj) => obj is AttributeComparisonSource other && Equals(other);
        public static bool operator ==(AttributeComparisonSource left, AttributeComparisonSource right) => left.Equals(right);
        public static bool operator !=(AttributeComparisonSource left, AttributeComparisonSource right) => !left.Equals(right);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        #endregion
    }
}
