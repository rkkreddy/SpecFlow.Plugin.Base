REM in order to build: First pack the base, and publish it.
REM then update the nuget package reference for the implementations, then pack, and publish them
REM ignore NU5128, because no actual dlls are included in a lib or ref, they're all included as a resource, you get a warning
REM then rethink this file a few times over.

dotnet pack Itamaram.SpecFlow.Plugin.Base\Itamaram.SpecFlow.Plugin.Base.csproj -c Release -p:Version=%1 -o .

for %%p in (Itamaram.Csv.SpecFlowPlugin Itamaram.Excel.SpecFlowPlugin) do (
	dotnet pack %%p\%%p.csproj -c Release -p:Version=%1 -o .
)