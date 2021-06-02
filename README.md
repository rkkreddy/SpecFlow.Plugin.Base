# About
This project offers a base package, which allows for generating SepcFlow plugins for example generation with almost no boilerplate code. By following a few short steps, any data source can be used to pipe test cases into a SpecFlow scenario outline.

Two sample implementations are provided in this repo, one for consuming csv files and the other for excel spreadsheets. Both are incredibly naive and are meant to be expanded upon based on actual requirements.
Regardless, due to what appears to be quite high demand, they're both available from the NuGet Gallery under the names `Itamaram.Excel.SpecFlowPlugin` and `Itamaram.Csv.SpecFlowPlugin`

# How to - `Itamaram.Excel.SpecFlowPlugin`
The plugin generates examples (ie test cases) from an excel spreadsheet. Each line is converted into an example, with each cell being mapped to a column.

Only two steps are needed to get started:

1. install the nuget package `Itamaram.Excel.SpecFlowPlugin`
1. Add an `excel:<relative path to feature file>[:<worksheet name]` tag to your example.
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

The repo contains two sample projects which implement the above, one for [full dotnet](Itamaram.Excel.SpecFlowPlugin.IntegrationTests), and the other for [dotnet core](Itamaram.Excel.SpecFlowPlugin.DotNetCore.IntegrationTests). 
## How to skip first row and consider it as header row
1. Add a JSON file by name `ItamaramExcelConfig.json` to project's main directory
2. Add entry as below
    `{
  "SkipFirstRow": true
} 
`
1. Set value as `false` to not to skip the first row and read it as data row
1. Make sure to have all Example table column headers in Excel sheet's first row
1. Excel sheet can have the columns in any order
1. Excel sheet can have more columns than required and this provides an advantage to use the same sheet for other scenarios
# How to create a new plugin for example generation
1. Create a project outputting an assembly with a name ending in `.SpecFlowPlugin`  
   This can be achieved by either using this suffix in the project name, or adding the `AssemblyName` to the `csproj` file.
1. Ensure the project targets both `net471` and `core2.1`. This is achieved by including `<TargetFrameworks>net471;netcoreapp2.1</TargetFrameworks>` in your `csproj`. 
1. If the plugin is to be packed as a `nupkg` (recommended), add to the first `PropertyGroup` of your `csproj` the element `<BuildOutputTargetFolder>build</BuildOutputTargetFolder>`. 
1. Install the package `Itamaram.SpecFlow.Plugin.Base`
1. At this point, building the project would generate a `.props` file under the `build` directory. When packed, this ensures your plugin gets automatically registered with SpecFlow.
1. Create a class deriving `Itamaram.SpecFlow.Plugin.Base.NamedTagExampleGenerator`
You'll need to implement two things:
   1. `TagName` - All tags starting with this string will be handled by this generator. It need not start with a `@`. Note that multiple generators can handle a single tag (and an Examples block can have multiple tags).
   1. `GetRowsInternal` - returns examples, one row at a time. Takes the following arguments:
      * `string` `args` - once a tag is stripped of its expected prefix (`TagName`), the next character is assumed to be a separator and is skipped, and the rest of the tag is passed into this method as args. eg: given the `TagName` "excel", "excel-book1.xlsx" will yield the args value of "book1.xlsx". So will "excel:book1.xlsx" as well as as "excel!book1.xlsx" and so on.
      **IMPORTANT!** "excel book1.xlsx" will not result in an empty value of `args` as SpecFlow uses whitespace to separate tags, which means two different tags will be processed, "excel" and "book1.xlsx"
      * `string` `dir` - The directory of the `.feature` file
      * `string` `specfile` - The name (including extension) of the `.feature` file
      * ` IEnumerable<string>` `header` - The header values of the `Example` block
1. Create an empty class inheriting from `Itamaram.SpecFlow.Plugin.Base.ExamplesGeneratorPlugin<T>` where `T` is your own implementation of `NamedTagExampleGenerator`.
1. Add the following assembly attribute: `[assembly: GeneratorPlugin(typeof(Foo))]` where `Foo` is your implementation of `ExamplesGeneratorPlugin`.  
Did you know? The assembly attribute can live in any file, it doesn't have to be places in `AssemblyInfo.cs`.
1. Call `dotnet pack` to build your plugin. All the magic is already inside. You can now add the resultant package to any SpecFlow project and get those examples generated.
  
  ## A Note on debugging
  While developing your plugin you can configure a test project to load your plugin by adding the following to your `csproj` file:
  ```xml
  <ItemGroup>
    <SpecFlowGeneratorPlugins Include="path\to\bin\MyPlugin.SpecFlowPlugin.dll" />
  </ItemGroup>
  ```
It is important to note though that SpecFlow uses the built in assembly loading, which caches the assembly in the App Domain, this means your plugin might not be reloaded on build. In order to avoid this, either restart Visual Studio to clear the cache, or rename the resulting dll (and above reference to it).

Bonus note, `Debugger.Launch()` is your best friend.