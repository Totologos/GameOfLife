# Game of life
Game of life is a cellular automaton [https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life] displayed on Dos windows. 
It is developped in c# only for fun! 

Features :
- Conway's Game of Life rules:
  - Any live cell with fewer than two live neighbors dies, as if by underpopulation.
  - Any live cell with two or three live neighbors lives on to the next generation.
  - Any live cell with more than three live neighbors dies, as if by overpopulation.
  - Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.
- Used a double buffer to suppess blink effect on console (dos windows)
- Application detects the end of game board evolution. In this case, an random cell or a sprite is added on the game board until the evolution resumes

![game of life screenshot](https://github.com/Totologos/GameOfLife/blob/master/window_snipping.PNG)
