namespace BoulderBuddy.Models.ViewModels
{
    public class RouteViewModel(Routes route, List<CommentsViewModel> comments, List<AscentsViewModel> ascents, List<GradeRatings> ratings, float ascentsAverage)
    {
        public Routes Route { get; set; } = route;
        public List<CommentsViewModel> RouteComments { get; set; } = comments;
        public List<AscentsViewModel> AscentData { get; set; } = ascents;
        public List<GradeRatings> GradeRatings { get; set; } = ratings;

        public float AscentsAverage { get; set; } = ascentsAverage;

        public int AscentsSuccessful = 0;
        public int AscentsTotal = 0;

        #region Radios
        public string Radio_Ascents_Status_Success = "";
        public string Radio_Ascents_Status_Attempt = "";
        public string Radio_Ascents_Status_Blank = "";
        #endregion

        #region Progress Bar
        public int Progress_Grading_Easy = 0;
        public int Progress_Grading_Fair = 0;
        public int Progress_Grading_Hard = 0;
        public int Progress_Ascents_Success = 0;
        public int Progress_Ascents_Attempt = 0;
        #endregion
    }
}
