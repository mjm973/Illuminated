  # Illuminated Project proposal

- **Project Name:** *Illuminated*
- **Team Member:** Mateo, Ming and Shantanu
- **Check our:** [Technical Specifications](https://github.com/mjm973/Illuminated/blob/master/technical_specifications.md)


## GENERAL OVERVIEW

### Okay, so: what __is__ *Illuminated*?

*Illuminated* is an adventurous and thrilling first person shooter experience. It inverts your expectations on usual shooter games: instead of seeing and shooting the opponents, you have to illuminate the environment to uncover the position of the other players before taking the killing shot.

Playing with the concept of lighting and visibility, *Illuminated* merges intense action and stealth game play in a novel way. The environment itself is visible but you cannot fully see it. While another person might be present, you cannot see him or her without casting a light either. *Illuminated* allows you to see part of the environment layout but that’s not the whole picture - pun intended.


###  Why do you use Virtual Reality as the medium?

This project is uniquely suited to VR, as a player’s concept of spatiality is different on a 2D screen than viewing it in real surroundings. VR enables a more intuitive and kinesthetic mode of exploration. Creating a procedural experience, we plan to modify the fundamental ways physics and reality work and we need the immersion of VR to make the experience believable yet refreshing to the player.

Social experience is at the core of *Illuminated* for a few reasons. First, it is meant to be a competitive experience, so having at least two people engaging in the space is crucial. The main interactions arise between players in conflict, rather than between a player and their environment. Second, while one can explore the environment by oneself, the act of revealing and finding a second player is also central to the game, as it becomes a chase between actors with incomplete information. The tension builds slowly, spiking when another player is finally revealed, before they dart back into the invisible, and the search is on once more. 

> “The thrill is in the chase, never in the capture” - Doctor Who


### What kind of experience or emotion do I get playing *Illuminated*?

By flipping one of physical reality’s core rules around, we are forcing the player(s) into an experience where they must re-learn how to interact with the world under the tension of being constantly hunted. We aim to encourage competitive play where the winner is the last player standing. In order to encourage active play, we will penalize players who spend too long wondering in one place or not initializing any illumination.

Our space is designed to reflect a relatable environment. It withholds its characters unless lit up by the characters. We want to ease the players into familiarity with *Illuminated* rules slowly: the more they explore what seems to be an abstract world, the more they understand the world better and come out on top.


### What is somethig unique or unusual about *Illuminated*?

The core concept of the experience is that we hide players’ existences at first but they will have to reveal themselves when exploring the environment or finding others. In this way, the weapon also becomes a weakness: shooting or lighting up others exposes players themselves too.

The concept of exposing both yourself and your opponents provides a blend of action and stealth gameplay. Often, action and stealth mechanics in games do not cooperate, but coexist. Sneak around unless you’re spotted and then gun down your enemies. Or defeat everyone in sight so that you may slip back into the shadows. One mechanic is employed until it outlives its usefulness, then gives way to the other. *Illuminated* challenges this relationship by combining its action and stealth elements into a single mechanic. Attacking and hiding are intertwined within the act of shooting: the attack *is* the stealth.


## DESIGN OVERVIEW
### Fictional Context

“Congratulations, you’ve been selected. Run. Learn. Survive. And stay out of the light.”

Once the a player lands in this world, she will realize that she has been thrown into a far future tournament arena. For couple seconds, the arena is lit up and she will see lamps and shadows of other players somewhere close or afar. The next second, the lights are shut. She will have to start the exploration and the hunting.

The layout of the space is designed to emphasize careful but bold gameplay. By giving the players one central “landmark” to orient themselves around, and having features on the walls that are only revealed by illuminating them, we encourage users to create their own cognitive map to understand this world through. By designing the space in such a way that it is possible to understand it purely as a series of edges, with no node connecting too many edges at once, we also provide balance in competitive gameplay. This works because in an area designed this way, the player is never in a situation that feels unfair: even though there are areas with multiple entrances and exits, a careful player will be able to make sure they only ever engage one player at a time. Simultaneously, a risk-loving player will be able to play a more aggressive campaign, which invariably also opens up that player to attack. All players are always vulnerable to being attacked, but a careful player will never find themselves in a situation they’re unable to defend themselves in.

### Core Interactions

The core interaction revolves around shooting and illuminating the world around you. The two acts are inextricably linked: the act of shooting is bound to the act of illuminating, as each weapon has both its own shooting and illuminating mechanics. You don’t have a weapon and a light: your weapon is the light. When placed in an environment that is featureless unless illuminated, along with other players that are out to hunt you, these linked mechanics mean your strengths and weaknesses are bound to a single action. You need to shoot to hunt the other players, and you need to shoot to light up the space and reveal their positions, but that same light can also expose you and leave you vulnerable. Given that player are individual actors avoiding one another, the act of searching the space with your light will always be a tense and rewarding interaction, for at any moment you can find your next target - or they can find you.

### Visual Aesthetic

We will create a low-poly surreal art style, to feed into the emotions of unease and unfamiliarity we wish to evoke in the players. We will use Blender and 3ds Max to make 3D models and texture shaders for lighting mechanisms. 

![moodboard](https://github.com/mjm973/Illuminated/blob/master/illuminated_moodboard.png)

### Sonic Environment

Our soundscape is calm, atmospheric, and not constant or overwhelming, but rather eventful and event-based. Music and ambient sounds fit the stark and surreal aesthetic of the space. Sound effects should be short and linked to specific key event. Sound effects triggered by player(s)’ actions, like projectile collisions, grenade explosions, being illuminated etc. We want to have a more ‘discrete’ feel to them than the music, featuring a sharp attack and a quick decay that lasts only as long as the event they are tied to.

### The Interface

*Illuminated*’s interface lives within the fiction. Ammunition and state of the players’ weapons is shown as displays hovering on their surface, while a player’s health will be displayed in a bracelet on the players’ arms. The circumference of the bracelet has lights all along it - the number of lights active represents the player’s remaining health.
The players join the room and spawn into a room with the game title on a wall, a light switch and a single light bulb illuminating the room in the game’s style. A sign above the lightswitch indicates that this is the switch to join the lobby. Once the player flicks this switch, the lights go out and the player spawns into the game world.

## EXPERIENCE OVERVIEW

### Overall Experience
When players put on the headset, they will enter a room as an interface to join the lobby. From there, the UI will allow them to connect to the network, and they will be transported to the arena. Upon arrival, players see two things: a lamppost, illuminating them, and an identical spawn point opposite to them with another player. After a few seconds, the lights go out, and players see their opponents disappear. From there, it’s up to the players to teleport around, shoot their weapons, and illuminate the arena around them. They hunt each other while avoiding the light until there is only one person standing. At that point, the entire area is fully illuminated, and the player can see both the space they inhabited and the remains of their opponents. They have won, but at what cost?

### Specific Moment

Picture the beginning of the match. The lights have just gone out, and the short glimpse you had of the other players is now gone. You start moving around, as you realize your opponents could have gone basically anywhere. You look around, look at your weapon, and look around again. You fire, only to realize your weapon produces light: the details in the arena around you become visible, but so do you. Before you know it, you see a projectile coming your way - someone has seen you. You teleport out of the way, trying to outsmart your foe and wondering where they might have gone next. You fire, and - Aha! Your guess was right: you see your opponent right where you wanted them. Before they realize what’s going on, you make your attack. Your shot connects, but this isn’t over. You teleport out of the light into the safety of your featureless environment, ready to plan your next assault.

## CONCLUSION

In conclusion, *Illuminated* features unexplored and exciting gameplay mechanics that blend and merge in a unique way. It challenges player expectations and rewards active gameplay, while still providing an experience with satisfying mechanics that is interesting and fun for less involved players to enjoy. The overarching narrative gives a tense, exciting context to its action and helps contextualize the action. *Illuminated* uses VR hardware to its fullest extent, providing not only a more immersive experience, but one that relies heavily on the spatial versatility of VR. 
