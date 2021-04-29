using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Scm.Focus.Domain.Common.Models;
using Scm.Focus.Utils.ModelGenerator.Models;
using Scm.Focus.Utils.ModelGenerator.Utilities;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Focus.Utils.ModelGenerator
{
    public class ModelManager
    {

        public string StringConnection { get; set; }
        //public string EntityLogicalName { get; set; }
        public IOrganizationService Service { get; set; }
        public EntityMappingSettings CommonSettings { get; set; }

        //public List<string> AddedAttributes { get; set; }
        //public List<string> AddedEnums { get; set; }

        private const string GlobalNamespace = "Scm.Focus.Domain.{0}.Models";
        private const string ImportedNamespaceCollections = "System.Collections.Generic";
        private const string ImportedNamespaceDomainCommon = "Scm.Focus.Domain.Common";
        private const string ImportedNamespaceDomainCommonModels = "Scm.Focus.Domain.Common.Models";
        private const string DerivedClassType = "DomainEntity";
        private const string EnumDefinitionType = "EnumDefinitions";
        private const string StateDefinitionType = "StateDefinitions";
        private const string AttributeDefinitionType = "AttributeDefinitions";
        private const string EntityDefinitionsType = "EntityDefinitions";
        private const string EntitiesDefinitionType = "EntitiesDefinitions";
        private const string RelationshipDefinitionType = "RelationshipDefinitions";
        private const string OneToManyRelationshipDefinitionType = "OneToManyRelationships";
        private const string ManyToOneRelationshipDefinitionType = "ManyToOneRelationships";
        private const string ManyToManyRelationshipDefinitionType = "ManyToManyRelationships";
        private const string ActivityPointerLogicalName = "activitypointer";
        private const string RolesDefinitionType = "RolesDefinitions";
        private const string RolesDefinitionStructName = "RoleNames";

        public ModelManager(string stringConnection)
            : this(stringConnection, null) { }

        public ModelManager(string stringConnection, EntityMappingSettings commonSettings)
        {
            this.StringConnection = stringConnection ??
                throw new NullReferenceException(nameof(stringConnection));

            this.CommonSettings = commonSettings;

            this.Service = CrmConnection.GetService(stringConnection) ??
                 throw new Exception("Cannot connect to CRM with the given string connection");
        }


        public void GenerateRolesDefinitions()
        {
            var targetUnit = new CodeCompileUnit();

            CodeNamespace @namespace = new CodeNamespace(ImportedNamespaceDomainCommon);
            CodeTypeDeclaration targetClass = GetTargetClassEnums(RolesDefinitionType);

            @namespace.Types.Add(targetClass);
            targetUnit.Namespaces.Add(@namespace);

            var roles = MetadataProvider.GetRolesNames(this.Service);
            CodeTypeDeclaration entitiesStructDefinition = new CodeTypeDeclaration(RolesDefinitionStructName);
            entitiesStructDefinition.TypeAttributes = TypeAttributes.Public;
            entitiesStructDefinition.IsStruct = true;
            CompleteRolesDefinition(roles, entitiesStructDefinition, CommonSettings);
            targetClass.Members.Add(entitiesStructDefinition);

            var outputFile = CommonSettings?.GlobalRolesDefinitionOutputFile ?? $"{RolesDefinitionType}.cs";
            GenerateCSharpCode(outputFile, targetUnit);
        }


        public void GenerateEntitiesDefinitions()
        {
            var targetUnit = new CodeCompileUnit();

            CodeNamespace @namespace = new CodeNamespace(ImportedNamespaceDomainCommon);
            CodeTypeDeclaration targetClass = GetTargetClassEnums(EntityDefinitionsType);

            @namespace.Types.Add(targetClass);
            targetUnit.Namespaces.Add(@namespace);

            var entities = MetadataProvider.GetEntitiesMetadata(this.Service);
            CodeTypeDeclaration entitiesStructDefinition = new CodeTypeDeclaration(EntitiesDefinitionType);
            entitiesStructDefinition.TypeAttributes = TypeAttributes.Public;
            entitiesStructDefinition.IsStruct = true;
            CompleteEntitiesDefinition(entities, entitiesStructDefinition, CommonSettings);
            targetClass.Members.Add(entitiesStructDefinition);

            var outputFile = CommonSettings?.GlobalEntitiesDefinitionOutputFile ?? $"{EntitiesDefinitionType}.cs";
            GenerateCSharpCode(outputFile, targetUnit);
        }


        public void GenerateGlobalEnums()
        {
            var addedEnums = new List<string>();
            var targetUnit = new CodeCompileUnit();

            CodeNamespace @namespace = new CodeNamespace(ImportedNamespaceDomainCommon);
            CodeTypeDeclaration targetClass = GetTargetClassEnums(EnumDefinitionType);

            @namespace.Types.Add(targetClass);
            targetUnit.Namespaces.Add(@namespace);

            var enums = MetadataProvider.GetGlobalEnumsMetadata(this.Service);
            var orderedEnums = GetOrderedEnumMetadataList(enums, CommonSettings);
            foreach (OptionSetMetadataBase item in enums)
            {
                ProcessOptionSetMetadata(item, targetClass, addedEnums);
            }

            var outputFile = CommonSettings?.GlobalEnumsOutputFile ?? $"{EnumDefinitionType}.cs";
            GenerateCSharpCode(outputFile, targetUnit);
        }



        public void GenerateEntityModel(string entityLogicalName, EntityMappingSettings entitySettings)
        {

            var targetUnit = new CodeCompileUnit();

            var settings = MergeSettings(entitySettings, CommonSettings);
            var metadata = MetadataProvider.GetEntityMetadata(this.Service, entityLogicalName);
            var pluralName = GetPluralEntityName(entityLogicalName, settings);

            CodeNamespace @namespace = GetNamespace(pluralName);
            CodeTypeDeclaration targetClass = GetTargetClassDomainEntity(entityLogicalName, settings);

            @namespace.Types.Add(targetClass);
            targetUnit.Namespaces.Add(@namespace);


            AddConstructor(targetClass, entityLogicalName, metadata);
            AddGetterPrimaryAttributeId(targetClass, entityLogicalName, metadata);
            AddEntityReferenceMappings(targetClass, entityLogicalName, metadata);
            AddProperties(targetClass, metadata, settings, metadata.IsActivity.HasValue && metadata.IsActivity.Value);

            var outputFile = settings?.OutputFile ?? $"{entityLogicalName}.cs";
            GenerateCSharpCode(outputFile, targetUnit);
        }

        private void ProcessOptionSetMetadata(OptionSetMetadataBase item, CodeTypeDeclaration targetClass, List<string> addedEnums)
        {
            var name = item.Name;
            var preparedEnum = GetEnumName(item.Name, CommonSettings);
            if (addedEnums.IndexOf(preparedEnum) > -1)
            {
                preparedEnum = GetEnumName(item.Name, CommonSettings, true);
            }
            addedEnums.Add(preparedEnum);

            CodeTypeDeclaration enumerable = new CodeTypeDeclaration(preparedEnum);
            enumerable.Comments.Add(new CodeCommentStatement("<summary>", true));
            enumerable.Comments.Add(new CodeCommentStatement(item.Description?.UserLocalizedLabel?.Label, true));
            enumerable.Comments.Add(new CodeCommentStatement("</summary>", true));
            enumerable.IsEnum = true;


            var addedOptions = new List<string>();
            OptionMetadataCollection options = null;
            if (item is OptionSetMetadata)
            {
                options = ((OptionSetMetadata)item).Options;
                foreach (var option in options)
                {
                    string prefixed = GetPreparedName(addedOptions, option.Label.UserLocalizedLabel.Label);
                    CodeMemberField f = new CodeMemberField()
                    {
                        Name = prefixed,
                        InitExpression = new CodePrimitiveExpression(option.Value)
                    };
                    addedOptions.Add(prefixed);
                    enumerable.Members.Add(f);
                }
            }
            else if (item is BooleanOptionSetMetadata)
            {
                //Do nothing
            }

            targetClass.Members.Add(enumerable);
        }

        private string GetPreparedName(List<string> addedOptions, string optionLabel)
        {
            var rawName = optionLabel;
            var replacedSpecial = ReplaceSpecialCharacters(rawName);
            var capitalized = CapitalizeSentence(replacedSpecial);
            var normalized = NormalizeString(capitalized);
            var prefixed = normalized;
            if (normalized.Length > 0 && Char.IsDigit(normalized[0]))
            {
                prefixed = $"_{normalized}";
            }
            int counter = 1;
            while (addedOptions.IndexOf(prefixed) > -1)
            {
                prefixed = $"{prefixed}{counter++}";
            }

            return prefixed;
        }

        private static CodeNamespace GetNamespace(string pluralName)
        {
            CodeNamespace @namespace = new CodeNamespace(string.Format(GlobalNamespace, pluralName));
            @namespace.Imports.Add(new CodeNamespaceImport(ImportedNamespaceCollections));
            @namespace.Imports.Add(new CodeNamespaceImport(ImportedNamespaceDomainCommon));
            @namespace.Imports.Add(new CodeNamespaceImport(ImportedNamespaceDomainCommonModels));
            return @namespace;
        }

        private CodeTypeDeclaration GetTargetClassEnums(string enumName)
        {
            CodeTypeDeclaration targetClass = new CodeTypeDeclaration(enumName);
            targetClass.IsClass = true;
            targetClass.TypeAttributes = TypeAttributes.Public;
            return targetClass;
        }


        private CodeTypeDeclaration GetTargetClassDomainEntity(string entityLogicalName, EntityMappingSettings settings)
        {
            CodeTypeDeclaration targetClass = new CodeTypeDeclaration(GetDomainName(entityLogicalName, settings));
            targetClass.IsClass = true;
            targetClass.IsPartial = true;
            targetClass.TypeAttributes = TypeAttributes.Public;
            targetClass.BaseTypes.Add(new CodeTypeReference(DerivedClassType));
            return targetClass;
        }

        public void AddEntityReferenceMappings(CodeTypeDeclaration targetClass,
                                               string logicalName,
                                               EntityMetadata metadata)
        {

            CodeMemberMethod methodAdd = new CodeMemberMethod();
            methodAdd.Attributes =
                MemberAttributes.Private;
            methodAdd.Name = "AddEntityReferenceMapping";
            var fieldAttributeName = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "attributeName");
            var fieldEntityName = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "entityLogicalName");
            methodAdd.Parameters.Add(fieldAttributeName);
            methodAdd.Parameters.Add(fieldEntityName);
            methodAdd.Statements.Add(new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    "EntityReferenceMapping.Add",
                    new CodeArgumentReferenceExpression("attributeName"),
                    new CodeArgumentReferenceExpression("entityLogicalName")));

            CodeMemberMethod methodInitialize = new CodeMemberMethod();
            methodInitialize.Attributes =
                MemberAttributes.Private;
            methodInitialize.Name = "InitializeEntityReferenceMappings";

            foreach (var item in metadata.Attributes.Where(
                k => k.AttributeType == AttributeTypeCode.Lookup))
            {
                var typed = (LookupAttributeMetadata)item;
                var atrributeName = typed.LogicalName;
                var relatedEntity = typed.EntityLogicalName;

                methodInitialize.Statements.Add(new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    "AddEntityReferenceMapping",
                    new CodePrimitiveExpression(atrributeName),
                    new CodePrimitiveExpression(relatedEntity)));
            }

            targetClass.Members.Add(methodInitialize);
            targetClass.Members.Add(methodAdd);
        }

        public void AddConstructor(CodeTypeDeclaration targetClass, string logicalName, EntityMetadata metadata)
        {
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes =
                MemberAttributes.Public;
            constructor.BaseConstructorArgs.Add(new CodePrimitiveExpression(logicalName));
            targetClass.Members.Add(constructor);

            constructor.Statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(),
                    "InitializeEntityReferenceMappings"));
            constructor.Statements.Add(
                new CodeAssignStatement(
                    new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "PrimaryIdAttribute"),
                    new CodePrimitiveExpression(metadata.PrimaryIdAttribute)));
        }


        public void AddGetterPrimaryAttributeId(CodeTypeDeclaration targetClass, string logicalName, EntityMetadata metadata)
        {
            CodeMemberMethod primaryAttributeIdMethod = new CodeMemberMethod();
            primaryAttributeIdMethod.Attributes =
                MemberAttributes.Public | MemberAttributes.Override;
            primaryAttributeIdMethod.Name = "GetPrimaryAttributeId";
            primaryAttributeIdMethod.ReturnType = new CodeTypeReference(typeof(System.String));

            CodeMethodReturnStatement returnStatement = new CodeMethodReturnStatement();
            returnStatement.Expression = new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(EntityDefinitionsType), "PrimaryIdAttribute");

            primaryAttributeIdMethod.Statements.Add(returnStatement);
            targetClass.Members.Add(primaryAttributeIdMethod);
        }


        private void AddProperties(CodeTypeDeclaration targetClass, EntityMetadata metadata, EntityMappingSettings settings, bool isActivity)
        {

            var preidcate = new Func<AttributeMetadata, bool>((k) =>
            {
                return k.AttributeType != null &&
                    new AttributeTypeCode[] {
                        AttributeTypeCode.String,
                        AttributeTypeCode.Boolean,
                        AttributeTypeCode.Customer,
                        AttributeTypeCode.DateTime,
                        AttributeTypeCode.Decimal,
                        AttributeTypeCode.Double,
                        AttributeTypeCode.Integer,
                        AttributeTypeCode.Lookup,
                        AttributeTypeCode.Memo,
                        AttributeTypeCode.Money,
                        AttributeTypeCode.Owner,
                        AttributeTypeCode.PartyList,
                        AttributeTypeCode.Picklist,
                        AttributeTypeCode.State,
                        AttributeTypeCode.Status,
                        AttributeTypeCode.Uniqueidentifier,
                        AttributeTypeCode.Virtual
                }.ToList().IndexOf((AttributeTypeCode)k.AttributeType) > -1;
            });

            var targetAttributes = metadata.Attributes.ToList();
            if (isActivity)
            {
                var activityMetadata = MetadataProvider.GetEntityMetadata(this.Service, ActivityPointerLogicalName);
                var currentAttributes = targetAttributes;
                var attributesForAdd = new List<AttributeMetadata>();
                foreach (var item in activityMetadata.Attributes)
                {
                    if (currentAttributes.FirstOrDefault(k => k.LogicalName == item.LogicalName) == null)
                    {
                        attributesForAdd.Add(item);
                    }
                }
                currentAttributes.AddRange(attributesForAdd);
                targetAttributes = currentAttributes;
            }
            List<AttributeMetadata> orderedMetadataList = GetOrderedAttributeMetadataList(targetAttributes, settings);

            var addedAttributes = new List<string>();
            var addedEnums = new List<string>();
            CodeTypeDeclaration targetEnumClass = GetTargetClassEnums(EnumDefinitionType);
            targetEnumClass.TypeAttributes = TypeAttributes.Public;

            CodeTypeDeclaration targetStateStatusClass = GetTargetClassEnums(StateDefinitionType);
            targetStateStatusClass.TypeAttributes = TypeAttributes.Public;

            CodeTypeDeclaration attributesStructDefinition = new CodeTypeDeclaration(AttributeDefinitionType);
            attributesStructDefinition.TypeAttributes = TypeAttributes.Public;
            attributesStructDefinition.IsStruct = true;

            CodeTypeDeclaration entityStructDefinition = new CodeTypeDeclaration(EntityDefinitionsType);
            entityStructDefinition.TypeAttributes = TypeAttributes.Public;
            entityStructDefinition.IsStruct = true;

            CompleteEntityDefinition(metadata, entityStructDefinition);

            CodeTypeDeclaration targetRelationshipsClass = GetTargetClassEnums(RelationshipDefinitionType);
            targetRelationshipsClass.TypeAttributes = TypeAttributes.Public;


            var relationshipNames = new List<string>();
            var standarOneToManyRelationships = GetFilteredOneToManyRelationshipsMetadataList(metadata, settings, false);
            var customOneToManyRelationships = GetFilteredOneToManyRelationshipsMetadataList(metadata, settings, true);
            var standarManyToOneRelationships = GetFilteredManyToOneRelationshipsMetadataList(metadata, settings, false);
            var customManyToOneRelationships = GetFilteredManyToOneRelationshipsMetadataList(metadata, settings, true);
            var standarManyToManyRelationships = GetFilteredManyToManyRelationshipsMetadataList(metadata, settings, false);
            var customManyoManyRelationships = GetFilteredManyToManyRelationshipsMetadataList(metadata, settings, true);

            CodeTypeDeclaration manyToOneDeclaration = GetTargetClassEnums(ManyToOneRelationshipDefinitionType);
            manyToOneDeclaration.TypeAttributes = TypeAttributes.Public;

            var manyToOneStandard = CompleteManyToOneRelationshipDefinition(standarOneToManyRelationships, settings, relationshipNames);
            var manyToOneCustom = CompleteManyToOneRelationshipDefinition(customOneToManyRelationships, settings, relationshipNames);
            manyToOneDeclaration.Members.AddRange(manyToOneStandard.ToArray());
            manyToOneDeclaration.Members.AddRange(manyToOneCustom.ToArray());

            CodeTypeDeclaration oneToManyDeclaration = GetTargetClassEnums(OneToManyRelationshipDefinitionType);
            oneToManyDeclaration.TypeAttributes = TypeAttributes.Public;

            var oneToManyStandard = CompleteOneToManyRelationshipDefinition(standarManyToOneRelationships, settings, relationshipNames);
            var oneToManyCustom = CompleteOneToManyRelationshipDefinition(customManyToOneRelationships, settings, relationshipNames);
            oneToManyDeclaration.Members.AddRange(oneToManyStandard.ToArray());
            oneToManyDeclaration.Members.AddRange(oneToManyCustom.ToArray());

            CodeTypeDeclaration manyToManyDeclaration = GetTargetClassEnums(ManyToManyRelationshipDefinitionType);
            manyToManyDeclaration.TypeAttributes = TypeAttributes.Public;

            var manyToManyStandard = CompleteManyToManyRelationshipDefinition(standarManyToManyRelationships, settings, relationshipNames);
            var manyToManyCustom = CompleteManyToManyRelationshipDefinition(customManyoManyRelationships, settings, relationshipNames);
            manyToManyDeclaration.Members.AddRange(manyToManyStandard.ToArray());
            manyToManyDeclaration.Members.AddRange(manyToManyCustom.ToArray());

            targetRelationshipsClass.Members.Add(manyToOneDeclaration);
            targetRelationshipsClass.Members.Add(oneToManyDeclaration);
            targetRelationshipsClass.Members.Add(manyToManyDeclaration);


            var currentAttribute = string.Empty;
            try
            {
                foreach (var item in orderedMetadataList.Where(preidcate))
                {

                    if (item.LogicalName == "scm_business")
                    {

                    }

                    currentAttribute = item.LogicalName;
                    var fieldName = $"_{item.LogicalName}";
                    CodeMemberField field = new CodeMemberField();
                    field.Attributes = MemberAttributes.Private;
                    field.Name = fieldName;
                    field.Type = new CodeTypeReference(typeof(string));
                    field.InitExpression = new CodePrimitiveExpression(item.LogicalName);
                    targetClass.Members.Add(field);

                    CodeMemberProperty property = new CodeMemberProperty();
                    property.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                    var preparedAttribute = GetPropertyName(item.LogicalName, settings);
                    if (addedAttributes.IndexOf(preparedAttribute) > -1)
                    {
                        preparedAttribute = GetPropertyName(item.LogicalName, settings, true);
                    }
                    addedAttributes.Add(preparedAttribute);
                    property.Name = preparedAttribute;
                    property.HasGet = true;
                    property.HasSet = true;
                    var type = GetType(item);
                    if (type != null)
                    {
                        property.Type = new CodeTypeReference(type);
                        property.Comments.Add(new CodeCommentStatement("<summary>", true));
                        property.Comments.Add(new CodeCommentStatement($"Property getter/setter: {item.LogicalName} ({item.AttributeTypeName})", true));
                        property.Comments.Add(new CodeCommentStatement(item.Description?.UserLocalizedLabel?.Label, true));
                        property.Comments.Add(new CodeCommentStatement("</summary>", true));


                        CodeMemberField fieldDefinition = new CodeMemberField();
                        fieldDefinition.Attributes = MemberAttributes.Public | MemberAttributes.Const;
                        fieldDefinition.Name = preparedAttribute;
                        fieldDefinition.Type = new CodeTypeReference(typeof(string));
                        fieldDefinition.InitExpression = new CodePrimitiveExpression(item.LogicalName);
                        fieldDefinition.Comments.Add(new CodeCommentStatement("<summary>", true));
                        fieldDefinition.Comments.Add(new CodeCommentStatement($"Property name: {item.LogicalName} ({item.AttributeTypeName.Value})", true));
                        fieldDefinition.Comments.Add(new CodeCommentStatement(item.Description?.UserLocalizedLabel?.Label, true));
                        fieldDefinition.Comments.Add(new CodeCommentStatement("</summary>", true));
                        attributesStructDefinition.Members.Add(fieldDefinition);


                        var getMethod = new CodeMethodReturnStatement(new CodeArgumentReferenceExpression($"GetValue<{type.FullName}>({fieldName})"));
                        property.GetStatements.Add(getMethod);

                        var setMethod = new CodeMethodInvokeExpression(
                            new CodeThisReferenceExpression(), "SetValue",
                            new CodeExpression[] {
                        new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName),
                        new CodePropertySetValueReferenceExpression(),
                            });
                        property.SetStatements.Add(setMethod);
                        targetClass.Members.Add(property);


                        if (item is PicklistAttributeMetadata)
                        {
                            var typed = (PicklistAttributeMetadata)item;
                            var enumAttributeMetadata = typed.OptionSet;
                            var name = enumAttributeMetadata.Name;
                            if (addedEnums.IndexOf(name) == -1)
                            {
                                ProcessOptionSetMetadata(enumAttributeMetadata, targetEnumClass, new List<string>());
                            }
                            addedEnums.Add(name);
                        }
                        else if (item is MultiSelectPicklistAttributeMetadata)
                        {
                            var typed = (MultiSelectPicklistAttributeMetadata)item;
                            var enumAttributeMetadata = typed.OptionSet;
                            var name = enumAttributeMetadata.Name;
                            if (addedEnums.IndexOf(name) == -1)
                            {
                                ProcessOptionSetMetadata(enumAttributeMetadata, targetEnumClass, new List<string>());
                            }
                            addedEnums.Add(name);
                        }
                        else if (item is StateAttributeMetadata)
                        {
                            var typed = (StateAttributeMetadata)item;
                            var enumAttributeMetadata = typed.OptionSet;
                            var name = enumAttributeMetadata.Name;
                            if (addedEnums.IndexOf(name) == -1)
                            {
                                ProcessOptionSetMetadata(enumAttributeMetadata, targetStateStatusClass, new List<string>());
                            }
                            addedEnums.Add(name);
                        }
                        else if (item is StatusAttributeMetadata)
                        {
                            var typed = (StatusAttributeMetadata)item;
                            var enumAttributeMetadata = typed.OptionSet;
                            var name = enumAttributeMetadata.Name;
                            if (addedEnums.IndexOf(name) == -1)
                            {
                                ProcessOptionSetMetadata(enumAttributeMetadata, targetStateStatusClass, new List<string>());
                            }
                            addedEnums.Add(name);
                        }
                        else
                        {

                        }
                    }

                }

                targetClass.Members.Add(entityStructDefinition);
                targetClass.Members.Add(targetRelationshipsClass);
                targetClass.Members.Add(targetStateStatusClass);
                targetClass.Members.Add(attributesStructDefinition);
                targetClass.Members.Add(targetEnumClass);
            }
            catch (Exception)
            {
                throw;
            }

        }

        private void CompleteRolesDefinition(List<string> roles, CodeTypeDeclaration entityStructDefinition, EntityMappingSettings settings)
        {
            var addedRoles = new List<string>();

            foreach (var role in roles)
            {
                CodeMemberField roleDefinition = new CodeMemberField();
                roleDefinition.Attributes = MemberAttributes.Public | MemberAttributes.Const;
                var preparedAttribute = NormalizeString(role);

                if (addedRoles.IndexOf(preparedAttribute) == -1)
                {
                    roleDefinition.Name = preparedAttribute;
                    roleDefinition.Type = new CodeTypeReference(typeof(string));
                    roleDefinition.InitExpression = new CodePrimitiveExpression(role);
                    entityStructDefinition.Members.Add(roleDefinition);
                }
                else
                {

                }
                addedRoles.Add(preparedAttribute);
            }
        }

        private void CompleteEntitiesDefinition(EntityMetadata[] entitiesMetadata, CodeTypeDeclaration entityStructDefinition, EntityMappingSettings settings)
        {
            var addedEntities = new List<string>();

            var orderedEntities = GetOrderedEntitiesMetadataList(entitiesMetadata, settings);
            foreach (var entity in orderedEntities)
            {
                CodeMemberField entityLogicalNameDefinition = new CodeMemberField();
                entityLogicalNameDefinition.Attributes = MemberAttributes.Public | MemberAttributes.Const;
                var preparedAttribute = GetPropertyName(entity.SchemaName, settings);

                if (addedEntities.IndexOf(preparedAttribute) > -1)
                {
                    preparedAttribute = GetPropertyName(entity.SchemaName, settings, true);
                }
                addedEntities.Add(preparedAttribute);

                entityLogicalNameDefinition.Name = preparedAttribute;
                entityLogicalNameDefinition.Type = new CodeTypeReference(typeof(string));
                entityLogicalNameDefinition.InitExpression = new CodePrimitiveExpression(entity.LogicalName);
                entityStructDefinition.Members.Add(entityLogicalNameDefinition);
            }
        }


        private List<CodeMemberProperty> CompleteManyToManyRelationshipDefinition(List<ManyToManyRelationshipMetadata> relationships, EntityMappingSettings settings, List<string> names)
        {
            string currentType = "DomainManyToManyRelationship";
            var properties = new List<CodeMemberProperty>();
            foreach (var item in relationships)
            {
                var preparedAttribute = GetRelationshipName(item.SchemaName, settings);
                if (IsRelationshipNameRepeated(preparedAttribute, names))
                {
                    preparedAttribute = GetRelationshipName(item.SchemaName, settings, true);
                }
                names.Add(preparedAttribute);


                CodeMemberProperty property = new CodeMemberProperty();
                property.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                property.Name = preparedAttribute;
                property.HasGet = true;
                property.HasSet = false;
                property.Type = new CodeTypeReference(currentType);
                property.Comments.Add(new CodeCommentStatement("<summary>", true));
                property.Comments.Add(new CodeCommentStatement($"Relationship: {item.SchemaName}", true));
                property.Comments.Add(new CodeCommentStatement($"From {item.Entity1LogicalName} to {item.Entity2LogicalName} by intersection {item.SchemaName}", true));
                property.Comments.Add(new CodeCommentStatement("</summary>", true));
                var getMethod = new CodeMethodReturnStatement(new CodeArgumentReferenceExpression($"new {currentType}(\"{item.Entity1LogicalName}\", \"{item.Entity1IntersectAttribute}\", \"{item.Entity2LogicalName}\", \"{item.Entity2IntersectAttribute}\", \"{item.SchemaName}\")"));
                property.GetStatements.Add(getMethod);
                properties.Add(property);
            }
            return properties;
        }

        private List<CodeMemberProperty> CompleteOneToManyRelationshipDefinition(List<OneToManyRelationshipMetadata> relationships, EntityMappingSettings settings, List<string> names)
        {
            string currentType = "DomainOneToManyRelationship";
            var properties = new List<CodeMemberProperty>();
            foreach (var item in relationships)
            {
                var preparedAttribute = GetRelationshipName(item.SchemaName, settings);
                if (IsRelationshipNameRepeated(preparedAttribute, names))
                {
                    preparedAttribute = GetRelationshipName(item.SchemaName, settings, true);
                }
                names.Add(preparedAttribute);

                CodeMemberProperty property = new CodeMemberProperty();
                property.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                property.Name = preparedAttribute;
                property.HasGet = true;
                property.HasSet = false;
                property.Type = new CodeTypeReference(currentType);
                property.Comments.Add(new CodeCommentStatement("<summary>", true));
                property.Comments.Add(new CodeCommentStatement($"Relationship: {item.SchemaName}", true));
                property.Comments.Add(new CodeCommentStatement($"From {item.ReferencingEntity} to {item.ReferencedEntity} by {item.ReferencingAttribute}", true));
                property.Comments.Add(new CodeCommentStatement("</summary>", true));
                var getMethod = new CodeMethodReturnStatement(new CodeArgumentReferenceExpression($"new {currentType}(\"{item.ReferencingEntity}\", \"{item.ReferencingAttribute}\", \"{item.ReferencedEntity}\", \"{item.ReferencedAttribute}\")"));
                property.GetStatements.Add(getMethod);
                properties.Add(property);
            }
            return properties;
        }

        private List<CodeMemberProperty> CompleteManyToOneRelationshipDefinition(List<OneToManyRelationshipMetadata> relationships, EntityMappingSettings settings, List<string> names)
        {
            string currentType = "DomainManyToOneRelationship";
            var properties = new List<CodeMemberProperty>();
            foreach (var item in relationships)
            {
                var preparedAttribute = GetRelationshipName(item.SchemaName, settings);
                if (IsRelationshipNameRepeated(preparedAttribute, names))
                {
                    preparedAttribute = GetRelationshipName(item.SchemaName, settings, true);
                }
                names.Add(preparedAttribute);

                CodeMemberProperty property = new CodeMemberProperty();
                property.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                property.Name = preparedAttribute;
                property.HasGet = true;
                property.HasSet = false;
                property.Type = new CodeTypeReference(currentType);
                property.Comments.Add(new CodeCommentStatement("<summary>", true));
                property.Comments.Add(new CodeCommentStatement($"Relationship: {item.SchemaName}", true));
                property.Comments.Add(new CodeCommentStatement($"From {item.ReferencingEntity} to {item.ReferencedEntity} by {item.ReferencedAttribute}", true));
                property.Comments.Add(new CodeCommentStatement("</summary>", true));
                var getMethod = new CodeMethodReturnStatement(new CodeArgumentReferenceExpression($"new {currentType}(\"{item.ReferencingEntity}\", \"{item.ReferencingAttribute}\", \"{item.ReferencedEntity}\", \"{item.ReferencedAttribute}\")"));
                property.GetStatements.Add(getMethod);
                properties.Add(property);
            }
            return properties;
        }


        private List<OneToManyRelationshipMetadata> GetFilteredOneToManyRelationshipsMetadataList(EntityMetadata metadata, EntityMappingSettings settings, bool filterByPrefix)
        {
            var listOfPrefixed = new List<OneToManyRelationshipMetadata>();
            var listOfStandard = new List<OneToManyRelationshipMetadata>();
            foreach (var prefix in settings.TrimPrefixes)
            {
                var prefixComposed = $"{prefix}_";
                foreach (var item in metadata.OneToManyRelationships)
                {
                    if (item.SchemaName.StartsWith(prefixComposed))
                    {
                        listOfPrefixed.Add(item);
                    }
                    else
                    {
                        listOfStandard.Add(item);
                    }
                }
            }

            return filterByPrefix
                    ? listOfPrefixed
                    : listOfStandard;
        }


        private List<OneToManyRelationshipMetadata> GetFilteredManyToOneRelationshipsMetadataList(EntityMetadata metadata, EntityMappingSettings settings, bool filterByPrefix)
        {
            var listOfPrefixed = new List<OneToManyRelationshipMetadata>();
            var listOfStandard = new List<OneToManyRelationshipMetadata>();
            foreach (var prefix in settings.TrimPrefixes)
            {
                var prefixComposed = $"{prefix}_";
                foreach (var item in metadata.ManyToOneRelationships)
                {
                    if (item.SchemaName.StartsWith(prefixComposed))
                    {
                        listOfPrefixed.Add(item);
                    }
                    else
                    {
                        listOfStandard.Add(item);
                    }
                }
            }

            return filterByPrefix
                    ? listOfPrefixed
                    : listOfStandard;
        }

        private List<ManyToManyRelationshipMetadata> GetFilteredManyToManyRelationshipsMetadataList(EntityMetadata metadata, EntityMappingSettings settings, bool filterByPrefix)
        {
            var listOfPrefixed = new List<ManyToManyRelationshipMetadata>();
            var listOfStandard = new List<ManyToManyRelationshipMetadata>();
            foreach (var prefix in settings.TrimPrefixes)
            {
                var prefixComposed = $"{prefix}_";
                foreach (var item in metadata.ManyToManyRelationships)
                {
                    if (item.SchemaName.StartsWith(prefixComposed))
                    {
                        listOfPrefixed.Add(item);
                    }
                    else
                    {
                        listOfStandard.Add(item);
                    }
                }
            }

            return filterByPrefix
                    ? listOfPrefixed
                    : listOfStandard;
        }


        private bool IsRelationshipNameRepeated(string name, List<string> names)
        {
            return names.IndexOf(name) > -1;
        }


        private void CompleteEntityDefinition(EntityMetadata entityMetadata, CodeTypeDeclaration entityStructDefinition)
        {
            Type type = entityMetadata.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(type.GetProperties());
            foreach (var prop in props)
            {

                try
                {
                    object propValue = prop.GetValue(entityMetadata, null);

                    CodeMemberField entityLogicalNameDefinition = new CodeMemberField();
                    entityLogicalNameDefinition.Attributes = MemberAttributes.Public | MemberAttributes.Const;
                    entityLogicalNameDefinition.Name = prop.Name;

                    var propertyType = prop.PropertyType;
                    var targetType = typeof(string);
                    bool setToString = true;
                    if (propertyType == typeof(string)
                        || propertyType == typeof(int)
                        || propertyType == typeof(bool)
                        || propertyType == typeof(decimal)
                        || propertyType == typeof(float)
                        || propertyType == typeof(double)
                        || propertyType == typeof(char))
                    {
                        targetType = propertyType;
                        setToString = false;
                    }
                    entityLogicalNameDefinition.Type = new CodeTypeReference(targetType);
                    if (propValue == null)
                    {
                        entityLogicalNameDefinition.InitExpression = new CodePrimitiveExpression(null);
                    }
                    else
                    {
                        entityLogicalNameDefinition.InitExpression = new CodePrimitiveExpression(setToString ? propValue.ToString() : propValue);
                    }
                    entityStructDefinition.Members.Add(entityLogicalNameDefinition);
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }

        private List<OptionSetMetadataBase> GetOrderedEnumMetadataList(List<OptionSetMetadataBase> enums, EntityMappingSettings settings)
        {
            var orderedMetadataList = new List<OptionSetMetadataBase>();
            foreach (var prefix in settings.TrimPrefixes)
            {
                var prefixComposed = $"{prefix}_";
                foreach (var item in enums.Where(k => !k.Name.StartsWith(prefixComposed)))
                {
                    orderedMetadataList.Add(item);
                }
            }
            foreach (var item in enums)
            {
                if (!orderedMetadataList.Any(k => k.Name == item.Name))
                {
                    orderedMetadataList.Add(item);
                }
            }
            return orderedMetadataList;
        }

        private List<AttributeMetadata> GetOrderedAttributeMetadataList(List<AttributeMetadata> attributesMetadata, EntityMappingSettings settings)
        {
            var orderedMetadataList = new List<AttributeMetadata>();
            foreach (var prefix in settings.TrimPrefixes)
            {
                var prefixComposed = $"{prefix}_";
                foreach (var item in attributesMetadata.Where(k => !k.LogicalName.StartsWith(prefixComposed)))
                {
                    orderedMetadataList.Add(item);
                }
            }
            foreach (var item in attributesMetadata)
            {
                if (!orderedMetadataList.Any(k => k.LogicalName == item.LogicalName))
                {
                    orderedMetadataList.Add(item);
                }
            }
            return orderedMetadataList;
        }

        private List<EntityMetadata> GetOrderedEntitiesMetadataList(EntityMetadata[] metadata, EntityMappingSettings settings)
        {
            var orderedMetadataList = new List<EntityMetadata>();
            foreach (var prefix in settings.TrimPrefixes)
            {
                var prefixComposed = $"{prefix}_";
                foreach (var item in metadata.Where(k => !k.LogicalName.StartsWith(prefixComposed)))
                {
                    orderedMetadataList.Add(item);
                }
            }
            foreach (var item in metadata)
            {
                if (!orderedMetadataList.Any(k => k.LogicalName == item.LogicalName))
                {
                    orderedMetadataList.Add(item);
                }
            }
            return orderedMetadataList;
        }

        private string GetEnumName(string logicalName, EntityMappingSettings settings, bool isRepeated = false)
        {
            string newName = logicalName;
            if (settings == null)
            {
                newName = CapitalizeSentence(newName);
                return newName;
            }
            if (settings.GlobalEnums.Keys.Contains(logicalName))
            {
                return settings.GlobalEnums[logicalName];
            }

            if (settings.TrimPrefix == null)
            {
                settings.TrimPrefix = false;
            }

            if ((bool)settings.TrimPrefix && !isRepeated)
            {
                foreach (var prefix in settings.TrimPrefixes)
                {
                    var prefixComplete = $"{prefix}_";
                    if (logicalName.Length >= prefixComplete.Length
                        && newName.Substring(0, prefixComplete.Length) == prefixComplete)
                    {
                        newName = newName.Substring(prefixComplete.Length);

                        break;
                    }
                }
            }

            if ((bool)settings.Capitalize)
            {
                var words = newName.Split('_');
                newName = string.Join("", words.Select(k => { return Capitalize(k); }));
            }
            return newName;
        }

        private string CapitalizeSentence(string newName)
        {
            var words = newName.Split(new char[] { '_', ' ', '.', ',', '-', '|' });
            newName = string.Join("", words.Select(k => { return Capitalize(k); }));
            return newName;
        }

        private string GetRelationshipName(string logicalName, EntityMappingSettings settings, bool isRepeated = false)
        {
            string newName = logicalName;
            if (settings == null)
            {
                var words = newName.Split('_');
                newName = string.Join("", words.Select(k => { return Capitalize(k); }));
                return newName;
            }
            if (settings.Mapping.Keys.Contains(logicalName))
            {
                return settings.Mapping[logicalName];
            }

            if (settings.TrimPrefix == null)
            {
                settings.TrimPrefix = false;
            }

            if ((bool)settings.TrimPrefix && !isRepeated)
            {
                foreach (var prefix in settings.TrimPrefixes)
                {
                    var prefixComplete = $"{prefix}_";
                    if (logicalName.Length >= prefixComplete.Length
                        && newName.Substring(0, prefixComplete.Length) == prefixComplete)
                    {
                        newName = newName.Substring(prefixComplete.Length);

                        break;
                    }
                }
            }

            if ((bool)settings.Capitalize)
            {
                var words = newName.Split('_');
                newName = string.Join("", words.Select(k => { return Capitalize(k); }));
            }

            return newName;
        }

        private string GetPropertyName(string logicalName, EntityMappingSettings settings, bool isRepeated = false)
        {
            string newName = logicalName;
            if (settings == null)
            {
                var words = newName.Split('_');
                newName = string.Join("", words.Select(k => { return Capitalize(k); }));
                return newName;
            }
            if (settings.Mapping.Keys.Contains(logicalName))
            {
                return settings.Mapping[logicalName];
            }

            if (settings.TrimPrefix == null)
            {
                settings.TrimPrefix = false;
            }

            if ((bool)settings.TrimPrefix && !isRepeated)
            {
                foreach (var prefix in settings.TrimPrefixes)
                {
                    var prefixComplete = $"{prefix}_";
                    if (logicalName.Length >= prefixComplete.Length
                        && newName.Substring(0, prefixComplete.Length) == prefixComplete)
                    {
                        newName = newName.Substring(prefixComplete.Length);

                        break;
                    }
                }
            }

            if ((bool)settings.Capitalize)
            {
                var words = newName.Split('_');
                newName = string.Join("", words.Select(k => { return Capitalize(k); }));
            }
            return newName;
        }

        public void GenerateCSharpCode(string fileName, CodeCompileUnit targetUnit)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";

            var path = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (StreamWriter sourceWriter = new StreamWriter(fileName))
            {
                provider.GenerateCodeFromCompileUnit(
                    targetUnit, sourceWriter, options);
            }
        }


        private Type GetType(AttributeMetadata metadata)
        {
            if (metadata.AttributeType == AttributeTypeCode.String)
            {
                return typeof(string);
            }
            else if (metadata.AttributeType == AttributeTypeCode.Boolean)
            {
                return typeof(bool);
            }
            else if (metadata.AttributeType == AttributeTypeCode.Customer)
            {
                return typeof(DomainEntityReference);
            }
            else if (metadata.AttributeType == AttributeTypeCode.DateTime)
            {
                return typeof(System.DateTime);
            }
            else if (metadata.AttributeType == AttributeTypeCode.Decimal)
            {
                return typeof(decimal);
            }
            else if (metadata.AttributeType == AttributeTypeCode.Double)
            {
                return typeof(double);
            }
            else if (metadata.AttributeType == AttributeTypeCode.Integer)
            {
                return typeof(int);
            }
            else if (metadata.AttributeType == AttributeTypeCode.Lookup)
            {
                return typeof(DomainEntityReference);
            }
            else if (metadata.AttributeType == AttributeTypeCode.Memo)
            {
                return typeof(string);
            }
            else if (metadata.AttributeType == AttributeTypeCode.Money)
            {
                return typeof(DomainMoney);
            }
            else if (metadata.AttributeType == AttributeTypeCode.Owner)
            {
                return typeof(DomainEntityReference);
            }
            else if (metadata.AttributeType == AttributeTypeCode.Picklist)
            {
                return typeof(DomainOptionSetValue);
            }
            else if (metadata.AttributeType == AttributeTypeCode.State)
            {
                return typeof(DomainOptionSetValue);
            }
            else if (metadata.AttributeType == AttributeTypeCode.Status)
            {
                return typeof(DomainOptionSetValue);
            }
            else if (metadata.AttributeType == AttributeTypeCode.Uniqueidentifier)
            {
                return typeof(Guid);
            }
            else if (metadata.AttributeType == AttributeTypeCode.PartyList)
            {
                return typeof(DomainPartyList);
            }
            else if (metadata.AttributeType == AttributeTypeCode.Virtual)
            {
                if (metadata is MultiSelectPicklistAttributeMetadata)
                {
                    return typeof(DomainOptionSetValueCollection);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
            return null;
        }

        private string GetDomainName(string entityLogicalName, EntityMappingSettings settings)
        {
            return settings?.EntityDomainName
                ?? Capitalize(entityLogicalName);
        }

        private string GetPluralEntityName(string entityLogicalName, EntityMappingSettings settings)
        {
            return settings?.PluralNamespaceName
                ?? Capitalize(GetPlural(entityLogicalName));
        }

        private string GetPlural(string entityName)
        {
            string plural = string.Empty;
            if (entityName.ToLower().Last() == 'y')
            {
                plural = string.Format("{0}ies", entityName.Substring(0, entityName.Length - 2));
            }
            else if (entityName.ToLower().Last() == 's')
            {
                plural = string.Format("{0}", entityName);
            }
            else
            {
                plural = string.Format("{0}s", entityName);
            }
            return plural;
        }


        private string Capitalize(string name)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(name);
        }


        private static EntityMappingSettings MergeSettings(EntityMappingSettings entitySettings, EntityMappingSettings commonSettings)
        {
            var settings = commonSettings;

            settings.PluralNamespaceName = entitySettings.PluralNamespaceName;
            settings.OutputFile = entitySettings.OutputFile;
            settings.EntityDomainName = entitySettings.EntityDomainName;

            foreach (var item in entitySettings.Mapping)
            {
                if (!commonSettings.Mapping.ContainsKey(item.Key))
                {
                    settings.Mapping.Add(item.Key, entitySettings.Mapping[item.Key]);
                }
            }

            foreach (var item in entitySettings.GlobalEnums)
            {
                if (!commonSettings.GlobalEnums.ContainsKey(item.Key))
                {
                    settings.GlobalEnums.Add(item.Key, entitySettings.GlobalEnums[item.Key]);
                }
            }

            foreach (var item in entitySettings.TrimPrefixes)
            {
                if (!commonSettings.TrimPrefixes.Contains(item))
                {
                    settings.TrimPrefixes.Add(item);
                }
            }

            if (entitySettings.TrimPrefix != null && (bool)entitySettings.TrimPrefix)
            {
                settings.TrimPrefix = true;
            }
            if (entitySettings.Capitalize != null && (bool)entitySettings.Capitalize)
            {
                settings.Capitalize = true;
            }

            return settings;
        }


        private static string NormalizeString(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return RemoveSpecialCharacters(RemoveDiacritics(text));
            }
            return null;
        }



        private static string ReplaceSpecialCharacters(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return text.Replace("+", "Plus")
                    .Replace("-", "Minus")
                    .Replace("=", "Equals")
                    .Replace(">=", "GreatherOrEqualThan")
                    .Replace("<=", "SmallerOrEqualThan")
                    .Replace(">", "GreatherThan")
                    .Replace("<", "SmallerThan");
            }
            return null;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }


        private static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
