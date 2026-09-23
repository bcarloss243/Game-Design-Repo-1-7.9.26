# Writing First Weather

Bergen owns the dialogue. The current Ink file is a revisable working draft, separated from Unity presentation and simulation logic.

**File:** Assets/HalcyonSlice/Narrative/FirstWeather.ink

Each named knot is a scene; each nonempty paragraph becomes one screen. Rewrite paragraphs freely. Keep conditional alternatives on the same paragraph and keep choices gathering to the next paragraph: this adapter stores the scene and page number, not a general Ink call stack. Arbitrary branching knots, loops, or paragraph-count changes within a conditional need an adapter/save migration change. After a major rewrite, begin a new game rather than trusting an old page index.

Example:

```ink
=== scene_name ===
A paragraph of your dialogue. # speaker:Lola # portrait:lola
* [A choice the player can read. # effect:share]
* [Another choice. # effect:askgarden]
-
{shared:Words after sharing.|Words after the other decision.} # speaker:Molly
-> END
```

Choice effect tags belong **inside the square brackets**. They are hidden from players and applied only when the choice is committed. Supported effects: decor:fern, decor:boat, decor:postcard, routine, hum, skip, mentor:circulation, mentor:ecology, outside, support, share, askgarden, tea, return. Add a new effect in SliceState.Apply before using it.

Available portrait tags: molly, lola, alaric. Speaker names are free text. Optional panel tags `fracture` and `threshold` hold quiet comic gutters. Event tags trigger once and are persisted: call-start, call-end, message-jules, assessment, corridor, lola-arrival, lola-touch, lola-reciprocity, disclosure, settling, harvest-lights. Preserve them when rewriting the relevant beat. Events run before the displayed paragraph is reevaluated, so its words can reflect the resulting state.

Ink variables mirror the simulation: pressure/zone; medicated/rebound/tolerance; studied/rested/socialized; mentor/decor; panic/recovery/academic_warning; shared/assisted/solved; response/invitation/shortage; called_home/messaged_jules; day/first_clarity. Use these to change what Molly notices, sentence rhythm, available choice wording, or later responses. Do not explain hidden meters in fiction.

The two-day sequence is morning1 → class1 → selected study/rest/social scenes → optional callhome/messagejules → night → morning2 → class2 → greenhouse → water interaction → aftergarden → Lola’s dispatch. The map and evening menu control movement between scenes.

Unity recompiles the source when it imports. `Halcyon → Compile story` can force a refresh. Do not edit the compiled JSON. After edits, run `Verify Story and Rules`, then in Play mode run `Verify v0.3 screens` and `Playtest complete route`. Check the Console and the current report files. Read the lines aloud yourself: fit checks cannot judge voice, implication, rhythm or authenticity.

Keep short pages without forced auto-advance. Default body text is 30 at the 1600×900 design resolution; larger text is 33. Three choices share one row, so short labels are helpful. Word-count timing is an estimate, never evidence of an actual human playtest.
