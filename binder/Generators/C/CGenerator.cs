﻿using System;
using System.Collections.Generic;
using IKVM.Reflection;
using CppSharp.AST;
using CppSharp.Types;

namespace MonoManagedToNative.Generators
{
    public class CGenerator : Generator
    {
        public CGenerator(Driver driver) : base(driver) { }

        public override List<Template> Generate(Assembly assembly)
        {
            var moduleGen = new AstGenerator(Driver.Options);
            var module = moduleGen.Visit(assembly) as TranslationUnit;

            var headers = new CHeaders(Driver, module);
            var sources = new CSources(Driver, module);

            var templates = new List<Template> { headers, sources };
            foreach (var template in templates)
                template.Assembly = assembly;

            return templates;
        }
    }

    public class CBlockKind
    {
        public const int Includes = BlockKind.LAST + 11;
        public const int Function = BlockKind.LAST + 12;
        public const int Class = BlockKind.LAST + 13;
        public const int Typedef = BlockKind.LAST + 13;
    }

    public abstract class CTemplate : Template, IDeclVisitor<bool>
    {
        public TranslationUnit Unit;

        public CTemplate(Driver driver, TranslationUnit unit) : base(driver)
        {
            Unit = unit;
        }

        public override string Name
        {
            get { return Unit.Name; }
        }

        public string GeneratedIdentifier(string id)
        {
            return "__" + id;
        }

        string PrintCILType(CILType type, TypeQualifiers quals)
        {
            if (type.Type == typeof(string))
                return "const char*";

            throw new NotImplementedException();
        }

        public CppTypePrinter CTypePrinter
        {
            get
            {
                var cTypePrinter = new CppTypePrinter { PrintScopeKind = CppTypePrintScopeKind.Qualified };
                cTypePrinter.CILTypePrinter += PrintCILType;
                return cTypePrinter;
            }
        }

        public void VisitDeclContext(DeclarationContext ctx)
        {
            foreach (var decl in ctx.Declarations)
                decl.Visit(this);
        }

        public void GenerateFilePreamble()
        {
            WriteLine("/*");
            WriteLine(" * This is autogenerated code.");
            WriteLine(" * Do not edit this file or all your changes will be lost after re-generation.");
            WriteLine(" */");
        }

        public void GenerateMethodSignature(Method method)
        {
            var @class = method.Namespace as Class;
            var retType = method.ReturnType.Visit(CTypePrinter);

            Write("{0} {1}_{2}(", retType, @class.QualifiedName, method.Name);

            Write(GenerateParametersList(method.Parameters));

            Write(")");
        }

        public string GenerateParametersList(List<Parameter> parameters)
        {
            var types = new List<string>();
            foreach (var param in parameters)
                types.Add(CTypePrinter.VisitParameter(param));
            return string.Join(", ", types);
        }

        #region IDeclVisitor methods

        public virtual bool VisitClassDecl(Class @class)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitClassTemplateDecl(ClassTemplate template)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitClassTemplateSpecializationDecl(ClassTemplateSpecialization specialization)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitDeclaration(Declaration decl)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitEnumDecl(Enumeration @enum)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitEnumItemDecl(Enumeration.Item item)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitEvent(Event @event)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitFieldDecl(Field field)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitFriend(Friend friend)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitFunctionDecl(Function function)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitFunctionTemplateDecl(FunctionTemplate template)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitMacroDefinition(MacroDefinition macro)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitMethodDecl(Method method)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitNamespace(Namespace @namespace)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitNonTypeTemplateParameterDecl(NonTypeTemplateParameter nonTypeTemplateParameter)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitParameterDecl(Parameter parameter)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitProperty(Property property)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitTemplateParameterDecl(TypeTemplateParameter templateParameter)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitTemplateTemplateParameterDecl(TemplateTemplateParameter templateTemplateParameter)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitTypeAliasDecl(TypeAlias typeAlias)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitTypeAliasTemplateDecl(TypeAliasTemplate typeAliasTemplate)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitTypedefDecl(TypedefDecl typedef)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitVariableDecl(Variable variable)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
