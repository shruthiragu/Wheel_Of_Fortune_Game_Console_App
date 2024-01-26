﻿using System;
using System.Collections;
using System.Collections.Generic;
using LeapWoF.Interfaces;

namespace LeapWoF
{

    /// <summary>
    /// The GameManager class, handles all game logic
    /// </summary>
    public class GameManager
    {

        /// <summary>
        /// The input provider
        /// </summary>
        private IInputProvider inputProvider;

        /// <summary>
        /// The output provider
        /// </summary>
        private IOutputProvider outputProvider;

        private string TemporaryPuzzle;
        public List<string> charGuessList = new List<string>();
        private string ChallengePhrase = "H_l_o W_r_d";
        

        public GameState GameState { get; private set; }

        public GameManager() : this(new ConsoleInputProvider(), new ConsoleOutputProvider())
        {

        }

        public GameManager(IInputProvider inputProvider, IOutputProvider outputProvider)
        {
            if (inputProvider == null)
                throw new ArgumentNullException(nameof(inputProvider));
            if (outputProvider == null)
                throw new ArgumentNullException(nameof(outputProvider));

            this.inputProvider = inputProvider;
            this.outputProvider = outputProvider;

            GameState = GameState.WaitingToStart;
        }

        /// <summary>
        /// Manage game according to game state
        /// </summary>
        public void StartGame()
        {
            InitGame();

            while (true)
            {

                PerformSingleTurn();

                if (GameState == GameState.RoundOver)
                {
                    StartNewRound();
                    continue;
                }

                if (GameState == GameState.GameOver)
                {
                    outputProvider.WriteLine("Game over");
                    break;
                }
            }
        }
        public void StartNewRound()
        {
            TemporaryPuzzle = "Hello world";            

            // update the game state
            GameState = GameState.RoundStarted;
        }

        public void PerformSingleTurn()
        {
            outputProvider.Clear();
            DrawPuzzle();
            outputProvider.WriteLine("Type 1 to spin, 2 to solve");
            GameState = GameState.WaitingForUserInput;

            var action = inputProvider.Read();

            switch (action)
            {
                case "1":
                    Spin();
                    break;
                case "2":
                    Solve();
                    break;
            }

        }

        /// <summary>
        /// Draw the puzzle
        /// </summary>
        private void DrawPuzzle()
        {
            outputProvider.WriteLine("The puzzle is:");
            outputProvider.WriteLine(TemporaryPuzzle);
            outputProvider.WriteLine();
        }

        /// <summary>
        /// Spin the wheel and do the appropriate action
        /// </summary>
        public void Spin()
        {
            outputProvider.WriteLine("Spinning the wheel...");
            //TODO - Implement wheel + possible wheel spin outcomes
            GuessLetter();
        }

        public void Solve()
        {
            outputProvider.Write("Please enter your solution:");
            var guess = inputProvider.Read();
        }
        public void GuessLetter()
        {
            outputProvider.Write("Please guess a letter: ");
            var guessedInput = inputProvider.Read();

            //Take only the first character from the guessed input
            string guess = guessedInput.Substring(0, 1);
            outputProvider.WriteLine($"Guessed Letter is: {guess}");
            if (!charGuessList.Contains(guess))
            {
                if (TemporaryPuzzle.Contains(guess))
                {
                    //Get all locations of guessed letter
                    var posList = GetAllIndicesOfGuessedLetterInChallengePhrase(guess);
                    
                    //Replace underscore with matching letters
                    foreach (var pos in posList)
                    {
                        ChallengePhrase = ChallengePhrase.Substring(0, pos) + guess + ChallengePhrase.Substring(pos + 1);
                        outputProvider.WriteLine("Here is the updated challenge phrase: " + ChallengePhrase);
                    }                    
                    charGuessList.Add(guess);
                }
                else
                {
                    //If letter is not in word, let player know and keep playing
                    outputProvider.WriteLine("Hard luck, try again!");
                }
            } else
            {
                outputProvider.WriteLine("You have already guessed that letter! Please try again.");
            }
            
        }

        private List<int> GetAllIndicesOfGuessedLetterInChallengePhrase(string guess)
        {
            int start = 0;
            int end = ChallengePhrase.Length - 1;
            int at = 0;
            int count = 0;
            var posList = new List<int>();
            while ((start <= end) && (at > -1))
            {
                // start+count must be a position within the string
                count = end - start;
                at = ChallengePhrase.IndexOf(guess, start, count);
                if (at == -1) break;
                //Debug: Console.Write("{0} ", at);
                posList.Add(at);
                start = at + 1;
            }
            return posList;
        }

        /// <summary>
        /// Optional logic to accept configuration options
        /// </summary>
        public void InitGame()
        {

            outputProvider.WriteLine("Welcome to Wheel of Fortune!");
            StartNewRound();
        }
    }
}
