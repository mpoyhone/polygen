﻿using Polygen.Core.ClassModel;
using Polygen.Core.Parser;

namespace Polygen.Plugins.Base.Models.ClassModelMapping
{
    /// <summary>
    /// Defines a mapping from one class-like desing model attribute to another.
    /// </summary>
    public class AttributeMapping
    {
        /// <summary>
        /// Source design model attribute reference.
        /// </summary>
        public ClassAttributeReference Source { get; set; }
        
        /// <summary>
        /// Destination design model attribute reference.
        /// </summary>
        public ClassAttributeReference Destination { get; set; }
        
        /// <summary>
        /// Mapping element parse location for error messages.
        /// </summary>
        public IParseLocationInfo ParseLocation { get; set; }
    }
}