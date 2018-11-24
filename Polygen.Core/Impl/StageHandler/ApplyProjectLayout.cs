﻿using System.Collections.Generic;
using System.Linq;
using Polygen.Core.DesignModel;
using Polygen.Core.OutputConfiguration;
using Polygen.Core.Project;
using Polygen.Core.Stage;
using Polygen.Core.TargetPlatform;
using Polygen.Core.Utils;

namespace Polygen.Core.Impl.StageHandler
{
    /// <summary>
    /// Applies the configure project layout into the output model configurations of design models and namepaces.
    /// </summary>
    public class ApplyProjectLayout : StageHandlerBase
    {
        /// <summary>
        /// Cached list of all design models.
        /// </summary>
        private List<IDesignModel> _allDesignModels;

        public ApplyProjectLayout(): base(StageType.ApplyProjectLayout, "Core")
        {
        }

        public IDesignModelCollection DesignModelCollection { get; set; }
        public IProjectLayout ProjectLayout { get; set; }
        public IProjectCollection ProjectCollection { get; set; }
        public ITargetPlatformCollection TargetPlatformCollection { get; set; }

        public override void Execute()
        {
            foreach (var entry in ProjectLayout.Entries)
            {
                foreach (var outputConfiguration in GetOutputConfigurations(entry))
                {
                    UpdateOutputConfiguration(entry, outputConfiguration);
                }
            }
        }

        private void UpdateOutputConfiguration(ProjectLayoutEntry entry, IOutputConfiguration outputConfiguration)
        {
            if (entry.Outputs != null)
            {
                foreach (var output in entry.Outputs)
                {
                    outputConfiguration.RegisterOutputFolder(output.OutputModelTypeFilter, GetProjectFolder(output));
                }
            }

            if (entry.TargetPlatforms != null)
            {
                foreach (var targetPlatformEntry in entry.TargetPlatforms)
                {
                    var targetPlatform = TargetPlatformCollection.GetTargetPlatform(targetPlatformEntry.TargerPlatformName);

                    outputConfiguration.RegisterTargetPlatformForDesignModelType(targetPlatformEntry.DesignModelType, targetPlatform, true);
                }
            }
        }

        private IEnumerable<IOutputConfiguration> GetOutputConfigurations(ProjectLayoutEntry entry)
        {
            if (entry.LocationFilters == null)
            {
                yield return DesignModelCollection.RootNamespace.OutputConfiguration;
            }

            foreach (var locationFilter in entry.LocationFilters)
            {
                if (locationFilter.NamespaceFilter != null)
                {
                    foreach (var ns in DesignModelCollection.GetAllNamespaces())
                    {
                        if (locationFilter.NamespaceFilter.Match(ns.Name) == Filter.MatchStatus.Included)
                        {
                            yield return ns.OutputConfiguration;
                        }
                    }
                }
                else 
                {
                    foreach (var designModel in GetAllDesignModels())
                    {
                        if (locationFilter.DesignModelTypeFilter != null)
                        {
                            if (locationFilter.DesignModelTypeFilter.Match(designModel.DesignModelType) == Filter.MatchStatus.Included)
                            {
                                yield return designModel.OutputConfiguration;
                            }
                        }
                        else if (designModel.FullyQualifiedName != null)
                        {
                            if (locationFilter.DesignModelNameFilter.Match(designModel.FullyQualifiedName) == Filter.MatchStatus.Included)
                            {
                                yield return designModel.OutputConfiguration;
                            }
                        }
                    }
                }
            }
        }

        private IProjectFolder GetProjectFolder(ProjectLayoutOutput entry)
        {
            var project = default(IProject);
            
            if (!string.IsNullOrWhiteSpace(entry.ProjectType))
            {
                project = ProjectCollection.GetFirstProjectByType(entry.ProjectType);
            }
            else
            {
                project = ProjectCollection.GetProjectByName(entry.ProjectName);
            }

            return project.GetFolder(entry.FolderPath);
        }

        private IEnumerable<IDesignModel> GetAllDesignModels()
        {
            if (_allDesignModels == null)
            {
                _allDesignModels = DesignModelCollection.GetAllDesignModels().ToList();
            }

            return _allDesignModels;
        }
    }
}
