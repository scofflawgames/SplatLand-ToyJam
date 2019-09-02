# Array

Instances of the Array plugin are indexed by numbers, beginning from 0 and counting up. Arrays do not allow for "holes" i.e. missing elements.

## Usage

```
import "Array";

var array = Array();

array.Push(1);
array.Push(2);
array.Push(3);

print array[1]; //"2"

var copy = array[:]; //copy an array
var backwards = array[::-1]; //reverse the array

array[1] = "foo"; //assign to an element

print Array.IsArray(array); //true
```

## IsArray(x)

This returns true if "x" is an instance of Array, otherwise it returns false.

# Instances

Instances are the result of calling `Array()`.

## Indexing

Arrays can be indexed in several ways. Elements can be accessed using traditional bracket notation:

```
array[x];
```

However, "slice notation" is also available:

```
array[x:y];
```

Here, "x" and "y" indicate two indexes within the array, and returns a new array containing only the elements between those two indexes (inclusive). If either is omitted, then the first and last elements respectfully are used.

A third argument is possible:

```
array[x:y:z];
```

Here, "z" indicates how to count the elements - a positive number starts from the beginning of the array, while a negative one counts from the end. Also, if a number other than 1 or -1 is used, then every nth element is selected:

```
print array; //output: [1,2,3,4,5,6,7,8,9,10]
print array[::2]; //output: [1,3,5,7,9]
print array[::-2]; //output: [10,8,6,4,2]
print array[::-2][::-1]; //output: [2,4,6,8,10]
```

0 cannot be used as the third argument.

## Push(x)

This function inserts the value of "x" at the end of the array.

## Pop()

This function deletes the value at the end of the array, and returns that value.

## Unshift(x)

This function inserts the value of "x" at the beginning of the array.

## Shift()

This function deletes the value at the beginning of the array, and returns that result.

## Length()

This function returns the length of the array.

## Sort(cb(a, b))

This function sorts the elements according to the callback "cb". "cb" may be called any number of times during the sorting process.

"cb" must be a function that takes two parameters, "a" and "b" and returns a number. If "a" is less than "b", then the returned number should be negative. If they are equal, then it should be 0. Otherwise it should be positive.

```
//how to sort an array of numbers
//note that if there are any non-numbers, then the program will fail
array.Sort((a, b) => a - b);
```

## Insert(i, x)

This function inserts the value of "x" at the index "i", shifting the remaining elements up 1 index.

## Delete(i)

This function deletes the value at the index "i", shifting the remianing elements down 1 index.

## ContainsValue(x)

This function returns true if the array contains the value of "x".

## Every(cb(x))

This function calls "cb" once for every element in the array, with that element passed in as "x". If any call to "cb" returns false, then the function exits early and returns false. Otherwise the function returns true.

## Any(cb(x))

This function calls "cb" once for every element in the array, with that element passed in as "x". If any call to "cb" returns true, then the function exits early and returns true. Otherwise the function returns false.

## Filter(cb(x))

This function calls "cb" once for every element in the array, with that element passed in as "x". This returns a new array instance that contains every element for which the call to "cb" returned true.

## ForEach(cb(x))

This function calls "cb" once for every element in the array, with that element passed in as "x".

## Map(cb(x))

This function calls "cb" once for every element in the array, with that element passed in as "x". It returns a new array instance with the result of each call to "cb" as it's elements.

## Reduce(default, cb(acc, x))

This function calls "cb" once for every element in the array, with that element passed in as "x", and the value of the previous call passed in as "acc". For the first call to "cb", "default" is used for "acc".

## Concat(x)

This function requires an array instance as "x". This function returns a new array instance which contains the contents of the current array followed by the contents of "x".

## Clear()

This function removes the contents of the array instance, leaving an empty array.

## Equals(x)

This function requires an array instance as "x". It returns false if the current array and "x" have a different number of elements. Then, it iterates over each pair of elements from the current array and "x" and compares them, returning false on the first mismatch found. Otherwise it returns true.

## ToString()

This function returns a string representation of the array. Each element is converted to a string, and separated by commas. Finally, the whole string is surrounded by brackets.

Nesting an array within it's own data structure will cause the inner reference to be printed as "\<circular reference\>".

