Playing with Structural Typing in c#
====================================

This is just some throw-away sample code I created after seeing the way TypeScript had structural typing.

I'm sure I've used libraries in the past (though can't think of any right now!) that had methods that took
parameter types when I'd rather pass in an anonymous object - like a javascript option hash.  It's a bit
difficult to achieve in a strongly-typed language so the code to use it is obviously more verbose than TypeScript,
but I've tried to keep it as easy as possible.

Usage
=====

Imagine an interface:

    public interface IDoSomethings
    {
        string Name { get; set; }
        int Quantity { get; set; }
        string Calculate(int num);
    }

Now you need to pass an instance of IDoSomethings into a method, but you don't want to have to create a class for it.  
You can do this instead:

    var anonObject = A.New<IDoSomethings>(new { Name="Test", Quantity=11 });

If the interface contains a method, you can create these too, by passing in a Func/Action with a matching signature:

    var anonObject = A.New<IDoSomething>(new { Calculate = new Func<int, string>(i => "The answer is " + i) });
    var answer = anonObject.Calculate(7); //returns "The answer is 7"

LIMITATIONS
===========
Doesn't work if you pass in a method which has a closure.  This *won't* work:

    var number = 42;
    var anonObject = A.New<IDoSomething>(new { Calculate = new Func<int, string>(i => "The answer is " + i + number) });
    var result = anonObject.Calculate(8); // throws exception

However, if 'number' is a class member, rather than a local variable, you can get it to work by passing in 'this' as the second parameter, like this:

    private int number = 42;
    ...
    var anonObject = A.New<IDoSomething>(new { Calculate = new Func<int, string>(i => "The answer is " + i + number) });
    var result = anonObject.Calculate(8); //returns "The answer is 50"


_Bottom line / TLDR_: Don't use this library!  It's silly and is just some hacky code I was playing with.  Use it to take idea's from only.

