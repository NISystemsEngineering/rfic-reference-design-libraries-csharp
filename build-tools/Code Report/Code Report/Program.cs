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
        #region Types
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
            [Option("ListDocs", HelpText = "Determines if documetnation should be listed in the report", Required = false)]
            public bool ListDescriptions { get; set; }
            [Option("ExludedPaths", Default = new string[] { "bin", "obj", "Solution", "Assembly" }, Required = false, Separator = '=')]
            public string[] ExcludedPaths { get; set; }
        }
        internal enum MemberTypes { Method, Type }
        internal struct MemberData
        {
            internal MemberTypes MemberType;
            internal string Description;
            internal SortedList<string, string> paramList;
        }
        internal struct MethodDocumentationParsingResults
        {
            internal string ReturnValueDescription;
            internal SortedList<string, string> ParameterDescriptions;
        }
        internal struct DocumentationParsingResults
        {
            internal string MemberDescription;
            internal MethodDocumentationParsingResults MethodDocumentation;
        }
        #endregion
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
           {
               PerformSearch(o);
           });
        }
        #region Parsing Functions
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
                DocumentationParsingResults memberDocumentation = ParseDocumentation(member);
                switch (member.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                        MethodDeclarationSyntax method = (MethodDeclarationSyntax)member;
                        // Get the return data type
                        TypeSyntax returnType = method.ReturnType;
                        if (returnType.ToString().ToLower() != "void")
                        {
                            paramList.Add("ReturnValue;Type=" + returnType.ToString(), memberDocumentation.MethodDocumentation.ReturnValueDescription);
                        }
                        // Get all input and output parameters
                        foreach (ParameterSyntax param in method.ParameterList.Parameters)
                        {
                            string parameterName = param.Identifier.ToString();
                            memberDocumentation.MethodDocumentation.ParameterDescriptions.TryGetValue(parameterName, out string parameterDescription);
                            paramList.Add(param.Identifier.ToString(), parameterDescription);
                        }
                        memData = new MemberData { MemberType = MemberTypes.Method, Description = memberDocumentation.MemberDescription, paramList = paramList };
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
                                DocumentationParsingResults paramDoc = ParseDocumentation(param);
                                for (int i = 0; i < param.Declaration.Variables.Count; i++)
                                {
                                    var variableDeclaration = param.Declaration.Variables[i];
                                    paramList.Add(variableDeclaration.Identifier.ToString(), paramDoc.MemberDescription);
                                }
                            }
                        }
                        memData = new MemberData { MemberType = MemberTypes.Type, Description = memberDocumentation.MemberDescription, paramList = paramList };
                        membersList.Add(myStruct.Identifier.ToString(), memData);
                        break;
                    case SyntaxKind.EnumDeclaration:
                        // A few enums are defined in the class modules
                        EnumDeclarationSyntax myEnum = (EnumDeclarationSyntax)member;
                        memData = new MemberData { MemberType = MemberTypes.Type, Description = memberDocumentation.MemberDescription, paramList = paramList };
                        membersList.Add(myEnum.Identifier.ToString(), memData);
                        break;
                    case SyntaxKind.ClassDeclaration:
                        // Recursively parse any sub-classes that are found
                        ClassDeclarationSyntax subClass = (ClassDeclarationSyntax)member;
                        xmlWriter.WriteStartElement("Class");
                        xmlWriter.WriteAttributeString("Name", subClass.Identifier.ToString());
                        DocumentationParsingResults classDoc = ParseDocumentation(subClass);
                        if (o.ListDescriptions)
                            xmlWriter.WriteElementString("Description", classDoc.MemberDescription);
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
            if (membersList.Count > 0)
            {
                // Now, write all the data we have gathered for this class that has been sorted to the file
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0) xmlWriter.WriteStartElement("Methods");
                    else xmlWriter.WriteStartElement("Types");
                    foreach (KeyValuePair<string, MemberData> pair in membersList)
                    {
                        if ((i == 0 && pair.Value.MemberType == MemberTypes.Method) ||
                            (i == 1 && pair.Value.MemberType == MemberTypes.Type))
                        {
                            xmlWriter.WriteStartElement(pair.Value.MemberType.ToString());
                            xmlWriter.WriteAttributeString("Name", pair.Key);
                            if (o.ListDescriptions) xmlWriter.WriteElementString("Description", pair.Value.Description);
                            if (o.ListParameters)
                            {
                                xmlWriter.WriteStartElement("Parameters");
                                foreach (KeyValuePair<string, string> paramPair in pair.Value.paramList)
                                {
                                    xmlWriter.WriteStartElement("Parameter");
                                    xmlWriter.WriteAttributeString("Name", paramPair.Key);
                                    if (o.ListDescriptions) xmlWriter.WriteElementString("Description", paramPair.Value);
                                    xmlWriter.WriteEndElement();
                                    //xmlWriter.WriteElementString(paramPair.Value, paramPair.Key);
                                }
                                xmlWriter.WriteEndElement();
                            }
                            xmlWriter.WriteEndElement();
                        }
                    }
                    xmlWriter.WriteEndElement();
                }
            }

        }
        public static DocumentationParsingResults ParseDocumentation(CSharpSyntaxNode member)
        {
            DocumentationParsingResults results = new DocumentationParsingResults();
            results.MethodDocumentation.ParameterDescriptions = new SortedList<string, string>();
            if (member.HasLeadingTrivia)
            {
                try
                {
                    var trivia = member.GetLeadingTrivia().Single(t =>
                        t.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia || t.Kind() == SyntaxKind.MultiLineCommentTrivia);

                    // Convert the trivia to a string, and strip out the tabs and /// 
                    string rawTrivia = Regex.Replace(trivia.ToString(), @"^\s+\/\/\/", "", RegexOptions.Multiline);
                    // Strip <see> tags from the text, but leave in place the referenced function or type
                    // i.e. <see cref="DownloadWaveform(NIRfsg, Waveform)"/> becomes DownloadWaveform
                    rawTrivia = Regex.Replace(rawTrivia, @"<see cref=""([\w\.]+)(?:\(.*\))?""\/>", "$1", RegexOptions.Multiline);
                    // Strip <paramRef> tags from the text, leaving in place the referenced parameter name
                    rawTrivia = Regex.Replace(rawTrivia, @"<paramref name=""(\w+)""\/>", "$1", RegexOptions.Multiline);
                    // Clear <para> tags
                    rawTrivia = Regex.Replace(rawTrivia, @"<\/*para>", "", RegexOptions.Multiline);

                    // The XML structure of the documentation is "rootless", so adding simple root node avoids parser errors
                    rawTrivia = "<root>" + rawTrivia + "</root>";
                    // Parse the documentation XML
                    XElement documentation = XElement.Parse(rawTrivia);

                    // Get the summary element which contains the description for the function, parameter, or type
                    string memberDescription = documentation.Element("summary").Value;
                    // Remove newlines from the description
                    memberDescription = Regex.Replace(memberDescription, "[\n\r]", "");
                    results.MemberDescription = memberDescription;

                    try
                    {
                        string returnValue = documentation.Element("returns").Value;
                        // Remove newlines from the description
                        returnValue = Regex.Replace(returnValue, "[\n\r]", "");
                        results.MethodDocumentation.ReturnValueDescription = returnValue;
                    }
                    catch (System.NullReferenceException) { }

                    List<XElement> parameters = documentation.Elements("param").ToList();
                    foreach (XElement param in parameters)
                    {
                        string parameterName = param.Attribute("name").Value;
                        string parameterDescription = param.Value;
                        // Remove newlines from the description
                        parameterDescription = Regex.Replace(parameterDescription, "[\n\r]", "");
                        results.MethodDocumentation.ParameterDescriptions.Add(parameterName, parameterDescription);
                    } 

                }
                // If no leading syntax is found, we catch the exception here and will return an empty struct
                catch (System.InvalidOperationException) { }
            }
            return results;
        }
        #endregion
    }
}
