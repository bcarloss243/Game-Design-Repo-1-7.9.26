# Halcyon Academy: First Weather

A playable capstone slice by Bergen Carloss. Version 0.1, September 8, 2026.

Play Molly's first two days at Halcyon Academy. Make a room your own, choose a mentor, spend a limited afternoon, and meet Lola in the greenhouse. Earlier choices change the next morning and the classroom scene. A two-stage water distribution puzzle leads to an explicit ending.

## Play in Unity

Open the existing **My project** project in Unity 6000.3.14f1. Choose **Halcyon → 1 Open First Weather**, then press **Play**. Double-click the Game tab to expand the view if needed.

The new scene is `Assets/HalcyonSlice/Scenes/FirstWeather.unity`. It is separate from the original `SampleScene`. All new code, art and documentation are inside `Assets/HalcyonSlice`.

## Controls

- Click a bright map marker to enter a location. Begin each morning at the residence.
- Click dialogue choices. Space advances pages that have no choice.
- Tab / Shift+Tab select buttons; Enter activates them.
- Escape pauses or closes a menu. J opens the journal. M mutes audio.
- Scroll the journal, help and credits with the mouse wheel, or Up/Down arrows.
- Settings provide larger story text, reduced motion and mute. No dialogue or puzzle has a timer. Information uses words and numbers as well as color.

New Story replaces this prototype's single save. Continue resumes after the last action. Progress and settings are stored locally through Unity PlayerPrefs. Pausing stops the playtime counter, and leaving the game window automatically pauses. The standalone app and editor can have separate save locations.

## Complete the slice

1. Finish the residence morning and inaugural class.
2. Choose up to two different afternoon activities: study, rest, or company at the canal. Their time and Pressure effects appear before selection.
3. Return to the residence and sleep, then finish the second morning and class.
4. Find the greenhouse. Talk with Lola, then distribute six measures across three beds.
5. Test the first distribution, adapt to the changed light, and test again. Alternatively leave a note to finish with the garden still in progress.
6. Finish the conversation and select the greenhouse on the map to see the result screen.

**Garden Restored** means both water conditions were balanced. **Work in Progress** means you chose to return to the work another day. Both complete the narrative and preserve Molly and Lola's connection. Hints carry no penalty. Pressure is a fictional measure of strain; it does not gate the greenhouse or determine whether Molly deserves a relationship.

The study-and-rest route has 40 narrative pages and approximately 2,240 words. That is an estimated 9–11 minutes of reading at 200–250 words per minute, plus interaction. This is a content estimate, not a claim about measured human playtime. Faster clicking can shorten any run.

## Build and verify

The **Halcyon** menu provides story/rules checks, a current-screen text overflow check in Play Mode, and a Mac build command. Build output goes to `Builds/FirstWeather-Mac` in the Unity project. The builder selects only the new scene. Unity also refreshes some project, rendering and font settings during a build; inspect those changes before committing the project.

Automated checks validate activity budgets and repeats, Pressure boundaries, overnight carryover, serialization, invalid saves, finite water supply, both puzzle conditions, asset loading, and larger-text fit across story branches. See `Verification.txt` and `QA.md` for actual results and remaining limitations.

## Prototype scope

This is an illustrated narrative game with a city navigation screen, branching scenes and a small puzzle. The illustrations are static, with gentle screen fades and a Pressure needle; it is not a freely walkable 3D city. Room personalization currently changes text and journal state, not furniture placement. The map highlights destination markers rather than revealing whole geographic regions. The garden's printed requirements intentionally make this first puzzle approachable.

Draft mentor/classmate names, dialogue and the floating garden project were added for this prototype and remain open to Bergen's revision. Longer Ward storylines, a full romance system, investigation, live instrument comparisons, expanded apartment decoration, animated character sprites and a campaign economy are beyond this slice.

## Credits and provenance

World, characters and creative direction: Bergen Carloss. Programming, draft writing and original synthesized music/ambience/UI sounds: created with OpenAI Codex under Bergen's direction. Four new environment illustrations: generated with OpenAI image generation using the supplied Lola/Molly reference for style and character continuity.

The title artwork is the unmodified reference supplied by Bergen. Its original creator and publication rights were not identified in this session and should be recorded before public distribution. This prototype does not assert ownership over that reference. See `ArtProvenance.md` and the in-game Credits screen.

Unity UI, TextMesh Pro and Input System handle presentation and controls. The slice uses native Unity audio. Existing Ink, DOTween and FMOD integrations remain in the parent project. Installed dependency notices are included in `ThirdPartyNotices.txt`.

## Capstone handoff

The game is one component of the assignment. Human playtests, the journal/reflection, the pitch recording and the submission link must be completed with real evidence. `CapstonePlaytests.md` provides a blank record and a focused testing plan. Automated checks do not count as the three required human playtests.
