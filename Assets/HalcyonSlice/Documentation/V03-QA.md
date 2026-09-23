# First Weather v0.3 — verification record

September 23, 2026. Unity 6000.3.14f1, macOS editor on the development machine. No standalone build was created or tested for this revision.

## Automated evidence

| Check | Result | Report |
|---|---|---|
| Native Ink compilation, choices, state and actual-font story fit | 1,534 assertions, zero issues; 108 complete simulated routine/activity/mentor routes | V03-Rules.txt |
| Runtime title, map, academy, evening, garden, ending, twelve story scenes and nine dialogs | 4,283 assertions, zero text-fit issues; default and larger body text | V03-Screens.txt |
| Academy motion, pause/reduced motion, frame-time safety, camera bounds/zoom anchors, entry gating and save isolation | 555 checks including 406 text measurements, zero issues | AcademyVerification.txt |
| Map seven phases × two text settings × three pressure values | 1,764 text measurements, zero issues | MapInterfaceVerification.txt |
| Button-driven complete study/rest route | 86 actions, Garden Restored, valid final state, original player save unchanged | V03-Playthrough.txt |
| Button-driven complete social/rest route, skip both mornings | 80 actions, threshold panic/recovery and Work in Progress, valid final state, original save unchanged | V03-Fallback-Playthrough.txt |

The simulator is a branch matrix, not an exhaustive proof of every combination. The button runs exercise actual runtime buttons and transitions, but are automated editor runs, not human playtests or measurements of reading comfort. The study/rest route has 1,610 narrative words: approximately 6.4–8.1 minutes at 200–250 words/minute, plus choices and interaction. Players can click through faster; there is no artificial five-minute timer.

## Visual and interaction review

Inspected the title, dialogue/journal and academy in Unity’s game view. The first exterior assembly still looked miniature; it was replaced with one coherent court/building perspective. The revised students have painted heads, uniforms and full bodies; they move along paved routes and central stairs. The portrait dial initially failed to draw its portrait; custom mesh initialization was corrected and the portrait is visible.

Verified the visible academy zoom control and preview transitions, pause/resume, and journal typing. Direct scroll/drag automation did not provide conclusive physical-trackpad evidence; camera math and motion handlers pass automated tests. Bergen should check drag/trackpad comfort on the intended machine. No claim is made that an automated pointer gesture equals a human trackpad test.

During manual journal testing, Escape restored the input field’s previous value. The input is configured to retain edits on Escape and the game closes/saves the journal. Retest passed: typed “Molly and Jules: my journal test.”, closed with Escape and reopened with J; the entry remained visible. This was a save-isolated preview.

## Known limits and submission gates

- No game-breaking defect was observed in the two completed editor routes. This does not prove absence of all defects or compatibility with a new standalone export.
- Two crowd designs and four simple poses repeat; they are prototype motion, not finished walk cycles. Other rooms use subtle lighting/mote/reflection ambience rather than fully rigged characters.
- The frame is designed for a 16:9 safe area; other window shapes letterbox. Text was measured at both provided settings, but broader display/accessibility testing remains for human sessions.
- The Ink adapter supports a fixed paragraph spine with state-dependent text/choices. Arbitrary knot branching and paragraph-count changes in conditional branches require code/save work.
- Full quarter progression, wider Ward travel and later reveals are deliberately beyond this slice. No fabricated functionality is implied by their narrative seeds.
- The supplied feedback contains three themes with no participant/version/session metadata. Three genuine documented sessions remain necessary. Reflection/pitch materials are drafts; no video has been recorded or URL invented.
- Title-reference publication provenance still needs confirmation before distribution. Existing third-party notices are retained and new font notices are bundled.
- Packaging and release upload are deferred at Bergen’s request. Prior Mac ZIPs are older versions.
