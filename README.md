# Halcyon Academy · First Weather

A Unity narrative prototype by **Bergen Carloss**.

## Current release: v0.3.0 — September 24, 2026

**[Download the playable Mac prototype](https://github.com/bcarloss243/Game-Design-Repo-1-7.9.26/releases/tag/v0.3.0)**. Choose `Halcyon-First-Weather-v0.3.0-Mac.zip`, extract it, and open the app. Requires macOS 12+; Apple Silicon and Intel binaries included, execution tested on Apple M3. Unity is not required. The app is not Apple notarized; see the included START HERE for first-launch instructions.

- **Painted world and interface:** September title art direction, Cinzel Decorative headings, Cormorant body text, indigo/gold panels, new character portraits and environments. The academy is a compact two-storey building in a coherent enclosed courtyard. Painted walking figures replace geometric pedestrians; clock, light, foreground and water remain live layers. The district pans/zooms and shows the matching academy. Room and courtyard views conceal outside weather.
- **Choices with consequences:** three morning routines, pressure-sensitive prose and sound, helpful Vapeur with a floor and cumulative stability, rebound, persisted morning variation, academic preparation and threshold-based recovery costs. Lola’s presence changes the instrument without a relationship score.
- **Two-day loop:** evening call/message/sleep options, first-person notes and a typed journal, family dispatches, Rootwork-led scarce-water choices with different mentor priorities, and Lola’s final message instead of a scorecard.
- **Writer access:** dialogue is editable in [FirstWeather.ink](Assets/HalcyonSlice/Narrative/FirstWeather.ink), with a [writing guide](Assets/HalcyonSlice/Documentation/WRITING-GUIDE.md). This remains a working draft for Bergen to author.

![Revised academy courtyard](Assets/HalcyonSlice/Resources/HalcyonAcademy/AcademyForecourt.png)

## Open in Unity

1. Clone with Git LFS installed, then retrieve the large assets with `git lfs pull`.
2. Open the project with **Unity 6000.3.14f1**.
3. Choose **Halcyon → 1 Open First Weather**, then press Play.
4. Begin or continue a story. Start at the residence, attend class, choose afternoon activities, return home, then play the second day through Lola’s message.

Click or use Tab/Shift+Tab and Enter. Space advances pages without choices; Esc pauses; J opens the journal; M toggles audio. Drag/scroll explores the map and academy; zoom/recenter buttons provide alternatives. Settings include larger story text and reduced motion. One local save; a new story replaces it. Developer previews isolate the save: stop and restart Play mode for normal play.

The original SampleScene is preserved. This is layered 2D artwork with animated details, not a freely rotatable 3D city. The two student designs and their simple pose cycles are prototype crowd assets.

## Verification and review

The revised story/rules check passes **1,534 assertions across 108 simulated routes**. Runtime checks pass **4,283 screen assertions**, **555 academy checks**, and **1,764 map text measurements**. Complete button-driven editor runs reach both **Garden Restored** and **Work in Progress**, including the high-pressure recovery route. The study/rest route contains **1,610 narrative words**, about 6.4–8.1 minutes at 200–250 words/minute before interactions; this is an estimate, not measured human playtime.

See [current QA and limits](Assets/HalcyonSlice/Documentation/V03-QA.md) and the [critique checklist](Assets/HalcyonSlice/Documentation/REVISION-CHECKLIST.md). The full quarter, broader Ward travel and later revelations are outside this slice. The release copy also completed a full standalone route with quit/relaunch/continue and journal persistence verified. See [release QA](Assets/HalcyonSlice/Documentation/RELEASE-QA.md) and the [rubric checklist](Assets/HalcyonSlice/Documentation/RUBRIC-CHECKLIST.md). Bergen is finishing the reflection and human playtest documents separately; the pitch video also requires its own URL.

## Documents and credits

- [Play instructions and known limits](Assets/HalcyonSlice/Documentation/README.md)
- [Feedback evidence and three-session worksheet](Assets/HalcyonSlice/Documentation/CapstonePlaytests.md)
- [Iteration journal](Assets/HalcyonSlice/Documentation/IterationJournal.md)
- [Reflection draft](Assets/HalcyonSlice/Documentation/Reflection-Draft.md)
- [Five-minute pitch script](Assets/HalcyonSlice/Documentation/PitchVideo-Script-Draft.md)
- [Art provenance](Assets/HalcyonSlice/Documentation/V03-ArtProvenance.md)
- [Dependency notices](Assets/HalcyonSlice/Documentation/ThirdPartyNotices.txt)
- [Version history](CHANGELOG.md)

Bergen Carloss: concept, world, characters and creative direction. OpenAI Codex assisted with programming, draft writing and original synthesized audio. OpenAI image generation produced reference-guided prototype illustrations. Cinzel Decorative and Cormorant are bundled under SIL OFL 1.1. Ink, DOTween, Unity components and the preexisting FMOD integration retain their notices; FMOD is by Firelight Technologies Pty Ltd. Bergen created the original title drawing and developed the final illustration with ChatGPT; this is credited in-game and in the package.
