namespace BoulderBuddy.Models.Models.ViewModels
{
    public class AscentsSectionViewModel
    {
        public List<AscentsViewModel> AscentData { get; set; }

        public float AscentsAverage { get; set; }

        public int AscentsSuccessful = 0;
        public int AscentsTotal = 0;

        #region Radios
        public string Radio_Status_Success = "";
        public string Radio_Status_Attempt = "";
        public string Radio_Status_Blank = "";
        #endregion

        #region Progress Bar
        public int Progress_Success = 0;
        public int Progress_Attempt = 0;
        #endregion
    }
}
