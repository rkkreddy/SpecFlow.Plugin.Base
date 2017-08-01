# TL;DR Or "I'm just here to load up examples from excel"
* install the nuget package `Itamaram.Excel.SpecFlowPlugin`
* Add an `excel:<relative path to feature file>[:<worksheet name]` tag to your example.
It should look like:
```gherkin
Feature: SpecFlowFeature
	I want to load some excel stuffs

Scenario Outline: Load From Excel
	Given I have followed the instructions
	Then I should've loaded <A> <B> <C>

	@excel:Book1.xlsx:Sheet1
	Examples: 
	|A|B|C|
```

# The long version
This project is meant to assist in the creation of SpecFlow plugins for generation of examples.
By implementing the right base classes and packaging your own plugin, any data source can be used to pipe test cases into a SpecFlow scenario outline.
Two sample implementations are provided in this repo, one for consuming csv files and the other for excel spreadsheets. Both are incredibly naive and are meant to be expanded upon based on actual requirements.
Regardless, due to what appears to be quite high demand, they're both available from the NuGet Gallery under the names `Itamaram.Excel.SpecFlowPlugin` and `Itamaram.Csv.SpecFlowPlugin`

## How to create a new plugin
It's slightly more tricky than I thought. Save yourself sometime and follow the steps below.
1. Create a project outputting an assembly with a name ending in `.SpecFlowPlugin`
1. Install the package `Itamaram.SpecFlow.Plugin.Base`
1. Create an App.config.transform file ([sample](https://github.com/Itamaram/SpecFlow.Plugin.Base/blob/master/Itamaram.Excel.SpecFlowPlugin/App.config.transform))
This will register the plugin with SpecFlow. Note that your plugin's name is the name of your dll minus the `.SpecFlowPlugin` suffix.
Don't forget to ensure the transform file ends up in your package's `content` directory.
1. Create a class deriving `Itamaram.SpecFlow.Plugin.Base.NamedTagExampleGenerator`
You'll need to implement two things:
   1. `TagName` - All tags starting with this string will be handled by this generator. It need not start with a `@`. Note that multiple generators can handle a single tag (and an Examples block can have multiple tags).
   1. `GetRowsInternal` - returns examples, one row at a time. Takes the following arguments:
      * `string` `args` - once a tag is stripped of its expected prefix (`TagName`), the next character is assumed to be a separator and is skipped, and the rest of the tag is passed into this method as args. eg: given the `TagName` "excel", "excel-book1.xlsx" will yield the args value of "book1.xlsx". So will "excel:book1.xlsx" as well as as "excel!book1.xlsx" and so on.
      **IMPORTANT!** "excel book1.xlsx" will not result in an empty value of `args` as SpecFlow uses whitespace to separate tags, which means two different tags will be processed, "excel" and "book1.xlsx"
      * `string` `dir` - The directory of the `.feature` file
      * `string` `specfile` - The name (including extension) of the `.feature` file
      * ` IEnumerable<string>` `header` - The header values of the `Example` block
1. Create a class inheriting from `Itamaram.SpecFlow.Plugin.Base.ExamplesGeneratorPlugin<T>` where `T` is your own implementation of `NamedTagExampleGenerator`.
1. Add the following assembly attribute: `[assembly: GeneratorPlugin(typeof(Foo))]` where `Foo` is your implementation of `ExamplesGeneratorPlugin`.
Did you know? The assembly attribute can live in any file, it doesn't have to be places in `AssemblyInfo.cs`.
1. Ensure that when packaging your plugin as a nuget package, the resulting assembly **as well as all dependent assemblies** are placed in the `tools` directory of your package, rather than `lib`. **NOTE: The -Tool flag for nuget pack does not copy dependencies!** 
The sample plugins in this repo achieve this by adding the following element to the nuspec file:
  ```xml
  <files>
    <file src="bin\$configuration$\*.dll" target="tools" />
  </files> 
  ```
  
  ## A Note on debugging
  While developing your plugin you can configure a test project to load your plugin by pointing at your bin directory eg:
  ```xml
<?xml version="1.0" encoding="utf-8" ?>
  <configuration>
    <specFlow>
      <plugins>
        <add name="MyPlugin" type="Generator" path="c:\src\MyPlugin\MyPlugin\bin\debug"/>
      </plugins>
    </specFlow>
</configuration>
  ```
When doing so, you'll might encounter SpecFlow has in place a caching mechanism for plugins, which means your plugin might not be reloaded on build. In order to refresh the SpecFlow cache, simply change the `name` attribute in the config file (to say `MyPlugin1`, save, and then revert the change and save again. The plugin should be correctly reloaded.
