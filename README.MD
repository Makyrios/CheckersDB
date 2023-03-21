# Checkers

Simple implementation of board game Checkers with local data base.


## Game rules

The game rules are based on a combination of international, English, and russian checkers. Players can move their pieces diagonally, capturing their opponent's pieces by jumping over them. If a player reaches the opposite side of the board, their piece is crowned and gains an additional option to move in any diagonal direction on any number of squares.

## Local database

This project includes a local database that works on serialization in .json files. The database contains information about users and played games. Users can create an account, log in, and view their game history and statistics. The database also stores user ratings, which are used to determine the player's skill level in matchmaking.

## Game types

Standard Game: Players input their rating on their own.\
AllInRatingGame: Players play on the lowest rating between them.\
Training Game: Rating equals 0.

## User account types

Base Game Account - default account without any bonuses.\
BonusGameAccount - wins give more rating and loses take half of lost rating.\
StreakGameAccount - user gets bonus points if they win several games in a row.

## Future implementations

Features to implement:

Play against bots, controlled by AI.\
Multiplayer support.\
More game and account types.

## Screenshots
![1](https://user-images.githubusercontent.com/43245644/226625034-fa6d862f-b5d8-48ff-ac5c-b607defcb4a5.png)
![2](https://user-images.githubusercontent.com/43245644/226625062-5941e4f5-a3ce-49bc-828e-b3d2ccf9283e.png)
![3](https://user-images.githubusercontent.com/43245644/226625088-48aed30f-6a20-403a-b0d0-b6f2882498f6.png)
![4](https://user-images.githubusercontent.com/43245644/226625104-ab02aeeb-a9b3-46f2-945a-7e00ad672071.png)
![5](https://user-images.githubusercontent.com/43245644/226625126-8f187f88-c4c0-4835-89e1-a74f61eeb7b7.png)
![6](https://user-images.githubusercontent.com/43245644/226625553-71da2a58-675b-4263-aa9d-0837c311721b.png)
