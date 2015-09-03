﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using System.Xml.Linq;
using XMLDataTypes;

namespace JLPT_Game.Components
{
    class VocabularyGame : DrawableGameComponent
    {
        #region Field

        Texture2D blank;
        SpriteFont text1;
        SpriteFont text2;
        SpriteFont text3;
        SpriteFont text4;
        SpriteFont text5;
        SpriteFont text6;
        SubmissionOfKanji[] vocabulary;

        bool pressed = false;

        int selectItemNumber = 0;

        Rectangle mainText;
        Rectangle[] choose;
        Vector2[] chooseText;

        int round = 0;
        int wins = 0;

        int CorectKanji;
        int[] kanjiIndex =new int [6];
        int CorectKanjiIndex;

        public bool lotery = false;

        Random random;

        bool correctAnswer = false;
        int goodAnswer = 0;
        int wrongAnswer = 0;
        int goodKanjiAnswer = 0;
        int wrongKanjiAnswer = 0;

        VocabularyList vocabularyList;

        //KeyboardState currentKeyboard;
        //KeyboardState previousKeyboard;

        public bool pauseGame = false;

        SubmissionOfKanji[] submission;
        int [] submissionIndex;

        bool showReading = false;

        bool replay = false;
        List <int> rep;
        int l_rep = 0;
        int max_rep = 25;

        #endregion


        #region Initialization

        public VocabularyGame(Game game)
            : base(game)
        {
            random = new Random();

            this.vocabularyList = new VocabularyList(game);
            game.Components.Add(this.vocabularyList);

            rep = new List<int>();
        }
        ~ VocabularyGame() 
        {
            Game.Components.Remove(this.vocabularyList);
            vocabularyList.Dispose();
            vocabularyList = null;
        }

        public SpriteBatch spriteBatch
        {
            get
            {
                return (SpriteBatch)this.Game.Services.GetService(typeof(SpriteBatch));
            }
        }

        #endregion


        #region BasicComponentMethod

        public override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            text1 = Game.Content.Load<SpriteFont>("kanjiText1");
            text2 = Game.Content.Load<SpriteFont>("kanjiText2");
            text3 = Game.Content.Load<SpriteFont>("kanjiText3");
            text4 = Game.Content.Load<SpriteFont>("test");
            text5 = Game.Content.Load<SpriteFont>("kanjiText4");
            text6 = Game.Content.Load<SpriteFont>("kanjiText5");

            vocabulary = Game.Content.Load<SubmissionOfKanji[]>("XMLFile2");

            blank = new Texture2D(GraphicsDevice, 1, 1);
            blank.SetData(new[] { Color.White });

            int maxIndex = 0;
            int minPoints = 80;

            for (int i = 0; i < vocabulary.Length; i++)
            {
                if (vocabulary[i].priority >= minPoints)
                {
                    maxIndex++;
                }
            }

            int index = 0;
            submission = new SubmissionOfKanji[maxIndex];
            submissionIndex = new int[maxIndex];

            for (int i = 0; i < vocabulary.Length; i++)
            {
                if (vocabulary[i].priority >= minPoints)
                {
                    submission[index] = vocabulary[i];
                    submissionIndex[index] = i;

                    index++;
                }
            }


            SetKanjiGameRound();

            constructWirtualMenu();

            base.LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            /*
            this.previousKeyboard = this.currentKeyboard;
            this.currentKeyboard = Keyboard.GetState();

            if (this.currentKeyboard.IsKeyDown(Keys.Space))
            {
                if (!this.previousKeyboard.IsKeyDown(Keys.Space))
                {
                    if (showReading) showReading = false;
                    else showReading = true;
                }
            }
             * */

            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, MouseState d, Vector2 position)
        {
            if (!pauseGame)
            {
                /*
                if (lotery)
                {
                    SetKanjiGameRound();
                    lotery = false;

                    wins = 0;
                    round = 0;
                }
                 * */

                string test1 = submission[CorectKanji].reading;
                string test2 = submission[CorectKanji].signs;

                spriteBatch.Begin();

                string score = wins + " / " + round;
                if (replay) score += " [powtórka]";
                spriteBatch.DrawString(text4, score, new Vector2(10, 5), Color.Black);


                // solution

                if (round > 0)
                {
                    Rectangle solutionBox = new Rectangle(GraphicsDevice.Viewport.Width - 520, 5, 500, 37);
                    spriteBatch.Draw(blank, solutionBox, Color.Black);

                    //int widthSolution = 40;
                    string goodKanji = submission[goodAnswer].reading;
                    //int measureString = text5.MeasureString(goodKanji);

                    if (correctAnswer)
                    {
                        if ((position.X >= solutionBox.X && position.X <= solutionBox.X + solutionBox.Width) &&
                            (position.Y >= solutionBox.Y && position.Y <= solutionBox.Y + solutionBox.Height))
                        {
                            if (d.LeftButton == ButtonState.Pressed)
                            {
                                spriteBatch.Draw(blank, solutionBox, Color.Green);
                                spriteBatch.DrawString(text5, goodKanji, new Vector2((int)(solutionBox.X + solutionBox.Width / 2 - text5.MeasureString(goodKanji).X / 2), (int)(solutionBox.Y + solutionBox.Height / 2 - text5.MeasureString(goodKanji).Y / 2)), Color.Black);
                                pressed = true;
                            }
                            else if (pressed)
                            {
                                pressed = false;
                                pauseGame = true;
                                vocabularyList.index = goodKanjiAnswer;
                            }
                            else
                            {
                                spriteBatch.Draw(blank, solutionBox, Color.DarkGreen);
                                spriteBatch.DrawString(text5, goodKanji, new Vector2((int)(solutionBox.X + solutionBox.Width / 2 - text5.MeasureString(goodKanji).X / 2), (int)(solutionBox.Y + solutionBox.Height / 2 - text5.MeasureString(goodKanji).Y / 2)), Color.White);
                            }
                        }
                        else
                        {
                            spriteBatch.Draw(blank, solutionBox, Color.Green);
                            spriteBatch.DrawString(text5, goodKanji, new Vector2((int)(solutionBox.X + solutionBox.Width / 2 - text5.MeasureString(goodKanji).X / 2), (int)(solutionBox.Y + solutionBox.Height / 2 - text5.MeasureString(goodKanji).Y / 2)), Color.Black);
                        }
                    }
                    else
                    {
                        string wrongKanji = submission[wrongAnswer].reading;

                        Rectangle solutionBox1 = new Rectangle((int)solutionBox.X,
                                                               (int)solutionBox.Y,
                                                               (int)solutionBox.Width / 2,
                                                               (int)solutionBox.Height);

                        Rectangle solutionBox2 = new Rectangle((int)(solutionBox.X + solutionBox.Width / 2),
                                                               (int)solutionBox.Y,
                                                               (int)solutionBox.Width / 2,
                                                               (int)solutionBox.Height);

                        if ((position.X >= solutionBox1.X && position.X <= solutionBox1.X + solutionBox1.Width) &&
                            (position.Y >= solutionBox1.Y && position.Y <= solutionBox1.Y + solutionBox1.Height))
                        {
                            if (d.LeftButton == ButtonState.Pressed)
                            {
                                spriteBatch.Draw(blank, solutionBox1, Color.Red);
                                spriteBatch.DrawString(text5, wrongKanji, new Vector2((int)(solutionBox1.X + solutionBox1.Width / 2 - text5.MeasureString(wrongKanji).X / 2), (int)(solutionBox1.Y + solutionBox1.Height / 2 - text5.MeasureString(wrongKanji).Y / 2)), Color.Black);
                                pressed = true;
                            }
                            else if (pressed)
                            {
                                pressed = false;
                                pauseGame = true;
                                vocabularyList.index = wrongKanjiAnswer;
                            }
                            else
                            {
                                spriteBatch.Draw(blank, solutionBox1, Color.DarkRed);
                                spriteBatch.DrawString(text5, wrongKanji, new Vector2((int)(solutionBox1.X + solutionBox1.Width / 2 - text5.MeasureString(wrongKanji).X / 2), (int)(solutionBox1.Y + solutionBox1.Height / 2 - text5.MeasureString(wrongKanji).Y / 2)), Color.White);
                            }
                        }
                        else
                        {
                            spriteBatch.Draw(blank, solutionBox1, Color.Red);
                            spriteBatch.DrawString(text5, wrongKanji, new Vector2((int)(solutionBox1.X + solutionBox1.Width / 2 - text5.MeasureString(wrongKanji).X / 2), (int)(solutionBox1.Y + solutionBox1.Height / 2 - text5.MeasureString(wrongKanji).Y / 2)), Color.Black);
                        }


                        if ((position.X >= solutionBox2.X && position.X <= solutionBox2.X + solutionBox2.Width) &&
                            (position.Y >= solutionBox2.Y && position.Y <= solutionBox2.Y + solutionBox2.Height))
                        {
                            if (d.LeftButton == ButtonState.Pressed)
                            {
                                spriteBatch.Draw(blank, solutionBox2, Color.Green);
                                spriteBatch.DrawString(text5, goodKanji, new Vector2((int)(solutionBox2.X + solutionBox2.Width / 2 - text5.MeasureString(goodKanji).X / 2), (int)(solutionBox2.Y + solutionBox2.Height / 2 - text5.MeasureString(goodKanji).Y / 2)), Color.Black);
                                pressed = true;
                            }
                            else if (pressed)
                            {
                                pressed = false;
                                pauseGame = true;
                                vocabularyList.index = goodKanjiAnswer;
                            }
                            else
                            {
                                spriteBatch.Draw(blank, solutionBox2, Color.DarkGreen);
                                spriteBatch.DrawString(text5, goodKanji, new Vector2((int)(solutionBox2.X + solutionBox2.Width / 2 - text5.MeasureString(goodKanji).X / 2), (int)(solutionBox2.Y + solutionBox2.Height / 2 - text5.MeasureString(goodKanji).Y / 2)), Color.White);
                            }
                        }
                        else
                        {
                            spriteBatch.Draw(blank, solutionBox2, Color.Green);
                            spriteBatch.DrawString(text5, goodKanji, new Vector2((int)(solutionBox2.X + solutionBox2.Width / 2 - text5.MeasureString(goodKanji).X / 2), (int)(solutionBox2.Y + solutionBox2.Height / 2 - text5.MeasureString(goodKanji).Y / 2)), Color.Black);
                        }
                    }
                }


                //
                // must "constructWirtualMenu()" -> realized

                spriteBatch.Draw(blank, mainText, Color.White);

                int lineSpacing = 15;

                int textSpacing = lineSpacing + mainText.Y;

                Vector2 meaningPosition = new Vector2((int)((mainText.X + mainText.Width / 2) - text3.MeasureString(test1).X / 2), textSpacing);

                spriteBatch.DrawString(text3, test1, meaningPosition, Color.Black);


                textSpacing += (int)(text3.MeasureString(test1).Y + lineSpacing);

                for (int i = 0; i < test2.Length; i++)
                {
                    if (test2[i] == '[')
                    {
                        test2 = test2.Remove(i - 1);
                        break;
                    }
                }

                test2 = "(" + test2 + ")";

                Vector2 readingPosition = new Vector2((int)((mainText.X + mainText.Width / 2) - text5.MeasureString(test2).X / 2), textSpacing);

                spriteBatch.DrawString(text5, test2, readingPosition, Color.Black);



                //
                // must "constructWirtualMenu()" -> realized

                for (int i = 0; i < 6; i++)
                    if ((position.X >= choose[i].X && position.X <= choose[i].X + choose[i].Width &&
                                (position.Y >= choose[i].Y && position.Y <= choose[i].Y + choose[i].Height)))
                    {
                        if (d.LeftButton == ButtonState.Pressed)
                        {
                            spriteBatch.Draw(blank, choose[i], Color.White);
                            spriteBatch.DrawString(text6, submission[kanjiIndex[i]].meaning, new Vector2(choose[i].X + (choose[i].Width / 2) - (text6.MeasureString(submission[kanjiIndex[i]].meaning).X / 2), choose[i].Y + (choose[i].Height / 2) - (text6.MeasureString(submission[kanjiIndex[i]].meaning).Y / 2)), Color.Black);
                            pressed = true;
                        }
                        else if (pressed)
                        {
                            pressed = false;

                            round++;

                            if (i == CorectKanjiIndex)
                            {
                                wins++;
                                correctAnswer = true;
                                goodAnswer = kanjiIndex[i];
                                goodKanjiAnswer = submissionIndex[goodAnswer];

                                if (replay)
                                {
                                    rep.Remove(goodAnswer);
                                }
                            }
                            else
                            {
                                correctAnswer = false;
                                goodAnswer = kanjiIndex[CorectKanjiIndex];
                                goodKanjiAnswer = submissionIndex[goodAnswer];
                                wrongAnswer = kanjiIndex[i];
                                wrongKanjiAnswer = submissionIndex[wrongAnswer];

                                if (!replay)
                                {
                                    rep.Add(goodAnswer);
                                }
                            }

                            if (!replay)
                            {
                                l_rep++;

                                if (l_rep == max_rep && rep.Count !=0) replay = true;
                                else if (l_rep == max_rep && rep.Count == 0) l_rep = 0;
                            }
                            else
                            {
                                if (rep.Count == 0)
                                {
                                    replay = false;
                                    l_rep = 0;
                                }
                            }

                            SetKanjiGameRound();
                            //selectItemNumber = i + 1;
                        }
                        else
                        {
                            spriteBatch.Draw(blank, choose[i], Color.Black);
                            spriteBatch.DrawString(text6, submission[kanjiIndex[i]].meaning, new Vector2(choose[i].X + (choose[i].Width / 2) - (text6.MeasureString(submission[kanjiIndex[i]].meaning).X / 2), choose[i].Y + (choose[i].Height / 2) - (text6.MeasureString(submission[kanjiIndex[i]].meaning).Y / 2)), Color.White);
                        }
                    }
                    else
                    {
                        spriteBatch.Draw(blank, choose[i], Color.White);
                        spriteBatch.DrawString(text6, submission[kanjiIndex[i]].meaning, new Vector2(choose[i].X + (choose[i].Width / 2) - (text6.MeasureString(submission[kanjiIndex[i]].meaning).X / 2), choose[i].Y + (choose[i].Height / 2) - (text6.MeasureString(submission[kanjiIndex[i]].meaning).Y / 2)), Color.Black);
                    }

                spriteBatch.End();
            }
            else
            {
                //kanjiList.Draw(gameTime, d, position);
                vocabularyList.Draw(gameTime, d, position);
                returnButton(d, position);
            }

            base.Draw(gameTime);
        }

        #endregion


        #region privateMethods

        private void constructWirtualMenu()
        {
            mainText = new Rectangle(
                (int)0,
                (int)(GraphicsDevice.Viewport.Height / 10),
                (int)(GraphicsDevice.Viewport.Width),
                (int)(GraphicsDevice.Viewport.Height / 4));

            //

            int upSpace = 15;
            int downSpace = 50;

            Rectangle chooseText = new Rectangle(
                (int)0,
                (int)(mainText.Y + mainText.Height + upSpace),
                (int)(GraphicsDevice.Viewport.Width),
                (int)(GraphicsDevice.Viewport.Height - (mainText.Y + mainText.Height + upSpace) - downSpace));

            //spriteBatch.Draw(blank, chooseText, Color.White);


            int upChooseSpace = 15;
            int chooseHeight = (int)(((chooseText.Height - (upChooseSpace * 4)) / 3));
            int chooseWidth = 380;
            int centerChooseSpace = 20;

            int choosePosition = upChooseSpace + chooseText.Y;

            choose = new Rectangle[6];
            //chooseText = new Vector2(0.0f,0.0f)[6];

            choose[0] = new Rectangle(
                (int)(GraphicsDevice.Viewport.Width / 2 - chooseWidth - (centerChooseSpace / 2)),
                (int)(choosePosition),
                (int)(chooseWidth),
                (int)(chooseHeight));

            //spriteBatch.Draw(blank, choose[0], Color.White);


            choose[1] = new Rectangle(
                (int)(GraphicsDevice.Viewport.Width / 2 + (centerChooseSpace / 2)),
                (int)(choosePosition),
                (int)(chooseWidth),
                (int)(chooseHeight));

            //spriteBatch.Draw(blank, choose[1], Color.White);

            choosePosition += (chooseHeight + upChooseSpace);


            choose[2] = new Rectangle(
                (int)(GraphicsDevice.Viewport.Width / 2 - chooseWidth - (centerChooseSpace / 2)),
                (int)(choosePosition),
                (int)(chooseWidth),
                (int)(chooseHeight));

            //spriteBatch.Draw(blank, choose[2], Color.White);


            choose[3] = new Rectangle(
                (int)(GraphicsDevice.Viewport.Width / 2 + (centerChooseSpace / 2)),
                (int)(choosePosition),
                (int)(chooseWidth),
                (int)(chooseHeight));

            //spriteBatch.Draw(blank, choose[3], Color.White);

            choosePosition += (chooseHeight + upChooseSpace);


            choose[4] = new Rectangle(
                (int)(GraphicsDevice.Viewport.Width / 2 - chooseWidth - (centerChooseSpace / 2)),
                (int)(choosePosition),
                (int)(chooseWidth),
                (int)(chooseHeight));

            //spriteBatch.Draw(blank, choose[4], Color.White);


            choose[5] = new Rectangle(
                (int)(GraphicsDevice.Viewport.Width / 2 + (centerChooseSpace / 2)),
                (int)(choosePosition),
                (int)(chooseWidth),
                (int)(chooseHeight));
        }


        public void SetKanjiGameRound()
        {
            int max_tab = submission.Length;
            bool repeat = false;
            bool r1 = true;

            for (int i = 0; i < 6; i++)
            {
                int number=0;

                if (!replay || !r1)
                {
                    if (!r1 && CorectKanjiIndex == i) continue;
 
                    number = random.Next(0, max_tab);
                }
                else
                {
                    number = random.Next(0, rep.Count);
                    CorectKanjiIndex = random.Next(0, 6);
                    kanjiIndex[CorectKanjiIndex] = rep[number];
                    r1 = false;

                    if (CorectKanjiIndex == 0) continue;
                    else number = random.Next(0, max_tab);
                }

              check:
                for (int j = 0; j < i; j++)
                {
                    if (kanjiIndex[j] == number || number == CorectKanji)
                    {
                        number++;
                        if (number == max_tab) number = 0;
                        repeat = true;
                        break;
                    }
                }

                if (repeat)
                {
                     repeat = false;
                     goto check;
                }

                kanjiIndex[i] = number;
            }

            if (!replay) CorectKanjiIndex = random.Next(0, 6);
            CorectKanji = kanjiIndex[CorectKanjiIndex];
        }


        private void returnButton(MouseState d, Vector2 position)
        {
            spriteBatch.Begin();

            int border = 35;
            string t1 = "powrót do gry";

            Rectangle textPosition = new Rectangle(
                    (int)(GraphicsDevice.Viewport.Width - (2*border + text4.MeasureString(t1).X)),
                    (int)(GraphicsDevice.Viewport.Height - (border + text4.MeasureString(t1).Y)),
                    (int)(text4.MeasureString(t1).X + 2 * border),
                    (int)(text4.MeasureString(t1).Y + border));


            Vector2 miPosition = new Vector2((int)((textPosition.X + textPosition.Width / 2) - text4.MeasureString(t1).X / 2), textPosition.Y + 5);

            if ((position.X >= textPosition.X && position.X <= textPosition.X + textPosition.Width) &&
                (position.Y >= textPosition.Y && position.Y <= textPosition.Y + textPosition.Height))
            {
                if (d.LeftButton == ButtonState.Pressed)
                {
                    spriteBatch.Draw(blank, textPosition, Color.DarkGreen);
                    spriteBatch.DrawString(text4, t1, miPosition, Color.White);
                    pressed = true;
                }
                else if (pressed)
                {
                    pressed = false;
                    pauseGame = false;
                }
                else
                {
                    spriteBatch.Draw(blank, textPosition, Color.Black);
                    spriteBatch.DrawString(text4, t1, miPosition, Color.White);
                }
            }
            else
            {
                spriteBatch.Draw(blank, textPosition, Color.LightGray);
                spriteBatch.DrawString(text4, t1, miPosition, Color.Black);
            }

            spriteBatch.End();
        }

        #endregion
    }
}
