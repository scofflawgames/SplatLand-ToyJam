# String

Unlike other libraries, the string library is built into the string type itself. This means you don't have to use `import "String";` to access the string libraries.

## Usage

```
var greeting = "Hello world";

print greeting[0:4]; //Hello
print greeting[::-1]; //dlrow olleH

greeting[0:4] = "Goodnight";

print greeting; //Goodnight world

print greeting.IndexOf("oo"); //1
print greeting.Length(); //11
```

## Indexing

Strings can be indexed in several ways. characters can be accessed using traditional bracket notation:

```
str[x];
```

However, "slice notation" is also available:

```
str[x:y];
```

Here, "x" and "y" indicate two indexes within the string, and returns a new substring beginning at "x" and ending at "y" (inclusive). If either is omitted, then the first and last character positions respectfully are used.

Replacing parts of a string with another string is possible using slice notation:

```
str = "hello world";
str[6:11] = "user";
print str; //hello user
```

A third argument is possible:

```
str[x:y:z];
```

Here, "z" indicates how to count the characters - a positive number starts from the beginning of the array, while a negative one counts from the end. Also, if a number other than 1 or -1 is used, then every nth element is selected:

```
print str; //Hello world
print str[::2]; //Hlowrd
print str[::-2]; //drwolH
```

0 cannot be used as the third argument.

## Length()

This function returns the length of the calling string.

## ToLower()

This function returns a new string, which is the same as the calling string, except all characters are lower case.

## ToUpper()

This function returns a new string, which is the same as the calling string, except all characters are upper case.

## Replace(pat, rep)

For each instance of "pat" that it finds in the calling string, it replaces it with "rep", then returns the new string.

## Trim(chars)

Every character in the string "chars" is removed from the calling string's beginning and end, and the new string is returned.

## IndexOf(str)

This function returns the position of the first instance of "str" in the calling string.

## LastIndexOf(str)

This function returns the position of the last instance of "str" in the calling string.

