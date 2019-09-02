# Toy Language

Welcome to the reference for the Toy programming language!

I'm still developing this language and discovering all of it's incedental nooks and crannys, but hopefully the core of the language is solid enough that I can begin to teach people to use it. This guide on how to use the language will also double as the official documentation for the time being.

If you're familiar with JavaScript, you will see a lot of similarities. However, there are also differences that I hope will make my language stand out in time. I don't expect it to reach JavaScript's popularity, but I would like it to be useful.

# Other Documents

A full break down of the language's [grammar is available](grammar) for those who are interested.

A list of [available libraries](reference_libraries.md) are included.

One of the more interesting features of Toy are the [plugins](reference_plugins.md), which are detailed separately from the language. Array and Dictionary are included as examples for people who want to write their own.

If you want to use Toy with the Unity game engine, start with the [Unity reference](reference_unity.md), or the [Unity tutorial](tutorial_unity.md).

# Basics

## Print

Let's start with the most fundemental keyword - `print`.

```
print "hello world";
```

`print` is followed by a single argument that resolves to either a literal or a variable, which is converted to a string and then outputted to the console, followed by a newline.

`print` is a keyword, rather than a library function. This is partially due to Toy's origin as a [lox derivative](http://craftinginterpreters.com/), but I've decided to keep it, as it makes it easy to debug code without requiring a library of some kind (and all of the baggage that comes with). You *can* use it with parentheses, but they're not required at all.

## Comments

Toy supports two types of comments, `//single line` and `/* block */`. Comments are used to leave notes for yourself in the code. Comments are ignored.

```
var a = "foo"; //this is a comment

/*
so is this
*/
```

## Datatypes

These are the literal datatypes currently built into the language:

* `null`
* Boolean - the literals `true` and `false`
* Number - any double-precision floating point number
* String - a string of characters enclosed in quotation marks `""`
* Function - functions are data
* Plugin - used by external plugins

## Truthyness

Everything is considered "truthy" except:

* the literal `null`
* the literal `false`
* the number 0

## Operators

The following mathematical operators are available - if you want to know what they mean, a quick google search will help:

```
+  -  *  /  %  +=  -=  *=  /=  %=  ++(prefix)  --(prefix)  (postfix)++  (postfix)--
```

Likewise, the following logical operators are available:

```
!  !=  ==  <  >  <=  >=  &&  ||  ?:
```

All of the mathematical operators can be used on numbers, while `+` and `+=` can be used on strings for concatenation. Also, `==` and `!=` can be used on strings and numbers to compare them; comparing numbers to booleans will use their truthiness. Plugin instances are equal to themselves, but not others of the same type (unless specifically coded that way with the Equals() method, see [plugins](plugins.md)). Functions only equal themselves.

Remember, `&&` is more tightly bound than `||`.

There is also the pipe operator: `|>`, which is syntactic sugar for nesting calls:

```
//replace this:
print increment(double(increment(double(5)))); //23

//with this:
print 5 |> double |> increment |> double |> increment; //23
```

## Var and Const

Side Note: For those familiar with JavaScript, know that there is no variable hoisting in Toy. If you don't know what this means, ignore this.

To declare a variable, use the keyword `var`:

```
var foo = 42;
var bar = "hello world";
```

Variables can be used in place of literals at any time, and can be altered and re-assigned. Constants are different - once they are assigned at declaration, they cannot be changed or redefined:

```
const foo = 42;
foo++; //Error!
```

Otherwise, constants are just like variables. Plugins declared as const can alter their internal state, but can't be redefined:

```
const array = Array();
array.Push("foo"); //Good!
array = Array(); //Error!
```

## If-Else

The keywords `if` and `else` are used to determine different paths that the program can take, based on a condition. If the condition is truthy, then the `if`-path executes, otherwise the `else`-path does. The else keyword and path are optional.

```
if (1 < 2) {
	print "this will print to the console";
} else {
	print "this will not";
}
```

multiple `if`-`else` statements can be chained:

```
if (value == 1) {
	print "one";
} else if (value == 2) {
	print "two";
} else if (value == 3) {
	print "three";
} else {
	print "unknown value";
}
```

The braces around the different paths are optional, but if they are omitted, they will be implicitly inserted.

## While and Do-While

The `while` keyword loops over a block of code as long as the condition is truthy:

```
var counter = 0;
while (counter < 10) {
	print counter;
	counter++;
}
```

Another way to use the `while` keyword is in the `do`-`while` loop, which is the same as a normal `while` loop, except that it is executed at least once before the condition is checked. Note that the `do`-`while` clause must end with a semicolon.

```
var counter = 0;
do {
	print counter;
	counter++;
} while (counter < 10);
```

The braces around the body of the `while` and `do`-`while` loops are optional, but if they are omitted, they will be implicitly inserted.

## For

`while` loops can be quite verbose - a way to write a loop quickly and concisely is with the `for` loop. The first clause in the for loop is executed before the loop begins, then the second clause is checked for truthyness before each execution of the loop body. Finally, the third clause is executed after each execution of the loop body:

```
for (var i = 0; i < 10; i++) {
	print i;
}
```

Side Note: You *can* declare a `const` in the first clause instead of a `var`, but it won't be very usefull 99.999% of the time.

The braces around the body of the `for` loop are optional, but if they are omitted, they will be implicitly inserted.

## Foeach-In and Foreach-Of

Coming Soon: Iteration loops!

## Break and Continue

During a loop, you may want to exit or restart early for some reason. For this, the `break` and `continue` keywords are provided.

```
//exit a loop early
for (var i = 0; i < 10; i++) {
	print i;
	if (i >= 7) {
		break;
	}
}

//restart a loop early
for (var i = 0; i < 10; i++) {
	if (i >= 7) {
		continue;
	}
	print i;
}
```

Coming Soon: `break x;` and `continue x;` i.e. breaking out of multiple nested loops!

## Functions and Return

Unlike many other mainstream languages, functions are not statements, but expressions. They are also ONLY first class functions, and do not have names attached; they must be stored in a variable or constant like any other datatype. Note that since functions are just expressions, a function delcaration MUST end with a semicolon.

Delcaring a function can be done in two ways - with the funtion keyword, or with the arrow operator.

```
const f = function() {
	print "foo";
};

const g = () => {
	print "bar";
};
```

A function can be called by appending a pair of parentheses after it, and arguments can be passed to it by placing values between those parentheses, separated by commas. An incorrect number of arguments passed is a runtime error.

```
const f = function(arg) {
	print arg;
};

f("foo"); //Good!
f(); //Error!
f("foo", "bar"); //Error!
```

when a function is declared with the arrow operator, if there is exactly 1 argument, then the parentheses can be omitted. Similarly, if there is only 1 expression statement in the function body, then the braces can be omitted, which causes the result of the expression statement to be returned.

```
const caller = function(cb) {
	return cb("arg");
}

caller(x => x); //very concise identity function

array.Sort((a, b) => a - b); //a more practical example
```

Values can be extracted from function calls using the `return` keyword:

```
const f = function() {
	return "foo";
};

var bar = f(); //bar = "foo"
```

Closures are explicitly supported, so inaccessible, or "private", variables can be created this way:

```
const makeCounter = function() {
	var counter = 0; //the inaccessible variable
	return function() {
		return ++counter;
	};
};

const counter = makeCounter():

print counter(); //1
print counter(); //2
print counter(); //3
```

Very complex structures can be created using functions this way, including mimicing classes.

## Classes, Inheritance and Prototypes

No.

## Assert

The `assert` keyword is an oddball. It takes 1 required parameter, an optional second parameter, and requires parentheses. If the first parameter resolves to be falsy, then the program terminates, and then if a string is provided as the second parameter, it prints that string. Note that assert can only take a string as the second parameter - anything else will cause an error before the program runs.

```
assert(true, "This is fine"); //Good!
assert(false); //Error!
assert(true, 42); //The program won't run!
```

## Import As

`import` is used to load external libraries, plugins and external \*.toy files. Several libraries and two plugins are provided by default. The import keyword can only take a string literal as it's argument, followed by an optional `as alias`, and can only be used at the global scope; if either of these conditions are not met, it will cause an error before the program runs.

You can see a list of [libraries](reference_libraries.md) and [plugins](reference_plugins.md) via those links.

```
import "Standard";

print Clock(); //the clock function is provided by Standard

import "Standard" as std;

print std.Clock(); //Standard is bundled into "std"

import "external.toy"; //runs the external file, and merges the environemnt into this one

import "external2.toy" as External2; //runs the external file, then stores the environment into a variable
```

