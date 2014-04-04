RoverScience
============

**UPDATED RELEASE 1.0**

RoverScience is a KSP plugin that attempts to add more interactive functionality to the science system FOR rovers. The rover can be either manned, or unmanned (the former will be revised eventually).

Firstly, RoverScience will not function unless the Rover Terminal is opened. This is done through the right-click menu of the Rover Brain part.

To do science with a rover, you must have a vessel with at least one wheel in contact with the ground. Wherever you land first with your wheels will establish a **landing spot**. Science is analyzed from **science spots**. To find a science spot you simply have to drive around; however, the farther you are from the landing spot the higher the chance of finding a **science spot**. Once a science spot has been found, the terminal will show the distance and bearing to the spot; heading and rel. heading will be shown as well. Simply drive to the spot (within 3m) and information will be shown about the spot.

Spots are given a random "potential science" where there is a:

- 2% chance of generating a base value of (400-500) science.
- 8% chance (200-400)
- 65% chance (70-200)
- 70% chance (30-70)
- else: (0 - 30)

Your rover will give out predictions on what the science spot potential may be - however this is subject to a chance accuracy, where the it basically rolls a chance percentage and, if successful, shows you the correct prediction. Otherwise it'll simply show you any other potential as a random mislead. It becomes somewhat important to improve the accuracy, which can be done through upgrades in the upgrade menu.

Science is rolled from top to bottom so there is always a chance to generate the higher science values (albeit very low). A scalar modifier is applied for whichever body you are on. Furthermore the maximum science extractable from a body also differs from body to body. Logically, Kerbin's values should be very low.

The farther your science spot spawns, the more science it'll have (increased by a factor determined by its distance). If the spot is a low potential, and very far, it will still gain a distance boost, but obviously +50% of 10 science will only be 15, so be wary of driving out far for horrible spots.

You can specify what distance maximum you'd like to scan for science spots on the console. You'll be restricted by a locked maximum distance which you can only increase through the upgrade menu.

Biomes are not yet implemented yet (I believe so, but only because it hasn't been tested yet).


## NOTES
#### HEADING
Heading is calculated from wherever the ship reference is pointing forward. Which means if you use a pod or most of the probe cores they will point straight up to the sky - **the displayed heading will WILDLY fluctuate in this case**. There are then TWO solutions: you can either use the stock chair part, or position Rover Brain like in the screenshot below. Rover Brain has CommandModule attached to it, which means you can use its "control from here" right-click button to reorient the ship's attitude to a horizontal reference.

Make sure the pointy bit POINTS FORWARD! **As long as it points forward, you can place it anywhere on your rover.**
![image](http://i.imgur.com/Jr0Unyb.png)
![image](http://i.imgur.com/dPSQmY7.png)



#### TURN ON THE TERMINAL
RoverScience is designed not to function unless the rover terminal is up. You can't really do anything without the terminal up, and it seemed more robust to just tie the terminal to RoverScience activity than to add a separate activate/shutdown button.

![image](http://i.imgur.com/tup2z9z.png)



#### CAREFUL WHEN ANALYZING!
When over a science spot you are shown the science potential of the spot, not the actual science value. This is to add a decision-making aspect to choosing whether or not to analyze a spot, as you can either get a great value or a low value, but it's up to you to make the decision to take the chance.

**Each analysis you commit to will slowly degrade a rover's return of science. For the first 3 analyses you'll get normal values, and anything past that will slowly degrade the amount of science you get back. Soak up those high potentials as much as possible!**

ALSO be CAREFUL - take care not to accidentally "delete" the actual science data itself after analyzing as you'll be there without any science data and with a 30 day cooldown.


#### THE MENUS
![image](http://i.imgur.com/NfbzPL1.png)

Everything is pretty straight forward.
