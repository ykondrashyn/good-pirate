using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

internal static class ManagedAssemblyAudit
{
    private static readonly StringComparer Ordinal = StringComparer.Ordinal;

    private static IEnumerable<TypeDefinition> AllTypes(IEnumerable<TypeDefinition> roots)
    {
        foreach (TypeDefinition type in roots.OrderBy(t => t.MetadataToken.ToInt32()))
        {
            yield return type;
            foreach (TypeDefinition nested in AllTypes(type.NestedTypes))
                yield return nested;
        }
    }

    private static string Tsv(string value)
    {
        if (value == null)
            return string.Empty;
        return value.Replace("\\", "\\\\")
            .Replace("\t", "\\t")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n");
    }

    private static string Token(IMetadataTokenProvider provider)
    {
        return "0x" + provider.MetadataToken.ToUInt32().ToString("X", CultureInfo.InvariantCulture);
    }

    private static string Sha256(byte[] data)
    {
        using (SHA256 hash = SHA256.Create())
            return string.Concat(hash.ComputeHash(data).Select(b => b.ToString("x2", CultureInfo.InvariantCulture)));
    }

    private static string Sha256File(string path)
    {
        using (FileStream stream = File.OpenRead(path))
        using (SHA256 hash = SHA256.Create())
            return string.Concat(hash.ComputeHash(stream).Select(b => b.ToString("x2", CultureInfo.InvariantCulture)));
    }

    private static string MethodSignature(MethodDefinition method)
    {
        string generic = method.HasGenericParameters
            ? "<" + string.Join(",", method.GenericParameters.Select(p => p.Name)) + ">"
            : string.Empty;
        return method.ReturnType.FullName + " " + method.DeclaringType.FullName + "::" +
               method.Name + generic + "(" +
               string.Join(",", method.Parameters.Select(p => p.ParameterType.FullName)) + ")";
    }

    private static string Operand(
        object operand,
        IReadOnlyDictionary<Instruction, int> instructionIndexes)
    {
        if (operand == null)
            return string.Empty;
        Instruction target = operand as Instruction;
        if (target != null)
            return "instruction:" + instructionIndexes[target].ToString(CultureInfo.InvariantCulture);
        Instruction[] targets = operand as Instruction[];
        if (targets != null)
            return "instructions:" + string.Join(",", targets.Select(t => instructionIndexes[t]));
        MethodReference method = operand as MethodReference;
        if (method != null)
            return "method:" + method.FullName;
        FieldReference field = operand as FieldReference;
        if (field != null)
            return "field:" + field.FullName;
        TypeReference type = operand as TypeReference;
        if (type != null)
            return "type:" + type.FullName;
        VariableDefinition variable = operand as VariableDefinition;
        if (variable != null)
            return "variable:" + variable.Index + ":" + variable.VariableType.FullName;
        ParameterDefinition parameter = operand as ParameterDefinition;
        if (parameter != null)
            return "parameter:" + parameter.Index + ":" + parameter.ParameterType.FullName;
        string text = operand as string;
        if (text != null)
            return "string64:" + Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        float single = operand is float ? (float)operand : float.NaN;
        if (!float.IsNaN(single))
            return "float:" + single.ToString("R", CultureInfo.InvariantCulture);
        double number = operand is double ? (double)operand : double.NaN;
        if (!double.IsNaN(number))
            return "double:" + number.ToString("R", CultureInfo.InvariantCulture);
        IFormattable formattable = operand as IFormattable;
        return operand.GetType().FullName + ":" +
               (formattable == null
                   ? operand.ToString()
                   : formattable.ToString(null, CultureInfo.InvariantCulture));
    }

    private static string MethodBodyHash(MethodDefinition method)
    {
        if (!method.HasBody)
            return string.Empty;
        MethodBody body = method.Body;
        Dictionary<Instruction, int> indexes = body.Instructions
            .Select((instruction, index) => new { instruction, index })
            .ToDictionary(item => item.instruction, item => item.index);
        StringBuilder canonical = new StringBuilder();
        canonical.Append("init=").Append(body.InitLocals ? "1" : "0")
            .Append("|max=").Append(body.MaxStackSize)
            .Append("|locals=");
        foreach (VariableDefinition variable in body.Variables)
            canonical.Append(variable.Index).Append(':').Append(variable.VariableType.FullName).Append(';');
        canonical.AppendLine();
        for (int index = 0; index < body.Instructions.Count; index++)
        {
            Instruction instruction = body.Instructions[index];
            canonical.Append(index).Append('|')
                .Append(instruction.OpCode.Code).Append('|')
                .Append(Operand(instruction.Operand, indexes)).AppendLine();
        }
        foreach (ExceptionHandler handler in body.ExceptionHandlers)
        {
            canonical.Append("eh|").Append(handler.HandlerType).Append('|')
                .Append(handler.TryStart == null ? -1 : indexes[handler.TryStart]).Append('|')
                .Append(handler.TryEnd == null ? -1 : indexes[handler.TryEnd]).Append('|')
                .Append(handler.HandlerStart == null ? -1 : indexes[handler.HandlerStart]).Append('|')
                .Append(handler.HandlerEnd == null ? -1 : indexes[handler.HandlerEnd]).Append('|')
                .Append(handler.FilterStart == null ? -1 : indexes[handler.FilterStart]).Append('|')
                .Append(handler.CatchType == null ? string.Empty : handler.CatchType.FullName)
                .AppendLine();
        }
        return Sha256(Encoding.UTF8.GetBytes(canonical.ToString()));
    }

    private static bool CompilerGenerated(ICustomAttributeProvider provider)
    {
        return provider.HasCustomAttributes && provider.CustomAttributes.Any(attribute =>
            attribute.AttributeType.FullName ==
            "System.Runtime.CompilerServices.CompilerGeneratedAttribute");
    }

    public static int Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.Error.WriteLine(
                "usage: ManagedAssemblyAudit MANAGED_DIR ASSEMBLY_CENSUS_TSV METHOD_INDEX_TSV");
            return 2;
        }

        string managedDirectory = Path.GetFullPath(args[0]);
        string censusPath = Path.GetFullPath(args[1]);
        string methodPath = Path.GetFullPath(args[2]);
        Directory.CreateDirectory(Path.GetDirectoryName(censusPath));
        Directory.CreateDirectory(Path.GetDirectoryName(methodPath));

        DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();
        resolver.AddSearchDirectory(managedDirectory);
        ReaderParameters parameters = new ReaderParameters
        {
            AssemblyResolver = resolver,
            InMemory = true,
            ReadSymbols = false,
            ReadingMode = ReadingMode.Immediate
        };

        string[] assemblies = Directory.GetFiles(managedDirectory, "*.dll")
            .OrderBy(path => Path.GetFileName(path), Ordinal)
            .ToArray();

        using (StreamWriter census = new StreamWriter(censusPath, false, new UTF8Encoding(false)))
        using (StreamWriter methods = new StreamWriter(methodPath, false, new UTF8Encoding(false)))
        {
            census.NewLine = "\n";
            methods.NewLine = "\n";
            census.WriteLine(
                "assembly\trelative_path\tsha256\tbytes\tmvid\ttype_count\tmethod_count\tmethods_with_body\tnontrivial_methods\tfield_count\tproperty_count\tevent_count\tresource_count\tassembly_references\tstatus\terror");
            methods.WriteLine(
                "method_id\tassembly\tmetadata_token\tdeclaring_type\tmethod\tfull_signature\tvisibility\tstatic\tabstract\tvirtual\tcompiler_generated\thas_body\tcode_size\tinstruction_count\texception_handler_count\tbody_sha256\tstatus\terror");

            foreach (string assemblyPath in assemblies)
            {
                string assemblyName = Path.GetFileName(assemblyPath);
                try
                {
                    using (AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyPath, parameters))
                    {
                        List<TypeDefinition> types = AllTypes(assembly.MainModule.Types).ToList();
                        List<MethodDefinition> assemblyMethods = types
                            .SelectMany(type => type.Methods)
                            .OrderBy(method => method.MetadataToken.ToInt32())
                            .ToList();
                        string references = string.Join(",", assembly.MainModule.AssemblyReferences
                            .Select(reference => reference.Name)
                            .OrderBy(name => name, Ordinal));
                        census.WriteLine(string.Join("\t", new[]
                        {
                            Tsv(assemblyName),
                            Tsv("assets/bin/Data/Managed/" + assemblyName),
                            Sha256File(assemblyPath),
                            new FileInfo(assemblyPath).Length.ToString(CultureInfo.InvariantCulture),
                            assembly.MainModule.Mvid.ToString("D"),
                            types.Count.ToString(CultureInfo.InvariantCulture),
                            assemblyMethods.Count.ToString(CultureInfo.InvariantCulture),
                            assemblyMethods.Count(method => method.HasBody).ToString(CultureInfo.InvariantCulture),
                            assemblyMethods.Count(method => method.HasBody && method.Body.Instructions.Count > 1)
                                .ToString(CultureInfo.InvariantCulture),
                            types.Sum(type => type.Fields.Count).ToString(CultureInfo.InvariantCulture),
                            types.Sum(type => type.Properties.Count).ToString(CultureInfo.InvariantCulture),
                            types.Sum(type => type.Events.Count).ToString(CultureInfo.InvariantCulture),
                            assembly.MainModule.Resources.Count.ToString(CultureInfo.InvariantCulture),
                            Tsv(references),
                            "ok",
                            string.Empty
                        }));

                        foreach (MethodDefinition method in assemblyMethods)
                        {
                            string visibility = method.IsPublic ? "public" :
                                method.IsFamily ? "protected" :
                                method.IsAssembly ? "internal" :
                                method.IsFamilyOrAssembly ? "protected_internal" : "private";
                            methods.WriteLine(string.Join("\t", new[]
                            {
                                Tsv(assemblyName + ":" + Token(method)),
                                Tsv(assemblyName),
                                Token(method),
                                Tsv(method.DeclaringType.FullName),
                                Tsv(method.Name),
                                Tsv(MethodSignature(method)),
                                visibility,
                                method.IsStatic ? "1" : "0",
                                method.IsAbstract ? "1" : "0",
                                method.IsVirtual ? "1" : "0",
                                CompilerGenerated(method) || CompilerGenerated(method.DeclaringType) ? "1" : "0",
                                method.HasBody ? "1" : "0",
                                method.HasBody ? method.Body.CodeSize.ToString(CultureInfo.InvariantCulture) : "0",
                                method.HasBody ? method.Body.Instructions.Count.ToString(CultureInfo.InvariantCulture) : "0",
                                method.HasBody ? method.Body.ExceptionHandlers.Count.ToString(CultureInfo.InvariantCulture) : "0",
                                MethodBodyHash(method),
                                "ok",
                                string.Empty
                            }));
                        }
                    }
                }
                catch (Exception error)
                {
                    census.WriteLine(string.Join("\t", new[]
                    {
                        Tsv(assemblyName),
                        Tsv("assets/bin/Data/Managed/" + assemblyName),
                        Sha256File(assemblyPath),
                        new FileInfo(assemblyPath).Length.ToString(CultureInfo.InvariantCulture),
                        string.Empty, "0", "0", "0", "0", "0", "0", "0", "0", string.Empty,
                        "error", Tsv(error.GetType().FullName + ": " + error.Message)
                    }));
                }
            }
        }
        return 0;
    }
}
