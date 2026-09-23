// FIRST WEATHER — working narrative draft for Bergen Carloss.
// One paragraph = one screen. Keep the same number of paragraphs in conditional branches.
// Choice text goes in brackets. effect tags connect decisions to the simulation.
// Speaker, portrait and event tags are optional. See WRITING-GUIDE.md.
VAR pressure = 54
VAR zone = 2
VAR medicated = false
VAR rebound = false
VAR studied = false
VAR rested = false
VAR socialized = false
VAR shared = false
VAR solved = false
VAR assisted = false
VAR panic = false
VAR recovery = false
VAR academic_warning = false
VAR first_clarity = false
VAR tolerance = 0
VAR mentor = ""
VAR decor = ""
VAR response = ""
VAR invitation = ""
VAR shortage = ""
VAR called_home = false
VAR messaged_jules = false
VAR day = 1
-> END

=== morning1 ===
The desk faces away from the window. Quietest wing, Father said. A Vapeur dispenser fits the nightstand so neatly that the carpenter must have had its measurements before he had Molly's. She tries the chair. Exactly her height. # speaker:Molly Duvernay # portrait:molly
Outside, a boat leaves a mint-colored seam in the canal. Someone on the landing rehearses their introduction. "Molly Duvernay," she tries, quietly. Too formal. Just Molly, then. She puts her father's photograph face down while she unpacks, then stands it up again. # speaker:Molly # portrait:molly
There is room on the sill for one thing she packed herself. # speaker:The windowsill
* [A fern cutting from home # effect:decor:fern]
* [A small brass boat # effect:decor:boat]
* [A postcard of the old city # effect:decor:postcard]
-
{decor == "fern":The cutting leans toward the canal. She turns it back. It leans again.|{decor == "boat":The brass boat lists on its stand. She props it with an academy envelope.|The postcard shows a street before it became a canal. New Orleans, the back says. Her grandmother's handwriting has almost rubbed away.}} # speaker:Molly
"The first lift is always the worst," Alaric says in the message. "Your grandmother said that about boats. You needn't give a speech. Marchand knows which table to put you at." A pause. "And eat breakfast, please. Leadership can wait until after toast." # speaker:Alaric · dispatch # portrait:alaric
The Vapeur case is older than Molly. Beneath the clasp, an engraving has worn into a shallow groove. She knows where to press without looking. The hum is already there, behind the cupboard latch, under the water in the pipes. # speaker:Molly # portrait:molly
* [Take the Vapeur # effect:routine]
* [Sit with the hum # effect:hum]
* [Leave for class # effect:skip]
-
{medicated:The latch clicks. The room becomes easier to sort: cup, books, bag. She eats while the toast is still warm.|{tolerance > 0:She sits. At first there is only the cupboard's thin rattling. Then another note, lower, nowhere near the cupboard. She grips the chair until the bell ends.|She closes the case. At the door she goes back for her bag, which is already on her shoulder.}} # speaker:Molly # portrait:molly
On the landing, the student has settled on "Hello." Molly holds the door for them. # speaker:The first morning
-> END

=== class1 ===
Twenty-four places. On the brass plaque: HALCYON ACADEMY, founded by Alaric Duvernay. Beneath it, seventy years of the Cloche's shelter over the flooded Mississippi basin, workshops named in careful rows. Molly finds her father's name before she finds her own seat. # speaker:The first cohort
Someone is practicing scales upstairs. At home in the Forge, every sound had a job. There was never music. Here, at the meeting of the Archives, Vieux Carré and Tremé, a trumpet keeps trying the same troublesome interval. # speaker:Molly # portrait:molly
The student beside the growing table has rolled both sleeves past the elbow. A flowering balcony is taking over her plumbing diagram. When she lifts her pencil, a speck of soil falls onto a very precise measurement. Molly looks away before she is caught looking. # speaker:Molly
Marchand has set out the Ironwork pump. Thibodaux's Greenwork table smells of damp earth. Molly's registration card reads: "Per parental recommendation, this concentration is not available as a primary track." A smaller line allows a minor. # speaker:Track registration
* [Marchand · Ironwork # effect:mentor:circulation]
* [Thibodaux · Greenwork minor # effect:mentor:ecology]
-
{mentor == "circulation":"The six measures are all you have," Marchand says. "Tell me what still works when a pipe fails. The kitchen needs a harvest before we can plan another season." He leaves her first attempt beside the clean paper. "Keep that one. It's how we'll know what changed."|Thibodaux turns the restriction card face down, without tearing it. "A minor still gets a stool." His own Vapeur case lies beside his spectacles. "Take what helps you. We can argue about timetables after lunch. Keep a reserve for the young roots; that is how we have a garden next season."} # speaker:{mentor == "circulation":Marchand|Thibodaux}
"Beaumont's Brasswork rehearsal is open," Thibodaux adds. "Fontenot has the Bridgework lists. Delacroix's Inkwork room is upstairs." Marchand starts the pump again. The six little cups empty faster than Molly expects. # speaker:The faculty
{medicated && zone == 1:For once she can follow the whole demonstration. She spots a loose washer before the model spills. Marchand slides the screwdriver toward her.|{zone >= 3:The pipe knocks. Someone coughs. The pipe knocks again. She has missed a word she needs.|Molly draws three branches in her notebook. She wishes loving the city felt less like having to explain him.}} # speaker:Molly # portrait:molly
Tomorrow, each student will present a plan. Marchand puts a blank attendance card beside the model. Molly writes the deadline twice. The girl with the botanical diagram has drawn a bench. # speaker:The assignment
-> END

=== study ===
The reading room keeps the river outside one thick pane of glass. Molly chooses the table nearest the door. Her first drawing distributes the water equally. It looks very fair. It leaves the seedlings thirsty. # speaker:The reading room
{mentor == "circulation":She sketches a bypass under Marchand's first question: what survives a broken pipe?|She writes Thibodaux's question in the margin: who comes back to water it? Then draws a place to sit.} She keeps the untidy sheet. # speaker:Molly # portrait:molly
{medicated:By closing time the plan has labels she can follow. She checks the sums once, then packs it.|{zone >= 3:Six cups. Three branches. The hum swallows a line. She starts that line again.|The note behind the lamps has changed pitch. Molly moves her pencil away from the glass and can still feel it in her fingers.}} # speaker:Molly
-> END

=== rest ===
She loosens her collar and sets an ordinary kitchen timer. Upstairs, someone puts a record on, then changes their mind before the song has quite begun. Molly has forgotten to take off one shoe. # speaker:The residence
{medicated:The chair holds her. She wakes when the record turns over, with a crease on her cheek and the second shoe still on.|{zone >= 3:The quiet makes room for it. Pipe. Wall. Teeth. She puts both feet on the floor.|In the quiet, the low note seems to come from the canal as well as the fittings. She opens the window. It does not get louder.}} # speaker:Molly # portrait:molly
On the sill, {decor == "fern":the fern has made a new green hook|{decor == "boat":the crooked little boat has gathered a stripe of afternoon light|the postcard curls at one corner}}. She eats the toast cold. # speaker:Molly
-> END

=== social ===
A student has occupied three canal steps with a sandwich, a notebook, and a jacket that is somehow still dripping. "Jules. The canal and I had an introductory disagreement." Molly makes the mistake of laughing with her mouth full. # speaker:Canal steps
"Someone asked if I'd chosen a field. I said dry land." Jules shifts the notebook. It is a proposal for a Ward radio relay. "Brasswork, really. The Forge doesn't play anything. Beaumont says silence is a choice too." # speaker:Jules
"Duvernay? Would your father read this?" Molly says probably before deciding whether she wants to ask him. Jules brightens and begins crossing things out. She wonders which part of the last ten minutes they would have had without her surname. # speaker:Molly # portrait:molly
A boat goes under the bridge carrying a whole tree in a pot. They watch its branches negotiate the arch. Then Jules offers the other half of the sandwich and gets mustard on the proposal. # speaker:Canal steps
-> END

=== callhome ===
"There she is." Alaric has loosened his collar. Behind him, a row of fittings waits on the workbench. "I've had the west vent moved. You shouldn't feel it from the desk now." # speaker:Alaric # portrait:alaric # event:call-start
Molly says the room is beautiful. It is. He asks about Marchand. {mentor == "ecology":She tells him Thibodaux had a spare place. "For the minor," he says. "Of course."|She tells him about the bypass. He begins drawing a better one with his finger, just below the camera.} # speaker:Molly # portrait:molly
"Your grandmother used to leave every window open. Drove my father mad." He smiles. "You have her ears. Always hearing some tiny thing." Molly opens her mouth. He asks whether the dispenser arrived. # speaker:Alaric # portrait:alaric # event:call-end
She says yes. After they disconnect, she sits with the little dark reflection of her face on the screen. The room's vent makes no sound at all. # speaker:Molly
-> END

=== messagejules ===
{socialized:"Made it home dry," Jules writes. A second message corrects this to "Mostly." The radio proposal follows, with Molly's name on the cover. She asks them to take the name off until she has read it.|She finds Jules in the cohort list and types a question about the rehearsal. The answer arrives with a riverboat timetable and three exclamation marks.} # speaker:Jules · dispatch # event:message-jules
{socialized:"Fair. Sorry. Got excited." The revised cover says JULES in letters large enough for both of them. Molly saves the attachment.|"Beaumont takes Vapeur before every rehearsal. Says it lets him hear everyone else." Molly looks at the case, then at the name of the piece Jules sent.} # speaker:Evening messages
-> END

=== night ===
Windows gather along the canal. Across the water, a kitchen lifts its shutters for the last customers. Molly checks tomorrow's bag. {studied:The folded plan catches on the clasp.|There is a blank sheet where the plan should be.} # speaker:The first night
The lamps brighten together. Only for a moment. Alaric's harvest circuit catches the load; the light holds steady in the rooms all along the bank. No one at the kitchen even looks up. # speaker:The canal # panel:harvest # event:harvest-lights
{called_home:She leaves the screen facedown beside the case.|She starts a message to her father and saves it without sending.} {rested:Her shoulders have dropped.|Her shoulders ache.} The boat with the tree comes back empty. # speaker:Molly
-> END

=== morning2 ===
The room is the same temperature. Breakfast has come under a clean cloth. Molly slept, she knows she slept. The hum is waiting before she opens her eyes. # speaker:The second morning # portrait:molly
"The new relay inspection ran late," her mother writes. "Did you sleep? I put the soft scarf in the side pocket. Your grandmother used to wrap hers around the kettle so it wouldn't knock." Molly checks the side pocket. # speaker:Mum · dispatch
"Marchand has the family diagram if you need it," Alaric writes. "Don't spend the morning trying to manage without help. There is a fresh cartridge in the nightstand." # speaker:Alaric · dispatch # portrait:alaric
{socialized:Jules has sent a photograph of two seats. "Near the door. And no surname on the proposal."|A rehearsal notice from Beaumont has reached the whole cohort. His short note thanks Bayou Saint-Jean's clinic for helping him return to music.} # speaker:Dispatches
The case rests in its fitted hollow. Molly reaches for the clasp, then leaves her hand there. # speaker:Molly # portrait:molly
* [Take the Vapeur # effect:routine]
* [Sit with the hum # effect:hum]
* [Leave for class # effect:skip]
-
{medicated:The edge comes off the morning. She can finish her mother's message without rereading it. She sends a photograph of the scarf.|{rebound:She misses the clasp twice. The empty space where yesterday's steadiness was seems to have a pulse. She holds the mug in both hands.|{tolerance > 1:There are two notes again. Today they beat against each other. She waits long enough to hear where they separate.|She goes downstairs before she changes her mind. The handrail hums through her sleeve.}}} # speaker:Molly
{studied:The plan is in her bag.|The blank page is in her bag.} {rested:She recognizes the tightness in her hand and uncurls it.|She catches herself smoothing her collar twice.} # speaker:Before class
-> END

=== class2 ===
The model is already running. Marchand sets the attendance cards face down. {mentor == "ecology":Thibodaux makes room for Molly beside the growing table.|There is a screwdriver waiting at Molly's place.} Under the pump's ordinary rattle, the lower note climbs. # speaker:The second class
{studied:Her diagram survives the first question. Marchand keeps it on the table. "We'll use this for the next group."|Marchand waits for the missing diagram, then writes a revision appointment on her card. "The office copies your father. Standard arrangement."} # speaker:Marchand # event:assessment
{panic:Pipe against glass. The wrong air. Teeth together. A card turned over. Her name again. No space behind it.|{zone >= 3:The answer is somewhere on the page. She can see its first word. The rest keeps slipping behind the pump's rattle.|She answers. Then the note rises through the table, strong enough that she takes her hand away. No one else moves.}} # speaker:Molly # portrait:molly # panel:fracture
* [{zone >= 3:Outside.|Ask to step outside.} # effect:outside]
* [{zone >= 3:Help.|Ask someone to come with her.} # effect:support]
-
{response == "support":{socialized:Jules is standing before she can repeat it. They take the bag, leave the proposal, and hold the door.|{mentor == "ecology":Thibodaux puts down his pen. "I'll come." He waits for her to choose the door.|Marchand sets the cards aside and asks another student to stop the pump. He carries her bag himself.}}|{mentor == "ecology":Thibodaux moves his stool out of her way. She gets the door open on the second try.|Marchand steps away from the door. Molly keeps one hand on the wall until she reaches it.}} # speaker:The doorway
{panic:The afternoon field seminar is crossed off the card. Recovery timetable. Thibodaux writes tomorrow's reduced workload below it, then asks where she wants to wait.|{academic_warning:The revision appointment is still on her card. She folds it inward.|Marchand's note about the diagram is still on her card. She folds it inward.}} # speaker:The corridor # event:corridor
Water. Stairwell. Leaves moving behind a frosted pane. Molly reads the last door twice. It says GREENHOUSE both times. # speaker:Molly # panel:threshold
-> END

=== greenhouse ===
At the far table, someone says, with considerable feeling, "Oh, absolutely not," to a very small plant. A pirogue is tied below the side door. Its paddle has left a wet stripe all the way to the workbench. # speaker:The greenhouse # event:lola-arrival
The rolled sleeves. The balcony drawn around a pipe. Molly has seen her before. "Lola Landry," the girl says, briskly. "BioAg demonstration. Circulation resilience under variable demand." Then she notices Molly holding the wall. "Oh. Chair's behind you." # speaker:Lola # portrait:lola
Lola presses two fingers into the soil, closes her eyes, and moves a valve before the leaves shift. "This one wants more than it can hold. I know, sweetheart. So do most of us." A sketchbook lies open under her wrist: roots drawn through the supports of a house. # speaker:Lola # portrait:lola # event:lola-touch
"Sorry. I can go." Lola has turned sharply toward a message light. Molly watches her face for the polite relief. "Wait," Lola says. "Mémère's neighbor. I missed the morning boat after helping her dress. Let me answer." # speaker:Molly # portrait:molly
Lola sets down her patched bag. "There. She's eaten. I'm in trouble for fussing." Her shoulders fall. "You can stay, cher. I was cross with the clock." Molly has already rehearsed her apology twice. # speaker:Lola # portrait:lola
* [Tell her about class. # effect:share]
* [Ask what she felt in the roots. # effect:askgarden]
-
{shared:Molly gets as far as the attendance card. Lola pushes the kettle within reach, then waits. "I keep saying sorry," Molly says. "I noticed," Lola says, and catches herself. "Sorry. Now there's two of us."|"They call it good observation here," Lola says. "Even when I close my eyes. Back home we call it Rootwork." She rubs soil from her thumb. "Sometimes I practice saying their version before class."} # speaker:Lola # portrait:lola # event:lola-reciprocity
"Mémère's place has real soil under the steps. Wild water, moss right down to the rail, little lights in the grass at night. I'm trying to take a garden home that doesn't need me standing over it." Lola taps the pump. "At present I've built a very attractive dependency." # speaker:Lola # portrait:lola
{decor == "fern":"I've got a fern cutting," Molly says. Lola asks which way its new leaf bends. Molly knows the answer.|{decor == "boat":Molly sketches the crooked brass boat on the edge of the plan. Lola turns the drawing sideways. "That hull would rock less."|Molly takes out the old postcard. Lola recognizes a landing in the picture. "Mémère calls it by that name still."}} Lola moves the second stool toward the table. # speaker:Molly # portrait:molly
"Six measures," Molly says. "Yes." Lola rests a hand on the flowers. "And more wanting them than that." Their sleeves touch over the same pipe. Neither moves the stool back. # speaker:The workbench
-> END

=== aftergarden ===
{solved:The raft stays level. Lola checks the beds in the order Molly chose. "The {shortage} get the short share. We'll have to come back for them."|They leave the pump low and a note beside the raft. Lola draws an arrow toward the {shortage}. "First job tomorrow."} # speaker:Lola # portrait:lola
{mentor == "circulation":Molly draws the bypass Marchand asked for. Lola follows it with one muddy finger. "Good. Now draw where Mémère sits while she fixes it."|Molly draws a bench beside the valves. Lola adds a removable board beneath it. "Thibodaux would like the bench. Marchand would make us show the pipe."} {assisted:"I'm glad you asked," Lola says.|"You had an opinion," Lola says. Molly had not noticed until then.} # speaker:The plan
"Sometimes I hear the storm before the instruments change." Molly has said it. She looks at the pencil instead of Lola. # speaker:Molly # portrait:molly # event:disclosure
"Vapeurs don't work on me," Lola says. "Thibodaux helped me try. They work for him. For me, the roots were still loud." Her hand lies flat on the table. "There's a lot that science doesn't explain." Molly puts her hand beside it. # speaker:Lola # portrait:lola
{first_clarity:The blue catches her eye. For a moment the hum is a whole chord and the room is too sharply edged. Molly lifts her hand. "Did you—" Lola looks up.|{!medicated && tolerance >= 2:The needle rests in green. The case has been closed all day. Molly checks it once more, as if it might start again when she looks away.|{medicated:Molly can hear Lola finish a sentence without losing the middle. They work out where to put the second valve.|The needle has settled. Molly can feel the grain of the table under her palm.}}} # speaker:Molly # portrait:molly # event:settling
"Tomorrow I'm taking these seedlings down to the water. Come if you'd like." Lola says it while looking at the raft, as if leaving Molly somewhere else to look too. # speaker:Lola # portrait:lola
* [I'd like that. See you tomorrow. # effect:return]
* [Could we start with tea? # effect:tea]
-
{invitation == "tea":"Tea first," Lola says. "That was a yes, wasn't it?" Molly laughs too quickly, then manages a smaller yes. Lola puts the second cup beside the kettle.|"Tomorrow," Lola repeats. She writes Molly's name beside her own on the edge of the plan. Molly watches the whole word appear.} # speaker:Lola # portrait:lola
Molly leaves the greenhouse by the side door. {recovery:The recovery card is still in her pocket.|The class card is still in her pocket.} She turns back once to check the little landing. She knows where to tie a boat now. # speaker:Tomorrow
-> END
