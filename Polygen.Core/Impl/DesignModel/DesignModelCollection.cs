﻿using Polygen.Core.DesignModel;
using System.Collections.Generic;
using System.Linq;
using Polygen.Core.Exceptions;
using Polygen.Core.Parser;
using Polygen.Core.Utils;

namespace Polygen.Core.Impl.DesignModel
{
    public class DesignModelCollection : IDesignModelCollection
    {
        private readonly Dictionary<string, INamespace> _namespaceMap = new Dictionary<string, INamespace>();
        private readonly ListDictionary<string, IDesignModel> _designModelsByTypeMap = new ListDictionary<string, IDesignModel>();

        public DesignModelCollection()
        {
            RootNamespace = new Namespace(new OutputConfiguration.OutputConfiguration(null));
        }

        public INamespace RootNamespace { get; }

        public INamespace DefineNamespace(string ns)
        {
            if (!_namespaceMap.TryGetValue(ns, out var res))
            {
                var parts = ns.Split('.');
                var parent = RootNamespace;

                foreach (var part in parts)
                {
                    var name = parent?.Name != null ? $"{parent.Name}.{part}" : part;

                    if (!_namespaceMap.TryGetValue(name, out var segment))
                    {
                        segment = new Namespace(part, parent);
                        ((Namespace)parent).AddChild(segment);
                        _namespaceMap.Add(segment.Name, segment);
                    }

                    parent = segment;
                }

                res = parent;
            }

            return res;
        }

        public IEnumerable<IDesignModel> GetByType(string type)
        {
            return _designModelsByTypeMap.GetOrEmpty(type);
        }

        public INamespace GetNamespace(string name, IParseLocationInfo parseLocation = null)
        {
            if (!_namespaceMap.TryGetValue(name, out var res))
            {
                if (parseLocation != null)
                {
                    throw new ParseException(parseLocation, $"Namespace '{name}' not found");
                }
            }

            return res;
        }

        public IEnumerable<INamespace> GetAllNamespaces()
        {
            return _namespaceMap.Values;
        }

        public void AddDesignModel(IDesignModel designModel)
        {
            _designModelsByTypeMap.Add(designModel.DesignModelType, designModel);
        }

        public IEnumerable<IDesignModel> GetAllDesignModels()
        {
            return _designModelsByTypeMap
                .Values
                .SelectMany(x => x);
        }
    }
}
