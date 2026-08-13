# Project rules for Claude

## What this is

LustigeFehler is a joke program for Windows. It reads a list of fake error messages from
`Config.xml` and shows them one after another as message boxes, forever, with a random button set,
a random icon and a random pause of up to 2.3 seconds in between. The application has no window of
its own and no way to quit it from the user interface, the only way to stop it is the task manager.
The repository is **not** published as a NuGet package: no `GeneratePackageOnBuild`, no push
script. What is shipped is an Inno Setup installer, and that installer is tracked in this
repository.

One solution `src/LustigeFehler.sln` with exactly one project:

- `src/LustigeFehler/LustigeFehler.csproj`, `OutputType` `WinExe`, Windows Forms, application icon
  `Funny.ico`.

Layout inside `src/LustigeFehler`:

- `Program.cs`: `Main` with `[STAThread]`, `Application.Run(new Main())`. See the quirks, that call
  is never reached.
- `Main.cs` plus `Main.Designer.cs` and `Main.resx`: the form. `Configure` loads the configuration
  and then hands over to `SpamUser`, which never returns. `GetRandomMessage`, `GetRandomButtons`
  and `GetRandomIcon` do one thing each, keep new logic in that shape. The designer file and the
  resx hold nothing but the form size and the icon.
- `Config.cs`: the deserialized root, a single `List<Message> Messages`. `Message.cs`: one entry,
  `Name` is the body text of the message box, `Caption` is its title.
- `Import.cs`: loads `Config.xml` through `XDocument` and `XmlSerializer`.
- `Config.xml`: the payload, 534 `Message` entries, German. `CopyToOutputDirectory=Always`, the
  same for `License.txt` and `Readme.txt`.
- `GlobalUsings.cs`: all usings of the project.

`Setup` holds `LustigeFehler-Setup.iss` (Inno Setup script), `build-setup-files.bat` (cleans `bin`
and `obj`, publishes, deletes the `*.pdb`) and the built installer `LustigeFehler-Setup.exe`.

Repository root: `README.md` (the only user documentation), `Changelog.md`, `License.txt` (MIT),
`Screenshot.png`, `.gitattributes` and `.gitignore`. There is no `Updating.md`, no `HowToUse.md`
and no `.github` folder.

## Build

```powershell
dotnet build src/LustigeFehler.sln -c Release
```

There are no tests, there is no test project. A behaviour change is verified by publishing and
starting the executable, and by looking at the message boxes it produces.

- Single target framework `net9.0-windows`, no multi-targeting. `RuntimeIdentifiers` is `win-x64`.
- All build properties live directly in `src/LustigeFehler/LustigeFehler.csproj`. There is **no**
  `Directory.Build.props` in this repository.
- `TreatWarningsAsErrors` is enabled, so every warning breaks the build, NuGet warnings (`NU****`)
  from restore included. A clean build reports zero warnings, keep it that way.
- `NU1803` (HTTP source usage during restore) is the one warning suppressed via `NoWarn`. Fix
  warnings instead of extending that list. `NuGetAudit` and `NuGetAuditMode=all` are on, so a
  vulnerable transitive package fails the build too.
- The only package reference is `GitVersion.MsBuild`.
- Versions come from GitVersion out of the git tags, for example `1.0.8-1` for the first commit
  after tag `1.0.7`. Never edit a version property or an assembly version by hand.
- Restore needs nuget.org. If a private feed is configured globally on the machine and answers 404
  for public packages, restore fails with `NU1301`. Then build with an explicit source:
  `dotnet build src/LustigeFehler.sln --source https://api.nuget.org/v3/index.json`.

## Code conventions

Follow the surrounding code, it is consistent throughout every file:

- File header comment block with `<copyright file="..." company="Hämmer Electronics">` and a
  `<summary>`, then the file-scoped namespace. `Main.Designer.cs` is exempt, it is generated code
  with a block scoped namespace and German comments, leave it alone.
- XML doc comments on every type and every member, private members included, no exceptions.
- `Nullable`, `ImplicitUsings` and `LangVersion latest` are enabled.
- New `using` directives go into `GlobalUsings.cs`, inside the existing `#pragma warning disable
  IDE0065` block, never at the top of a file. The editorconfig requires usings inside the namespace
  (`csharp_using_directive_placement=inside_namespace:warning`), which global usings cannot
  satisfy, that is what the pragma is for. Do not add other pragmas. The comment text in that block
  is German because Visual Studio generated it, leave it alone.
- Fields, properties, methods and events are always accessed with `this.` qualification
  (`dotnet_style_qualification_for_*` at severity `warning`).
- `src/.editorconfig` also enforces braces everywhere, no multiple blank lines, four spaces, CRLF,
  UTF-8, file scoped namespaces, `System` usings sorted first and `IDE0005` as warning. Analyzer
  warnings are fixed, not silenced.

## Known quirks

Do not silently "clean up" these, they are existing behaviour:

- **The constructor never returns.** `Main()` calls `Configure`, which calls `SpamUser`, which is a
  `while (true)` loop. `Application.Run(new Main())` in `Program.cs` is therefore never reached and
  no message loop ever starts. The message boxes work anyway because `MessageBox.Show` pumps its
  own modal loop. Consequence: the process cannot be closed from the user interface at all, it has
  to be killed. That is the joke, not a defect.
- **The form is never shown.** `SetVisibleCore` passes `false` to the base implementation no matter
  what it was called with. Even if the message loop were reached, the form would stay invisible.
- **`Message.Name` is the message text.** Not a name, not an identifier. `Caption` is the title of
  the message box. Renaming either property changes the `Config.xml` file format, because
  `XmlSerializer` maps by element name.
- **`SpamUser` swallows every exception.** The `catch` in the loop is empty on purpose, a broken
  entry must not stop the program. The `Thread.Sleep` sits after the `MessageBox.Show` inside the
  `try`, so an exception skips it. An empty message list therefore spins the loop at full CPU load
  with nothing on screen.
- **`Config.xml` is UTF-8 without BOM and says so.** The declaration `encoding="utf-8"` matches the
  bytes, the German umlauts are real two byte sequences. Do not "repair" that file, and do not add
  a BOM to it.
- **The `.iss` and `Readme.txt` are Windows-1252 without BOM.** Both carry German umlauts
  (`Hämmer Electronics` respectively the German half of the readme) as single `0xE4` bytes. Inno
  Setup 6 only reads a file as UTF-8 if a BOM is present, otherwise it interprets the system code
  page, so this works on a Windows-1252 machine and produces `HÃ¤mmer Electronics` elsewhere. Any
  editor that saves "UTF-8 without BOM" silently breaks it.
- **The installer is tracked although `.gitignore` excludes `*.exe`.** `Setup/LustigeFehler-Setup.exe`
  was added with `git add -f` and has to be added that way again for every release. It grows the
  repository by the size of the installer on every release, permanently.
- **Inno Setup warns about `PrivilegesRequired`.** The quick launch icon uses `{userappdata}` and
  is limited to Windows 7 and older via `OnlyBelowVersion: 0,6.1`, so it never applies in practice.
- **AppVeyor badge without CI in the repository.** `README.md` links an AppVeyor build that is
  configured outside of this repository. There is no pipeline file here.
- **`src/LustigeFehler.sln.DotSettings`** is tracked and holds nothing but a ReSharper user
  dictionary (`Fehler`, `H_00E4mmer`, `Lustige`). Leave it alone.
- **`.gitattributes` sets `* text=auto`**, every rule of the Visual Studio template below it is
  commented out. Any binary file that must not be normalized needs its own rule.

## Releasing

1. Make the change.
2. Add an entry at the top of `Changelog.md` in the existing format:
   `* **Version 1.0.8.0 (2026-08-13)** : Short description.`
3. Set `MyAppVersion` in `Setup/LustigeFehler-Setup.iss` to the same four part version. Keep the
   encoding and the CRLF line endings of that file.
4. Commit that.
5. Tag the commit with the plain version number, no `v` prefix (`1.0.8`, `1.0.7`, ...). The
   existing tags are lightweight tags, create new ones the same way. The tag has to exist
   **before** the installer is built, otherwise GitVersion burns a prerelease version into the
   shipped executable.
6. Run `Setup/build-setup-files.bat`, then compile `Setup/LustigeFehler-Setup.iss` with `ISCC.exe`.
7. `git add -f Setup/LustigeFehler-Setup.exe` and commit it, by convention with the message
   `Updated setup.`.
8. Push the commits and the tag.

The version in the `Changelog.md` has four parts (`1.0.8.0`), the tag has three (`1.0.8`).
GitVersion turns the tag into the assembly version, so an untagged commit produces something like
`1.0.8-1+Branch.master.Sha...`.

In this environment `NoDefaultCurrentDirectoryInExePath` is set, so `cmd` does not search the
current directory for executables. The batch file has to be started as `call .\build-setup-files.bat`
from the `Setup` folder, because its `cd ..\src` is relative to the start directory.

## Git

- **Never amend a commit.** No `git commit --amend`, not for a typo in the message, not to add a
  forgotten file, not even when the commit is still local. Write a follow-up commit instead. The
  release versions come from tags on exact commits, an amended commit leaves its tag pointing at a
  commit that no longer exists in the branch.

## Writing style

- Commit messages are written **in English only**: short, precise subject line, explanatory body
  when needed.
- Code comments and comments in project files such as `.csproj` are **always English**, regardless
  of the language used in the conversation.
- **No em dashes or en dashes** (`—`, `–`), neither in prose, commit messages, code comments nor
  documentation. Use a regular hyphen, comma, colon, parentheses or a separate sentence.
- The user visible strings of the application are German, they come from `Config.xml` and are not
  localized. Everything else, including the error message box of the configuration loader, is
  English.
- German texts (documentation, chat replies) always use real umlauts and ß, never ASCII
  transliterations such as `ae`, `oe`, `ue` or `ss`. Identifiers, file names and configuration keys
  stay unchanged where umlauts are technically undesirable.
