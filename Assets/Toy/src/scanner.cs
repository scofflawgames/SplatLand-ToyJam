using System;
using System.Collections.Generic;

using static Toy.TokenType;

namespace Toy {
	public class Scanner {
		//members
		readonly string source;
		List<Token> tokenList = new List<Token>();

		int start = 0;
		int current = 0;
		int line = 1;

		Dictionary<string, TokenType> tokenDictionary = new Dictionary<string, TokenType>() {
			{"null", NIL},
			{"print", PRINT},
			{"import", IMPORT},
			{"as", AS},
			{"var", VAR},
			{"const", CONST},
			{"true", TRUE},
			{"false", FALSE},
			{"function", FUNCTION},
			{"return", RETURN},
			{"if", IF},
			{"else", ELSE},
			{"do", DO},
			{"while", WHILE},
			{"for", FOR},
			{"foreach", FOREACH},
			{"in", IN},
			{"of", OF},
			{"break", BREAK},
			{"continue", CONTINUE},
			{"assert", ASSERT}
		};

		//methods
		public Scanner(string src) {
			source = src;
		}

		//scanning method
		public List<Token> ScanTokens() {
			while (!CheckAtEnd()) {
				start = current;
				ScanNextToken();
			}

			tokenList.Add(new Token(EOF, "", null, line));
			return tokenList;
		}

		void ScanNextToken() {
			char tok = Advance();
			switch(tok) {
				//handle comments / block comments / slashes
				case '/':
					if (Match('/')) {
						while (Peek() != '\n' && !CheckAtEnd()) Advance();
					} else if (Match('*')) {
						//eat the block comment
						while (!(Peek() == '*' && PeekNext() == '/') && !CheckAtEnd()) Advance();
						if (!CheckAtEnd()) {
							Advance(); //eat *
							Advance(); //eat /
						}
					} else if (Peek() == '=') {
						AddToken(SLASH_EQUAL);
						Advance();
					} else {
						AddToken(SLASH);
					}
					break;

				//handle whitespace
				case '\n':
					line++;
					break;

				case '\r':
				case '\t':
				case ' ':
					break;

				//handle single-char tokens
				case '(': AddToken(LEFT_PAREN); break;
				case ')': AddToken(RIGHT_PAREN); break;
				case '{': AddToken(LEFT_BRACE); break;
				case '}': AddToken(RIGHT_BRACE); break;
				case '[': AddToken(LEFT_BRACKET); break;
				case ']': AddToken(RIGHT_BRACKET); break;
				case ';': AddToken(SEMICOLON); break;
				case ',': AddToken(COMMA); break;

				//handle double-char tokens
				case '+':
					AddToken(Match('+') ? PLUS_PLUS : Match('=') ? PLUS_EQUAL : PLUS);
					break;

				case '-':
					AddToken(Match('-') ? MINUS_MINUS : Match('=') ? MINUS_EQUAL : MINUS);
					break;

				case '*':
					AddToken(Match('=') ? STAR_EQUAL : STAR);
					break;

				case '%':
					AddToken(Match('=') ? MODULO_EQUAL : MODULO);
					break;
	
				case '=':
					AddToken(Match('>') ? EQUAL_GREATER : Match('=') ? EQUAL_EQUAL : EQUAL);
					break;

				case '!':
					AddToken(Match('=') ? BANG_EQUAL : BANG);
					break;

				case '<':
					AddToken(Match('=') ? LESS_EQUAL : LESS);
					break;

				case '>':
					AddToken(Match('=') ? GREATER_EQUAL : GREATER);
					break;

				//these can ONLY be doubles
				case '&':
					if (Match('&')) {
						AddToken(AND_AND);
					} else {
						ErrorHandler.Error(line, "Unexpected character (Expected '&')");
					}
					break;

				case '|':
					if (Match('|')) {
						AddToken(OR_OR);
					} else if (Match('>')) {
						AddToken(OR_GREATER);
					} else {
						ErrorHandler.Error(line, "Unexpected character (Expected '|' or '>')");
					}
					break;

				//these can be single OR triple
				case '.':
					if (Match('.')) {
						if (Match('.')) {
							AddToken(DOT_DOT_DOT);
						} else {
							ErrorHandler.Error(line, "Unexpected character (Expected '...')");
						}
					} else {
						AddToken(DOT);
					}
					break;

				//ternary operator
				case '?': AddToken(QUESTION); break;
				case ':': AddToken(COLON); break;

				//handle longer lexemes
				case '"':
					HandleString();
					break;

				//otherwise
				default:
					if (CheckIsDigit(tok)) {
						HandleNumber();
					} else if (CheckIsAlpha(tok)) {
						HandleIdentifier();
					} else {
						ErrorHandler.Error(line, "Unexpected character");
					}
					break;
			}
		}

		void HandleString() {
			//read the string
			while (Peek() != '"' && !CheckAtEnd()) {
				if (Peek() == '\n') {
					line++;
				}
				Advance();
			}

			//unterminated string
			if (CheckAtEnd()) {
				ErrorHandler.Error(line, "Unterminated string");
				return;
			}

			//the closing "
			Advance();

			string str = source.Substring(start + 1, current-start - 2);
			AddToken(STRING, str);
		}

		void HandleNumber() {
			while (CheckIsDigit(Peek())) {
				Advance();
			}

			//look for a fractional part
			if (Peek() == '.' && CheckIsDigit(PeekNext())) {
				//consume the dot
				Advance();

				while(CheckIsDigit(Peek())) {
					Advance();
				}
			}

			AddToken(NUMBER, Convert.ToDouble(source.Substring(start, current-start)));
		}

		void HandleIdentifier() {
			while(CheckIsAlphaNumeric(Peek())) {
				Advance();
			}

			string text = source.Substring(start, current-start);

			if (tokenDictionary.ContainsKey(text)) {
				AddToken(tokenDictionary[text]);
			} else {
				AddToken(IDENTIFIER);
			}
		}

		//helpers
		bool CheckAtEnd() {
			return current >= source.Length;
		}

		char Advance() {
			current++;
			return source[current-1];
		}

		bool Match(char expected) {
			if (CheckAtEnd()) return false;
			if (source[current] != expected) return false;
			current++;
			return true;
		}

		char Peek() {
			if (CheckAtEnd()) return '\0';
			return source[current];
		}

		char PeekNext() {
			if (current+1 >= source.Length) return '\0';
			return source[current + 1];
		}

		bool CheckIsDigit(char c) {
			return c >= '0' && c <= '9';
		}

		bool CheckIsAlpha(char c) {
			return (c >= 'A' && c <= 'Z') ||
				(c >= 'a' && c <= 'z') ||
				c == '_';
		}

		bool CheckIsAlphaNumeric(char c) {
			return CheckIsAlpha(c) || CheckIsDigit(c);
		}

		void AddToken(TokenType type, object literal = null) {
			string text = source.Substring(start, current-start);
			tokenList.Add(new Token(type, text, literal, line));
		}
	}
}