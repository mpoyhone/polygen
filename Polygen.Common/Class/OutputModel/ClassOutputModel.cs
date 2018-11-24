﻿using Polygen.Core.DesignModel;
using Polygen.Core.Project;
using Polygen.Core.Utils;
using System.Collections.Generic;
using System.Diagnostics;

namespace Polygen.Common.Class.OutputModel
{
    [DebuggerDisplay("ClassOutputModel: {ClassNamespace}.{ClassName}")]
    public class ClassOutputModel : Core.Impl.OutputModel.OutputModelBase
    {
        private readonly SortedSet<string> _namespaceImportSet = new SortedSet<string>();
        private readonly LazyList<Attribute> _attributeList = new LazyList<Attribute>();
        private readonly List<string> _modifierList = new List<string>();
        private readonly LazyList<Property> _fieldList = new LazyList<Property>();
        private readonly LazyList<Method> _constructorList = new LazyList<Method>();
        private readonly List<Property> _propertyList = new List<Property>();
        private readonly List<Method> _methodList = new List<Method>();

        public ClassOutputModel(string type, INamespace ns, IDesignModel designModel, IProjectFile file = null) : base(type, ns, designModel, file)
        {
        }

        public bool IsInterface { get; set; }
        public string ClassNamespace { get; set; }
        public string ClassName { get; set; }

        public ISet<string> NamespaceImports => _namespaceImportSet;
        public List<Attribute> Attributes => _attributeList.Value;
        public List<string> Modifiers => _modifierList;
        public List<Property> Fields => _fieldList.Value;
        public List<Method> Constructors => _constructorList.Value;
        public List<Property> Properties => _propertyList;
        public List<Method> Methods => _methodList;

        public string ModifiersString => string.Join(" ", _modifierList);
    }
}
