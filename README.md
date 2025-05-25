# lost-mazer

<img src="https://github.com/user-attachments/assets/d374b283-8f34-49c2-9672-b4090dcbbb84"
     alt="Lost Mazer preview"
     width="300"
     align="right" />

**Lost Mazer** is a halloween-themed 3D horror game built in Unity where players must navigate a haunted maze, uncover a chilling mystery, and escape from a terrifying stalker.

### Story and Objective

Step into the shadows as Ben, who wakes up trapped inside a haunted maze with no memory of how or why he got there. As he searches for a way out with his fellow *Jerry the Lantern*, notes scattered throughout the maze provide cryptic insights into the sinister nature of the place, and begin to stir disturbing memories, making Ben question everything he thought he knew.

The game features two unique maze maps. Your objective in each is to find a hidden key that unlocks the door to the next level. But it won’t be easy, as movement makes noise, and *The Red Man* is always listening!<br>
Walk carefully, remember the route you take, and stay hidden to avoid being caught. As you progress to the next level, new challenges will test your observation and memory.

The game also contains a Multiplayer minigame mode, where you can battle your friend and get points for taking them down!

Progress and settings are saved using **PlayerPrefs**, and high scores are managed through **Backendless**.<br>
You can also save your progress during a game and reload it back to where you were.

## Requirements
| Category                          | Criteria                                                                                                               |
|:----------------------------------|:-----------------------------------------------------------------------------------------------------------------------|
| Menu and in-game User Interface   | Meaningful menus<br>Graphical design of the UI<br>HUD<br>&nbsp;&nbsp;Basic<br>&nbsp;&nbsp;Advanced, e.g. map           |
| Game levels                       | Complexity of scenes<br>Originality and novelty of assets                                                              |
| Non-player animated character     | Transitions between multiple animations<br>Use of animation layers<br>Nav mesh and waypoints<br>Advanced NPC behaviour |
| Save and reload the game state    | Save state of game object to file<br>Reloading of game and game objects from file                                      |
| Keep and save user score          | Store score e.g. using PlayerPrefs<br>Backendless for storing the score                                                |
| General                           | Overall gameplay experience<br>Presentation and attention to detail<br>Other points of note                            |
| Bonus                             | Multi-player (PUN)<br>Ragdoll<br>Cut scenes<br>Simple audio triggers                                                   |

## Preview
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/1.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/2.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/3.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/4.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/5.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/6.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/7.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/8.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/9.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/10.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/11.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/12.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/13.png)

## Controls
- **Cutscenes**
  - Tab: Skip entire cutscene

- **Gameplay**
  - **Movement:**
    - W: Move Forward
    - A: Move Left
    - S: Move Backwards
    - D: Move Right
    - Q: Peak Left
    - E: Peak Right
    - SPACE: Jump
    - CTRL: Crouch
    - Shift: Run

  - **Other:**
    - X: Toggle Lantern
    - Left Click: Interact
    - Right Click: Zoom In

  - Esc: Pause/Unpause Game

## Download

You can download the game by clicking one of the following links:

#### For Windows
- [Download (408.07MB)](https://www.google.com)

#### For macOS
- [Download (399.81MB)](https://www.google.com)

#### For Linux
- [Download (403.46MB)](https://www.google.com)

## Run the Game Locally
Alternatively to downloading the game, you can clone the project and run it locally. <br>
### Clone Project
1. To clone the project, run the following in Git: 
```
git clone https://github.com/danialjivraj/lost-mazer
```
2. Once finished, open your Unity Hub, press `Add` -> `Add project from disk`, then select the cloned project folder.<br>
3. After adding, click on the project to open it in Unity. It may take a couple minutes to open it for the first time.

**Note:** The game has to be cloned. Do **not** download it as a ZIP as it will not fully download everything due to LFS.

### Multiplayer Setup

To enable multiplayer, you'll need a Photon account and obtain an App ID:

1. Go to [Photon Dashboard](https://dashboard.photonengine.com/). You need to log in or create an account first.

2. Click `Create a New App`.
   - Application Type: `Multiplayer Game`
   - Photon SDK: `Realtime`
   - App Name: (Any name you like, e.g., `Lost Mazer`)

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Press `Create`. You will then be given an `App ID` — copy this ID.<br>

3. Paste the `App ID` into the appropriate field inside the Unity project (through the [PUN Wizard](https://doc.photonengine.com/pun/current/getting-started/initial-setup) pop up or under `PhotonServerSettings` if the popup does not appear).

### Backendless Setup
1. In order to see the highscores, you will need to have an account in [Backendless](https://backendless.com/).

2. After creating and logging in, you will be prompted to `Create New App`. You can select `Blank New App` and name your application to anything you want.

3. Go to `Database` found on the sidebar and add a new table by pressing the `+` icon, name the table `HighScores` and press `Create`.

4. With the table now created, go to `Schema` and `Table Editor`. Press `New` and create the following:
   - Name: `playerId`, Type: `STRING`
   - Name: `level`, Type: `INT`, Default Value: `1`
   - Name: `score`, Type: `INT`, Default Value: `0`
   
5. In the code, go to `Assets/_Scripts/BackendlessConfig.cs`, where you will see the following:
```
    public const string BASE_URL = "https://";
    public const string APP_ID = "";
    public const string REST_API_KEY = "";
```
You'll need to paste your APIs from Backendless. You can do this by going to `Manage` in the sidebar, where you will see the following:<br>
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Backendless.png)

Copy the `Backendless subdomain` and paste it inside the quotes after `https://` for `BASE_URL`. <br>
Copy the `Application ID` and paste it inside the quotes for `APP_ID`. <br>
Copy the `REST API key` and paste it inside the quotes for `REST_API_KEY`. <br>

## Credits
[CREDITS.md](https://github.com/danialjivraj/lost-mazer/blob/main/CREDITS.md)

## Disclaimer
[DISCLAIMER.md](https://github.com/danialjivraj/lost-mazer/blob/main/DISCLAIMER.md)
