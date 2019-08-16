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

namespace Code_Report
{
    class Program
    {
        public static void Main()
        { 

            string[] csFiles = Directory.GetFiles(@"C:\github\rf-adv-reference-design-libraries\Source\", "*.cs", SearchOption.AllDirectories);
            Array.Sort(csFiles);

            string programText;
            XmlWriter xml = XmlWriter.Create("Test.xml");
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
            //Console.Write(xmlText.ToString());

            /*
            MemberDeclarationSyntax firstMember = root.Members[0];
            Console.WriteLine($"The first member is a {firstMember.Kind()}.");
            var helloWorldDeclaration = (NamespaceDeclarationSyntax)firstMember;
            Console.WriteLine($"There are {helloWorldDeclaration.Members.Count} members declared in this namespace.");
            Console.WriteLine($"The first member is a {helloWorldDeclaration.Members[0].Kind()}.");

            var programDeclaration = (ClassDeclarationSyntax)helloWorldDeclaration.Members[0];
            Console.WriteLine($"There are {programDeclaration.Members.Count} members declared in the {programDeclaration.Identifier} class.");
            Console.WriteLine($"The first member is a {programDeclaration.Members[0].Kind()}.");
            var mainDeclaration = (MethodDeclarationSyntax)programDeclaration.Members[0];
            Console.WriteLine($"The return type of the {mainDeclaration.Identifier} method is {mainDeclaration.ReturnType}.");
            Console.WriteLine($"The method has {mainDeclaration.ParameterList.Parameters.Count} parameters.");
            foreach (ParameterSyntax item in mainDeclaration.ParameterList.Parameters)
                Console.WriteLine($"The type of the {item.Identifier} parameter is {item.Type}.");
            Console.WriteLine($"The body text of the {mainDeclaration.Identifier} method follows:");
            Console.WriteLine(mainDeclaration.Body.ToFullString());

            var argsParameter = mainDeclaration.ParameterList.Parameters[0];*/

        }
        static void ParseClass(ClassDeclarationSyntax myClass, XmlWriter xmlWriter)
        {
            string className = myClass.Identifier.ToString();
            xmlWriter.WriteStartElement("Class");
            xmlWriter.WriteAttributeString("Name", myClass.Identifier.ToString());
            foreach (MemberDeclarationSyntax classMember in myClass.Members)
            {
                switch (classMember.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                        MethodDeclarationSyntax method = (MethodDeclarationSyntax)classMember;
                        List<string> parameterList = new List<string>();
                        StringBuilder sb = new StringBuilder();
                        //Console.WriteLine($"Method {method.Identifier} found with members {sb.ToString()}");
                        xmlWriter.WriteStartElement("Method");
                        xmlWriter.WriteAttributeString("Name", method.Identifier.ToString());
                        foreach (ParameterSyntax param in method.ParameterList.Parameters)
                        {
                            //xmlWriter.WriteElementString("Param", param.Identifier.ToString());
                        }
                        xmlWriter.WriteEndElement();
                        break;
                    /*case SyntaxKind.PropertyDeclaration:
                        break;*/
                    case SyntaxKind.StructDeclaration:
                        StructDeclarationSyntax myStruct = (StructDeclarationSyntax)classMember;
                        xmlWriter.WriteStartElement("Type");
                        xmlWriter.WriteAttributeString("Name", myStruct.Identifier.ToString());
                        xmlWriter.WriteEndElement();
                        Console.WriteLine($"Struct {myStruct.Identifier} found with members {myStruct.TypeParameterList}.");
                        break;
                    case SyntaxKind.EnumDeclaration:
                        EnumDeclarationSyntax myEnum = (EnumDeclarationSyntax)classMember;
                        xmlWriter.WriteStartElement("Type");
                        xmlWriter.WriteAttributeString("Name", myEnum.Identifier.ToString());
                        xmlWriter.WriteEndElement();
                        Console.WriteLine($"Enum {myEnum.Identifier} found.");
                        break;
                    case SyntaxKind.ClassDeclaration:
                        ClassDeclarationSyntax subClass = (ClassDeclarationSyntax)classMember;
                        ParseClass(subClass, xmlWriter);
                        break;
                    default:
                        //Console.WriteLine($"Member {classMember.Kind()} found.");
                        break;
                }
            }
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
        }
    }
}
