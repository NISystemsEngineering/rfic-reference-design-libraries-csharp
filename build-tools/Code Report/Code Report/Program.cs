using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using System.Xml;
using System.Collections.Generic;
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
                        foreach (ParameterSyntax param in method.ParameterList.Parameters)
                        {
                            paramList.Add(param.Identifier.ToString(), "Param");
                        }
                        memData = new MemberData { MemberTypeName = "Method", paramList = paramList };
                        membersList.Add(method.Identifier.ToString(), memData);
                        break;
                    case SyntaxKind.StructDeclaration:
                        StructDeclarationSyntax myStruct = (StructDeclarationSyntax)classMember;
                        foreach (MemberDeclarationSyntax structMem in myStruct.Members)
                        {
                            // Some structs have methods defined for them as well
                            if (structMem.Kind() == SyntaxKind.FieldDeclaration)
                            {
                                FieldDeclarationSyntax var = (FieldDeclarationSyntax)structMem;
                                paramList.Add(var.Declaration.Variables[0].Identifier.ToString(), "Param");
                            }
                        }
                        memData = new MemberData { MemberTypeName = "Type", paramList = paramList };
                        membersList.Add(myStruct.Identifier.ToString(), memData);
                        break;
                    case SyntaxKind.EnumDeclaration:
                        EnumDeclarationSyntax myEnum = (EnumDeclarationSyntax)classMember;
                        memData = new MemberData { MemberTypeName = "Type", paramList = paramList };
                        membersList.Add(myEnum.Identifier.ToString(), memData);
                        break;
                    case SyntaxKind.ClassDeclaration:
                        ClassDeclarationSyntax subClass = (ClassDeclarationSyntax)classMember;
                        ParseClass(subClass, xmlWriter);
                        break;
                    default:
                        break;
                }
            }
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
            xmlWriter.Flush();
        }
    }
}
