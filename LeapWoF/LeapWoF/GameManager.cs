﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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

        public string ChallengePhrase;
        private string TemporaryPuzzle;
        public List<string> charGuessList = new List<string>();        
        

        public GameState GameState { get; private set; }

        private string currentPuzzle; // JOSH: New property to store the current puzzle phrase
        private string solution; // JOSH: New property to store the solution to the puzzle

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
            //Library to random pick an element from an array
            Random rnd = new Random();

            // Sentence is created from three elements that are randomly selected from different arrays.
            var subjects = new string[] { "I", "You", "Kim", "Shruthi", "Josh", "Andrea", "People", "We", "They", "Mary" };
            var verbs = new string[]
            {
                  "will search for", "will get", "will find", "attained", "found", "will start interacting with",
                    "will accept", "accepted", "loved", "will paint"
            };
            var objects = new string[] 
            { 
                "an offer", "an apple", "a car","an orange", "the treasure", "a surface", "snow", 
                "alligators", "good code", "dogs", "cookies", "foxes", "aubergines", "zebras" 
            };

            // r is the index randomly selected from the sequence of the lenght of the array
            int r = rnd.Next(subjects.Length);
            var randomSubject = subjects[r];
            // r is recalculate to avoid the same index element is taken from different arrays and also because arrays have different lenghts.
            r = rnd.Next(verbs.Length);
            var randomVerb = verbs[r];
            r = rnd.Next(objects.Length);
            var randomObject = objects[r];

            // This is the sentence to be guessed
            TemporaryPuzzle =  $"{randomSubject} {randomVerb} {randomObject}";

            // This logic replaces with a "-" every char from the sentence except for the space.
            string space = " ";
            string ChallengePhrase = "";
            foreach (char i in TemporaryPuzzle)
            {
                if (space.Contains(i))
                {
                    ChallengePhrase += " ";
                }
                else
                {
                    ChallengePhrase += "-";
                }
            }


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
            outputProvider.WriteLine(ChallengePhrase);
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
            outputProvider.WriteLine();
            var guess = inputProvider.Read();
            // JOSH: Check if the guess is correct
            if (string.Equals(guess, solution, StringComparison.OrdinalIgnoreCase))
            {
                outputProvider.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                outputProvider.WriteLine("Congratulations, you've solved the puzzle!");
                System.Threading.Thread.Sleep(1500);
                Console.ResetColor();
                outputProvider.WriteLine();
                GameState = GameState.GameOver;
            }
            else
            {
                outputProvider.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                outputProvider.WriteLine("Sorry, but that's not the correct solution.");
                Console.ResetColor();
                outputProvider.WriteLine();

                outputProvider.WriteLine("Press any key to try again...");
                inputProvider.Read();

                // Future enhancement: add logic for changing to next player's turn
            }
        }
        public void GuessLetter()
        {
            outputProvider.Write("Please guess a letter: ");
            var guessedInput = inputProvider.Read();

            //Take only the first character from the guessed input
            string guess = guessedInput.Substring(0, 1);
            outputProvider.WriteLine($"Guessed Letter is: {guess}");
            guess = guess.ToUpper();
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
                    if (string.Equals(ChallengePhrase, TemporaryPuzzle, StringComparison.OrdinalIgnoreCase))
                    {
                        outputProvider.WriteLine("Congratulations!!! You won!");
                        GameState = GameState.GameOver;
                    }
                    charGuessList.Add(guess);
                    outputProvider.WriteLine("Press any key to continue...");
                    inputProvider.Read();
                }
                else
                {
                    //If letter is not in word, let player know and keep playing
                    outputProvider.WriteLine("Hard luck, try again!");
                    outputProvider.WriteLine("Press any key to continue...");
                    inputProvider.Read();
                }
            } else
            {
                outputProvider.WriteLine("You have already guessed that letter! Please try again.");
                outputProvider.WriteLine("Press any key to continue...");
                inputProvider.Read();
            }
            
        }

        private List<int> GetAllIndicesOfGuessedLetterInChallengePhrase(string guess)
        {
            int start = 0;
            int end = ChallengePhrase.Length;
            int at = 0;
            int count = 0;
            var posList = new List<int>();
            while ((start <= end) && (at > -1))
            {
                // start+count must be a position within the string
                count = end - start;
                at = TemporaryPuzzle.IndexOf(guess, start, count);
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
