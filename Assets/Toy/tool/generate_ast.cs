using System;
using System.IO;

namespace Tool {
	class GenerateAst {
		static int Main(string[] args) {
			if (args.Length != 1) {
				Console.Error.WriteLine("Usage: generate_ast <outdir>");
				return -1;
			}

			DefineAst(args[0], "Expr", new string[] {
				"Variable: Token name",
				"Assign: Expr left, Token oper, Expr right",
				"Increment: Token oper, Variable variable, bool prefix",
				"Literal: object value",
				"Logical: Expr left, Token oper, Expr right",
				"Unary: Token oper, Expr right",
				"Binary: Expr left, Token oper, Expr right",
				"Call: Expr callee, Token paren, List<Expr> arguments",
				"Index: Expr callee, Expr first, Expr second, Expr third, Token bracket",
				"Pipe: Expr callee, Token pipe, Expr following",
				"Function: List<Expr> parameters, List<Stmt> body",
				"Property: Expr expression, Token name",
				"Grouping: Expr expression",
				"Ternary: Expr cond, Expr left, Expr right"
			});

			DefineAst(args[0], "Stmt", new string[] {
				"Print: Expr expression",
				"Import: Token keyword, Expr library, Expr alias",
				"If: Expr cond, Stmt thenBranch, Stmt elseBranch",
				"Do: Stmt body, Expr cond",
				"While: Expr cond, Stmt body",
				"For: Stmt initializer, Expr cond, Expr increment, Stmt body",
				"Break: Token keyword",
				"Continue: Token keyword",
				"Return: Token keyword, Expr value",
				"Block: List<Stmt> statements, bool breakable",
				"Var: Token name, Expr initializer",
				"Const: Token name, Expr initializer",
				"Assert: Token keyword, Expr cond, Expr message",
				"Pass: Token keyword",
				"Expression: Expr expression"
			});

			return 0;
		}

		static void DefineAst(string outDir, string baseName, string[] types) {
			StreamWriter outStream = new StreamWriter($"{outDir}/{baseName.ToLower()}.cs");

			//libraries
			outStream.WriteLine("using System.Collections.Generic;");
			outStream.WriteLine("");

			//the namespace
			outStream.WriteLine("namespace Toy {");

			//the base class
			outStream.WriteLine("\tpublic abstract class " + baseName + " {");
			outStream.WriteLine("\t\tpublic abstract R Accept<R>(" + baseName + "Visitor<R> visitor);");
			outStream.WriteLine("\t}");
			outStream.WriteLine("");

			//the visitor interface
			DefineVisitor(outStream, baseName, types);

			outStream.WriteLine("");

			//each type
			foreach(string type in types) {
				string[] arr = type.Split(new char[] {':'});
				string className = arr[0].Trim();
				string members = arr[1].Trim();
				DefineType(outStream, baseName, className, members);
			}

			outStream.WriteLine("}");
			outStream.Close();
		}

		static void DefineType(StreamWriter outStream, string baseName, string className, string types) {
			outStream.WriteLine("\tpublic class " + className + " : " + baseName + " {");

			//get each field
			string[] fields = types.Split(new char[] {','});

			//constructor
			outStream.WriteLine("\t\tpublic " + className + "(" + types + ") {");

			//assign each field
			foreach (string field in fields) {
				string name = field.Trim().Split(new char[] {' '})[1];
				outStream.WriteLine("\t\t\tthis." + name + " = " + name + ";");
			}

			outStream.WriteLine("\t\t}");
			outStream.WriteLine("");

			//visitor pattern
			outStream.WriteLine("\t\tpublic override R Accept<R>(" + baseName + "Visitor<R> visitor) {");
			outStream.WriteLine("\t\t\treturn visitor.Visit(this);");
			outStream.WriteLine("\t\t}");

			outStream.WriteLine("");

			//public members
			foreach (string field in fields) {
				outStream.WriteLine("\t\tpublic " + field.Trim() + ";");
			}

			outStream.WriteLine("\t}");
			outStream.WriteLine("");
		}

		static void DefineVisitor(StreamWriter outStream, string baseName, string[] types) {
			//the visitor interface
			outStream.WriteLine("\tpublic interface " + baseName + "Visitor<R> {");

			foreach(string type in types) {
				string typeName = type.Split(new char[] {':'})[0].Trim();
				outStream.WriteLine($"\t\tR Visit({typeName} {baseName});");
			}

			outStream.WriteLine("\t}");
		}
	}
}