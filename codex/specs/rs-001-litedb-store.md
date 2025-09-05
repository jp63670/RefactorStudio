# rs-001: Implement LiteDB-backed ProjectStore

## Goal
Persist Projects using LiteDB with async APIs; wire DI; add tests.

## Acceptance
- New: RefactorStudio.Core/Persistence/LiteDbProjectStore.cs
- interface: IProjectStore unchanged (Create/Open/Recent/Save)
- DB file per user profile: %LOCALAPPDATA%\RefactorStudio\refactorstudio.db
- DI: register LiteDbProjectStore in MauiProgram.cs instead of InMemory
- Tests: RefactorStudio.Tests/ProjectStore_LiteDbTests.cs covering create/open/recent/save
- Build + tests pass in CI

## Steps
1) Add LiteDB package if missing.
2) Implement LiteDbProjectStore with an index on Project.Id.
3) Replace DI registration InMemoryProjectStore → LiteDbProjectStore.
4) Add xUnit tests; use a temp db path.
5) Update README with persistence path and troubleshooting.
