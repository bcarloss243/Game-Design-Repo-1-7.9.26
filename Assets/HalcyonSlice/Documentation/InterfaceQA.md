# Interface v0.2 verification

September 8, 2026. Automated and editor checks performed by Codex; these are not human capstone playtests.

- The new code compiled in Unity 6000.3.14f1.
- The custom panel renderer initially failed to display correctly. Adding its required CanvasRenderer and selecting the current UI mesh-generation path fixed the backplates, vignette, pins and needle; the corrected screen was visually inspected.
- The first text-fit check identified the title's text bounds and a wrapped Pause label. Both were corrected.
- The final map layout check reported **0 issues across 1,596 text measurements**: seven existing map phases, normal/larger text, and Pressure 0, 54 and 100. See MapInterfaceVerification.txt.
- Inspected the arrival and afternoon maps. The afternoon view displayed the remaining two slots, study's +12 Pressure / one-slot cost, and company at the canal's −9 Pressure / one-slot cost.
- Clicked the timetable fold control and observed the expanded card replaced by the compact Open timetable button.
- Keyboard Tab navigated through Journal, Settings, Pause and the available residence. Its focus treatment appeared; Enter opened the morning scene.
- Clicking the residence also opened the existing morning scene. The new instrument fit beside the original dialogue layout at the larger story-text setting.
- The story and progression files (SliceStory.cs and SliceState.cs) have no changes in this pass. Existing story verification is run again by the Mac build command.
- Editor interface previews suppress story saving. The preview machinery is excluded from standalone builds. Normal new-story/continue actions restore normal saving.

## Mac build and save continuity

- The v0.2 Mac build succeeded with zero errors and one warning. Unity Services returned HTTP 403 for native symbol upload; this did not prevent the local build. BuildReport.txt records the build result.
- The existing story verifier passed 3,295 assertions. Its estimated reading time is not a measured human playtime.
- The release app was ad hoc signed and passed strict signature verification. It is not notarized. Runtime verification was performed on Apple M3; the Intel target has not been run on Intel hardware.
- Launched the packaged v0.2 app and selected Continue your story. The existing save opened on Day 1 at 15:00, with the Reading Room marked Prepared, Pressure 57 / Elevated, and one of two afternoon slots left.
- Inspected the new type, map labels, instrument scale/needle and timetable in the standalone app. Folded and reopened the timetable without choosing a story activity.
- No exceptions or errors appeared in the standalone player log during this check. The app is left at the resumed map for Bergen's review.

The earlier QA report in this folder describes v0.1. This document records the focused v0.2 checks; a complete manual story playthrough was not repeated for this presentation-only update.

Remaining: human usability and visual-direction review, final typography/interaction treatment for the other screens, trackpad map navigation, layered depth, animated environments, and Bergen's dialogue revisions. No improvement in measured player engagement or completion time is claimed.
