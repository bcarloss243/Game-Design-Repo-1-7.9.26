# Change log

## First Weather v0.1 — September 8, 2026

### Playable addition

The earlier repository contained the GUI and Pressure-gauge prototype. This update adds a separate, playable two-day narrative slice in `Assets/HalcyonSlice`, while preserving the original scene and custom scripts.

- **Progression:** Begin each morning at the residence, attend class, choose a mentor, spend up to two afternoon slots, and sleep to advance to day two. Study, rest and company are distinct choices with visible Pressure effects and later narrative consequences.
- **Molly and Lola:** A second-day classroom scene reflects preparation, rest and social support. The greenhouse conversation offers choices about disclosure and Lola's own project. Pressure and puzzle success do not gate their connection.
- **Garden puzzle:** Allocate six measures of water among three beds, then redistribute them when the light changes. Incorrect attempts provide feedback; hints have no penalty. Solving both stages produces **Garden Restored**; choosing to return later produces **Work in Progress**. Both end the story.
- **Presentation:** A city map, residence, academy and greenhouse in the supplied engraved/painterly visual direction; the original supplied cover; synthesized score, ambience and interface sounds.
- **Usability:** Title/new story, one local save and Continue, journal, pause, safe quit, automatic pause when the window loses focus, keyboard buttons, larger story text, reduced motion, mute, help, credits and a full dependency license viewer.
- **Unity tools:** Menu commands to open the new scene, verify story/rules, check visible text fit in Play Mode, and build the Mac prototype.

### Evidence

- **3,295 automated assertions passed**, covering activity budgets, repeat prevention, Pressure bounds, overnight carryover, save serialization, invalid allocations, both garden conditions, image loading, and larger-text fit across the tested branch matrix.
- The study/rest route contains **40 pages / 2,241 narrative words**, approximately **9–11 minutes of reading** at 200–250 words per minute, plus interaction. This is not measured human playtime.
- A full editor route reached **Garden Restored**, with study/rest choices retained and final Pressure **13 / Clarity**. A native route continued from an existing first-class save reached **Work in Progress**, with Jules's support retained and final Pressure **38 / Steady**.
- The Mac build succeeded with **zero errors and two warnings**. A Unity Services symbol-upload warning and a pending editor/import-change warning are documented in QA. The updated license viewer was exercised in the player.
- The release copy launched on Apple Silicon. Both ZIP archives passed integrity checks; the extracted Mac app retained executable permissions and passed deep, strict signature verification after local ad hoc signing. It is not notarized, and Intel execution and first launch on another Mac remain untested.

### Scope and remaining work

This is a static illustrated narrative game with map navigation and a small puzzle. It does not implement a walkable 3D city, animated character sprites, furniture placement, the full romance system or later Ward storylines. New dialogue and supporting names remain drafts for Bergen's review.

The capstone's participant playtests, measured completion times, reflection/journal, pitch recording and submission upload remain outstanding. Cover attribution and distribution rights also remain to be recorded.

### Files and repository scope

The update adds `Assets/HalcyonSlice` with the scene, code, artwork, Unity metadata, credits, notices and documentation, plus this change log and the repository README. It does not replace `SampleScene` or the earlier prototype scripts.

Unity-generated rendering, font-cache and project-setting changes from the local build are left uncommitted. Compiled applications, build caches and release ZIPs remain outside the source commit. The Mac build can be reproduced through the Halcyon menu with the corresponding Unity build support installed.
