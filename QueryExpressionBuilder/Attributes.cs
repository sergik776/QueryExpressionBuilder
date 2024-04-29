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

        /// <summary>
        /// Filtration by contains function
        /// </summary>
        public class ContainsAttribute : BasePredicateAttribute
        {
            public ContainsAttribute(string propertyName) : base (propertyName)
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
        /// Filtration func GreaterOrEqual
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
        /// Filtration func LessOrEqual
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

        /// <summary>
        /// Filtration by equals function
        /// </summary>
        public class EqualsAttribute : BasePredicateAttribute
        {
            public EqualsAttribute(string propertyName) : base(propertyName)
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
        /// <summary>
        /// Name of property in DB
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Constructor with name of property in DB
        /// </summary>
        /// <param name="propertyName"></param>
        public BasePredicateAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
