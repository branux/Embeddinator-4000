using CppSharp;
using CppSharp.AST;
using CppSharp.Passes;

namespace MonoEmbeddinator4000.Passes
{
    public class FieldToGetterSetterPropertyPass : TranslationUnitPass
    {
        public override bool VisitFieldDecl(Field field)
        {
            if (!VisitDeclaration(field))
                return false;

            if (field.Access == AccessSpecifier.Private)
                return false;

            if (field.IsImplicit)
                return false;

            field.GenerationKind = GenerationKind.None;

            var @class = field.Namespace as Class;

            var getter = new Method
            {
                Name = $"get_{field.Name}",
                Namespace = @class,
                ReturnType = field.QualifiedType,
                Access = field.Access,
                SynthKind = FunctionSynthKind.FieldAcessor,
                Field = field
            };

            var setter = new Method
            {
                Name = $"set_{field.Name}",
                Namespace = @class,
                ReturnType = new QualifiedType(new BuiltinType(PrimitiveType.Void)),
                Access = field.Access,
                SynthKind = FunctionSynthKind.FieldAcessor,
                Field = field
            };

            var param = new Parameter
            {
                Name = "value",
                QualifiedType = field.QualifiedType,
            };
            setter.Parameters.Add(param);

            var property = new Property
            {
                Name = field.Name,
                Namespace = field.Namespace,
                GetMethod = getter,
                SetMethod = setter,
                Field = field,
                QualifiedType = field.QualifiedType
            };

            @class.Declarations.Add(property);

            Diagnostics.Debug($"Getter/setter property created from field {field.QualifiedName}");

            return true;
        }
    }
}
