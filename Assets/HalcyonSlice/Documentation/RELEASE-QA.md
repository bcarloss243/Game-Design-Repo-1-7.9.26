# First Weather v0.3.0 — release verification

September 24, 2026. This record covers the packaged Mac application, following the September 23 editor revision. These are agent-operated technical checks, not participant playtests.

## Build

- Unity 6000.3.14f1; FirstWeather scene; non-development StandaloneOSX export.
- Build result: Succeeded; zero errors, one warning; reported size 211,017,758 bytes.
- Warning: Unity Services returned HTTP 403 for native-symbol upload. This did not prevent export or play.
- Bundle identifier: `com.bergencarloss.halcyon.firstweather`; version `0.3.0`; minimum macOS 12. Universal executable contains arm64 and x86_64.
- Exported app was copied into the release folder and ad hoc signed, including nested plugins. Deep, strict signature verification passed. It is not Apple Developer ID signed or notarized.
- Execution tested on Apple M3. Intel and a second Mac remain untested.

## Standalone playthrough

Started a new game at the title and completed both days using the release copy, with no observed crash or progression blocker. Route: fern; Vapeur both mornings; Thibodaux/Greenwork minor; study and rest; call Alaric; message Jules; typed journal; ask for class support; tell Lola about class; request a puzzle hint; allocate 2/2/2 in both rounds; ask for tea. The final screen displayed **Complete · Garden Restored**, with the matching tea invitation.

Verified in the actual application:

- Title/name, New Story, credits and the full license viewer.
- Day/time, narrative-page and activity-state indicators; academy entry and unlocked greenhouse.
- Mentor and study/rest consequences, two-slot budget, evening interactions.
- Typed journal retained after Escape/reopen and after a full app quit/relaunch.
- Pause/resume, automatic pause on losing focus, safe in-game Quit.
- Continue after relaunch restored the same evening and choices, then allowed the remaining story to finish.
- Incomplete garden allocation returned feedback; a hint and valid allocations remained usable.
- Explicit completion/result, closing message and Return to title.
- Settings toggles for larger text, reduced motion and audio changed state. Defaults were restored afterward.

Final saved state: day 2, finished=true, solved=true, view=ending, pressure=20, mentor=ecology, studied/rested/calledHome/messagedJules=true. Recorded active time was 1,143 seconds (about 19 minutes), including technical checks and reading. This is **not** a measurement of normal human playtime. The representative story route has 1,610 words, about 6.4–8.1 minutes at 200–250 words/minute before interaction. Fast clicking can finish sooner; no artificial timer is imposed.

The player log showed orderly input/physics shutdown. It contained nonfatal thread-finalization and ComputeBuffer-disposal warnings; no runtime exception or crash was observed. The earlier complete editor routes also exercised Work in Progress and the high-pressure recovery path; those alternate branches were not repeated in this standalone session.

## Existing regression evidence

- 1,534 assertions over 108 simulated routes, rerun before this build.
- 4,283 runtime screen assertions; default/larger text.
- 555 academy checks; 1,764 map text measurements.
- Complete button-driven editor runs to Garden Restored and Work in Progress.

See the individual report timestamps and V03-QA.md. These checks are complementary to human feedback, not substitutes for the three required documented playtests.

## Packaging procedure

Include the tested app, START HERE, credits, complete third-party notices, known issues, build report and rubric checklist. Create a ZIP preserving Mac application metadata, then extract it and verify archive integrity, executable permissions and deep/strict signing again. Publish the archive as a GitHub Release asset, separate from GitHub’s automatically generated source archives. A SHA256SUMS file accompanies the download.

Reproduction: use Halcyon → 4 Build Mac Prototype, copy the resulting application into a release folder, run macOS `codesign --force --deep --sign -` on that copy, then `codesign --verify --deep --strict` before and after ZIP extraction. The Unity build menu alone does not notarize or package a release.
