using System.Collections.Generic;

using static Toy.TokenType;

namespace Toy {
	class Parser {
		readonly List<Token> tokenList;
		int current = 0;

		public Parser(List<Token> tokens) {
			tokenList = tokens;
		}

		public List<Stmt> ParseStatements() {
			List<Stmt> statements = new List<Stmt>();
			while(!CheckAtEnd()) {
				statements.Add(DeclarationRule());
			}
			return statements;
		}

		//grammar rules
		Stmt DeclarationRule() {
			try {
				if (Match(VAR)) return VarDeclarationRule();
				if (Match(CONST)) return ConstDeclarationRule();

				return StatementRule();
			} catch(ErrorHandler.ParserError) {
				Synchronize();
				return null;
			}
		}

		Stmt VarDeclarationRule() {
			Token name = Consume(IDENTIFIER, "Expected variable name");

			Expr initializer = null;
			if (Match(EQUAL)) {
				initializer = ExpressionRule();
			}

			Consume(SEMICOLON, "Expected ';' after variable declaration");
			return new Var(name, initializer);
		}

		Stmt ConstDeclarationRule() {
			Token name = Consume(IDENTIFIER, "Expected constant name");
			Consume(EQUAL, "Expected assignment in constant declaration");
			Expr initializer = ExpressionRule();

			Consume(SEMICOLON, "Expected ';' after constant declaration");
			return new Const(name, initializer);
		}

		Stmt StatementRule(bool breakable = false) {
			if (Match(PRINT)) return PrintStmt();
			if (Match(IMPORT)) return ImportStmt();
			if (Match(IF)) return IfStmt();
			if (Match(DO)) return DoStmt();
			if (Match(WHILE)) return WhileStmt();
			if (Match(FOR)) return ForStmt();
			if (Match(BREAK)) return BreakStmt();
			if (Match(CONTINUE)) return ContinueStmt();
			if (Match(RETURN)) return ReturnStmt();
			if (Match(ASSERT)) return AssertStmt();
			if (Match(LEFT_BRACE)) return new Block(BlockStmt(), breakable);

			if (Match(SEMICOLON)) {
				//empty statement
				return new Pass(Previous());
			}

			return ExpressionStmt();
		}

		Stmt PrintStmt() {
			Expr expr = ExpressionRule();
			Consume(SEMICOLON, "Expected ';' after print statement");
			return new Print(expr);
		}

		Stmt ImportStmt() {
			Token keyword = Previous();
			Expr library = ExpressionRule();
			Expr alias = null;
			if (Match(AS)) {
				alias = ExpressionRule();
			}
			Consume(SEMICOLON, "Expected ';' after import statement");
			return new Import(keyword, library, alias);
		}

		Stmt IfStmt() {
			Consume(LEFT_PAREN, "Expected '(' after if statement");
			Expr cond = ExpressionRule();
			Consume(RIGHT_PAREN, "Expected ')' after if condition");

			Stmt thenBranch = StatementRule();

			//implicitly create a block if the branch isn't enclosed by one
			if (!(thenBranch is Block)) {
				thenBranch = new Block(new List<Stmt>() {thenBranch}, false);
			}

			Stmt elseBranch = null;
			if (Match(ELSE)) {
				elseBranch = StatementRule();

				//implicitly create a block if the branch isn't enclosed by one
				if (!(elseBranch is Block)) {
					elseBranch = new Block(new List<Stmt>() {elseBranch}, false);
				}
			}

			return new If(cond, thenBranch, elseBranch);
		}

		Stmt DoStmt() {
			Stmt body = StatementRule(true);

			//implicitly create a block if the body isn't enclosed by one
			if (!(body is Block)) {
				body = new Block(new List<Stmt>() {body}, true);
			}

			//read the while condition
			Consume(WHILE, "Expected 'while' following do statement");
			Consume(LEFT_PAREN, "Expected '(' after do-while statement");
			Expr cond = ExpressionRule();
			Consume(RIGHT_PAREN, "Expected ')' after do-while condition");
			Consume(SEMICOLON, "Expected ';' after do-while condition");

			return new Do(body, cond);
		}

		Stmt WhileStmt() {
			Consume(LEFT_PAREN, "Expected '(' after while statement");
			Expr cond = ExpressionRule();
			Consume(RIGHT_PAREN, "Expected ')' after while condition");

			Stmt body = StatementRule(true);

			//implicitly create a block if the body isn't enclosed by one
			if (!(body is Block)) {
				body = new Block(new List<Stmt>() {body}, true);
			}

			return new While(cond, body);
		}

		Stmt ForStmt() {
			Consume(LEFT_PAREN, "Expected '(' after for statement");

			//initializer
			Stmt initializer;
			if (Match(SEMICOLON)) {
				initializer = null;
			} else if (Match(VAR)) {
				initializer = VarDeclarationRule();
			} else if (Match(CONST)) {
				initializer = ConstDeclarationRule();
			} else {
				initializer = ExpressionStmt();
			}

			//condition
			Expr cond = null;
			if (!CheckTokenType(SEMICOLON)) {
				cond = ExpressionRule();
			}
			Consume(SEMICOLON, "Expected ';' after for loop condition");

			//increment
			Expr increment = null;
			if (!CheckTokenType(RIGHT_PAREN)) {
				increment = ExpressionRule();
			}
			Consume(RIGHT_PAREN, "Expected ')' after for clauses");

			//body
			Stmt body = StatementRule(true);

			//implicitly create a block if the body isn't enclosed by one
			if (!(body is Block)) {
				body = new Block(new List<Stmt>() {body}, true);
			}

			return new For(initializer, cond, increment, body);
		}

		Stmt BreakStmt() {
			Stmt stmt = new Break(Previous());
			Consume(SEMICOLON, "Expected ';' after break statement");
			return stmt;
		}

		Stmt ContinueStmt() {
			Stmt stmt = new Continue(Previous());
			Consume(SEMICOLON, "Expected ';' after continue statement");
			return stmt;
		}

		Stmt ReturnStmt() {
			Token keyword = Previous();
			Expr value = null;
			if (!CheckTokenType(SEMICOLON)) {
				value = ExpressionRule();
			}

			Consume(SEMICOLON, "Expected ';' at end of return statement");
			return new Return(keyword, value);
		}

		Stmt AssertStmt() {
			Token keyword = Previous();

			Consume(LEFT_PAREN, "Expected '(' after assert statement");
			Expr cond = ExpressionRule();
			Expr message = null;
			if (Match(COMMA)) {
				message = ExpressionRule();
			}
			Consume(RIGHT_PAREN, "Expected ')' after assert expressions");
			Consume(SEMICOLON, "Expected ';' after assert statement");

			return new Assert(keyword, cond, message);
		}

		List<Stmt> BlockStmt() {
			List<Stmt> statements = new List<Stmt>();

			while(!CheckTokenType(RIGHT_BRACE) && !CheckAtEnd()) {
				statements.Add(DeclarationRule());
			}

			Consume(RIGHT_BRACE, "Expected '}' after block");
			return statements;
		}

		Stmt ExpressionStmt() {
			Expr expr = ExpressionRule();
			Consume(SEMICOLON, "Expected ';' after expression");
			return new Expression(expr);
		}

		Expr ExpressionRule() {
			return AssignmentRule();
		}

		Expr AssignmentRule() {
			Expr expr = TernaryRule();

			if (Match(EQUAL, PLUS_EQUAL, MINUS_EQUAL, STAR_EQUAL, SLASH_EQUAL, MODULO_EQUAL)) {
				Token token = Previous();
				Expr value = AssignmentRule();

				if (expr is Variable || expr is Index || expr is Property) {
					return new Assign(expr, token, value);
				}

				throw new ErrorHandler.ParserError(token, "Invalid assignment target");
			}

			return expr;
		}

		Expr TernaryRule() {
			Expr expr = OrRule();

			//handle ternary operator
			if (Match(QUESTION)) {
				Expr left = ExpressionRule();
				Consume(COLON, "Expected ':' in ternary operator");
				Expr right = ExpressionRule();
				expr = new Ternary(expr, left, right);
			}

			return expr;
		}

		Expr OrRule() {
			Expr expr = AndRule();

			if (Match(OR_OR)) {
				Token token = Previous();
				Expr right = OrRule();
				expr = new Logical(expr, token, right);
			}

			return expr;
		}

		Expr AndRule() {
			Expr expr = EqualityRule();

			if (Match(AND_AND)) {
				Token token = Previous();
				Expr right = AndRule();
				expr = new Logical(expr, token, right);
			}

			return expr;
		}

		Expr EqualityRule() {
			Expr expr = ComaprisonRule();

			while(Match(EQUAL_EQUAL, BANG_EQUAL)) {
				Token token = Previous();
				Expr right = ComaprisonRule();
				expr = new Binary(expr, token, right);
			}

			return expr;
		}

		Expr ComaprisonRule() {
			Expr expr = AdditionRule();

			while(Match(LESS, GREATER, LESS_EQUAL, GREATER_EQUAL)) {
				Token token = Previous();
				Expr right = AdditionRule();
				expr = new Binary(expr, token, right);
			}

			return expr;
		}

		Expr AdditionRule() {
			Expr expr = MultiplicationRule();

			while(Match(PLUS, MINUS)) {
				Token token = Previous();
				Expr right = MultiplicationRule();
				expr = new Binary(expr, token, right);
			}

			return expr;
		}

		Expr MultiplicationRule() {
			Expr expr = UnaryRule();

			while(Match(STAR, SLASH, MODULO)) {
				Token token = Previous();
				Expr right = UnaryRule();
				expr = new Binary(expr, token, right);
			}

			return expr;
		}

		Expr UnaryRule() {
			if (Match(BANG, MINUS)) {
				Token token = Previous();
				Expr right = UnaryRule();
				return new Unary(token, right);
			}

			return PrefixRule();
		}

		Expr PrefixRule() {
			if (Match(PLUS_PLUS, MINUS_MINUS)) {
				Token token = Previous();
				Consume(IDENTIFIER, "Expected identifier after prefix increment/decrement operator");
				Variable variable = new Variable(Previous());
				return new Increment(token, variable, true);
			}

			return PostfixRule();
		}

		Expr PostfixRule() {
			if (CheckTokenType(IDENTIFIER) && (PeekNext().type == PLUS_PLUS || PeekNext().type == MINUS_MINUS)) {
				Variable variable = new Variable(Advance());
				Token token = Advance();
				return new Increment(token, variable, false);
			}

			return CallRule();
		}

		Expr CallRule() {
			Expr expr = PrimaryRule();

			while(true) {
				if (Match(LEFT_PAREN)) {
					expr = FinishCall(expr);
				} else if (Match(LEFT_BRACKET)) {
					expr = FinishIndex(expr);
				} else if (Match(DOT)) {
					Token name = Consume(IDENTIFIER, "Expected property name after '.'");
					expr = new Property(expr, name);
				} else if (Match(OR_GREATER)) {
					expr = FinishPipe(expr);
				} else {
					break;
				}
			}

			return expr;
		}

		Expr FinishCall(Expr callee) {
			List<Expr> arguments = new List<Expr>();

			if (!CheckTokenType(RIGHT_PAREN)) {
				do {
					if (arguments.Count > 255) {
						ErrorHandler.Error(Peek().line, "Can't have more than 255 arguments");
					}

					arguments.Add(ExpressionRule());
				} while(Match(COMMA));
			}

			Token paren = Consume(RIGHT_PAREN, "Expected ')' after call");

			return new Call(callee, paren, arguments);
		}

		Expr FinishIndex(Expr callee) {
			Expr first = null;
			Expr second = null;
			Expr third = null;

			//read first
			if (!CheckTokenType(RIGHT_BRACKET) && !CheckTokenType(COLON)) {
				first = ExpressionRule();
			} else if (CheckTokenType(COLON)) {
				first = new Literal(double.NegativeInfinity);
			}

			//colon present, read second
			if (Match(COLON)) {
				if (!CheckTokenType(RIGHT_BRACKET) && !CheckTokenType(COLON)) {
					second = ExpressionRule();
				} else {
					second = new Literal(double.PositiveInfinity);
				}
			}

			//colon present, read third
			if (Match(COLON)) {
				third = ExpressionRule();
			}

			Token bracket = Consume(RIGHT_BRACKET, "Expected ']' after index");

			return new Index(callee, first, second, third, bracket);
		}

		Expr FinishPipe(Expr callee) {
			Token pipe = Previous();

			Expr following = ExpressionRule();

			return new Pipe(callee, pipe, following);
		}

		Expr PrimaryRule() {
			if (Match(TRUE)) return new Literal(true);
			if (Match(FALSE)) return new Literal(false);
			if (Match(NIL)) return new Literal(null);

			if (Match(NUMBER, STRING)) { //numbers and strings are built-in literals
				return new Literal(Previous().literal);
			}

			if (Match(IDENTIFIER)) {
				if (CheckTokenType(EQUAL_GREATER)) {
					Expr variable = new Variable(Previous());
					Consume(EQUAL_GREATER, "Expected '=>' in arrow function");
					return FunctionRule(new List<Expr>() { variable }, true);
				}
				return new Variable(Previous()); //Variable accesses constants as well
			}

			//function keyword
			if (Match(FUNCTION)) {
				Consume(LEFT_PAREN, "Expected '(' after function keyword");

				List<Expr> expressions = new List<Expr>();

				if (!CheckTokenType(RIGHT_PAREN)) {
					do {
						expressions.Add(ExpressionRule());
					} while (Match(COMMA));
				}

				Consume(RIGHT_PAREN, "Expected ')' after function argument list");

				return FunctionRule(expressions, false);
			}

			if (Match(LEFT_PAREN)) {
				List<Expr> expressions = new List<Expr>();

				if (!CheckTokenType(RIGHT_PAREN)) {
					do {
						expressions.Add(ExpressionRule());
					} while (Match(COMMA));
				}

				Consume(RIGHT_PAREN, "Expected ')' after grouping or arrow function argument list");

				//arrow function
				if (Match(EQUAL_GREATER)) {
					return FunctionRule(expressions, true);
				} else {
					if (expressions.Count != 1) {
						throw new ErrorHandler.ParserError(Peek(), "Incorrect number of expressions in grouping, expected 1 found " + expressions.Count);
					}
					return new Grouping(expressions[0]);
				}
			}

			throw new ErrorHandler.ParserError(Peek(), "Expected expression");
		}

		Expr FunctionRule(List<Expr> parameters, bool isArrowFunction) {
			//read the  opening brace?
			bool hasBraces = false;
			if (!isArrowFunction || CheckTokenType(LEFT_BRACE)) {
				//using the function keyword OR optional braces
				hasBraces = true;

				Consume(LEFT_BRACE, "Expected '{' after function declaration");
			}

			//read the body
			List<Stmt> body = new List<Stmt>();

			//check for empty body
			if (hasBraces && CheckTokenType(RIGHT_BRACE)) {
				body.Add(new Pass( Advance() ));
				return new Function(parameters, body);
			}

			//check for one-line body
			if (!hasBraces) {
				//implicit return inserted
				body.Add( new Return(new Token(RETURN, "implicit return", null, Previous().line), ExpressionRule()) );
				return new Function(parameters, body);
			}

			do {
				body.Add(DeclarationRule());
			} while(!CheckTokenType(RIGHT_BRACE) && !CheckAtEnd());

			//read the closing brace
			Consume(RIGHT_BRACE, "Expected '}' after function definition");

			//finally
			return new Function(parameters, body);
		}

		//helpers
		bool Match(params TokenType[] types) {
			foreach (TokenType type in types) {
				if (CheckTokenType(type)) {
					Advance();
					return true;
				}
			}

			return false;
		}

		bool CheckTokenType(TokenType type) {
			if (CheckAtEnd()) {
				return false;
			}
			return Peek().type == type;
		}

		bool CheckAtEnd() {
			return Peek().type == EOF;
		}

		Token Peek() {
			return tokenList[current];
		}

		Token PeekNext() {
			return tokenList[current + 1];
		}

		Token Previous() {
			return tokenList[current - 1];
		}

		Token Advance() {
			if (!CheckAtEnd()) {
				current++;
			}
			return Previous();
		}

		Token Consume(TokenType type, string message) {
			if (CheckTokenType(type)) {
				return Advance();
			}

			throw new ErrorHandler.ParserError(Peek(), message);
		}

		//error handling
		void Synchronize() {
			Advance();

			while (!CheckAtEnd()) {
				if (Previous().type == SEMICOLON) return;

				switch(Peek().type) {
					case PRINT:
					case IMPORT:
					case VAR:
					case CONST:
					case RETURN:
					case IF:
					case DO:
					case WHILE:
					case FOR:
					case FOREACH:
					case BREAK:
					case CONTINUE:
					case ASSERT:
						return;
				}

				Advance();
			}
		}
	}
}