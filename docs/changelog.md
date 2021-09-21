# Changelog

## 1.1.3 (20/09/2021)

Features:

- Added support to merge dll and exe using ILMerge

Changes:

- Command line api is simplified and left with two commands only
- Argument parsing is done by PsburnCliParser.dll
- csharp_binder.cs now depends on PsburnCliParser.dll which depends on SharpZipLib
- Removed usage, examples and variables parsing from #@parser descriptor
- Removed no extract feature

Fixed:

- Argument parsing techniques

## 1.0.0 (22/06/2021)

Features:

- Initial release
