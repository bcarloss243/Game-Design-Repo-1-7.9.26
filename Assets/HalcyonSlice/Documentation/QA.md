# Verification record — First Weather v0.1

Development date: September 8, 2026. These checks were performed by Codex using Unity and its game window. They are not participant playtests for the capstone requirement.

## Automated verification

Unity 6000.3.14f1 compiled the slice. The story/rules verifier passed **3,295 assertions**, including every combination of preparation, rest, company, sharing and mentor represented by its branch matrix. Both garden outcome texts were included in the final branch fit check.

Checks cover activity access, repeat prevention, the two-slot limit, overnight consequences, Pressure limits, JSON round trips, corrupt allocation rejection, finite water supply, both garden solutions, image loading, and paragraph/choice fit at the larger story-text setting.

The study/rest route contains **40 pages and 2,241 narrative words**. The 9–11 minute reading estimate is derived from that word count; it is not measured player evidence.

## Editor playthrough performed

Completed the route with a fern, both morning routines, civic ecology mentor, study + rest, asking the lecturer for help, sharing the difficult morning with Lola, solving both garden stages, and choosing tea. The final screen showed **Story Complete — Garden Restored**, preparation and rest retained, and final Pressure **13 / Clarity**.

Observed checks:

- Only the residence was available on the first morning; the class unlocked afterward.
- Study consumed one slot and increased Pressure from 45 to 57.
- Rest consumed the second slot and reduced Pressure to 40. No third activity was available.
- Save & Title, followed by Continue, restored the evening with both flags intact.
- Sleep advanced to day two and reduced Pressure to 31.
- Space advanced narrative pages. Tab selected the mentor choice and Enter activated it.
- Larger text and reduced motion persisted across editor Play Mode restarts.
- Incorrect garden testing displayed useful feedback without ending the story.
- Allocation never exceeded six measures; the second light condition required redistribution.
- The hint displayed the second condition's requirements.
- Quit during the second puzzle stage exited Play Mode. Restart/Continue restored stage two and the 1/2/3 allocations before redistribution to 2/3/1.
- The final map led to the explicit completion screen through keyboard selection.
- The completion-screen text scan reported **zero overflow issues**.
- Switching away from the game showed its pause menu and preserved progress.

## Standalone Mac playthrough performed

Continued a native-app save at the first mentor choice. Chose circulation, spent an afternoon with Jules, ended the day with one unused slot, and took the second-day route without study or rest. The classroom reflected Jules's support. Asked Lola about her project, left the garden unfinished, and accepted the invitation to return. The result showed **Story Complete — Work in Progress**, preparation/rest absent, Jules present, and final Pressure **38 / Steady**. This complements the completed editor route; it is not a claim of a fresh-start native run.

Expanded the app window and checked the ending, scrollable credits, and full dependency license viewer, including navigation between license parts and back to the title. Native Command-Q produced normal shutdown output. No gameplay exception or crash was observed. The player log included a nonfatal ComputeBuffer disposal warning at shutdown.

## Build and packaging

The universal Mac build succeeded with **zero errors and two warnings**. Its executable contains Intel x86_64 and Apple Silicon arm64 architectures, with macOS 12.0 as the minimum system version. Only Apple Silicon execution was tested here.

The warnings concern a Unity Services HTTP 403 during symbol upload and pending editor/import changes when building. The newly added license viewer was subsequently exercised in the native player, confirming that the relevant runtime update was included. The game does not require Unity cloud services.

The release copy was signed again with an ad hoc signature after strict verification detected an invalid inherited signature in the existing Resonance Audio plugin. Deep, strict signature verification then passed. The signed release copy subsequently opened successfully at the title screen. This is a local development build, not an Apple-notarized distribution. Download and first launch on a separate Mac remain untested.

Unity refreshed project/rendering settings and the TextMesh Pro fallback glyph cache during the build. Those generated changes are excluded from the First Weather source commit and remain local for review. The repository README and change log describe the published source update.

## Limitations and remaining checks

The original SampleScene and original scripts are unchanged. A complete editor playthrough reached the ending without an observed slice exception or crash. This does not guarantee absence of defects on other hardware.

Illustrations are static. Room choice affects writing/journal state rather than the depicted furniture. Destination markers communicate map availability. The garden is an introductory allocation exercise with printed targets. Audio is a synthesized prototype score and ambience, without voice acting. Screen-reader support has not been implemented; keyboard input and visual readability are the current accessibility measures.

The editor log also contains pre-existing Unity Services HTTP 403 messages and a licensing handshake warning from editor startup. They did not block the local playthrough. The slice does not depend on Unity cloud services for gameplay or saving.

Human usability, actual completion times, perceived emotional tone, volume preferences and distribution from a separate machine still require participant testing. No three-playtest requirement is claimed complete here.
