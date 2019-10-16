using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System.Xml;
using System.Text.RegularExpressions;

namespace Code_Report
{
    class Program
    {
        private static string SearchPath;
        private static string ReportPath = "Report.xml";
        private static bool RecursiveSearch = true;
        private static bool ListParameters = false;
        public static void Main(string[] args)
        {
            string flag, value;
            #region Parse Arguments
            foreach (string argument in args)
            {
                // Matches argument syntax of: arg=value OR
                // arg="value"
                Regex rx = new Regex("(.+)=\"*([^\"]+)");
                Match match = rx.Match(argument);
                if (match.Success)
                {
                    flag = match.Groups[1].Value;
                    value = match.Groups[2].Value;
                    switch (flag)
                    {
                        case "SearchPath":
                            SearchPath = value;
                            break;
                        case "ReportPath":
                            ReportPath = value;
                            break;
                        case "Recursive":
                            RecursiveSearch = bool.Parse(value);
                            break;
                        case "ListParams":
                            ListParameters = bool.Parse(value);
                            break;
                        default:
                            break;
                    }
                }
                else
                    throw new System.ArgumentException($"The specified argument \"{argument}\" is not valid");
            }
            #endregion
            SearchOption searchMode = RecursiveSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            SearchPath = Path.GetFullPath(SearchPath);
            ReportPath = Path.GetFullPath(ReportPath);

            string[] csFiles = Directory.GetFiles(SearchPath, "*.cs", searchMode);
            Array.Sort(csFiles);

            string programText;
            XmlWriterSettings config = new XmlWriterSettings();
            config.Indent = true;
            XmlWriter xml = XmlWriter.Create(ReportPath, config);
            xml.WriteStartElement("Modules");
            
            foreach(string filePath in csFiles)
            {
                programText = File.ReadAllText(filePath);
                SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
                CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
                foreach (MemberDeclarationSyntax member in root.Members)
                {
                    if (member.Kind() == SyntaxKind.NamespaceDeclaration)
                    {
                        NamespaceDeclarationSyntax nameSpace = (NamespaceDeclarationSyntax)member;
                        foreach (MemberDeclarationSyntax subMember in nameSpace.Members)
                        {
                            if (subMember.Kind() == SyntaxKind.ClassDeclaration)
                            {
                                ClassDeclarationSyntax myClass = (ClassDeclarationSyntax)subMember;
                                ParseClass(myClass, xml);
                            }

                        }
                    }
                }
            }
            xml.WriteEndElement();
            xml.Flush();
            xml.Close();
        }
        internal struct MemberData
        {
            internal string MemberTypeName;
            internal SortedList<string, string> paramList;
        }
        static void ParseClass(ClassDeclarationSyntax myClass, XmlWriter xmlWriter)
        {
            string className = myClass.Identifier.ToString();
            xmlWriter.WriteStartElement("Class");
            xmlWriter.WriteAttributeString("Name", myClass.Identifier.ToString());

            // These sorted lists are utilized so that we can drill into the code in the order it is presented,
            // while also creating the sorted list that we want to export at the same time. We cannot merely
            // sort the items beforehand because we need to sort by item name, but do not have access to the
            // name beforehand until we determine the type of object.

            // We add specific data to the value attribute of the sorted list, while using the item name
            // as the key so that they are sorted properly.
            SortedList<string, MemberData> membersList = new SortedList<string, MemberData>();
            SortedList<string, string> paramList;
            MemberData memData;
            foreach (MemberDeclarationSyntax classMember in myClass.Members)
            {
                paramList = new SortedList<string, string>(); 
                switch (classMember.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                        MethodDeclarationSyntax method = (MethodDeclarationSyntax)classMember;
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
                        StructDeclarationSyntax myStruct = (StructDeclarationSyntax)classMember;
                        // Get all members of the struct
                        foreach (MemberDeclarationSyntax structMem in myStruct.Members)
                        {
                            // Some structs have methods defined for them as well
                            if (structMem.Kind() == SyntaxKind.FieldDeclaration)
                            {
                                FieldDeclarationSyntax var = (FieldDeclarationSyntax)structMem;
                                for (int i = 0; i < var.Declaration.Variables.Count; i++)
                                    paramList.Add(var.Declaration.Variables[i].Identifier.ToString(), "Param");
                            }
                        }
                        memData = new MemberData { MemberTypeName = "Type", paramList = paramList };
                        membersList.Add(myStruct.Identifier.ToString(), memData);
                        break;
                    case SyntaxKind.EnumDeclaration:
                        // A few enums are defined in the class modules
                        EnumDeclarationSyntax myEnum = (EnumDeclarationSyntax)classMember;
                        memData = new MemberData { MemberTypeName = "Type", paramList = paramList };
                        membersList.Add(myEnum.Identifier.ToString(), memData);
                        break;
                    case SyntaxKind.ClassDeclaration:
                        // Recursively parse any sub-classes that are found
                        ClassDeclarationSyntax subClass = (ClassDeclarationSyntax)classMember;
                        ParseClass(subClass, xmlWriter);
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
                if (ListParameters)
                {
                    foreach (KeyValuePair<string, string> paramPair in pair.Value.paramList)
                    {
                        xmlWriter.WriteElementString(paramPair.Value, paramPair.Key);
                    }
                }
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }
    }
}
