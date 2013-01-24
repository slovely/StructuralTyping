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
	}

Now you need to pass an instance of IDoSomethings into a method, but you don't want to have to create a class for it.  
You can do this instead:

    var anonObject = A.New<IDoSomethings>(new { Name="Test", Quantity=11 });

