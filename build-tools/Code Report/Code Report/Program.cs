using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using CommandLine;
using System.Linq;

namespace Code_Report
{
    class Program
    {
        public class Options
        {
            [Option("SearchPath", Required = true, HelpText = "Directory to search for .cs files", Separator = '=')]
            public string SearchPath { get; set; }
            [Option("ReportPath", Default = "Report.xml", HelpText = "Path to output the XML report to", Required = false, Separator = '=')]
            public string ReportPath { get; set; }
            [Option("Recursive", HelpText = "Determines if the search should recursively search through the search path for .cs files", Required = false, Default = true, Separator = '=')]
            public bool RecursiveSearch { get; set; }
            [Option("ListParams", HelpText = "Determines if function inputs/outputs and type parameters should be listed in the report", Required = false, Default = false, Separator = '=')]
            public bool ListParameters { get; set; }
            [Option("ExludedPaths", Default = new string[] { "bin", "obj", "Solution", "Assembly" }, Required = false, Separator = '=')]
            public string[] ExcludedPaths { get; set; }
        }
        /*private static string SearchPath;
        private static string ReportPath = "Report.xml";
        private static bool RecursiveSearch = true;
        private static bool ListParameters = false;*/
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
           {
               PerformSearch(o);
           });
        }
        public static void PerformSearch(Options o)
        {
            SearchOption searchMode = o.RecursiveSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            string SearchPath = Path.GetFullPath(o.SearchPath);
            string ReportPath = Path.GetFullPath(o.ReportPath);

            string[] csFiles = Directory.GetFiles(SearchPath, "*.cs", searchMode);

            Array.Sort(csFiles);

            // Configure XML document
            XmlWriterSettings config = new XmlWriterSettings { Indent = true };
            XmlWriter xml = XmlWriter.Create(ReportPath, config);
            xml.WriteStartElement("Modules");

            string programText;
            string fileExclusionString = string.Join("|", o.ExcludedPaths);

            foreach (string filePath in csFiles)
            {
                if (!Regex.IsMatch(filePath, fileExclusionString))
                {
                    xml.WriteStartElement("File");
                    xml.WriteAttributeString("Name", Path.GetFileNameWithoutExtension(filePath));
                    programText = File.ReadAllText(filePath);
                    SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
                    CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

                    foreach (MemberDeclarationSyntax member in root.Members)
                    {
                        if (member.Kind() == SyntaxKind.NamespaceDeclaration)
                        {
                            ParseNamespace(member, xml, o);
                        }
                    }
                    xml.WriteEndElement();
                }
            }
            xml.WriteEndDocument();
            xml.Flush();
            xml.Close();
        }
        internal struct MemberData
        {
            internal string MemberTypeName;
            internal SortedList<string, string> paramList;
        }
        static void ParseNamespace(MemberDeclarationSyntax member, XmlWriter xml, Options o)
        {
            NamespaceDeclarationSyntax nameSpace = (NamespaceDeclarationSyntax)member;
            ParseMembers(nameSpace.Members, xml, o);
        }
        static void ParseMembers(SyntaxList<MemberDeclarationSyntax> members, XmlWriter xmlWriter, Options o)
        {
            // These sorted lists are utilized so that we can drill into the code in the order it is presented,
            // while also creating the sorted list that we want to export at the same time. We cannot merely
            // sort the items beforehand because we need to sort by item name, but do not have access to the
            // name beforehand until we determine the type of object.

            // We add specific data to the value attribute of the sorted list, while using the item name
            // as the key so that they are sorted properly.
            SortedList<string, MemberData> membersList = new SortedList<string, MemberData>();
            SortedList<string, string> paramList;
            MemberData memData;
            foreach (MemberDeclarationSyntax member in members)
            {
                paramList = new SortedList<string, string>();
                switch (member.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                        MethodDeclarationSyntax method = (MethodDeclarationSyntax)member;
                        // Get the return data type
                        TypeSyntax returnType = method.ReturnType;
                        if (returnType.Kind() == SyntaxKind.IdentifierName)
                        {
                            IdentifierNameSyntax returnValue = (IdentifierNameSyntax)returnType;
                            paramList.Add(returnValue.Identifier.ToString(), "Param");
                        }
                        // Get all input and output parameters
                        foreach (ParameterSyntax param in method.ParameterList.Parameters)
                        {
                            paramList.Add(param.Identifier.ToString(), "Param");
                        }
                        memData = new MemberData { MemberTypeName = "Method", paramList = paramList };
                        membersList.Add(method.Identifier.ToString(), memData);
                        break;
                    case SyntaxKind.StructDeclaration:
                        StructDeclarationSyntax myStruct = (StructDeclarationSyntax)member;
                        // Get all members of the struct
                        foreach (MemberDeclarationSyntax structMem in myStruct.Members)
                        {
                            // Some structs have methods defined for them as well
                            if (structMem.Kind() == SyntaxKind.FieldDeclaration)
                            {
                                FieldDeclarationSyntax param = (FieldDeclarationSyntax)structMem;
                                for (int i = 0; i < param.Declaration.Variables.Count; i++)
                                {
                                    paramList.Add(param.Declaration.Variables[i].Identifier.ToString(), "Param");
                                    // Starting point for reading documentation from parameters
                                    /*if (param.HasLeadingTrivia)
                                    {
                                        var triva = param.GetLeadingTrivia().Single(t => 
                                            t.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia || t.Kind() == SyntaxKind.MultiLineCommentTrivia);
                                    }*/
                                }
                            }
                        }
                        memData = new MemberData { MemberTypeName = "Type", paramList = paramList };
                        membersList.Add(myStruct.Identifier.ToString(), memData);
                        break;
                    case SyntaxKind.EnumDeclaration:
                        // A few enums are defined in the class modules
                        EnumDeclarationSyntax myEnum = (EnumDeclarationSyntax)member;
                        memData = new MemberData { MemberTypeName = "Type", paramList = paramList };
                        membersList.Add(myEnum.Identifier.ToString(), memData);
                        break;
                    case SyntaxKind.ClassDeclaration:
                        // Recursively parse any sub-classes that are found
                        ClassDeclarationSyntax subClass = (ClassDeclarationSyntax)member;
                        xmlWriter.WriteStartElement("Class");
                        xmlWriter.WriteAttributeString("Name", subClass.Identifier.ToString());
                        ParseMembers(subClass.Members, xmlWriter, o);
                        xmlWriter.WriteEndElement();
                        break;
                    case SyntaxKind.NamespaceDeclaration:
                        // Recurse through the namespace and new members
                        ParseNamespace(member, xmlWriter, o);
                        break;
                    default:
                        break;
                }
            }
            // Now, write all the data we have gathered for this class that has been sorted to the file
            foreach (KeyValuePair<string, MemberData> pair in membersList)
            {
                xmlWriter.WriteStartElement(pair.Value.MemberTypeName);
                xmlWriter.WriteAttributeString("Name", pair.Key);
                if (o.ListParameters)
                {
                    foreach (KeyValuePair<string, string> paramPair in pair.Value.paramList)
                    {
                        xmlWriter.WriteElementString(paramPair.Value, paramPair.Key);
                    }
                }
                xmlWriter.WriteEndElement();
            }
        }
    }
}
