# Bakusou Zombie Source Code
 Source Code of Bakusou Zombie

Bakusou Zombie is hosted using Photon Pun 2, a multiplayer engine with its own CSCS. For the Project gameplay, I programmed the following:
- The main spawning system of the game where players can spawn and get synchronized over the network.
- Damage, Death, and respawn system.
- Match Manager that manages the overall functionality of the game, such as Game States, Timer, Timed Events, and Player in-game information.
- Character Movement system. Integrated Unity Starter Assets with Photon so it works over the network.
- Player health, Mana & Stamina functionality.
- Main Weapon System & Bullet Behaviors.
- Weapon & Health Pick Up.
- Jump Pads.

Main Menu System
- Allows players access to find rooms, create a room, the Setting Menu, view Credits, and Quit the Game.
- Includes Error screen just in-case if Photon Related error occurs. For example, each room must have a unique name, if a player attempt to create a room with the same name, the error screen will display.
- While in a room a Chat system is available for players to communicate with each other. Players can also hide the chat by pressing the TAB button.
- The Setting Menu allows players to change the current game's resolution, V-Sync, mouse sensitivity, and adjust the music volume.
- Status bar on the bottom to show player connection status