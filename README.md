# Termite
An Everybody Edits SWF resource parser in C#

# How to use
Download freegame.swf http://r.playerio.com/r/everybody-edits-su9rn58o40itdbnw69plyw/freegame.swf and place it in the build directory.

You have to have Java 8 Runtime or later installed (http://java.com/en/download/).

Extract the files ItemId.as and ItemManager.as and place them in the build folder. Start the app and you're ready to start extracting.

The app writes a JSON formatted array to the console, containing the following information about all blocks in the game:
- id
- layer (background or foreground)
- bmd (which system block package it is contained in, ex. decorations)
- the shop name
- description of the block (not many have these)
- item tab
- minimap color (in hexadecimal)
- block package
- the sprite offset in the respective image file (use bmd in conjunction with the consistent file-naming pattern to find the right image file.)

It is also able to resolve block ids in the form of ItemId.* to the actual numeric id, and does so by default.

# To Do
- ability to extract sprites from the image files using the sprite offset
