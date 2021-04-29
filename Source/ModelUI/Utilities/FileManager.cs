using ModelUI.Models;
using Scm.Focus.Utils.ModelGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelUI.Utilities
{
    public static class FileManager
    {
        private const string DefinitionsPathAppConfigNode = "definitionPath";
        private const string EntityDefinitionsPathAppConfigNode = "entityDomainsPath";
        private const string DomainsDefinitionsPathAppConfigNode = "domainsPath";
        private const string PrefixDomainsDefinitions = "Scm.Focus.Domain.";
        private const string PrefixInfrastructureDefinitions = "Scm.Focus.Infrastructure.Xrm.";
        private const string ExcludeDomainsWords = "Common;";
        private const string CommonFileName = "common.json";


        //templates
        private const string TemplatesPathAppConfigNode = "templatesPath";
        private const string TemplateDomainSharedProjectShProjConfigNode = "templateDomainSharedProjectShProj";
        private const string TemplateDomainSharedProjectProjItemsConfigNode = "templateDomainSharedProjectProjItems";
        private const string TemplateInfrastructureSharedProjectShProjConfigNode = "templateInfrastructureSharedProjectShProj";
        private const string TemplateInfrastructureSharedProjectProjItemsConfigNode = "templateInfrastructureSharedProjectProjItems";
        public enum TemplateType
        {
            DomainSharedProjectShProj = 1,
            DomainSharedProjectProjItems = 2,
            InfrastructureSharedProjectShProj = 3,
            InfrastructureSharedProjectProjItems = 4,
        }



        public static void DeleteDomain(string domainName)
        {
            var directoriesPath = SettingsManager.GetAppConfig(DomainsDefinitionsPathAppConfigNode);
            string domainFolder = $"{PrefixDomainsDefinitions}{domainName}";
            string infrastructureFolder = $"{PrefixInfrastructureDefinitions}{domainName}";
            Directory.Delete($@"{directoriesPath}\{domainFolder}", true);
            Directory.Delete($@"{directoriesPath}\{infrastructureFolder}", true);
        }

        public static void CreateNewDomain(string domainName)
        {
            var replacements = new Dictionary<string, string>();
            replacements.Add("DomainName", domainName);
            replacements.Add("DomainSharedProjectId", Guid.NewGuid().ToString());
            replacements.Add("DomainSharedProjectProjItemsId", Guid.NewGuid().ToString());
            replacements.Add("InfrastructureSharedProjectId", Guid.NewGuid().ToString());
            replacements.Add("InfrastructureSharedProjectProjItemsId", Guid.NewGuid().ToString());

            var sharedDomainProjectShProjReplacedText =
                FileManager.GetTemplateWithReplacements(FileManager.TemplateType.DomainSharedProjectShProj, replacements);
            var sharedDomainProjectProjItemsReplacedText =
                 FileManager.GetTemplateWithReplacements(FileManager.TemplateType.DomainSharedProjectProjItems, replacements);
            var sharedInfrastructureProjectShProjReplacedText =
                FileManager.GetTemplateWithReplacements(FileManager.TemplateType.InfrastructureSharedProjectShProj, replacements);
            var sharedInfrastructureProjectProjItemsReplacedText =
                 FileManager.GetTemplateWithReplacements(FileManager.TemplateType.InfrastructureSharedProjectProjItems, replacements);

            var directoriesPath = SettingsManager.GetAppConfig(DomainsDefinitionsPathAppConfigNode);
            string newDomainFolder = $"{PrefixDomainsDefinitions}{domainName}";
            string domainModelsFolder = $@"{newDomainFolder}\Models";
            string domainRepositoriesFolder = $@"{newDomainFolder}\Repositories";
            string domainServicesFolder = $@"{newDomainFolder}\Services";
            string newInfrastructureFolder = $"{PrefixInfrastructureDefinitions}{domainName}";
            string infrastructureRepositoriesFolder = $@"{newInfrastructureFolder}\Repositories";
            Directory.CreateDirectory($@"{directoriesPath}\{newDomainFolder}");
            Directory.CreateDirectory($@"{directoriesPath}\{domainModelsFolder}");
            Directory.CreateDirectory($@"{directoriesPath}\{domainRepositoriesFolder}");
            Directory.CreateDirectory($@"{directoriesPath}\{domainServicesFolder}");
            Directory.CreateDirectory($@"{directoriesPath}\{domainServicesFolder}\Implementations");
            Directory.CreateDirectory($@"{directoriesPath}\{newInfrastructureFolder}");
            Directory.CreateDirectory($@"{directoriesPath}\{infrastructureRepositoriesFolder}");

            File.WriteAllText($@"{directoriesPath}\{newDomainFolder}\{newDomainFolder}.shproj", sharedDomainProjectShProjReplacedText);
            File.WriteAllText($@"{directoriesPath}\{newDomainFolder}\{newDomainFolder}.projitems", sharedDomainProjectProjItemsReplacedText);
            File.WriteAllText($@"{directoriesPath}\{newInfrastructureFolder}\{newInfrastructureFolder}.shproj", sharedInfrastructureProjectShProjReplacedText);
            File.WriteAllText($@"{directoriesPath}\{newInfrastructureFolder}\{newInfrastructureFolder}.projitems", sharedInfrastructureProjectProjItemsReplacedText);
        }


        public static string GetTemplateWithReplacements(TemplateType template, Dictionary<string, string> replacementParameters)
        {
            var definitionsPath = SettingsManager.GetAppConfig(DefinitionsPathAppConfigNode);
            var tempaltesDefinitionsPath = SettingsManager.GetAppConfig(TemplatesPathAppConfigNode);

            var fileName = GetFilenameForTemplateType(template);
            var path = $@"{definitionsPath}\{tempaltesDefinitionsPath}\{fileName}";

            var contentWithoutReplacements = File.ReadAllText(path);
            var contentReplaced = contentWithoutReplacements;
            foreach (var item in replacementParameters)
            {
                contentReplaced = contentReplaced.Replace($"[[{item.Key}]]", item.Value);
            }
            return contentReplaced;
        }


        private static string GetFilenameForTemplateType(TemplateType type)
        {
            if (type == TemplateType.DomainSharedProjectShProj)
            {
                return SettingsManager.GetAppConfig(TemplateDomainSharedProjectShProjConfigNode);
            }
            else if (type == TemplateType.DomainSharedProjectProjItems)
            {
                return SettingsManager.GetAppConfig(TemplateDomainSharedProjectProjItemsConfigNode);
            }
            else if (type == TemplateType.InfrastructureSharedProjectShProj)
            {
                return SettingsManager.GetAppConfig(TemplateInfrastructureSharedProjectShProjConfigNode);
            }
            else if (type == TemplateType.InfrastructureSharedProjectProjItems)
            {
                return SettingsManager.GetAppConfig(TemplateInfrastructureSharedProjectProjItemsConfigNode);
            }
            return null;
        }

        public static string GetDestionationDomainPath(string domainName)
        {
            var basePath = SettingsManager.GetAppConfig(DomainsDefinitionsPathAppConfigNode);
            return $@"{basePath}\{PrefixDomainsDefinitions}{domainName}";
        }

        public static List<Domain> GetDomains()
        {
            List<Domain> domains = new List<Domain>();
            var directoriesPath = SettingsManager.GetAppConfig(DomainsDefinitionsPathAppConfigNode);
            var directories = Directory.GetDirectories(directoriesPath);

            var domainsPath = directories.Where(k => IsDomainDirectory(Path.GetFileName(k))).ToList();
            var infrastructurePath = directories.Where(k => IsInfrastructureDirectory(Path.GetFileName(k))).ToList();

            foreach (var domain in domainsPath)
            {
                Domain d = new Domain(GetDomainName(Path.GetFileName(domain)), domain);
                bool hasInfrastructure = DomainHasInfrastructure(domain, infrastructurePath);
                if (hasInfrastructure)
                {
                    var infrastructure = GetDomainInfrastructure(domain, infrastructurePath);
                    d.InfrastructurePath = infrastructure;
                }

                var servicesPath = $@"{domain}\Services";

                domains.Add(d);
            }

            return domains;
        }

        private static string GetDomainInfrastructure(string domainName, List<string> infrastructures)
        {
            var fileName = GetDomainName(Path.GetFileName(domainName));
            return infrastructures.FirstOrDefault(k =>
            {
                var pathFileName = Path.GetFileName(k);
                return pathFileName.Substring(pathFileName.Length - fileName.Length) == fileName;
            });
        }

        private static bool DomainHasInfrastructure(string domainName, List<string> infrastructures)
        {
            return GetDomainInfrastructure(domainName, infrastructures) != null;
        }

        private static string GetDomainName(string pathName)
        {
            return pathName.Substring(PrefixDomainsDefinitions.Length);
        }

        private static bool IsDomainDirectory(string pathName)
        {
            if (pathName.Length < PrefixDomainsDefinitions.Length)
            {
                return false;
            }
            var prefix = pathName.Substring(0, PrefixDomainsDefinitions.Length);
            var sufix = pathName.Substring(PrefixDomainsDefinitions.Length);
            if (prefix == PrefixDomainsDefinitions
                && sufix.IndexOf(".") == -1
                && ExcludeDomainsWords.Split(';').ToList().IndexOf(sufix) == -1)
            {
                return true;
            }
            return false;
        }

        private static bool IsInfrastructureDirectory(string pathName)
        {
            if (pathName.Length < PrefixInfrastructureDefinitions.Length)
            {
                return false;
            }
            var prefix = pathName.Substring(0, PrefixInfrastructureDefinitions.Length);
            var sufix = pathName.Substring(PrefixInfrastructureDefinitions.Length);
            if (prefix == PrefixInfrastructureDefinitions
                && sufix.IndexOf(".") == -1
                //&& ExcludeDomainsWords.Split(';').ToList().IndexOf(sufix) == -1
                )
            {
                return true;
            }
            return false;
        }



        public static EntityMappingSettings GetCommonMappingSettings()
        {
            EntityMappingSettings target = new EntityMappingSettings();

            var definitionsPath = SettingsManager.GetAppConfig(DefinitionsPathAppConfigNode);
            var entityDefinitionsPath = SettingsManager.GetAppConfig(EntityDefinitionsPathAppConfigNode);

            var path = $@"{definitionsPath}\{entityDefinitionsPath}";
            var files = Directory.GetFiles(path);

            var commonFile = files.FirstOrDefault(k => Path.GetFileName(k) == CommonFileName);
            if (commonFile != null)
            {
                target = ParserJson.ParseEntityMappingSetting(File.ReadAllText(commonFile));
            }

            return target;
        }


        public static EntityMappingSettings GetEntitiesDefinitionsEntityMappingSettings(string entityLogicalName)
        {
            EntityMappingSettings target = new EntityMappingSettings();

            var definitionsPath = SettingsManager.GetAppConfig(DefinitionsPathAppConfigNode);
            var entityDefinitionsPath = SettingsManager.GetAppConfig(EntityDefinitionsPathAppConfigNode);
            var path = $@"{definitionsPath}\{entityDefinitionsPath}";

            var files = Directory.GetFiles(path);

            var file = files.FirstOrDefault(k => Path.GetFileName(k) == $"{entityLogicalName}.json");
            if (file != null)
            {
                target = ParserJson.ParseEntityMappingSetting(File.ReadAllText(file));
            }

            return target;
        }


        public static void DeleteEntitiesDefinitionsTargetEntity(EntityTarget entity)
        {
            var definitionsPath = SettingsManager.GetAppConfig(DefinitionsPathAppConfigNode);
            var entityDefinitionsPath = SettingsManager.GetAppConfig(EntityDefinitionsPathAppConfigNode);
            var folder = $@"{definitionsPath}\{entityDefinitionsPath}";

            var fileName = $"{entity.LogicalName}.json";
            string path = $"{folder}\\{fileName}";
            File.Delete(path);
        }

        public static void AddNewTargetEntity(EntityTarget entity)
        {
            var definitionsPath = SettingsManager.GetAppConfig(DefinitionsPathAppConfigNode);
            var entityDefinitionsPath = SettingsManager.GetAppConfig(EntityDefinitionsPathAppConfigNode);
            var folder = $@"{definitionsPath}\{entityDefinitionsPath}";

            var fileName = $"{entity.LogicalName}.json";
            var json = ParserJson.Stringfy(entity);
            string path = $"{folder}\\{fileName}";
            File.WriteAllText(path, json);

        }


        public static void UpdateEntitiesDefinitionsTargetEntity(EntityTarget entity)
        {
            var definitionsPath = SettingsManager.GetAppConfig(DefinitionsPathAppConfigNode);
            var entityDefinitionsPath = SettingsManager.GetAppConfig(EntityDefinitionsPathAppConfigNode);
            var folder = $@"{definitionsPath}\{entityDefinitionsPath}";

            var fileName = $"{entity.LogicalName}.json";
            string path = $"{folder}\\{fileName}";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            AddNewTargetEntity(entity);
        }


        public static void UpdateEntityDefinitionsCommonDataTarget(CommonTarget data)
        {

            var definitionsPath = SettingsManager.GetAppConfig(DefinitionsPathAppConfigNode);
            var entityDefinitionsPath = SettingsManager.GetAppConfig(EntityDefinitionsPathAppConfigNode);
            var folder = $@"{definitionsPath}\{entityDefinitionsPath}";

            var fileName = CommonFileName;
            string path = $"{folder}\\{fileName}";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllText(path, ParserJson.Stringfy(data));
        }

        public static List<EntityTarget> GetEntitiesDefinitionsTargetEntities()
        {
            List<EntityTarget> entities = new List<EntityTarget>();

            var definitionsPath = SettingsManager.GetAppConfig(DefinitionsPathAppConfigNode);
            var entityDefinitionsPath = SettingsManager.GetAppConfig(EntityDefinitionsPathAppConfigNode);
            var folder = $@"{definitionsPath}\{entityDefinitionsPath}";

            var files = Directory.GetFiles(folder);

            foreach (var file in files.Where(k => Path.GetFileName(k) != CommonFileName))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var entity = ParserJson.ParseEntity(File.ReadAllText(file));
                entity.LogicalName = fileName;
                entities.Add(entity);
            }

            return entities;
        }

        public static CommonTarget GetEntitiesDefinitionsCommonTarget()
        {
            CommonTarget target = new CommonTarget();

            var definitionsPath = SettingsManager.GetAppConfig(DefinitionsPathAppConfigNode);
            var entityDefinitionsPath = SettingsManager.GetAppConfig(EntityDefinitionsPathAppConfigNode);
            var folder = $@"{definitionsPath}\{entityDefinitionsPath}";

            var files = Directory.GetFiles(folder);

            var commonFile = files.FirstOrDefault(k => Path.GetFileName(k) == CommonFileName);
            if (commonFile != null)
            {
                target = ParserJson.ParseCommonTarget(File.ReadAllText(commonFile));
            }

            return target;
        }




    }
}
