All rights of any files from this repository are reserved.

Unity version: 2018.2.18f1

======= Instruction =======

=== Controls ===

- Key P to train 10 000 times the main Brain (use it only for a single test).

- Mouse click to drive the car in Race mode.

- U: Go to SimpleMap.

- I: Go to AdvancedMap.

- Escape: Quit the Game.


=== Play ===

- Launch PlayMode

- Select Mode: Race / AI Training 

- Press Start.


=== Race Mode ===

Start the race with the current brain.
The brain in not trained before, you have to train it using the AI Training mode.

Each AIController use the main brain, which is currently the best one, and add a noise value.
This noise is applied to all output from the brain to make AI having different behaviours.


=== AI Training Mode  ===

Start Genetic algorithm. Generate a colony of brain, each one control a car.
The algorithm only keep 25% of the best (based on turn time), generate 50% of child and 25% of new brain.

You can increase the speed with the cursor on the bottom right, and stop the training at any time with the button Stop.
The best brain will be saved automaticaly.

You can check the output console to see the best times of the current generation and how many children and new AI are made.

## Authors

**Maxime "mrouffet" ROUFFET** - main developer (maximerouffet@gmail.com)