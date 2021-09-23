using System;

namespace _15TileGame
{
    public static class Timer
    {
        private static UILabel uiLabel;                 //The UI label element which displays time
        private static bool counting;                   //Whether or not the timer is currently counting up/down
        private static float timerValue;                //The current timer value in seconds
        private static float maxChallengeTime = 60f;    //The time (in seconds) to start counting down from when in challenge mode

        public static void Initialise(UILabel label)
        {
            uiLabel = label;
        }

        public static void ChangeMaxChallengeTime(bool increase)
        {
            //Increase/decrease the maximum time for challenge mode
            if (increase)
            {
                maxChallengeTime += 10f;
            }
            else
            {
                maxChallengeTime -= 10f;
            }

            //Clamp max time to between 10 seconds and 10 mins
            if(maxChallengeTime < 10f) { maxChallengeTime = 10f; }
            else if (maxChallengeTime > 600f) { maxChallengeTime = 600f; }
        }

        public static void StartTimer()
        {
            //Starts the timer counting up or down depending on GameType
            counting = true;
        }

        public static void ResetTimer(EGameType currentGameType)
        {
            //Stop the timer
            counting = false;

            //Reset the timer to its default value -
            //  0s for freeplay mode or the max challenge time for challenge mode
            if(currentGameType == EGameType.Freeplay)
            {
                timerValue = 0f;
            }
            else
            {
                timerValue = maxChallengeTime;
            }

            //Update the UI label to show the new time value
            UpdateLabelText();
        }

        public static void UpdateTimer(float deltaTime, EGameType currentGameType)
        {
            if (counting)
            {
                //If the timer is active, increase time for freeplay
                //  or count down for challenge mode
                if(currentGameType == EGameType.Freeplay)
                {
                    timerValue += deltaTime;
                }
                else
                {
                    timerValue -= deltaTime;
                }
                //Update the UI label to show the new time value
                UpdateLabelText();
            }
        }

        public static float GetTimerValue() { return timerValue; }

        public static string GetTimeString(bool challengeResult = false)
        {
            //Returns the current timer value as a formatted string
            float time = timerValue;
            if (challengeResult)
            {
                //For challenge mode results, subtract the timer value from its max value
                //  since we are counting down rather than up
                time = (maxChallengeTime - timerValue) + 1;
            }
            //TimeSpan allows the time value (in seconds) to be split into seconds and minutes
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            return timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
        }

        public static void UpdateLabelText()
        {
            //Set the label text to the formatted time string
            uiLabel.SetText(GetTimeString());
        }
    }
}
