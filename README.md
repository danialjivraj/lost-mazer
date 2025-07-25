# lost-mazer

<img src="https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/BenAndJerryCartoon.png"
     alt="Lost Mazer Ben and Jerry Cartoon"
     width="300"
     align="right" />

**Lost Mazer** is a first-person Halloween-themed 3D horror game built in Unity where players must navigate a haunted maze, uncover a chilling mystery, and escape from a terrifying stalker.<br>
The game contains a **multiplayer** minigame mode, where you can battle your friend and get points for taking them down!<br>
Settings and levels unlocked are saved using **PlayerPrefs**, and high scores are managed through **Backendless**.<br>
You can save your progress during a game and reload it to the same point where you left off thanks to **JSON serialization**.

### Story and Objective
Step into the shadows as Ben, who mysteriously wakes up trapped in a haunted maze with no memory of how and why he got there.<br>
You'll have to find a way out of the maze with your fellow *Jerry the Lantern*! Along your exploration, notes left by *someone* will be found scattered across both mazes, detailing insights and clues of the sinister place you're in, making Ben stir disturbing memories as he begins to question everything he once thought he knew.<br>

This game features two unique maze maps. Your objective in each is to find a hidden key, that unlocks a door to proceed to the next level. This won't be easy as any sort of noise you make can alert *The Red Man*!<br>
You'll have to walk carefully to avoid getting caught, and remember your path so you don't get lost inside the maze!

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
<details open>
<summary>Gameplay Video Previews</summary>

https://github.com/user-attachments/assets/e99ebb62-61b4-4bb0-b09a-5f1bd8b54e68

https://github.com/user-attachments/assets/c93d9339-24f2-4c21-8c8a-68a1309e699b

https://github.com/user-attachments/assets/3b9e577c-9a48-41f7-a03f-9c49bfca0c07

https://github.com/user-attachments/assets/b307ab76-0fd8-44d9-bc93-d3e2469e7d2b

</details>

<details>
<summary>Gameplay Image Previews</summary>

![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Gameplay1.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Gameplay2.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Gameplay3.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Gameplay4.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Gameplay5.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Gameplay6.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Gameplay7.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Gameplay8.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Gameplay9.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Gameplay10.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Gameplay11.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Gameplay12.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Gameplay13.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Gameplay14.png)
</details>

<details open>
<summary>Multiplayer Video Preview</summary>

https://github.com/user-attachments/assets/90773ad6-bf0f-40bb-aee7-baa8cdf92d47

</details>

<details>
<summary>Multiplayer Image Previews</summary>

![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Multiplayer1.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Multiplayer2.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Multiplayer3.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Multiplayer4.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Multiplayer5.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Multiplayer6.jpg)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Multiplayer7.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Multiplayer8.jpg)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Multiplayer9.jpg)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Multiplayer10.png)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Multiplayer11.jpg)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Multiplayer12.jpg)
![image](https://raw.githubusercontent.com/danialjivraj/lost-mazer/main/githubPreviews/Multiplayer13.jpg)

</details>

## Controls
- **Cutscenes**
  - Tab: Skip entire cutscene

- **Gameplay**
  - **Movement:**
    - W: Move Forward
    - A: Move Left
    - S: Move Backwards
    - D: Move Right
    - Q: Peek Left
    - E: Peek Right
    - Space: Jump
    - Ctrl: Crouch
    - Shift: Run

  - **Other:**
    - X: Toggle Lantern
    - Left Click: Interact
    - Right Click: Zoom In

  - Esc: Pause/Unpause Game

## Download

You can download the game by clicking one of the following links:

#### For Windows
- [Download (701.75MB)](https://www.mediafire.com/file/65j4vsn8cx2zaha/LostMazer-win.zip/file)

#### For macOS
- [Download (718.36MB)](https://www.mediafire.com/file/4uf57j8x5ja0dxl/LostMazer-mac.zip/file)

#### For Linux
- [Download (711.46MB)](https://www.mediafire.com/file/5ajypetiugdw9j7/LostMazer-linux.zip/file)

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

To enable multiplayer, you'll need a [Photon](https://www.photonengine.com/) account first and obtain an App ID:

1. Go to [Photon Dashboard](https://dashboard.photonengine.com/).

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
