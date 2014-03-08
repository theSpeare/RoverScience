RoverScience
============

RoverScience is a KSP plugin that attempts to add more interactive functionality to the science system FOR rovers. The rover can be either manned, or unmanned (the former will be revised eventually).


Firstly, RoverScience will not function unless the Rover Terminal is opened. This is done through the right-click menu of the Rover Brain part.

To do science with a rover, you must have a vessel with at least one wheel in contact with the ground. Wherever you land first with your wheels will establish a **landing spot**. Science is analyzed from **science spots**. To find a science spot you simply have to drive around; however, the farther you are from the landing spot the higher the chance of finding a **science spot**. Once a science spot has been found, the terminal will show the distance and bearing to the spot; heading and rel. heading will be shown as well. Simply drive to the spot (within 3m) and information will be shown about the spot.

Spots are given a random "potential science" where there is a:

- 2% chance of generating a base value of (400-700) science.
- 19% chance (150-400)
- 45% chance (70-150)
- 70% chance (20-70)
- else: (0 - 10)

Science is rolled from top to bottom so there is always a chance to generate the higher science values (albeit very low). A scalar modifier is applied for whichever body you are on. Furthermore the maximum science extractable from a body also differs from body to body. Logically, Kerbin's values should be very low.

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

**As of pre-release 2, there is no longer a delay between analyses. However, each analysis will decrease the amount of science returned. The science should be normal from analyses 0 to 3, but after that the science will begin to get lower and lower. Make sure you take those high potentials as soon as possible!**

ALSO be CAREFUL - take care not to accidentally "delete" the actual science data itself after analyzing as you'll be there without any science data and with a 30 day cooldown.
