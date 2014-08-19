# Irony Grammar Parsing

Last version supporting .Net 3.5 (for Unity):

- http://irony.codeplex.com/discussions/273250
- http://irony.codeplex.com/SourceControl/changeset/99afa2622130

Manually removed the CommandLine code from the main project to allow targetting of the .Net 2.0 subset.

# CjClutter.ObjLoader

Manually modified ObjLoader.Loader.Loaders.LoaderBase to replace the String.IsNullOrWhitespace method with:

	if (string.IsNullOrEmpty(currentLine) || currentLine.Trim().Length == 0 || currentLine[0] == '#')
