# Maintainer Changelog
Changelog format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

#### Types of changes  
- **Added** for new features.
- **Changed** for changes in existing functionality.
- **Deprecated** for soon-to-be removed features.
- **Removed** for now removed features.
- **Fixed** for any bug fixes.
- **Security** in case of vulnerabilities

_Please, always remove previous plugin version before updating!_

## [1.16.0] - 2022-09-18

### Added
- Add Deep search option to the References Finder

### Changed
- Improve Assets Map generation performance (thx Adam Kane)
- Reduce Assets Map file size
- Reduce References Finder UI CPU overhead
- Nicify property path at the Hierarchy Reference Finder results

### Removed
- Remove few obsolete APIs

### Fixed
- Fix reveal in prefabs could work incorrectly

## [1.15.0] - 2022-08-05

### Added
- Add support for SerializeReference attribute (thx CanBaycay)

### Changed
- Improve Duplicate component search accuracy

## [1.14.3] - 2022-06-24

### Fixed
- Fix scripts references might not appear in References Finder (thx CanBaycay)

## [1.14.2] - 2022-06-21

### Fixed
- Fix Addressable folders dependencies processing (thx sebas77)

## [1.14.1] - 2022-04-11

### Changed
- Improve Unity 2022.2 compatibility

## [1.14.0] - 2022-02-05

### Added
- Add "Ignore Editor assets" option (on by default) to Project Cleaner
  - In addition to Editor-only special folders, it now ignores assets under editor-only Assembly Definitions

### Removed
- Remove 'Editor Resources' and 'EditorResources' folders from the Project Cleaner builtin ignores

### Fixed
- Fix redundant .asmdef ignore added to the Project Cleaner while migrating from < 1.4.1 version
- Fix icons rendering performance degradation regression

## [1.13.2] - 2022-01-19
### Changed
- Improve few built-in icons rendering

## [1.13.1] - 2022-01-17
### Fixed
- Fix possible exception while scanning for missing references

## [1.13.0] - 2021-10-04
### Added
- Add automatic IDependenciesParser implementations integration in Unity 2019.2+
- Add Filters count to the filtering tabs titles
- Add Tools > Code Stage > Maintainer > Find Issues In Opened Scenes along with new StartSearchInOpenedScenes API
- Add Game Object > Maintainer > Find Issues In Opened Scenes menu
- Add MaintainerExtension base class for extensions
- Add DependenciesParser base class for dependency parsing extensions
- Add items count to the Issues Finder and Project Cleaner reports
- Add new support contact, let's chat at [Discord](https://discord.gg/FRK5HRzZvq)!

### Changed
- Improve Project Cleaner ignores context menu UX
- Allow searching issues at the unsaved Untitled scene
- Previous search results do not clear up anymore if new search was cancelled
- Improve Unity 2022.1 compatibility
- Make AssetInfo class public
- Change IDependenciesParser.GetDependenciesGUIDs signature to allow more flexibility:
  - accept AssetInfo instance
  - return IList
- Utilize IDependenciesParser for settings dependencies processing

### Deprecated
- Starting from Unity 2019.2+, deprecate redundant AssetDependenciesSearcher.AddExternalDependenciesParsers API
- Deprecate legacy .NET 3.5 (to be removed in next major upgrade in favor of newer .NET version)

### Fixed
- Issues Finder:
  - Fix some Game Objects still could be analyzed with Game Objects option disabled
  - Fix missing components reveal didn't properly fold inspectors
  - Fix Game Object context menus were active for non-GameObject selection
  - Fix checking scenes and Game Objects with disabled corresponding options regression
  - Fix rare case with duplicate issues in prefab variants without overrides
- References Finder:
  - Fix search terms were not reset after Unity restart causing a confusion
  - Fix rendering settings path in Exact References at Unity 2020.1+
- Fix possibility of rare filtering issues when having "Assets" in the project path
- Fix fallback fonts were not tracked properly leading to inclusion at the Project Cleaner
- Fix reveal in Prefab Variants didn't set proper Selection
- Fix window notification could not show up after window activation
- Fix minor UI issues
- Fix other minor issues

## [1.12.2] - 2021-08-18
### Fixed
- Fix possible NullReferenceException at scene settings processor (thx thygrrr)

## [1.12.1] - 2021-07-31
### Fixed
- Fix possible NullReferenceException at Issues Finder (thx NIkoloz)

## [1.12.0] - 2021-06-28
### Added
- Project Cleaner: 
  - Add new quick way to treat all scenes in project as used (thx Makaka Games)
  - Add new context menu action for found unused scene assets to treat them as used
### Changed
- Improve errors reporting and exceptions handling
- Project Cleaner: improve used scenes filtering UX
### Fixed
- Issues Finder: fix minor issues at missing references algorithm

## [1.11.0] - 2021-06-23
### Added
#### Issues Finder
  - improve generic missing references lookup in file assets
  - add MonoScript missing default properties lookup support
  - add Materials missing texture references lookup support (thx Makaka Games)
### Removed
- Remove additional path restrictions when dropping assets to path filters
### Fixed
- Issues Finder: fix minor UI issues

## [1.10.2] - 2021-06-17
### Changed
- Make References Finder take embedded packages into account (thx Mr. Pink)
### Fixed
- Fix subfolders processing at References Finder

## [1.10.1] - 2021-06-16
### Changed
- Switch from plain text changelog to markdown-driven format
### Fixed
- Fix References Finder picked more source assets then looking in folders in some cases
- Fix asset selection on reveal when using two-column Project Browser

1.10.0
- Issues Finder: add support for all UnityEventBase subclasses (thx Andrija Stepic)

1.9.1
- fix ReferencesFinder.FindAssetReferences ignored showResults flag (thx Rhys Patterson)

1.9.0
- improve filer editing UX: now filter can be edited in-place
- fix disabled filters behavior could lead to empty search results (thx Esviliance)

1.8.0
- add Path Includes to the Project Cleaner
- add Enabled toggle to all filters to quickly switch them without removing and re-adding

1.7.4
- add Shader Graph SubShaders dependencies support in Unity 2019.2 and newer (thx Gleb_Palchin)

1.7.3
- Issues Finder: Fix missing scripts removal on prefabs (thx sacb0y)

1.7.2
- Issues Finder: add Playables support to the Missing Scripts search

1.7.1
- fix prefab variants in nested prefabs processing

1.7.0
- raise minimal supported version to 2018.4.0
- Core:
  * add custom external dependencies support (thx Jesse)
- Project Cleaner:
  * improve accuracy using last build report (Unity 2019.3+)
  * allow searching with no ignored scenes
- References Finder:
  * add detailed references support for Lighting Settings asset (2020.1)
- remove legacy code
- fix Lighting window path for 2020.1
- fix possible null reference error while sorting outputs

1.6.5
- fix possible null reference exception while processing prefabs with missing scripts (thx Ramsay)

1.6.4
- fix revealing at prefabs within canvases (thx Nilo)

1.6.3
- fix readonly components collapse while revealing
- fix model prefabs opened in associated apps while traversing (thx UnconventionalWarfare)

1.6.2
- remove built-in Asset Store Browser support (to be in sync with Unity 2020.1 changes)
- fix possible prefab mode glitch while looking for references in Unity 2018.3+
- fix broken urls

1.6.1
- add workaround for nested prefabs and prefab variants direct dependencies Unity bug (thx Rhys)
- fix nested prefab instances overrides lookup
- fix reveal selection for prefab variants in some cases
- refactor minor code portions

1.6.0
- add new Hierarchy Objects Scope to the References Finder
  * add in-scene references search of Game Objects and Components
  * add Hierarchy context menus in addition to drag&drop
- add quick expand / collapse with right click at all tree views
- add package manifest for future use with UPM
- prevent prefab stage reopening while revealing in Unity 2018.3+
- update UI appearance a bit
- move foldout settings from Project Settings to User Settings
- avoid active scene switch where possible while revealing in scenes
- fix notification could stay for too long
- fix buttons deactivation at References Finder module
- fix "Material doesn't have a texture property '_MainTex'" error appearance
- fix AddressablesReferenceFinder compilation error in Unity 2020
- fix minor UI issues
- fix and refactor internals a bit

1.5.9
- update urls to https
- add settings API xml docs
- add support for UserSettings folder in Unity 2020.1 and newer

1.5.8
- fix compilation errors and warnings for some Unity versions (thx Dave)

1.5.7
- add Assembly Definition Reference support
- fix GUID Assembly Definition references were not tracked properly

1.5.6
- fix Unity Event processing in Unity 2019 (thx Rhys)

1.5.5
- make Maintainer to be aware about addressables (thx Sebastiano)
- References Finder:
  * flatten tree: no more infinite confusing tree navigation
  * flatten tree: tree construction, save and load performance increased dramatically
  * add Find References button to each child item for continuous search
  * move exact references to the resizable lower pane for clarity and easier navigation
  * add Includes filters
  * add double-click row action (show)
- Issues Finder:
  * fix incorrect messages for unsuccessful issue fix
  * fix inability to auto-clean missing references in some cases
- add scrolls to main window (shows at very small window size)
- fix reveal for components on objects with hidden components
- fix exact references lookup in the ScriptableObjects
- fix possible null ref exception in ReferencesFinder (thx Jeff)
- fix null ref exceptions for removed scriptable object scripts (thx Syganek)

1.5.4
- Issues Finder: 
  * add Shaders with errors lookup in Unity 2019.1 and newer
  * improve missing scripts find & removal in Unity 2019.1 and newer
- fix internal Assets Map could not update properly (thx Rusty)
- fix context ignore menu could behave incorrectly

1.5.3
- update project to Unity 2017.4
- add assembly definition file
- ignore .preset files in Project Cleaner by default
- improve reveal behavior in same dirty scene
- fix possible null reference exception (thx Xblade-Imperium42)
- fix core assets map didn't update properly in some cases
- remove some legacy code

1.5.2
- update project to Unity 5.6
- refactor files and folders (don't forget to remove previous version)
- add 2018.3 nested prefabs support across all modules (scan, reveal, etc.)
- Issues Finder: rewrite core and focus on most important issues
  * ground-up rewrite and refactor of core
  * add missing references support for UnityEvent methods
  * add missing references search and reset in the project settings
  * add missing references search and reset in some scene settings
  * add missing scripts on ScriptableObjects support
  * improve UI of the settings part a bit
  * deprecate Duplicate Scenes In Build (not possible anymore)
  * deprecate Duplicate Tags (not possible anymore)
  * deprecate Undefined Tags (not possible anymore)
  * deprecate Empty Array Items as non-informative and annoying
  * deprecate Disconnected Prefab as non-informative and annoying
  * deprecate empty components as annoying and false positive-prone
  * always use precise mode for duplicate components search
  * fix group settings switch did't work with modern .NET 4.x compiler
  * fix missing references cleanup to reset reference file id too
  * fix inconsistent terrain issue reveal didn't unfold proper component
- References Finder:
  * refactor of core elements
  * add Exact Reference support for ScriptableObjects instances
  * add Exact Reference support for Tiles in Tilemaps
  * add support for nested LightingAssets
  * minor improve UI and usability
- add Unity 2018.3 unified settings window support
- fix Maintainer window won't show up sometimes
- fix Maintainer window icon won't show up sometimes
- fix Maintainer settings change won't save on scene switch in some cases
- remove some legacy code
- improve and fix a lot of minor stuff across common core and all modules

1.5.1
- personal settings now stored at Library folder, ignore it in VCS (thx Unnar)
- Issues Finder:
  * now ignore added from the results context menu gets automatically applied
  * fixed component ignores were applied incorrectly
- Project Cleaner:
  * now assets added to the AssetBundles are treated as needed and auto-ignored
  * added option for project rescan after adding ignore from context menu
  * added *.extension ignore to the quick ignore actions
  * fixed possible data loss when using some specific file types in the project
  * fixed possible data loss when using SpriteAtlases with folders for packing
  * fixed possible data loss when using 'Include in Build' SpriteAtlases option
  * fixed possible default ignores cleanup leading to data loss
- References Finder:
  * now automatically expands found item when selecting it
  * now finds ScriptableObjects' scripts references from context menu (thx Fausto)
- improved Reveal feature multi-scene handling
- improved Reveal feature overall scene handling quality
- starting from Unity 2018.1 settings are saved now when exiting Unity
- default filters are case-insensitive now
- extension filters accuracy improved
- Unity crash on missing script context menu call worked around
- fixed path ignores couldn't be added with ignore case option
- fixed Maintainer window didn't focused in some cases when it should
- fixed Maintainer window did focused in some cases when it should not =)
- other minor changes

1.5.0
- Issues Finder:
  * now can remove HideFlags.HideInInspector missing scripts (thx NibbleByte3)
  * now takes exposed reference into account when looking for duplicates
  * now takes fixed buffer size into account when looking for duplicates
  * now takes int versions of Vectors, Bound, Rect, into account
  * now component filters tab accept drop of the Component-derived script files 
  * duplicates analyze performance improved
- References Finder:
  * now script references can be searched from the component context menu
  * performance improved a bit for projects with lots of external packages added
  * fixed NullRef exception for projects with specific asmdefs (thx Andrey)
- improved reveal code stability with Unity bugs workarounds
- fixed incorrect processing of the Packages files in some cases
- fixed parent folder ignores could not work on Windows (thx Can)
- fixed 1.4.3 regression where Type filer kind was exposed in path filters tabs
- minor UI improvements
- all code usings re-organized to prevent ambiguity with third-party classes
- some legacy code removed
- readme docs were slightly updated
- minor fixes

1.4.3
- now References Finder skips references at the packages
- Issues Finder will not find empty array items at ParticleSystemRenderer anymore (thx Daniele)
- fixed reveal reopened scene in some cases
- fixed drag&drop accepted for incorrect assets kind in few places

1.4.2
- added support for Unity 2018.2 API changes
- added support for Unity 2018.2 menu changes
- added support for Packages in Project Browser (Unity 2018.2b2+)
- switched to X.X.X version tracking format
- removed minimal window size restriction (window reopen required)
- fixed logo texture bleeding at some cases
- minor refactorings and performace improvements

1.4.1.0
- Project Cleaner:
  - now ensures scenes are saved before search
  - no longer treats .asmdef as garbage by default
  - no longer treats SpriteAtlases as garbage if sprites are used
- References Finder:
  - now finds Assembly Definition references in another asmdefs
- switched some core code to GUIDs improving overall stability
- minor changes in scenes state checks in all modules
- increased some icon lookup code stability
- fixed 2018.1b12 regression incompatibility (thx Fausto)
- fixed incorrect file names treating in some modules on Windows
- fixed incorrect files treating after rename \ move (thx Liuyang)
- fixed prefab instances selection in scene (thx silvershunt)
- fixed scene icon lookup in Unity 2018.1+
- other minor changes

1.4.0.1
- fixed GetTracker error for Unity 2018.1 (thx Little_Gorilla)
- add workaround for possible exception while iterating components
- removed unused variable compilation warning

1.4.0.0 [don't forget to remove previous Maintainer version!]
- minimum required Unity version raised up to Unity 5.4.6
- new References Finder module released to the public!
  - requires Unity 5.6 +
  - look where specific assets are referenced in whole project
  - checks all scenes, file assets, project settings, scene settings
  - find references to all or selected assets in project
  - results are cached making further searches blazingly fast
  - inspect results with comfort at searchable & sortable tree
  - sort find results by path, type, size, references count
  - navigate to the reference in scene, deep prefab, settings
  - doesn't requires Text Asset Serialization in project
- Project Cleaner:
  - new core shared with References Finder
  - up to x6 performance boost (1st run may be slower tho)
  - much more accurate and safer results
  - improved analysis of some areas
  - exposed default path filters at the Manage Filters... window
  - added more prompts for the destructive actions
  - more information at the folders scan phase
  - fixed stopping error at shader fallback lookup in some cases
- Issues Finder:
  - huge scenes processing performance doubled
  - default sorting direction changed to Ascending
  - improved support for the multi-scene setup (thx Knubb)
  - improved performance for the missing scripts fix (removal)
  - now checks and resets missing references at ScriptableObjects
  - fixed extra record for empty Material at ParticleSystem
- filtering system improved for better flexibility
- all path filters now have more options for additional accuracy
- improved read-only files removal
- added secret debug mode ;)
- removed lots of legacy code
- internal refactoring
- show target code works faster now if component presents in path
- lots of other minor improvements
- other minor fixes

1.3.1.0
- Project Cleaner:
  - auto-exclude for assets set in Player Settings (thx Sebastiano)
  - added workaround for Unity bug to avoid fallback shaders removal
- fixed additive scene opening (thx Fausto)
- minor performance improvements across all modules
- new Usages module preparation work (not ready yet)

1.3.0.2
- Fixed compatibility bug for Unity 5.5.0p* and Unity 5.6.0b* (thx Fausto)
- Added workaround for UAS tools bug with AssetStore.png files

1.3.0.1
- Issues Finder: fixed possible data loss when searching current scene only
- Project Cleaner: added support for the non-object assets

1.3.0.0
- Project Cleaner out from preview to beta:
  - new unused assets search option
  - now it finds unused assets (experimental mode, be careful!)
  - added Statistics subsection
  - output of the sizes of all found and all selected items
  - empty scenes search deprecated
  - now you can select which scenes to ignore via comfortable tab
  - now any item can be copied to clipboard
  - now all items have icons like in Project window
  - now each found item has delete button for quick deletion
  - fixed empty folders with items beginning from '.' false positives
- Issues Finder:
  - auto fix for Missing Components (removes them from Game Object)
  - auto fix for Missing References (resets them to None)
  - new Scene Includes and appropriate filtering level for precise control
  - Path Includes to let you specify where to look for issues
  - improved component issue showing: all collapse except target one
  - duplicates search now checks non-managed types correctly
  - fixed incorrect selection of the objects with issues in rare cases
  - fixed issues in prefab instances could be accidentally skipped
  - fixed exception when running search from scene which was deleted
  - fixed some duplicates and false positives, accuracy increased
  - fixed missing empty animation in some cases
  - fixed missing component duplicates when issue presents in prefab
  - fixed scene wasn't reloaded for analysis in some cases
  - more accurate prefab instances skipping in some cases
- icons, icons everywhere
- compact mode for records in all modules with expansion on mouse click
- added results sorting to all modules.
- now assets are saved before search in all modules
- added support for the Unity 5.5+ ParticleSystemRenderMode.None
- fixed incorrect id lookup in prefabs
- fixed incorrect submit on the enter at the filtering tabs
- fixed notifications were not erased when switching filtering tabs
- fixed few things to improve compatibility with Unity 5.5
- fixed default encoding was used for settings file instead of UTF-8
- code preparation for the results filtering
- refactorings and code improvements
- UI improvements

1.2.0.3
- fixed project won't build in Unity < 5.2 (thx mwgray)

1.2.0.2
- Issues Finder:
  - ignoring empty AudioSources on objects with standard FirstPersonController
  - indication of the initial scene opening added to the progress
  - fixed exception when trying to show removed object (thx toxic2k)
  - fixed progress bar not showed the Tags and Layers scan phase
- minor changes in code to improve readability
  
1.2.0.1
- Issues Finder internal ignores:
  - added Fabric components to the ignores for duplicates detection
  - improved detection of the TextMeshPro component

1.2.0.0
- Issues Finder:
  - new search option: Inconsistent Terrain Data
  - new search option: Sprite Renderer without sprite
  - new search option: Terrain Collider without Terrain Data
  - new search option: Audio Source without Audio Clip
  - new search option: Object with huge world position
  - new search option: Duplicate scenes in build
  - new search option: Duplicates in Tags and Layers settings
  - ignores system introduced:
    * path ignores (you may ignore assets by full or partial path)
    * components ignores (you may ignore specified components)
    * 3 ways to add to ignores: drag & drop, manual, from the results list
  - added option for duplicate components search to ignore component values
  - added hide button to each issue to let you remove it from the list
  - Component's Local Identifier in file shown if necessary
  - added a button to clear search results
  - SceneManager APIs (introduced in Unity 5.3) are fully supported now
  - moved search results storage to the temporary location
  - improved output for missing \ empty array items
  - nicer assets paths output
  - more details in the final console log after scan
  - more details in the progress bar window
  - prefabs instances scan performance improved
  - simplified clipboard-management code
  - TextMeshPro* components added to ignores for empty array items search
  - TextMeshPro component added to ignores for empty MeshFilter search
  - 2D Toolkit components (tk2d*) added to ignores for empty MeshFilter search
  - fixed not all nested items of prefab assets were checked (thx Onur Er)
  - fixed incorrect behavior in few cases related to the multi scene editing
  - fixed possible data loss when performing search in new unsaved scene
  - fixed extra issue record for SpriteRenderers when they have no material
  - fixed duplicate Empty Layer issue when looking in both scenes and prefabs
- new module: Project Cleaner!
  - allows to find and clean selected items
  - finds empty folders
    * can find and clean empty folders automatically on each script reload
  - finds empty scenes
  - optional delete to trash bin
  - has path ignores
  - preview version of module, will be improved a lot in future updates
- settings file moved to the ProjectSettings
- all files moved to the Assets/Plugins folder
- all menu items moved to the Tools > Code Stage > Maintainer
- public settings API changed
- minor improvements in reports
- significant refactorings for additional flexibility and future updates
- significant code cleanup
- minimum window height increased to 500 px
- spelling fixes in comments and docs

1.1.0.1
- Issues Finder:
  - fixed scenes paths comparison leading to unnecessary scene save prompt

1.1.0
- Issues Finder: 
  - new scene filtering mode: current scene only
  - new search option: Duplicate components
  - new Reset button added to let you quickly reset settings to defaults
  - new standard prefab icon added to the prefab assets records
  - now after search you return to the scene which was opened before search
  - now scene file is highlighted when you press "show" button
  - now all buttons for found issues are placed below issue description to avoid unnecessary scrolling
  - now all deep nested objects (level 2 and more) in instantiated prefabs are scanned as well
  - report header was re-worked a bit and now includes unity version
  - now Issues Finder may be called from user scripts, see "Using Maintainer from code" readme section
  - lot of tooltips added
  - attempt to fix rare Maintainer leakage
  - increased Unity 5 compatibility (deprecated API replaced)
  - scenes in build filtration optimizations
  - minor fixes and improvements
  - minor optimizations
  - minor refactorings
- Settings file is now re-created in case it was damaged and couldn't be read
- Troubleshooting readme section was supplemented by new item about Debug Inspector mode
- additions and fixes in readme

1.0.0
- first public release with initial Issues Finder module