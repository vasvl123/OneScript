/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace OneScript.Bundler.DotNet
{
    public static class CompileTools
    {
        private static CSharpCompilation CreateCompilationWithMscorlib(
            string assemblyOrModuleName,
            string code,
            CSharpCompilationOptions compilerOptions = null,
            IEnumerable<MetadataReference> references = null)
        {
            // create the syntax tree
            SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(code, null, "");

            // get the reference to mscore library
            MetadataReference mscoreLibReference =
                AssemblyMetadata
                    .CreateFromFile(typeof(string).Assembly.Location)
                    .GetReference();

            // create the allReferences collection consisting of
            // mscore reference and all the references passed to the method
            IEnumerable<MetadataReference> allReferences =
                new MetadataReference[] { mscoreLibReference };
            if (references != null)
            {
                allReferences = allReferences.Concat(references);
            }

            // create and return the compilation
            CSharpCompilation compilation = CSharpCompilation.Create
            (
                assemblyOrModuleName,
                new[] { syntaxTree },
                options: compilerOptions,
                references: allReferences
            );

            return compilation;
        }
    }
}
