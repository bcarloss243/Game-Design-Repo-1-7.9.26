# Living academy forecourt

September 9, 2026. First exterior study for the main academy, under Bergen Carloss's direction.

## Review in Unity

Open First Weather and press Play. For normal play, choose Continue or begin a story, then select **The Academy** on the map. The forecourt is accessible in every map phase. **Enter your classroom** appears only when class is due. **Back to district** leaves the grounds without spending a slot or changing Pressure.

For a preview that does not overwrite your story save, choose **Halcyon → 8 Preview Living Academy (Play Mode)**. This starts at the first class milestone. **Halcyon → 9 Verify Living Academy (Play Mode)** runs the academy checks and returns to that preview. Stop and restart Play Mode to leave preview mode.

## Controls and behavior

- **Entrance / Clocktower / Overview** move between framed views.
- **+ / −** buttons or keys zoom. **Home** restores the overview; arrow keys pan.
- Two-finger scrolling pans. Command/Ctrl + scrolling zooms around the pointer. Native pinch gestures are not implemented.
- A mouse-drag handler is included. UI automation did not produce a confirmed horizontal drag during this session; physical drag and trackpad comfort remain explicit user checks.
- The barometer and controls stay fixed while the scenery moves. The camera remains inside the scene bounds.
- Looking around leaves the underlying map/story state unchanged. Ambient clock movement is decorative and does not advance the timetable. Save/continue returns to the district overview.
- Pause and reduced motion freeze the ambient clock, students, banners, light modulation, leaves and water reflections. Reduced motion also removes camera easing and depth-plane offsets while retaining navigation.

## Presentation

The architecture, background setting and near reflecting channel are distinct layers. The facade retains its engraved illustration, with live clock hands, window/lantern light and banners attached to its coordinates. Eight small student figures walk separate forecourt/stair routes with moving limbs and endpoint fades. Water highlights and airborne leaves move independently. Forecourt ambience uses the prototype's existing synthesized water/hum at a different mix level.

This is 2.5D: an illustrated building rendered with its own sprite geometry and live surrounding objects. There is no free camera orbit or fully modeled architecture. Student figures are small procedural silhouettes rather than finished character animations. Dorm and classroom scenes, the city overview, and dialogue remain at their previous state.

## Artwork and reproducibility

The two new illustrations were generated with the built-in OpenAI image-generation tool on September 8, using the existing map and supplied Halcyon cover as references. Final selected prompts are in **AcademyArtPrompts.md**. Assets are in **Resources/HalcyonAcademy/AcademyBuilding.png** and **AcademyForecourt.png**.

The building exports contained painted checkerboards rather than an alpha channel, including a background-removal retry. The chosen first illustration remains unmodified. **AcademyFacadeMesh.json** records the sprite's horizontal geometry strips so Unity draws the architecture and excludes its surrounding bitmap background. **Tools/Halcyon/trace_facade_mesh.py** reproduces that geometry by reading the source pixels; it requires Python with Pillow and NumPy and does not rewrite the PNG. The game loads the precomputed geometry and does not require Python.

## Verification

- Compiled and exercised in Unity **6000.3.14f1** on Apple M3.
- **555 academy checks passed with zero issues**, including **406 text measurements**. Covered seven map phases, two text settings, class availability and entry, state preservation, preview save isolation, camera bounds, cursor-anchored zoom, geometry integrity, asset loading, student movement, pause, reduced motion and invalid/long frame times. See **AcademyVerification.txt**.
- The existing story/rules verifier passed **3,295 assertions**. The narrative and progression data files are unchanged.
- The first visual check exposed checkerboard edges and poor courtyard framing. The corrected sprite geometry and skyline composition were inspected in the running editor at overview, entrance and tower views.
- Observed students at different positions, a student using the entrance stairs, moving clock hands, and scroll-driven camera movement. The Entrance and Clocktower buttons kept the artwork's attached details aligned. Clicking **Enter your classroom** opened the existing first-class scene.
- Complete human playtesting, physical trackpad/drag comfort, Intel runtime performance and a full manual story replay were not performed for this update. No new standalone build or archive was made, following Bergen's requested workflow.
