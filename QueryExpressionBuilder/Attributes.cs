namespace QueryExpressionBuilder.Attributes
{
    /// <summary>
    /// Attribute to filtration strings
    /// </summary>
    public class String
    {
        
        /// <summary>
        /// Filtration func StartWith
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class StartWithAttribute : BasePredicateAttribute
        {
            /// <summary>
            /// Add attribute func StartWith
            /// </summary>
            /// <param name="propertyName">Name of property in DB</param>
            public StartWithAttribute(string propertyName) : base(propertyName)
            {
            }
        }
    }

    /// <summary>
    /// Attribute to filtration numbers and DateTime
    /// </summary>
    public class Numbers
    {
        /// <summary>
        /// Filtration func >=
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class GreaterOrEqualAttribute : BasePredicateAttribute
        {
            /// <summary>
            /// Add attribute func >=
            /// </summary>
            public GreaterOrEqualAttribute(string propertyName) : base(propertyName)
            {
            }
        }

        /// <summary>
        /// Filtration func <=
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class LessOrEqualAttribute : BasePredicateAttribute
        {
            /// <summary>
            /// Add attribute func <=
            /// </summary>
            public LessOrEqualAttribute(string propertyName) : base(propertyName)
            {
            }
        }
    }

    /// <summary>
    /// Base filtration class
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class BasePredicateAttribute : Attribute
    {
        public string PropertyName { get; set; }

        public BasePredicateAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
