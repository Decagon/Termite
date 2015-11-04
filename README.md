# Termite

Termite decompiles, extracts, and assembles blocks, decorations, ids, and much more into a developer friendly format from Everybody Edits. This means that if your application that depends on blocks (such as a minimap generator) or another EE-related resource can be updated with new blocks almost instantaneously.

# How to use
Download freegame.swf http://r.playerio.com/r/everybody-edits-su9rn58o40itdbnw69plyw/freegame.swf and place the swf in the build directory (i.e the Debug folder or the Release folder.)

You have to have Java 8 Runtime or later installed (http://java.com/en/download/).

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

# To Do
- ability to extract sprites from the image files using the sprite offset
