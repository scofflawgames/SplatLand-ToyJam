namespace Toy {
	public enum TokenType {
		//single character tokens
		LEFT_PAREN, RIGHT_PAREN,
		LEFT_BRACE, RIGHT_BRACE,
		LEFT_BRACKET, RIGHT_BRACKET,
		SEMICOLON, COMMA,

		//one or two character tokens
		PLUS, PLUS_EQUAL, PLUS_PLUS,
		MINUS, MINUS_EQUAL, MINUS_MINUS,
		STAR, STAR_EQUAL,
		SLASH, SLASH_EQUAL,
		MODULO, MODULO_EQUAL,

		BANG, BANG_EQUAL,
		EQUAL, EQUAL_EQUAL, EQUAL_GREATER, //EQUAL_GREATER is for the arrow syntax
		GREATER, GREATER_EQUAL,
		LESS, LESS_EQUAL,

		//these can ONLY be doubles
		AND_AND,
		OR_OR,
		OR_GREATER,

		//these can single OR triple
		DOT, DOT_DOT_DOT,

		//ternary operator
		QUESTION, COLON,

		//literals
		IDENTIFIER, NUMBER, STRING, PLUGIN,

		//keywords (nil = null)
		NIL, PRINT, IMPORT, AS, VAR, CONST, TRUE, FALSE, FUNCTION, RETURN,
		IF, ELSE, DO, WHILE, FOR, FOREACH, IN, OF, BREAK, CONTINUE, ASSERT,

		PASS, EOF
	}
}