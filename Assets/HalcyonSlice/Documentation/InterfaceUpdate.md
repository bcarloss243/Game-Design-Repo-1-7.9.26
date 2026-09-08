# Map interface study — v0.2

September 8, 2026. This is the first map UI and typography pass in the existing First Weather Unity slice.

## Changes

- A full-width city image with a restrained edge vignette replaces the permanent left sidebar.
- Cormorant Garamond headings and location names, fine brass rules, chamfered enamel labels and a compact, foldable timetable establish the map's visual language.
- Available locations have filled diamond pins; unavailable locations have hollow pins and written status. Available locations retain mouse and keyboard activation, with a focus outline.
- The Pressure instrument uses a new painted brass-and-ivory housing. Unity draws the 0–100 scale, colored zone band, numeric value and moving needle. It uses the existing Pressure state and thresholds. The same instrument now appears beside dialogue.
- The new presentation is kept in HalcyonMapPresentation.cs. Story text and progression rules are unchanged.

## Review in Unity

Open First Weather and press Play. Choose **Halcyon → 5 Preview Map Interface (Play Mode)** to inspect an arrival map without writing over the story save. Menu item 6 cycles through the existing map phases. Menu item 7 checks the text layout across phases and sizes.

Preview mode suppresses story saves for that editor session. Stop and restart Play Mode for normal play. Starting a new story or continuing from the title also restores normal save behavior. The preview controls are editor-only and do not appear in the Mac app.

## Scope

This pass establishes one map screen. It does not add panning/zooming, parallax, animated boats, new map geography or living room/classroom scenes. Dialogue layout outside the instrument, other menus, title layout, and all scene writing remain from v0.1. Bergen will supply the dialogue voice; existing lines remain placeholders.

## Sources

Display typeface: Cormorant Garamond SemiBold by Christian Thalmann and the Cormorant Project Authors. Downloaded from the author's repository, under SIL Open Font License 1.1. Source: https://github.com/CatharsisFonts/Cormorant . The original font and license accompany the project in Resources/HalcyonFonts; the license is also included in the in-game dependency notices.

Instrument housing: generated with the built-in OpenAI image-generation tool, using the supplied Halcyon cover as a style reference. Stored at Resources/HalcyonUI/BrassBarometer.png. Scale, needle and readings are live Unity graphics. The complete generation prompt is in BarometerPrompt.txt.

The existing map image and cover retain their v0.1 provenance. No new claim is made about their geographic precision or distribution rights.
