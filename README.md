# The assassin pegion project

So far i made a rough handling system, which should be extensible enough to fit our needs.

## Controls

You can press P to start moving, dont have to hold it.

Press control for freecam. This will hold your current flight control inputs, so you can get those sexy orbit shots with no problems.

Right mouse movement is roll, and vertical is, well, pitch.

No other keyboard inputs yet, but will probably add yaw, via AD, and speed control via WS. 

Wing flapping coming soon, and alongside it, velocity redirection so we can turn properly. 

Right now there is no gravity applied to le colombe, but it's orange and it's not really a dove yet, so it tracks.

## Visual indicators

The blue indicator is pointing north at all times, and does not follow any rotation, so it can be used to see where and how you're going. Will probably add some way to see it when it's behind die Taube. 

In the bottom right there's a speed indicator graph. The left most part of it shows you acceleration ( rough, and frankly a bug, but we can call it a feature )

You will see a red line that shows you the trajectory of the pëllumbi for the past 2 minutes. You can do loops and try to go through them later i guess.

## Known problems

Right now, it's annoying to constantly move the mouse in the direction you want to go, which will need to be fixed, but that comes along PID tuning.

The rotation numbers go haywire when the pigeon pitches more than 90 degrees, which will need to be locked in the future ( some code is there to fix it, but it's not finished yet ) 

The PID's can accumulate too much error at this point, and result in really high values, which really spins stuff around. It is only temporary, but will have to be fixed in the future. 

## PS

Idk, play around with values for the controls power, speed and whatnot, found at 
- Flight rig / Logic Rig / Movement Controller #1 / ( C ) Flight Core / ( V ) Pr Velocity ratio in characteristics
- Tech Rig / ( C ) Impact Wrench / ( V ) STR_push 

> No annotation means game object
> C is for component 
> V is for variable

Use ```git reset --hard``` to reset everything if you reach a situation that seems unmanageable. 

> you may encounter towers in your journey, which do not have collision. You also may fly through them at your leisure. Don't forget to drop your passport before tho

