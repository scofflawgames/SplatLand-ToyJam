using System.Collections;
using System.Collections.Generic;

namespace Toy {
	class Resolver : ExprVisitor<object>, StmtVisitor<object> {
		enum FunctionType {
			NONE,
			FUNCTION
		};

		enum LoopType {
			NONE,
			DO,
			WHILE,
			FOR
		};

		Interpreter interpreter;
		Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
		FunctionType currentFunctionType = FunctionType.NONE;
		LoopType currentLoopType = LoopType.NONE;

		//methods
		public Resolver(Interpreter interpreter) {
			this.interpreter = interpreter;
		}

		public object Visit(Print stmt) {
			Resolve(stmt.expression);
			return null;
		}

		public object Visit(Import stmt) {
			if (scopes.Count > 0) {
				throw new ErrorHandler.ResolverError(stmt.keyword, "Can only import at the global scope (no functions, loops or blocks)");
			}

			Resolve(stmt.library);

			if (stmt.alias != null) {
				Resolve(stmt.alias);
			}

			//a bit of type checking
			if (stmt.library == null || !(stmt.library is Literal && ((Literal)stmt.library).value is string)) {
				throw new ErrorHandler.ResolverError(stmt.keyword, "Import may only take a string literal for the library name");
			}

			if (stmt.alias != null) {
				if (!(stmt.alias is Variable)) {
					throw new ErrorHandler.ResolverError(stmt.keyword, "Import alias may only be an identifier");
				}
			}

			return null;
		}

		public object Visit(If stmt) {
			Resolve(stmt.cond);
			Resolve(stmt.thenBranch);
			if (stmt.elseBranch != null) {
				Resolve(stmt.elseBranch);
			}
			return null;
		}

		public object Visit(Do stmt) {
			LoopType enclosingLoopType = currentLoopType;
			currentLoopType = LoopType.DO;

			BeginScope();
			Resolve(stmt.body);
			Resolve(stmt.cond);
			EndScope();

			currentLoopType = enclosingLoopType;
			return null;
		}

		public object Visit(While stmt) {
			LoopType enclosingLoopType = currentLoopType;
			currentLoopType = LoopType.WHILE;

			BeginScope();
			Resolve(stmt.cond);
			Resolve(stmt.body);
			EndScope();

			currentLoopType = enclosingLoopType;
			return null;
		}

		public object Visit(For stmt) {
			LoopType enclosingLoopType = currentLoopType;
			currentLoopType = LoopType.FOR;

			BeginScope();
			Resolve(stmt.initializer);
			Resolve(stmt.cond);
			Resolve(stmt.increment);
			Resolve(stmt.body);
			EndScope();

			currentLoopType = enclosingLoopType;
			return null;
		}

		public object Visit(Break stmt) {
			if (currentLoopType == LoopType.NONE) {
				throw new ErrorHandler.ResolverError(stmt.keyword, "Can't break from outside of a loop");
			}

			return null;
		}

		public object Visit(Continue stmt) {
			if (currentLoopType == LoopType.NONE) {
				throw new ErrorHandler.ResolverError(stmt.keyword, "Can't continue from outside of a loop");
			}

			return null;
		}

		public object Visit(Return stmt) {
			if (currentFunctionType == FunctionType.NONE) {
				throw new ErrorHandler.ResolverError(stmt.keyword, "Can't return from outside of a function");
			}

			if (stmt.value != null) {
				Resolve(stmt.value);
			}
			return null;
		}

		public object Visit(Assert stmt) {
			Resolve(stmt.cond);

			//a bit of type checking
			if (stmt.message != null && !(stmt.message is Literal && ((Literal)stmt.message).value is string)) {
				throw new ErrorHandler.ResolverError(stmt.keyword, "Assert may only take a string literal as it's optional second argument");
			}

			return null;
		}

		public object Visit(Block stmt) {
			BeginScope();
			Resolve(stmt.statements);
			EndScope();
			return null;
		}

		public object Visit(Var stmt) {
			Declare(stmt.name);
			if (stmt.initializer != null) {
				Resolve(stmt.initializer);
			}
			Define(stmt.name);

			return null;
		}

		public object Visit(Const stmt) {
			Declare(stmt.name);
			Resolve(stmt.initializer);
			Define(stmt.name);
			return null;
		}

		public object Visit(Pass stmt) {
			//do nothing
			return null;
		}

		public object Visit(Expression stmt) {
			Resolve(stmt.expression);
			return null;
		}

		public object Visit(Variable expr) {
			if (scopes.Count > 0 && scopes.Peek().ContainsKey(expr.name.lexeme) && scopes.Peek()[expr.name.lexeme] == false) {
				ErrorHandler.Error(expr.name.line, "Can't read a local variable in it's own initializer");
			}

			ResolveLocal(expr);
			return null;
		}

		public object Visit(Assign expr) {
			if (expr.left is Variable) {
				ResolveLocal(expr.left);
			} else {
				Resolve(expr.left);
			}
			Resolve(expr.right);
			return null;
		}

		public object Visit(Increment expr) {
			Resolve(expr.variable);
			return null;
		}

		public object Visit(Literal expr) {
			return null;
		}

		public object Visit(Logical expr) {
			Resolve(expr.left);
			Resolve(expr.right);
			return null;
		}

		public object Visit(Unary expr) {
			Resolve(expr.right);
			return null;
		}

		public object Visit(Binary expr) {
			Resolve(expr.left);
			Resolve(expr.right);
			return null;
		}

		public object Visit(Call expr) {
			Resolve(expr.callee);
			foreach (Expr argument in expr.arguments) {
				Resolve(argument);
			}
			return null;
		}

		public object Visit(Index expr) {
			Resolve(expr.callee);

			if (expr.first == null) {
				throw new ErrorHandler.ResolverError(expr.bracket, "Expected literal or variable as an index");
			}

			Resolve(expr.first);
			if (expr.second != null) Resolve(expr.second);
			if (expr.third != null) Resolve(expr.third);
			return null;
		}

		public object Visit(Pipe expr) {
			Resolve(expr.callee);
			Resolve(expr.following);
			return null;
		}

		public object Visit(Function expr) {
			FunctionType enclosingFunctionType = currentFunctionType;
			currentFunctionType = FunctionType.FUNCTION;

			BeginScope();
			foreach (Expr param in expr.parameters) {
				Declare(((Variable)param).name);
				Define(((Variable)param).name);
			}
			Resolve(expr.body);
			EndScope();

			currentFunctionType = enclosingFunctionType;
			return null;
		}

		public object Visit(Property expr) {
			Resolve(expr.expression);
			return null;
		}

		public object Visit(Grouping expr) {
			Resolve(expr.expression);
			return null;
		}

		public object Visit(Ternary expr) {
			Resolve(expr.cond);
			Resolve(expr.left);
			Resolve(expr.right);
			return null;
		}

		//helpers
		public void Resolve(List<Stmt> statements) {
			foreach(Stmt stmt in statements) {
				Resolve(stmt);
			}
		}

		public object Resolve(Stmt stmt) {
			return stmt.Accept(this);
		}

		public object Resolve(Expr expr) {
			return expr.Accept(this);
		}

		void ResolveLocal(Expr expr) {
			for (int i = 0; i < scopes.Count; i++) {
				if (scopes.ToArray()[i].ContainsKey(((Variable)expr).name.lexeme)) {
					interpreter.Resolve(expr, i);
				}
			}
		}

		void BeginScope() {
			scopes.Push(new Dictionary<string, bool>());
		}

		void EndScope() {
			scopes.Pop();
		}

		void Declare(Token name) {
			if (scopes.Count == 0) return;

			if (scopes.Peek().ContainsKey(name.lexeme)) {
				throw new ErrorHandler.ResolverError(name, "A variable with this name has already been declared in this scope");
			}

			scopes.Peek()[name.lexeme] = false;
		}

		void Define(Token name) {
			if (scopes.Count == 0) return;

			scopes.Peek()[name.lexeme] = true;
		}
	}
}