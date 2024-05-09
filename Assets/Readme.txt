READ ME:

How to create workshop item, make map, and upload it to workshop. Make sure you have steam open before starting. Also make sure to open the "SampleScene" scene inside of the "Scenes" folder.

1. Click the play button at the top
2. Press button "Create new workshop item". When you press this, it will open it in your browser.
3. Click the play button again, to stop it from running.

You have basically just created an empty workshop item. I recommend giving the workshop item a name, you can do this in the browser page that opened.

4. Copy code that is shown in console. (It is automatically copied to your clipboard).
5. In hierarchy, go to "Steam Workshop".
6. Paste code into "Currently selected item ID".

This will ensure that you are actually updating to the right workshop item.

7. Make map, hower you'd like. You can import a model or use probuilder which is in the project by default.
(If using probuilder, make sure to export as FBX when done and put that in assetbundle instead. That is inside of the probuilder menu).
8. Put map inside of the "Map goes in here" parent gameobject (Make sure it is at 0,0,0).
9. Make that parent gameobject a prefab inside of the "AssetBundlesWanted" folder by dragging it straight into there.
10. Go to bottom of prefab in inspector, create new asset bundle and name it "map" (Or whatever you want).

This means your map is made, but you still need player spawn points, token spawn points, and any interactables you want to add.
Spawn points and token spawns, and one shop are neccessary for the game to function as intended.

11. Place interactable prefabs (computer, shop, spawn, token spawn) around the map
12. Put all of those inside of the "Everything goes in here except map" gameobject (Make sure it is at 0,0,0).
13. Make that parent gameobject a prefab inside of the same folder as earlier by dragging it in there (Same as step 9).
14. Go to bottom of prefab in inspector, create new asset bundle and name it "everything else" (Or whatever you want).

You have set up everything, now you just need to build it and send it off to steam.

15. Press 'Assets' at the top, and press "Create asset bundles".
16. Replace the "thumbnail.png" image with another png of the same name and also png (Optional).
17. Go to WorkshopHandler and put any patchnotes you want (Optional).
18. Click play button, then in game tab, press "Update workshop item"

If you have done everything right, you can test it by subscribing to your own workshop item in the browser and launching the normal game.