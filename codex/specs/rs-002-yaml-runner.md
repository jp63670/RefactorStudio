# rs-002: YAML Recipe Loader & Runner

## Goal
Load recipe YAML from RefactorStudio.Recipes/samples, run steps in order against the adapter, collect logs and outputs.

## Acceptance
- New: Core/Services/RecipeLoader.cs, RecipeRunnerYaml.cs
- YAML schema: id, version, description, steps[].name, steps[].prompt, outputs[]
- UI: Replace "Simulate Recipe Run" with "Run Recipe" (dropdown to pick a recipe file)
- Writes output files to ./outputs/<recipe-id>/<step>.txt (for now)
- Build passes; unit tests for loader (valid YAML, missing fields).

## Steps
1) Add YamlDotNet to Core.
2) Implement RecipeLoader.GetAll() and Load(path).
3) Implement RecipeRunnerYaml.RunAsync(RecipeYaml, ContextBundle, IModelAdapter).
4) Update HomePage to pick a recipe and run; show step logs.
5) Tests: loader happy path and error cases.
