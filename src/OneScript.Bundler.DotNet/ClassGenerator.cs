/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using System.IO;
using System.Linq;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine.Reflection;

namespace OneScript.Bundler.DotNet
{
    public class ClassGenerator
    {
        private const int offsetSize = 4;

        private readonly StringWriter _writer;
        private int _offset = 0;
        private string _offsetString = "";

        public ClassGenerator()
        {
            _writer = new StringWriter();
        }

        private int Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                _offsetString = new string(' ', _offset * offsetSize);
            }
        }

        public string GenerateClass(string classNamespace, UserScriptContextInstance context)
        {
            Offset = 0;
            WriteImports();
            BeginNameSpace(classNamespace);
            Offset++;
            WriteClass(context);
            EndNamespace();
            Offset--;

            return _writer.ToString();
        }

        private void WriteClass(UserScriptContextInstance context)
        {
            WriteLine($"class {context.SystemType.Name}");
            WriteLine("{");
            Offset++;

            WriteConstruction(context);
            WriteProperties(context);
            WriteMethods(context);

            Offset--;
            WriteLine("}");
        }

        private void WriteConstruction(UserScriptContextInstance context)
        {
            WriteLine("private readonly UserScriptContextInstance _instance;");

            WriteLine($"private {context.SystemType.Name}(UserScriptContextInstance instance)");
            WriteLine("{");
            Offset++;
            WriteLine("_instance = instance;");
            Offset--;
            WriteLine("}");
        }

        private void WriteProperties(UserScriptContextInstance context)
        {
            foreach (var propInfo in context.GetProperties())
            {
                // TODO: аннотации
                WriteLine($"public IValue {propInfo.Identifier}");
                WriteLine("{");
                Offset++;

                if (propInfo.CanGet)
                {
                    WriteLine("get { return _instance.GetPropValue("+propInfo.Index+") }");
                }

                if (propInfo.CanSet)
                {
                    WriteLine("set { _instance.SetPropValue("+propInfo.Index+", value) }");
                }

                Offset--;
                WriteLine("}");
            }
        }

        private void WriteMethods(UserScriptContextInstance context)
        {
            WriteLine("public dynamic AsDynamic() { return _instance; }");
            WriteLine("");

            foreach (var methodInfo in context.GetMethods())
            {
                if(!methodInfo.IsExport)
                    continue;

                // TODO: аннотации

                if (methodInfo.IsFunction)
                {
                    WriteLine("public IValue " + methodInfo.Name + "(");
                }
                else
                {
                    WriteLine("public void " + methodInfo.Name + "(");
                }

                Offset++;
                foreach (var param in methodInfo.Params)
                {
                    throw new NotImplementedException();
                }
                Offset--;
            }
        }

        private void BeginNameSpace(string classNamespace)
        {
            WriteLine($"namespace {classNamespace}");
            WriteLine("{");
        }

        private void EndNamespace()
        {
            WriteLine("}");
        }

        private void WriteImports()
        {
        }

        private void WriteLine(string content)
        {
            _writer.Write(_offsetString);
            _writer.Write(content);
            _writer.Write('\n');
        }
    }
}
