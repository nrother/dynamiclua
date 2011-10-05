DynamicLua 1.0 beta
===================
developed by Niklas Rother (niklas-rother.de)

DynamicLua is a wrapper for the LuaInterface heavily using the .NET 4 "dynamic" Feature. (LuaInterface is a Lua<->C# Bridge)
It makes it easier and more idiomatic to use Lua in C# Code.
DynamicLua is written in C# and includes unit tests for nearly everything. Most of the tests are converted from the LuaInterface project.
Although a copy of LuaInterface is included in the Repository, the DynamicLua work completely above LuaInterface.
The reason why it's included is easy: The current version has some bugs that affect DynamicLua, and these bugs are
fixed in the includes version. But it should be possibly to use DynamicLua even with the original version of LuaInterface.

You might download the binary or source distribution. The binary distribution contains only the three DLL Files and some Texts,
the source distribution the whole source code. The source code is packaged as a Visual Studio 2010 Solution, containing 6 projects:

"DynamicLua" is the main project containing the code for DynamicLua.dll
"DynamicLuaTest" contains the Unittests for DynamicLua, partly copied from the LuaInterface project
"lua" contains the source of Lua 5.1.4 coming with every release (this is written in C/C++!), plus some code from LuaInterface to make it CLR-Compliant
"LuaInterface" is the Original LuaInterface code (the Lua-C# Bridge), but with some community patches to fix some bugs.
"LuaTest" is a console app just containing some random tests for development. You can ignore it.

Documentation
-------------
Please refer to the official documentation on the Codeplex site: http://dynamiclua.codeplex.com/documentation

Bugs/Feedback
-------------
You might report bugs on the Codeplex site in the Issue Tracker: http://dynamiclua.codeplex.com/workitem/list/basic
You can also drop me an email, write to info (ät) niklas-rother (döt) de, in English or German.

Help
----
If you want to help this project, write an email to the adress above, any help is appreciated!

Licence
-------
Copyright 2011 Niklas Rother

DynamicLua is licensed under the Apache License, Version 2.0.
Lua and LuaInterface is licensed under the MIT Licence.

A full copy of the Licences can be found in the "LICENSE_Apache.txt"
and "LICENSE_MIT.txt" file. Please include the "NOTICE.txt" File,
containing attribution, that comes with this software in any derivative
work, as required by the Apache license.