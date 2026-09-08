# Halcyon Academy

A Unity game prototype by **Bergen Carloss**.

## Latest update: map interface v0.2 — September 8, 2026

The map now uses full-width artwork, Cormorant Garamond headings, brass details, smaller location labels, a foldable timetable and an illustrated Pressure barometer with a live needle. The instrument also appears beside dialogue. Existing story text, progression and saves remain compatible.

The map layout check found **zero issues across 1,596 text measurements**. The Mac app resumed an existing afternoon save at Pressure 57 with one slot remaining. See the [interface update](Assets/HalcyonSlice/Documentation/InterfaceUpdate.md) and [v0.2 verification](Assets/HalcyonSlice/Documentation/InterfaceQA.md).

### Current development workflow

At Bergen's direction, continue the next phase in **Unity and GitHub**. Do not rebuild or package the downloadable game until that phase is finished, unless Bergen explicitly requests a build. The dial's visual direction is approved. Bergen intends to write the dialogue; the existing draft remains provisional.

The recommended next milestone is an explorable map: smooth trackpad panning and controlled zoom, location markers anchored to the artwork, clear unlock states and a reliable return to the district overview. Layered depth and animated water, boats and lighting follow once movement is comfortable. These features are planned and are not included in v0.2.

## Playable foundation: First Weather v0.1

**First Weather** adds a playable, illustrated narrative slice covering Molly's first two days at Halcyon Academy. Choose a mentor, balance study, rest and company, carry those decisions into the next morning, and meet Lola in the greenhouse. A two-stage garden puzzle leads to one of two explicit story endings.

The study/rest route contains **40 narrative pages and 2,241 words**: an estimated **9–11 minutes of reading, plus choices and the puzzle**. This is a content estimate; actual completion time still needs human playtesting.

![The greenhouse in First Weather](Assets/HalcyonSlice/Resources/HalcyonArt/greenhouse.png)

## Open and play

1. Clone this repository with **Git LFS** installed, then run `git lfs pull` to retrieve the artwork and the project's other large assets.
2. Open the repository folder in **Unity 6000.3.14f1** through Unity Hub. Let Unity finish importing and compiling.
3. Choose **Halcyon → 1 Open First Weather** from the Unity menu.
4. Press **Play**. Choose **Begin a new story**, then start at the residence on the city map.

The scene is `Assets/HalcyonSlice/Scenes/FirstWeather.unity`. The original `Assets/Scenes/SampleScene.unity` and earlier prototype scripts are preserved.

Click locations and choices. **Space** advances pages without choices. **Tab / Shift+Tab** select buttons; **Enter** activates them. **Escape** pauses, **J** opens the journal, and **M** mutes audio. Settings include larger story text and reduced motion. Continue resumes the prototype's single local save; beginning a new story replaces it.

## What this update adds

- A city navigation screen and two-day story with morning, class, afternoon and sleep progression.
- Three room keepsakes, two mentor tracks, and two afternoon activity slots shared between study, rest and company.
- Pressure feedback and earlier choices that change the second-day classroom scene.
- A conversation with Lola, a six-measure water allocation puzzle with changing light, optional hints, and **Garden Restored** or **Work in Progress** endings.
- Title, pause, save/continue, journal, help, settings, credits and dependency license screens.
- Four new reference-guided environment illustrations, the supplied cover art, and original synthesized music, ambience and interface sounds.

This is an **illustrated narrative prototype with static environment art**, not a freely walkable 3D city. Room keepsakes affect writing and journal state. The dialogue, added mentor/classmate names and garden project are provisional content for review.

## Verification and build

The story/rules verifier passed **3,295 assertions**. A complete Unity editor route reached **Garden Restored**; a native Mac route from an existing save reached **Work in Progress**. Save/continue, puzzle recovery, keyboard navigation, larger text, credits and license navigation were checked. See the [verification record](Assets/HalcyonSlice/Documentation/QA.md) for the precise scope and limitations.

To build locally, install Unity's Mac Build Support and choose **Halcyon → 4 Build Mac Prototype** while Play Mode is stopped. Output goes to `Builds/FirstWeather-Mac`. The tested build contains Apple Silicon and Intel executables and requires macOS 12 or newer; execution was tested on Apple Silicon only. The locally packaged app is ad hoc signed, not notarized. Compiled apps and ZIP downloads are not included in this source commit.

## Documentation and credits

- [September 8 update and remaining work](CHANGELOG.md)
- [Map interface changes and artwork sources](Assets/HalcyonSlice/Documentation/InterfaceUpdate.md)
- [Map interface verification](Assets/HalcyonSlice/Documentation/InterfaceQA.md)
- [Full play instructions and prototype scope](Assets/HalcyonSlice/Documentation/README.md)
- [Test results and limitations](Assets/HalcyonSlice/Documentation/QA.md)
- [Human playtest worksheet](Assets/HalcyonSlice/Documentation/CapstonePlaytests.md)
- [Art provenance](Assets/HalcyonSlice/Documentation/ArtProvenance.md)
- [Third-party notices](Assets/HalcyonSlice/Documentation/ThirdPartyNotices.txt)

World, characters and creative direction: Bergen Carloss. Programming, draft writing and synthesized audio were created with OpenAI Codex under Bergen's direction. Four environment illustrations were generated with OpenAI image generation from supplied references. The supplied cover's original creator and distribution rights still need to be recorded; no new ownership claim is made over it.

The capstone still needs real participant playtests and timing evidence, the journal/reflection, the pitch recording and the final submission upload. Automated checks do not replace those requirements.
