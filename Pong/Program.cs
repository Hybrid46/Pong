using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Numerics;
using Rectangle = Raylib_cs.Rectangle;

class PongGame
{
    const int screenWidth = 800;
    const int screenHeight = 600;
    const int paddleSpeed = 6;
    const int ballBaseSpeed = 5;

    static Rectangle player1;
    static Rectangle player2;
    static Vector2 ballPosition;
    static Vector2 ballSpeed;
    static int player1Score = 0;
    static int player2Score = 0;
    static bool gamePaused = true;

    static void Main()
    {
        InitWindow(screenWidth, screenHeight, "Simple Pong - Raylib");
        SetTargetFPS(60);

        player1 = new Rectangle(50, screenHeight / 2 - 60, 20, 120);
        player2 = new Rectangle(screenWidth - 70, screenHeight / 2 - 60, 20, 120);
        ResetBall();

        while (!WindowShouldClose())
        {
            HandleInput();

            if (!gamePaused)
            {
                UpdateGame();
            }

            BeginDrawing();
            DrawGame();
            EndDrawing();
        }

        CloseWindow();
    }

    static void HandleInput()
    {
        if (IsKeyDown(KeyboardKey.W)) player1.Position = new Vector2(player1.Position.X, player1.Position.Y - paddleSpeed);
        if (IsKeyDown(KeyboardKey.S)) player1.Position = new Vector2(player1.Position.X, player1.Position.Y + paddleSpeed);

        if (IsKeyDown(KeyboardKey.Up)) player2.Position = new Vector2(player2.Position.X, player2.Position.Y - paddleSpeed);
        if (IsKeyDown(KeyboardKey.Down)) player2.Position = new Vector2(player2.Position.X, player2.Position.Y + paddleSpeed);

        if (IsKeyPressed(KeyboardKey.Space)) gamePaused = !gamePaused;
    }

    static void UpdateGame()
    {
        // Move ball
        ballPosition += ballSpeed;

        if (CheckCollisionCircleRec(ballPosition, 10, player1) ||
            CheckCollisionCircleRec(ballPosition, 10, player2))
        {
            ballSpeed.X *= -1.1f; // Reverse direction and increase speed

            Rectangle hitPaddle = CheckCollisionCircleRec(ballPosition, 10, player1) ? player1 : player2;
            float hitPosition = (ballPosition.Y - hitPaddle.Position.Y) / hitPaddle.Height;
            ballSpeed.Y = (hitPosition - 0.5f) * 10f; // -5 to +5 range
        }

        // Wall collisions
        if (ballPosition.Y <= 10 || ballPosition.Y >= screenHeight - 10)
        {
            ballSpeed.Y *= -1; // Reverse vertical direction
        }

        // Scoring
        if (ballPosition.X < 0)
        {
            player2Score++;
            ResetBall();
        }
        else if (ballPosition.X > screenWidth)
        {
            player1Score++;
            ResetBall();
        }

        player1.Position = new Vector2(player1.Position.X, Math.Clamp(player1.Position.Y, 0, screenHeight - player1.Height));
        player2.Position = new Vector2(player2.Position.X, Math.Clamp(player2.Position.Y, 0, screenHeight - player2.Height));
    }
    static void ResetBall()
    {
        ballPosition = new Vector2(screenWidth / 2, screenHeight / 2);
        ballSpeed = new Vector2(
            GetRandomValue(0, 1) == 0 ? ballBaseSpeed : -ballBaseSpeed,
            GetRandomValue(-ballBaseSpeed, ballBaseSpeed)
        );
        gamePaused = true;
    }

    static void DrawGame()
    {
        ClearBackground(Black);

        // Draw center line
        for (int i = 0; i < screenHeight; i += 30)
        {
            DrawRectangle(screenWidth / 2 - 5, i, 10, 20, DarkGray);
        }

        // Draw paddles
        DrawRectangleRec(player1, White);
        DrawRectangleRec(player2, White);

        // Draw ball
        DrawCircleV(ballPosition, 10, Green);

        // Draw scores
        DrawText(player1Score.ToString(), screenWidth / 4, 20, 40, White);
        DrawText(player2Score.ToString(), 3 * screenWidth / 4, 20, 40, White);

        // Draw instructions
        if (gamePaused)
        {
            DrawText("PRESS SPACE TO START", screenWidth / 2 - 180, screenHeight / 2, 30, Yellow);
        }

        DrawText("Player 1: W/S", 10, screenHeight - 30, 20, LightGray);
        DrawText("Player 2: UP/DOWN", screenWidth - 180, screenHeight - 30, 20, LightGray);
    }
}