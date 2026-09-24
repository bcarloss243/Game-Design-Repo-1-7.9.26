# v0.3 art and typography provenance

Creative direction: Bergen Carloss. All selected generated images were created with the built-in OpenAI image-generation tool and copied without raster manipulation. Unity performs ordinary texture UV selection, portrait masking, sprite-pose selection and layout cropping at runtime. No generative output is represented as hand-painted by a human artist; “painted” describes the visual treatment.

| Runtime asset | Source / disposition |
|---|---|
| HalcyonArtV3/cover.png | Bergen’s supplied “ChatGPT Image Sep 23, 2026, 12_49_11 PM.png”; Bergen confirmed on September 24, 2026 that she created the original drawing and developed the final illustration using ChatGPT. |
| HalcyonArtV3/ui-frame.png | exec-8fa5ed02-59cf-4bfe-9e90-2618df4ae656.png; generated indigo/gold interface material. |
| HalcyonArtV3/portraits.png | exec-8d898249-db9c-448d-9ee9-a144eaaafc0f.png; revised three-column Molly/Lola/Alaric atlas. |
| HalcyonArtV3/dorm.png | exec-cdb444ea-f688-43a2-ae8c-3bcc7fd5dc60.png; final painted room with enclosed exterior and compact academy. |
| HalcyonArtV3/map.png | exec-7425eb25-f469-46e5-9ec4-b316de6fc0ec.png; district corrected for compact academy and no top-edge sky. |
| HalcyonArtV3/academy.png | exec-379663b8-2db6-49f8-9218-e6edd26c5194.png; painted classroom. |
| HalcyonArtV3/greenhouse.png | exec-c0c8a351-9b4f-44f4-b5d1-da85c9769e5d.png; final greenhouse with opaque Cloche view. |
| HalcyonArtV3/students.png | exec-255b4ebb-db3a-4f38-ad06-b79d7c3e7796.png; native-alpha atlas, four columns/two students; simple repeating pose animation. |
| HalcyonAcademy/AcademyForecourt.png | exec-1750d422-e92f-43c7-8fbe-2dac07befaa2.png; final unified court/building perspective. |
| HalcyonAcademy/AcademyBuilding.png | exec-55ecdf1b-9884-4f95-8585-17cc087cdc8e.png; compact façade reference retained; final courtyard no longer renders this as a separate building cutout. |
| HalcyonUI/BrassBarometer | Existing generated housing from the prior approved dial pass; live scale/portrait/needle rendered in Unity. |

Prompts and iteration history: V03-ArtPrompts.md, MapCorrectionPrompt.md, UnifiedCourtPrompt.md and InteriorCorrectionPrompts.md. Earlier portraits, rooms, palace and sky-containing court variants listed in the prompt log were rejected/superseded. The original HalcyonArt directory remains historical and is not used by the v0.3 scene.

Cinzel Decorative: Natanael Gama, downloaded from the official Google Fonts repository, `ofl/cinzeldecorative`. Cormorant Garamond: Christian Thalmann and the Cormorant Project Authors, retained from the earlier interface pass. Both include their full SIL Open Font License 1.1 notices in Resources/HalcyonFonts and the in-game dependency viewer.

Audio is original synthesized prototype material. No commercial music recordings were added. For third-party engine/code/package notices see ThirdPartyNotices.txt and the title’s Credits & Provenance screen. The supplied cultural/architectural references guide fiction; these illustrations are not historical reconstructions or verified geography.
